Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    <ContractClass(GetType(IClock.ContractClass))>
    Public Interface IClock
        ''' <summary>
        ''' Returns a task which completes once the given time has been reached by the clock.
        ''' The resulting task is instantly ready if the given time has already been reached.
        ''' </summary>
        Function AsyncWaitUntil(ByVal time As TimeSpan) As task

        ''' <summary>
        ''' Determines the time elapsed on the clock since it was started.
        ''' </summary>
        ReadOnly Property ElapsedTime() As TimeSpan

        <ContractClassFor(GetType(IClock))>
        NotInheritable Class ContractClass
            Implements IClock
            Public Function AsyncWaitUntil(ByVal time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
                Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
                Throw New NotSupportedException
            End Function
            Public ReadOnly Property ElapsedTime() As TimeSpan Implements IClock.ElapsedTime
                Get
                    Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks >= 0)
                    Throw New NotSupportedException
                End Get
            End Property
        End Class
    End Interface
End Namespace
