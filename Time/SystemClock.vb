Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    Public Class SystemClock
        Implements IClock

        Public Function AsyncWait(ByVal dt As TimeSpan) As IFuture Implements IClock.AsyncWait
            Dim result = New FutureAction
            If dt.Ticks <= 0 Then
                result.SetSucceeded()
            Else
                Dim timer = New Timers.Timer(dt.TotalMilliseconds)
                AddHandler timer.Elapsed, Sub()
                                              timer.Dispose()
                                              result.SetSucceeded()
                                          End Sub
                timer.AutoReset = False
                timer.Start()
            End If
            Return result
        End Function

        <Pure()>
        Public Function StartTimer() As ITimer Implements IClock.StartTimer
            Return New SystemTimer()
        End Function

        Private Class SystemTimer
            Implements ITimer
            Private ReadOnly _startTime As ModInt32
            Public Sub New()
                _startTime = Environment.TickCount
            End Sub
            Public Function ElapsedTime() As TimeSpan Implements ITimer.ElapsedTime
                Return CUInt(Environment.TickCount - _startTime).Milliseconds
            End Function
        End Class
    End Class
End Namespace
