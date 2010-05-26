Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' A clock which advances relative to the system tick count.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
    Public Class SystemClock
        Implements IClock
        Private _elapsedTime As TimeSpan
        Private _lastTick As ModInt32
        Private ReadOnly _lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_lock IsNot Nothing)
            Contract.Invariant(_elapsedTime.Ticks >= 0)
        End Sub

        <ContractVerification(False)>
        Public Sub New()
            Me._lastTick = Environment.TickCount
        End Sub

        <ContractVerification(False)>
        Public Function AsyncWaitUntil(ByVal time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Dim result = New TaskCompletionSource(Of Boolean)
            Dim dt = time - ElapsedTime
            If dt.Ticks <= 0 Then
                result.SetResult(True)
            Else
                Dim timer = New Timers.Timer(dt.TotalMilliseconds)
                AddHandler timer.Elapsed, Sub()
                                              timer.Dispose()
                                              result.SetResult(True)
                                          End Sub
                timer.AutoReset = False
                timer.Start()
            End If
            Return result.Task
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            <ContractVerification(False)>
            Get
                SyncLock _lock
                    Dim tick = Environment.TickCount
                    _elapsedTime += (tick - _lastTick).UnsignedValue.Milliseconds
                    _lastTick = tick
                    Return _elapsedTime
                End SyncLock
            End Get
        End Property
    End Class
End Namespace
