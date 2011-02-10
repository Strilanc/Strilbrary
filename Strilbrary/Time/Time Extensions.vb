Imports Strilbrary.Threading
Imports Strilbrary.Misc

Namespace Time
    Public Module TimeExtensions
        '''<summary>Returns a clock which advances relative to the given clock, starting at the current time.</summary>
        <Extension()> <Pure()>
        Public Function Restarted(ByVal this As IClock) As RelativeClock
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of RelativeClock)() IsNot Nothing)
            Return New RelativeClock(parentClock:=this, timeOffsetFromParent:=-this.ElapsedTime)
        End Function

        ''' <summary>
        ''' Returns a task which completes after the given amount of time has passed on the clock.
        ''' The resulting task is instantly ready if the given time is non-positive.
        ''' </summary>
        <Extension()>
        Public Function AsyncWait(ByVal clock As IClock, ByVal dt As TimeSpan) As Task
            Contract.Requires(clock IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return clock.AsyncWaitUntil(clock.ElapsedTime + dt)
        End Function

        ''' <summary>
        ''' Begins periodically calling an action, and returns an IDisposable to end the repetition.
        ''' The first call happens after the period has elapsed (instead of immediately).
        ''' The period is from start time to start time, not end time to start time.
        ''' The action may be started again while it is running.
        ''' Beware repeating an action faster than the time it takes to complete.
        ''' </summary>
        <Extension()>
        Public Function AsyncRepeat(ByVal clock As IClock,
                                    ByVal period As TimeSpan,
                                    ByVal action As Action) As IDisposable
            Contract.Requires(clock IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IDisposable)() IsNot Nothing)

            Dim stopFlag = False
            Dim callback As Action
            Dim t = clock.ElapsedTime
            callback = Sub()
                           If stopFlag Then Return
                           t += period
                           clock.AsyncWaitUntil(t).ContinueWithAction(callback)
                           Call action()
                       End Sub
            t += period
            clock.AsyncWaitUntil(t).ContinueWithAction(callback)
            Return New DelegatedDisposable(Sub() stopFlag = True)
        End Function

#Region "Time Spans"
        'verification disabled due to lack of TimeSpan contracts
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute")>
        Public Function Minutes(ByVal quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerMinute)
        End Function
        'verification disabled due to lack of TimeSpan contracts
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond")>
        Public Function Seconds(ByVal quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerSecond)
        End Function
        'verification disabled due to lack of TimeSpan contracts
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond")>
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
