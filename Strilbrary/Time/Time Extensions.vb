Imports Strilbrary.Threading
Imports Strilbrary.Misc

Namespace Time
    Public Module TimeExtensions
        '''<summary>
        ''' Returns a clock which advances relative to the given clock.
        ''' The new clock's time zero occurs at the given clock's current time.</summary>
        <Extension()> <Pure()>
        Public Function Restarted(this As IClock) As RelativeClock
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of RelativeClock)() IsNot Nothing)
            Return New RelativeClock(parentClock:=this, timeOffsetFromParent:=-this.ElapsedTime)
        End Function

        ''' <summary>
        ''' Returns a task which completes after the given amount of time has passed on the clock.
        ''' The resulting task is instantly ready if the given time is non-positive.
        ''' </summary>
        <Extension()>
        Public Function AsyncWait(clock As IClock, dt As TimeSpan) As Task
            Contract.Requires(clock IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return clock.AsyncWaitUntil(clock.ElapsedTime + dt)
        End Function

        ''' <summary>
        ''' Begins periodically calling an action, and returns an IDisposable to end the repetition.
        ''' The first call happens after the period has elapsed (instead of immediately).
        ''' The start times will not drift relative to the clock over time.
        ''' The duration of the action does not affect the period or start times.
        ''' The action may be started again while it is still running if it fails to complete within the period.
        ''' </summary>
        ''' <remarks>Beware repeating an action faster than the time it takes to complete.</remarks>
        <Extension()>
        Public Function AsyncRepeat(clock As IClock,
                                    period As TimeSpan,
                                    action As Action) As IDisposable
            Contract.Requires(clock IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IDisposable)() IsNot Nothing)

            Dim cts = New CancellationTokenSource()
            Dim t = clock.ElapsedTime
            Call Async Sub()
                     Do
                         t += period
                         Await clock.AsyncWaitUntil(t)
                         If cts.Token.IsCancellationRequested Then Return
                         Call action()
                     Loop
                 End Sub
            Return New DelegatedDisposable(Sub() cts.Cancel())
        End Function

#Region "Time Spans"
        'verification disabled due to lack of TimeSpan contracts
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute")>
        Public Function Minutes(quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerMinute)
        End Function
        'verification disabled due to lack of TimeSpan contracts
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond")>
        Public Function Seconds(quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerSecond)
        End Function
        'verification disabled due to lack of TimeSpan contracts
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond")>
        Public Function Milliseconds(quantity As Long) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond)
            Return New TimeSpan(ticks:=quantity * TimeSpan.TicksPerMillisecond)
        End Function

        <Extension()> <Pure()>
        Public Function Minutes(quantity As Int32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return CLng(quantity).Minutes
        End Function
        <Extension()> <Pure()>
        Public Function Seconds(quantity As Int32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return CLng(quantity).Seconds
        End Function
        <Extension()> <Pure()>
        Public Function Milliseconds(quantity As Int32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond)
            Return CLng(quantity).Milliseconds
        End Function

        <Extension()> <Pure()>
        Public Function Minutes(quantity As UInt32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMinute)
            Return CLng(quantity).Minutes
        End Function
        <Extension()> <Pure()>
        Public Function Seconds(quantity As UInt32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerSecond)
            Return CLng(quantity).Seconds
        End Function
        <Extension()> <Pure()>
        Public Function Milliseconds(quantity As UInt32) As TimeSpan
            Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks = quantity * TimeSpan.TicksPerMillisecond)
            Return CLng(quantity).Milliseconds
        End Function

        <Extension()> <Pure()>
        Public Function Minutes(quantity As Double) As TimeSpan
            Return New TimeSpan(CLng(quantity * TimeSpan.TicksPerMinute))
        End Function
        <Extension()> <Pure()>
        Public Function Seconds(quantity As Double) As TimeSpan
            Return New TimeSpan(CLng(quantity * TimeSpan.TicksPerSecond))
        End Function
        <Extension()> <Pure()>
        Public Function Milliseconds(quantity As Double) As TimeSpan
            Return New TimeSpan(CLng(quantity * TimeSpan.TicksPerMillisecond))
        End Function
#End Region
    End Module
End Namespace
