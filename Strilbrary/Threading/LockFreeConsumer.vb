Imports System.Threading

Namespace Threading
    ''' <summary>
    ''' Consumes items produced by multiple producers.
    ''' </summary>
    ''' <remarks>
    ''' Ensures that enqueued items are consumed by ensuring at all exit points that either:
    ''' - the queue is empty
    ''' - or exactly one consumer exists
    ''' - or another exit point will be hit [by another thread]
    ''' </remarks>
    Public NotInheritable Class LockFreeConsumer(Of T)
        Implements IEnumerable(Of T)

        Private ReadOnly queue As New SingleConsumerLockFreeQueue(Of T)
        Private ReadOnly consumer As Action(Of T)
        Private ReadOnly context As SynchronizationContext
        Private running As Integer 'stores consumer state and is used as a semaphore

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(queue IsNot Nothing)
            Contract.Invariant(consumer IsNot Nothing)
            Contract.Invariant(context IsNot Nothing)
        End Sub

        ''' <param name="context">The synchronization context used to run queued actions.</param>
        ''' <param name="consumer">Consumes a queued item.</param>
        Public Sub New(context As SynchronizationContext, consumer As Action(Of T))
            Contract.Requires(context IsNot Nothing)
            Contract.Requires(consumer IsNot Nothing)
            Me.context = context
            Me.consumer = consumer
        End Sub

        '''<summary>Enqueues an item to be consumed by the consumer.</summary>
        Public Sub EnqueueConsume(item As T)
            queue.BeginEnqueue(item)
            TryBeginConsuming()
        End Sub
        ''' <summary>
        ''' Enqueues a sequence of items to be consumed by the consumer.
        ''' The items are guaranteed to end up adjacent in the queue.
        ''' </summary>
        Public Sub EnqueueConsume(items As IEnumerable(Of T))
            Contract.Requires(items IsNot Nothing)
            queue.BeginEnqueue(items)
            TryBeginConsuming()
        End Sub

        '''<summary>Returns true if consumer responsibilities were acquired by this thread.</summary>
        Private Function TryAcquireConsumer() As Boolean
            'Don't bother acquiring if there are no items to consume
            'This check is not stable but alright because enqueuers call this method after enqueuing
            'Even if an item is queued between the check and returning false, the enqueuer will call this method again
            'So we never end up with a non-empty idle queue
            If Not queue.HasItems Then Return False

            'Try to acquire consumer responsibilities
            Return Interlocked.Exchange(running, 1) = 0

            'Note that between the empty check and acquiring the consumer, all queued actions may have been processed.
            'Therefore the queue may be empty at this point, but that's alright. Just a bit of extra work, nothing unsafe.
        End Function
        '''<summary>Returns true if consumer responsibilities were released by this thread.</summary>
        Private Function TryReleaseConsumer() As Boolean
            Do
                'Don't release while there's still things to consume
                If queue.HasItems Then Return False

                'Release consumer responsibilities
                Interlocked.Exchange(running, 0)

                'It is possible that a new item was queued between the empty check and actually releasing
                'Therefore it is necessary to check if we can re-acquire in order to guarantee we don't leave a non-empty queue idle
                If Not TryAcquireConsumer() Then Return True

                'Even though we've now acquired consumer, we may have ended up with nothing to process!
                'So let's repeat this whole check for empty/release dance!
                'A caller could become live-locked here if other threads keep emptying and filling the queue.
                'But only consumer threads call here, and the live-lock requires that progress is being made.
                'So it's alright. We still make progress and we still don't end up in an invalid state.
            Loop
        End Function

        '''<summary>Start the consumer if there is work to do and it is not already running</summary>
        Private Sub TryBeginConsuming()
            If TryAcquireConsumer() Then
                context.Post(Sub() RunConsumer(), Nothing)
            End If
        End Sub
        '''<summary>Consumes queued items until there are none left.</summary>
        Private Sub RunConsumer()
            Do Until TryReleaseConsumer()
                Call consumer(queue.Dequeue())
            Loop
        End Sub

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return queue.GetEnumerator()
        End Function
        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace
