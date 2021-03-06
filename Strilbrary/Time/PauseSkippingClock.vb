﻿Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>
    ''' An <see cref="IClock" /> that advances relative to a backing Clock, but drops any unexpectedly large jumps in the elapsed time.
    ''' For example, if the system hibernates or the program is paused in a debugger and the backing Clock advances in real time, those periods will not be counted in the elapsed time.
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
        Private _lastBackingTime As Moment
        Private _lostTime As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_lock IsNot Nothing)
            Contract.Invariant(_backingClock IsNot Nothing)
            Contract.Invariant(_lostTime.Ticks >= 0)
        End Sub

        Public Sub New(backingClock As IClock)
            Contract.Requires(backingClock IsNot Nothing)
            Me._backingClock = backingClock
            Me._lastBackingTime = backingClock.Time
            Me._lostTime = 0.Seconds
            PeriodicPokeElapsedTime(New WeakReference(Me))
        End Sub
        ''' <summary>Periodically pokes the clock, allowing pauses to be detected by the lack of pokes.</summary>
        Private Shared Async Sub PeriodicPokeElapsedTime(weakMe As WeakReference)
            Do
                'check if we've been collected
                Dim [me] = DirectCast(weakMe.Target, PauseSkippingClock)
                If [me] Is Nothing Then Exit Do

                'poke elapsed time to indicate program is not paused
                [me].PeekPokeTime()

                'wait another period, being careful to allow garbage collection
                Dim c = [me]._backingClock
                [me] = Nothing
                Await c.Delay(TickPeriod)
            Loop
        End Sub

        ''' <summary>
        ''' Notes the duration since the last elapsed time on the backing clock, dropping too-large intervals as likely program pauses.
        ''' Returns the new elapsed time, after adjusting for pauses.
        ''' Result increases monotonically.
        ''' </summary>
        Private Function PeekPokeTime() As Moment
            SyncLock _lock
                Dim t = _backingClock.Time
                Dim dt = t - _lastBackingTime

                _lastBackingTime += dt
                If dt > PausePeriod Then _lostTime += dt

                Contract.Assume(_lostTime.Ticks >= 0)
                Contract.Assume((t - _lostTime).Ticks >= 0)
                Return New Moment((t - _lostTime).Ticks, Me)
            End SyncLock
        End Function

        Public Function Time() As Moment Implements IClock.Time
            Return PeekPokeTime()
        End Function

        <SuppressMessage("Microsoft.Contracts", "Ensures-Contract.Result(Of Task)() IsNot Nothing")>
        Public Async Function At(time As Moment) As Task Implements IClock.At
            'There may be a skipped pause on the backing Clock during the wait, meaning we have to wait longer
            'It might happen again and again, thus we need to loop-wait until the target time is actually reached
            While PeekPokeTime() < time
                'atomic read
                Dim lostTime As TimeSpan
                SyncLock _lock
                    lostTime = _lostTime
                End SyncLock

                Await _backingClock.At(New Moment((time + lostTime).Ticks, _backingClock))
            End While
        End Function
    End Class
End Namespace
