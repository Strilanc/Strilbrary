Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    <ContractClass(GetType(IClock.ContractClass))>
    Public Interface IClock
        ''' <summary>
        ''' Returns a future which becomes ready after the given amount of time has passed on the clock.
        ''' The resulting future is instantly ready if the given time is non-positive.
        ''' </summary>
        Function AsyncWait(ByVal dt As TimeSpan) As IFuture
        ''' <summary>
        ''' Returns a timer, which can report the time elapsed on the clock since its creation.
        ''' </summary>
        <Pure()>
        Function StartTimer() As ITimer

        <ContractClassFor(GetType(IClock))>
        NotInheritable Class ContractClass
            Implements IClock
            Public Function AsyncWait(ByVal dt As TimeSpan) As Threading.IFuture Implements IClock.AsyncWait
                Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
                Contract.Ensures(dt.Ticks > 0 OrElse Contract.Result(Of IFuture)().State = FutureState.Succeeded)
                Throw New NotSupportedException
            End Function
            <Pure()>
            Public Function StartTimer() As ITimer Implements IClock.StartTimer
                Contract.Ensures(Contract.Result(Of ITimer)() IsNot Nothing)
                Throw New NotSupportedException
            End Function
        End Class
    End Interface

    <ContractClass(GetType(ITimer.ContractClass))>
    Public Interface ITimer
        ''' <summary>
        ''' Returns the time elapsed on the parent clock since the timer's creation.
        ''' </summary>
        <Pure()>
        Function ElapsedTime() As TimeSpan

        <ContractClassFor(GetType(ITimer))>
        NotInheritable Class ContractClass
            Implements ITimer
            <Pure()>
            Public Function ElapsedTime() As TimeSpan Implements ITimer.ElapsedTime
                Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks >= 0)
                Throw New NotSupportedException
            End Function
        End Class
    End Interface
End Namespace
