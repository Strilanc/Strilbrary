Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>
    ''' A clock that advances relative to a backing clock, but drops any large period skips it detects
    ''' (such as the skip on a clock advancing in real time due to a debugger pause).
    ''' </summary>
    ''' <remarks>
    ''' Uses a periodic callback to detect skips.
    ''' Uses a weak reference to allow garbage collection despite the periodic callbacks.
    ''' </remarks>
    Public NotInheritable Class PauseSkippingClock
        Implements IClock

        Private Shared ReadOnly PausePeriod As TimeSpan = 5.Seconds
        Private Shared ReadOnly TickPeriod As TimeSpan = 3.Seconds

        Private ReadOnly _lock As New Object()
        Private ReadOnly _backingClock As IClock
        Private _lastElapsedTime As TimeSpan
        Private _lostTime As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_lock IsNot Nothing)
            Contract.Invariant(_backingClock IsNot Nothing)
            Contract.Invariant(_lostTime.Ticks >= 0)
            Contract.Invariant(_lastElapsedTime.Ticks >= _lostTime.Ticks)
        End Sub

        Public Sub New(backingClock As IClock)
            Contract.Requires(backingClock IsNot Nothing)
            Me._backingClock = backingClock
            Me._lastElapsedTime = backingClock.ElapsedTime
            Me._lostTime = 0.Seconds
            PeriodicPokeElapsedTime(New WeakReference(Me))
        End Sub
        ''' <summary>Periodically pokes the clock, allowing pauses to be detected by the lake of pokes.</summary>
        Private Shared Async Sub PeriodicPokeElapsedTime(weakMe As WeakReference)
            Do
                'check if we've been collected
                Dim [me] = DirectCast(weakMe.Target, PauseSkippingClock)
                If [me] Is Nothing Then Exit Do

                'poke elapsed time to indicate program is not paused
                [me].PeekPokeElapsedTime()

                'wait another period, being careful to allow garbage collection
                Dim c = [me]._backingClock
                [me] = Nothing
                Await c.AsyncWait(TickPeriod)
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
                Dim t = _backingClock.ElapsedTime
                Dim dt = t - _lastElapsedTime

                _lastElapsedTime += dt
                If dt > PausePeriod Then _lostTime += dt

                Contract.Assume(_lostTime.Ticks >= 0)
                Contract.Assume(_lastElapsedTime.Ticks >= _lostTime.Ticks)
                Contract.Assume((t - _lostTime).Ticks >= 0)
                Return t - _lostTime
            End SyncLock
        End Function

        Public ReadOnly Property GetElapsedTime() As TimeSpan Implements IClock.ElapsedTime
            Get
                Return PeekPokeElapsedTime()
            End Get
        End Property

        <SuppressMessage("Microsoft.Contracts", "Ensures-Contract.Result(Of Task)() IsNot Nothing")>
        Public Async Function AsyncWaitUntil(time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            '_lostTime can increase during waits, necessitating more waiting, so repeatedly wait until we reach the target time
            While PeekPokeElapsedTime() < time
                'atomic read
                Dim lostTime As TimeSpan
                SyncLock _lock
                    lostTime = _lostTime
                End SyncLock

                Await _backingClock.AsyncWaitUntil(time + lostTime)
            End While
        End Function
    End Class
End Namespace
