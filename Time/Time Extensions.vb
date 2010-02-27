Imports Strilbrary.Threading
Imports Strilbrary.Misc

Namespace Time
    Public Module TimeExtensions
        '''<summary>Returns a clock which advances relative to the given clock, starting at the current time.</summary>
        <Extension()> <Pure()>
        Public Function Restarted(ByVal this As IClock) As RelativeClock
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of RelativeClock)() IsNot Nothing)
            Return New RelativeClock(parentClock:=this, startingTime:=this.ElapsedTime)
        End Function

        ''' <summary>
        ''' Begins periodically calling an action, and returns an IDisposable to end the repitition.
        ''' The first call happens after the period has elapsed (instead of immediately).
        ''' </summary>
        <Extension()>
        Public Function AsyncRepeat(ByVal clock As IClock,
                                    ByVal period As TimeSpan,
                                    ByVal action As action) As IDisposable
            Contract.Requires(clock IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IDisposable)() IsNot Nothing)

            Dim stopFlag As Boolean
            Dim callback As Action
            callback = Sub()
                           If stopFlag Then Return
                           Call action()
                           clock.AsyncWait(period).CallOnSuccess(callback)
                       End Sub
            clock.AsyncWait(period).CallOnSuccess(callback)
            Return New DelegatedDisposable(Sub() stopFlag = True)
        End Function

#Region "Time Spans"
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

        <Extension()> <Pure()>
        Public Function Minutes(ByVal quantity As Double) As TimeSpan
            Return New TimeSpan(CLng(quantity * TimeSpan.TicksPerMinute))
        End Function
        <Extension()> <Pure()>
        Public Function Seconds(ByVal quantity As Double) As TimeSpan
            Return New TimeSpan(CLng(quantity * TimeSpan.TicksPerSecond))
        End Function
        <Extension()> <Pure()>
        Public Function Milliseconds(ByVal quantity As Double) As TimeSpan
            Return New TimeSpan(CLng(quantity * TimeSpan.TicksPerMillisecond))
        End Function
#End Region
    End Module
End Namespace
