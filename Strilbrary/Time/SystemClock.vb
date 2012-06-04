Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' An <see cref="IClock"/> that advances relative to actual real physical time.
    ''' </summary>
    Public NotInheritable Class SystemClock
        Implements IClock
        Private ReadOnly _stopWatch As Stopwatch

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_stopWatch IsNot Nothing)
        End Sub

        Public Sub New()
            Me._stopWatch = Stopwatch.StartNew()
            Contract.Assume(Me._stopWatch IsNot Nothing)
        End Sub

        Public Function At(time As Moment) As Task Implements IClock.At
            Dim dt = time - Me.Time
            If dt.Ticks <= 0 Then Return CompletedTask()
            Dim r = Task.Delay(dt)
            Contract.Assume(r IsNot Nothing)
            Return r
        End Function

        Public Function Time() As Moment Implements IClock.Time
            Return New Moment(_stopWatch.Elapsed.Ticks, Me)
        End Function
    End Class
End Namespace
