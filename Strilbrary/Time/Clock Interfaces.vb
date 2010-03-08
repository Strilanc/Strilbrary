Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    <ContractClass(GetType(IClock.ContractClass))>
    Public Interface IClock
        ''' <summary>
        ''' Returns a future which becomes ready after the given amount of time has passed on the clock.
        ''' The resulting future is instantly ready if the given time is non-positive.
        ''' </summary>
        Function AsyncWait(ByVal dt As TimeSpan) As Task

        ''' <summary>
        ''' Determines the time elapsed on the clock since it was started.
        ''' </summary>
        ReadOnly Property ElapsedTime() As TimeSpan

        <ContractClassFor(GetType(IClock))>
        NotInheritable Class ContractClass
            Implements IClock
            Public Function AsyncWait(ByVal dt As TimeSpan) As Task Implements IClock.AsyncWait
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
