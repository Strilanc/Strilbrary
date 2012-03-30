Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>
    ''' An <see cref="ITimer" /> that advances relative to a backing timer, but drops any unexpectedly large jumps in the elapsed time.
    ''' For example, if the system hibernates or the program is paused in a debugger and the backing timer advances in real time, those periods will not be counted in the elapsed time.
    ''' </summary>
    ''' <remarks>
    ''' Uses a periodic callback to detect skips.
    ''' Uses a weak reference to allow garbage collection despite the periodic callbacks.
    ''' </remarks>
    Public NotInheritable Class PauseSkippingTimer
        Implements ITimer

        Private Shared ReadOnly PausePeriod As TimeSpan = 5.Seconds
        Private Shared ReadOnly TickPeriod As TimeSpan = 3.Seconds

        Private ReadOnly _lock As New Object()
        Private ReadOnly _backingTimer As ITimer
        Private _lastBackingElapsedTime As TimeSpan
        Private _lostTime As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_lock IsNot Nothing)
            Contract.Invariant(_backingTimer IsNot Nothing)
            Contract.Invariant(_lostTime.Ticks >= 0)
            Contract.Invariant(_lastBackingElapsedTime.Ticks >= _lostTime.Ticks)
        End Sub

        Public Sub New(backingTimer As ITimer)
            Contract.Requires(backingTimer IsNot Nothing)
            Me._backingTimer = backingTimer
            Me._lastBackingElapsedTime = backingTimer.Time
            Me._lostTime = 0.Seconds
            PeriodicPokeElapsedTime(New WeakReference(Me))
        End Sub
        ''' <summary>Periodically pokes the clock, allowing pauses to be detected by the lack of pokes.</summary>
        Private Shared Async Sub PeriodicPokeElapsedTime(weakMe As WeakReference)
            Do
                'check if we've been collected
                Dim [me] = DirectCast(weakMe.Target, PauseSkippingTimer)
                If [me] Is Nothing Then Exit Do

                'poke elapsed time to indicate program is not paused
                [me].PeekPokeElapsedTime()

                'wait another period, being careful to allow garbage collection
                Dim c = [me]._backingTimer
                [me] = Nothing
                Await c.Delay(TickPeriod)
            Loop
        End Sub

        ''' <summary>
        ''' Notes the duration since the last elapsed time on the backing clock, dropping too-large intervals as likely program pauses.
        ''' Returns the new elapsed time, after adjusting for pauses.
        ''' Result increases monotonically.
        ''' </summary>
        Private Function PeekPokeElapsedTime() As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks >= 0)
            SyncLock _lock
                Dim t = _backingTimer.Time
                Dim dt = t - _lastBackingElapsedTime

                _lastBackingElapsedTime += dt
                If dt > PausePeriod Then _lostTime += dt

                Contract.Assume(_lostTime.Ticks >= 0)
                Contract.Assume(_lastBackingElapsedTime.Ticks >= _lostTime.Ticks)
                Contract.Assume((t - _lostTime).Ticks >= 0)
                Return t - _lostTime
            End SyncLock
        End Function

        Public ReadOnly Property Time() As TimeSpan Implements ITimer.Time
            Get
                Return PeekPokeElapsedTime()
            End Get
        End Property

        <SuppressMessage("Microsoft.Contracts", "Ensures-Contract.Result(Of Task)() IsNot Nothing")>
        Public Async Function At(time As TimeSpan) As Task Implements ITimer.At
            'There may be a skipped pause on the backing timer during the wait, meaning we have to wait longer
            'It might happen again and again, thus we need to loop-wait until the target time is actually reached
            While PeekPokeElapsedTime() < time
                'atomic read
                Dim lostTime As TimeSpan
                SyncLock _lock
                    lostTime = _lostTime
                End SyncLock

                Await _backingTimer.At(time + lostTime)
            End While
        End Function
    End Class
End Namespace
