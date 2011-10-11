Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' A clock which advances relative to actual real physical time.
    ''' </summary>
    Public NotInheritable Class PhysicalClock
        Implements IClock
        Private ReadOnly _timer As Stopwatch

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_timer IsNot Nothing)
        End Sub

        Public Sub New()
            Me._timer = Stopwatch.StartNew()
            Contract.Assume(Me._timer IsNot Nothing)
        End Sub

        Public Function AsyncWaitUntil(time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Dim dt = time - ElapsedTime
            If dt.Ticks <= 0 Then Return CompletedTask()
            Dim r = TaskEx.Delay(dt)
            Contract.Assume(r IsNot Nothing)
            Return r
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            Get
                Dim r = _timer.Elapsed
                Contract.Assume(r.Ticks >= 0)
                Return r
            End Get
        End Property
    End Class
End Namespace
