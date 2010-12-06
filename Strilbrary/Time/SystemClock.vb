Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' A clock which advances relative to the system tick count.
    ''' </summary>
    Public Class SystemClock
        Implements IClock
        Private _elapsedTime As TimeSpan
        Private _lastTick As ModInt32
        Private ReadOnly _lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_lock IsNot Nothing)
            Contract.Invariant(_elapsedTime.Ticks >= 0)
        End Sub

        Public Sub New()
            Me._lastTick = Environment.TickCount
            Me._elapsedTime = New TimeSpan(0)
            Contract.Assume(Me._elapsedTime.Ticks >= 0)
        End Sub

        Public Function AsyncWaitUntil(ByVal time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Dim dt = time - ElapsedTime
            If dt.Ticks <= 0 Then Return CompletedTask()

            Dim result = New TaskCompletionSource(Of NoValue)
            Dim timer = New Timers.Timer(dt.TotalMilliseconds)
            AddHandler timer.Elapsed, Sub()
                                          timer.Dispose()
                                          result.SetResult(Nothing)
                                      End Sub
            timer.AutoReset = False
            timer.Start()
            Contract.Assume(result.Task IsNot Nothing)
            Return result.Task
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            Get
                SyncLock _lock
                    Dim tick = Environment.TickCount
                    _elapsedTime += (tick - _lastTick).UnsignedValue.Milliseconds
                    _lastTick = tick
                    Contract.Assume(_elapsedTime.Ticks >= 0)
                    Return _elapsedTime
                End SyncLock
            End Get
        End Property
    End Class
End Namespace
