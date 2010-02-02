Namespace Time
    Public Module TimeExtensions
        '''<summary>Returns clock which advances relative to the given clock, starting at the current time.</summary>
        <Extension()>
        Public Function AfterReset(ByVal this As IClock) As RelativeClock
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of RelativeClock)() IsNot Nothing)
            Return New RelativeClock(parentClock:=this, startingtime:=this.ElapsedTime)
        End Function

        'verification disabled due to lack of TimeSpan contracts
        <ContractVerification(False)>
        <Extension()> <Pure()>
        Public Function Minutes(ByVal quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerMinute)
        End Function
        'verification disabled due to lack of TimeSpan contracts
        <ContractVerification(False)>
        <Extension()> <Pure()>
        Public Function Seconds(ByVal quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerSecond)
        End Function
        'verification disabled due to lack of TimeSpan contracts
        <ContractVerification(False)>
        <Extension()> <Pure()>
        Public Function Milliseconds(ByVal quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerMillisecond)
        End Function

        <Extension()> <Pure()>
        Public Function Minutes(ByVal quantity As Int32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return CLng(quantity).Minutes
        End Function
        <Extension()> <Pure()>
        Public Function Seconds(ByVal quantity As Int32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return CLng(quantity).Seconds
        End Function
        <Extension()> <Pure()>
        Public Function Milliseconds(ByVal quantity As Int32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond)
            Return CLng(quantity).Milliseconds
        End Function

        <Extension()> <Pure()>
        Public Function Minutes(ByVal quantity As UInt32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return CLng(quantity).Minutes
        End Function
        <Extension()> <Pure()>
        Public Function Seconds(ByVal quantity As UInt32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return CLng(quantity).Seconds
        End Function
        <Extension()> <Pure()>
        Public Function Milliseconds(ByVal quantity As UInt32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond)
            Return CLng(quantity).Milliseconds
        End Function
    End Module
End Namespace
