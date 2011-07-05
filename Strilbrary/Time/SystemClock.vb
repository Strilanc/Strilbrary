Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' A clock which advances relative to the system time.
    ''' </summary>
    Public NotInheritable Class SystemClock
        Implements IClock
        Private ReadOnly _timer As Stopwatch

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_timer IsNot Nothing)
        End Sub

        Public Sub New()
            Me._timer = Stopwatch.StartNew()
        End Sub

        Public Function AsyncWaitUntil(time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
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
                Return _timer.Elapsed
            End Get
        End Property
    End Class
End Namespace
