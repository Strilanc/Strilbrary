<ContractClass(GetType(IClock.ContractClass))>
Public Interface IClock
    ''' <summary>
    ''' Returns a task that completes once the clock's time is at least the given time.
    ''' The resulting task is instantly ready if the given time has already been reached.
    ''' </summary>
    Function At(time As Moment) As Task
    ''' <summary>
    ''' Returns the clock's time.
    ''' The result is volatile but never decreases (may increase between calls).
    ''' </summary>
    Function Time() As Moment

    <ContractClassFor(GetType(IClock))>
    MustInherit Class ContractClass
        Implements IClock
        Public Function At(time As Moment) As Task Implements IClock.At
            Contract.Requires(Object.Equals(Me, time.Basis))
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Throw New NotSupportedException
        End Function
        Public Function Time() As Moment Implements IClock.Time
            Contract.Ensures(Object.Equals(Me, Contract.Result(Of Moment)().Basis))
            Throw New NotSupportedException
        End Function
    End Class
End Interface
