Imports Strilbrary.Values
Imports Strilbrary.Exceptions

Namespace Threading
    '''<summary>Runs queued actions in order (without overlap) within a synchronization context, exposing the results as tasks.</summary>
    ''' <remarks>
    ''' Ensures that enqueued items are consumed by ensuring at all exit points that either:
    ''' - the queue is empty
    ''' - or exactly one consumer exists
    ''' - or another exit point will be hit [by another thread]
    ''' </remarks>
    <DebuggerDisplay("{ToString()}")>
    Public NotInheritable Class CallQueue
        Inherits SynchronizationContext

        Private ReadOnly queue As New SingleConsumerLockFreeQueue(Of Action)
        Private ReadOnly context As SynchronizationContext
        Private running As Integer 'stores consumer state and is used as a semaphore

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(queue IsNot Nothing)
            Contract.Invariant(context IsNot Nothing)
        End Sub

        ''' <param name="context">The synchronization context queued actions will run on.</param>
        Public Sub New(context As SynchronizationContext)
            Contract.Requires(context IsNot Nothing)
            Me.context = context
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
            If Not TryAcquireConsumer() Then Return
            context.Post(Sub()
                             SynchronizationContext.SetSynchronizationContext(Me)
                             Do Until TryReleaseConsumer()
                                 queue.Dequeue().Invoke()
                             Loop
                         End Sub, Nothing)
        End Sub

        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            queue.BeginEnqueue(Sub() d(state))
            TryBeginConsuming()
        End Sub
        Public Sub PostMany(actions As IEnumerable(Of Action))
            Contract.Requires(actions IsNot Nothing)
            queue.BeginEnqueue(actions)
            TryBeginConsuming()
        End Sub
        Public Overrides Function CreateCopy() As SynchronizationContext
            Dim c = context.CreateCopy()
            Contract.Assume(c IsNot Nothing)
            Return New CallQueue(c)
        End Function

        Public Overrides Function ToString() As String
            Return "Context: {0}, Items: {1}".Frmt(context, queue)
        End Function
    End Class
End Namespace
