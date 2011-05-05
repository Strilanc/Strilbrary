Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>A clock that advances while the program is running, but does not advance while execution is halted (eg. at debugger breakpoints).</summary>
    Public Class ProgramClock
        Implements IClock

        '''<summary>Used to check for pauses. Shared because it uses a single periodic callback instead of one-per-instance.</summary>
        Private Class SharedBackingClock
            Private Shared ReadOnly PausePeriod As TimeSpan = 5.Seconds
            Private Shared ReadOnly TickPeriod As TimeSpan = 3.Seconds

            '''<summary>Checked periodically to catch overly long periods.</summary>
            '''<remarks>Stored as a weak reference to allow cleanup when there are no ProgramClock instances justifying the periodic timer usage.</remarks>
            Private Shared _backClock As WeakReference
            Private Shared _lastElapsedTime As TimeSpan
            Private Shared _lostTime As TimeSpan
            Private Shared ReadOnly _lock As New Object()

            Private Sub New()
            End Sub

            Public Shared Function GetElapsedTime() As TimeSpan
                Dim t = PokeElapsedTime(scheduleNextPoke:=False)
                If Not t.HasValue Then Throw New Exceptions.InvalidStateException("Attempted to get elapsed time without a backing clock.")
                Return t.Value
            End Function
            Private Shared Function PokeElapsedTime(scheduleNextPoke As Boolean) As TimeSpan?
                SyncLock _lock
                    Dim clock = DirectCast(_backClock.Target, IClock)
                    If clock Is Nothing Then Return Nothing

                    Dim t = clock.ElapsedTime
                    Dim dt = t - _lastElapsedTime
                    _lastElapsedTime = t

                    If dt > PausePeriod Then _lostTime += dt
                    If scheduleNextPoke Then
                        clock.AsyncWait(TickPeriod).ContinueWithAction(Sub() PokeElapsedTime(scheduleNextPoke:=True))
                    End If

                    Return t - _lostTime
                End SyncLock
            End Function

            Public Shared Function AsyncWaitUntil(t As TimeSpan) As Task
                Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
                SyncLock _lock
                    Dim clock = DirectCast(_backClock.Target, IClock)
                    If clock Is Nothing Then Throw New Exceptions.InvalidStateException("Attempted to wait without a backing clock.")
                    Return clock.AsyncWaitUntil(t)
                End SyncLock
            End Function

            Public Shared Function GetBackingClockReferenceToHold() As Object
                Contract.Ensures(Contract.Result(Of Object)() IsNot Nothing)
                SyncLock _lock
                    Dim clock = DirectCast(_backClock.Target, IClock)
                    If clock Is Nothing Then
                        _lostTime = 0.Seconds
                        _lastElapsedTime = 0.Seconds
                        clock = New SystemClock()
                        _backClock = New WeakReference(clock)
                        PokeElapsedTime(scheduleNextPoke:=True)
                    End If
                    Return clock
                End SyncLock
            End Function
        End Class

        <SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification:="Prevents collection of singleton backing clock.")>
        Private ReadOnly _backingClockReference As Object
        Private ReadOnly _initialElapsedTime As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_backingClockReference IsNot Nothing)
        End Sub

        Public Sub New()
            Me._backingClockReference = SharedBackingClock.GetBackingClockReferenceToHold()
            Me._initialElapsedTime = SharedBackingClock.GetElapsedTime()
        End Sub

        Public Function AsyncWaitUntil(time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Return SharedBackingClock.AsyncWaitUntil(time + _initialElapsedTime)
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            Get
                Dim t = SharedBackingClock.GetElapsedTime() - _initialElapsedTime
                Contract.Assume(t.Ticks >= 0)
                Return t
            End Get
        End Property
    End Class
End Namespace
