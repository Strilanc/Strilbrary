Imports Strilbrary.Threading
Imports Strilbrary.Values
Imports Strilbrary.Collections

Namespace Time
    Public NotInheritable Class ClockTimer
        Public ReadOnly Clock As IClock
        Public ReadOnly StartTime As Moment

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(Clock IsNot Nothing)
        End Sub

        Public Sub New(clock As IClock, startTime As Moment)
            Contract.Requires(clock IsNot Nothing)
            Contract.Requires(Object.Equals(startTime.Basis, clock))
            Me.Clock = clock
            Me.StartTime = startTime
        End Sub

        Public Function ElapsedTime() As TimeSpan
            Return Clock.Time() - StartTime
        End Function

        Public Function At(targetElapsedTime As TimeSpan) As Task
            Return Clock.At(StartTime + targetElapsedTime)
        End Function

        Public Function Delay(duration As TimeSpan) As Task
            Return Clock.At(Clock.Time() + duration)
        End Function

        Public Function Restarted() As ClockTimer
            Return Clock.StartTimer()
        End Function
    End Class
End Namespace
