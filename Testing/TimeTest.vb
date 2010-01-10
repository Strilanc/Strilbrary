Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Time
Imports Strilbrary.Threading
Imports StrilbraryTests.IFutureExtensionsTest

<TestClass()>
Public Class TimeTest
    <TestMethod()>
    Public Sub TimeExtensionTest()
        Assert.IsTrue(1L.Minutes.Ticks = 1 * TimeSpan.TicksPerMinute)
        Assert.IsTrue(2L.Seconds.Ticks = 2 * TimeSpan.TicksPerSecond)
        Assert.IsTrue(3L.Milliseconds.Ticks = 3 * TimeSpan.TicksPerMillisecond)
        Assert.IsTrue(4.Minutes.Ticks = 4 * TimeSpan.TicksPerMinute)
        Assert.IsTrue(5.Seconds.Ticks = 5 * TimeSpan.TicksPerSecond)
        Assert.IsTrue(6.Milliseconds.Ticks = 6 * TimeSpan.TicksPerMillisecond)
        Assert.IsTrue(7UI.Minutes.Ticks = 7 * TimeSpan.TicksPerMinute)
        Assert.IsTrue(8UI.Seconds.Ticks = 8 * TimeSpan.TicksPerSecond)
        Assert.IsTrue(9UI.Milliseconds.Ticks = 9 * TimeSpan.TicksPerMillisecond)
    End Sub

    <TestMethod()>
    Public Sub ManualTimeTest()
        Dim c = New ManualClock()
        Assert.IsTrue(c.Time = 0.Seconds)
        c.Advance(5.Seconds)
        Assert.IsTrue(c.Time = 5.Seconds)
        c.Advance(4.Seconds)
        Assert.IsTrue(c.Time = 9.Seconds)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitTest_Positive()
        Dim c = New ManualClock()
        Dim f = c.AsyncWait(3.Seconds)
        Assert.IsTrue(f.State = FutureState.Unknown)
        c.Advance(2.Seconds)
        Assert.IsTrue(f.State = FutureState.Unknown)
        c.Advance(2.Seconds)
        Assert.IsTrue(f.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitTest_Multiple()
        Dim c = New ManualClock()
        Dim f2 = c.AsyncWait(2.Seconds)
        Dim f1 = c.AsyncWait(1.Seconds)
        Dim f3 = c.AsyncWait(3.Seconds)
        c.Advance(500.Milliseconds)
        Assert.IsTrue(f1.State = FutureState.Unknown)
        Assert.IsTrue(f2.State = FutureState.Unknown)
        Assert.IsTrue(f3.State = FutureState.Unknown)
        c.Advance(1.Seconds)
        Assert.IsTrue(f1.State = FutureState.Succeeded)
        Assert.IsTrue(f2.State = FutureState.Unknown)
        Assert.IsTrue(f3.State = FutureState.Unknown)
        c.Advance(1.Seconds)
        Assert.IsTrue(f2.State = FutureState.Succeeded)
        Assert.IsTrue(f3.State = FutureState.Unknown)
        c.Advance(1.Seconds)
        Assert.IsTrue(f3.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitTest_Instant()
        Dim c = New ManualClock()
        Dim f = c.AsyncWait(-1.Seconds)
        Assert.IsTrue(f.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub ManualTimerTest()
        Dim c = New ManualClock()
        c.Advance(5.Seconds)
        Dim t = c.StartTimer()
        Assert.IsTrue(t.ElapsedTime = 0.Seconds)
        c.Advance(3.Seconds)
        Assert.IsTrue(t.ElapsedTime = 3.Seconds)
        c.Advance(2.Seconds)
        Assert.IsTrue(t.Reset = 5.Seconds)
        Assert.IsTrue(t.ElapsedTime = 0.Seconds)
        Assert.IsTrue(t.Reset = 0.Seconds)
        c.Advance(2.Seconds)
        Assert.IsTrue(t.ElapsedTime = 2.Seconds)
    End Sub

    <TestMethod()>
    Public Sub SystemAsyncWaitTest_Positive()
        Dim c = New SystemClock()
        Dim f = c.AsyncWait(2.Seconds)
        BlockOnFuture(f, timeout:=1.Seconds)
        Assert.IsTrue(f.State = FutureState.Unknown)
        BlockOnFuture(f, timeout:=2.Seconds)
        Assert.IsTrue(f.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub SystemAsyncWaitTest_Instant()
        Dim c = New ManualClock()
        Dim f = c.AsyncWait(-1.Seconds)
        Assert.IsTrue(f.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub SystemTimerTest()
        Dim c = New SystemClock()
        Dim t = c.StartTimer()
        Threading.Thread.Sleep(50)
        Dim m = t.ElapsedTime
        Assert.IsTrue(m > 0.Milliseconds)
        Assert.IsTrue(t.Reset >= m)
        Assert.IsTrue(t.ElapsedTime >= 0.Milliseconds)
    End Sub
End Class
