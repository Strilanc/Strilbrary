Imports Strilbrary.Threading
Imports Strilbrary.Values
Imports Strilbrary.Collections

Namespace Time
    Public NotInheritable Class ClockTimer
        Private ReadOnly _clock As IClock
        Private ReadOnly _startTime As Moment

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_clock IsNot Nothing)
        End Sub

        Public Sub New(clock As IClock, startTime As Moment)
            Contract.Requires(clock IsNot Nothing)
            Contract.Requires(Object.Equals(startTime.Basis, clock))
            Me._clock = clock
            Me._startTime = startTime
        End Sub

        Public Function ElapsedTime() As TimeSpan
            Return _clock.Time() - _startTime
        End Function

        Public Function At(targetElapsedTime As TimeSpan) As Task
            Return _clock.At(_startTime + targetElapsedTime)
        End Function

        Public Function Delay(duration As TimeSpan) As Task
            Return _clock.At(_clock.Time() + duration)
        End Function

        Public Function Restarted() As ClockTimer
            Return _clock.StartTimer()
        End Function
    End Class
End Namespace
