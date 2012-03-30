Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>Used to track and react to a monotonically increasing measurement of time.</summary>
    <ContractClass(GetType(ITimer.ContractClass))>
    Public Interface ITimer
        ''' <summary>
        ''' Returns a task that completes once the timer's elapsed time is at least the given time.
        ''' The resulting task is instantly ready if the given time has already been reached.
        ''' </summary>
        Function At(time As TimeSpan) As Task

        ''' <summary>
        ''' Returns the timer's elapsed time.
        ''' The result never decreases but may be volatile (increasing between calls).
        ''' </summary>
        ReadOnly Property Time() As TimeSpan

        <ContractClassFor(GetType(ITimer))>
        MustInherit Class ContractClass
            Implements ITimer
            Public Function At(time As TimeSpan) As Task Implements ITimer.At
                Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
                Throw New NotSupportedException
            End Function
            Public ReadOnly Property Time() As TimeSpan Implements ITimer.Time
                Get
                    Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks >= 0)
                    Throw New NotSupportedException
                End Get
            End Property
        End Class
    End Interface
End Namespace
