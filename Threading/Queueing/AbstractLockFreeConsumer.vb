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
    Public MustInherit Class BaseLockFreeConsumer(Of T)
        Private ReadOnly queue As New SingleConsumerLockFreeQueue(Of T)
        Private running As Integer 'stores consumer state and is used as a semaphore

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(queue IsNot Nothing)
        End Sub

        '''<summary>Enqueues an item to be consumed by the consumer.</summary>
        Protected Sub EnqueueConsume(ByVal item As T)
            queue.BeginEnqueue(item)

            'Start the consumer thread if it is not already running
            If TryAcquireConsumer() Then
                Call StartRunning()
            End If
        End Sub
        ''' <summary>
        ''' Enqueues a sequence of items to be consumed by the consumer.
        ''' The items are guaranteed to end up adjacent in the queue.
        ''' </summary>
        Protected Sub EnqueueConsume(ByVal items As IEnumerable(Of T))
            queue.BeginEnqueue(items)

            'Start the consumer thread if it is not already running
            If TryAcquireConsumer() Then
                Call StartRunning()
            End If
        End Sub

        '''<summary>Returns true if consumer responsibilities were acquired by this thread.</summary>
        Private Function TryAcquireConsumer() As Boolean
            'Don't bother acquiring if there are no items to consume
            'This unsafe check is alright because enqueuers call this method after enqueuing
            'Even if an item is queued between the check and returning false, the enqueuer will call this method again
            'So we never end up with a non-empty idle queue
            If queue.WasEmpty Then Return False

            'Try to acquire consumer responsibilities
            Return Interlocked.Exchange(running, 1) = 0

            'Note that between the empty check and acquiring the consumer, all queued actions may have been processed.
            'Therefore the queue may be empty at this point, but that's alright. Just a bit of extra work, nothing unsafe.
        End Function
        '''<summary>Returns true if consumer responsibilities were released by this thread.</summary>
        Private Function TryReleaseConsumer() As Boolean
            Do
                'Don't release while there's still things to consume
                If Not queue.WasEmpty Then Return False

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

        '''<summary>Used to start the Run method using the child class' desired method (eg. on a new thread).</summary>
        Protected MustOverride Sub StartRunning()
        '''<summary>Consumes an item.</summary>
        Protected MustOverride Sub Consume(ByVal item As T)
        ''' <summary>Runs queued calls until there are none left.</summary>
        Protected Sub Run()
            Do Until TryReleaseConsumer()
                Consume(queue.Dequeue())
            Loop
        End Sub
    End Class
End Namespace
