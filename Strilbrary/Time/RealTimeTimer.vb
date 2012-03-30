Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' An <see cref="ITimer" /> that advances relative to actual real physical time.
    ''' </summary>
    Public NotInheritable Class RealTimeTimer
        Implements ITimer
        Private ReadOnly _timer As Stopwatch

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_timer IsNot Nothing)
        End Sub

        Public Sub New()
            Me._timer = Stopwatch.StartNew()
            Contract.Assume(Me._timer IsNot Nothing)
        End Sub

        Public Function At(time As TimeSpan) As Task Implements ITimer.At
            Dim dt = time - Time
            If dt.Ticks <= 0 Then Return CompletedTask()
            Dim r = Task.Delay(dt)
            Contract.Assume(r IsNot Nothing)
            Return r
        End Function

        Public ReadOnly Property Time As TimeSpan Implements ITimer.Time
            Get
                Dim r = _timer.Elapsed
                Contract.Assume(r.Ticks >= 0)
                Return r
            End Get
        End Property
    End Class
End Namespace
