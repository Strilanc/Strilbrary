Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Time
Imports Strilbrary.Threading
Imports StrilbraryTests.TaskExtensionsTest
Imports System.Collections.Generic

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
        Assert.IsTrue(10.0.Minutes.Ticks = 10.0 * TimeSpan.TicksPerMinute)
        Assert.IsTrue(11.0.Seconds.Ticks = 11.0 * TimeSpan.TicksPerSecond)
        Assert.IsTrue(12.0.Milliseconds.Ticks = 12.0 * TimeSpan.TicksPerMillisecond)
    End Sub

    <TestMethod()>
    Public Sub ManualTimeTest()
        Dim c = New ManualClock()
        Assert.IsTrue(c.ElapsedTime = 0.Seconds)
        c.Advance(5.Seconds)
        Assert.IsTrue(c.ElapsedTime = 5.Seconds)
        c.Advance(4.Seconds)
        Assert.IsTrue(c.ElapsedTime = 9.Seconds)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitUntilTest_Positive()
        Dim c = New ManualClock()
        Dim f = c.AsyncWaitUntil(3.Seconds)
        ExpectTaskToIdle(f)
        c.Advance(2.Seconds)
        ExpectTaskToIdle(f)
        c.Advance(2.Seconds)
        WaitForTaskToSucceed(f)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitUntilTest_Multiple()
        Dim c = New ManualClock()
        Dim f2 = c.AsyncWaitUntil(2.Seconds)
        Dim f1 = c.AsyncWaitUntil(1.Seconds)
        Dim f3 = c.AsyncWaitUntil(3.Seconds)
        c.Advance(500.Milliseconds)
        ExpectTaskToIdle(f1)
        ExpectTaskToIdle(f2)
        ExpectTaskToIdle(f3)
        c.Advance(1.Seconds)
        WaitForTaskToSucceed(f1)
        ExpectTaskToIdle(f2)
        ExpectTaskToIdle(f3)
        c.Advance(1.Seconds)
        WaitForTaskToSucceed(f2)
        ExpectTaskToIdle(f3)
        c.Advance(1.Seconds)
        WaitForTaskToSucceed(f3)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitUntilTest_Duplicate()
        Dim c = New ManualClock()
        Dim f2 = c.AsyncWaitUntil(1.Seconds)
        Dim f1 = c.AsyncWaitUntil(1.Seconds)
        c.Advance(500.Milliseconds)
        ExpectTaskToIdle(f1)
        ExpectTaskToIdle(f2)
        c.Advance(1.Seconds)
        WaitForTaskToSucceed(f1)
        WaitForTaskToSucceed(f2)
    End Sub
    <TestMethod()>
    Public Sub ManualAsyncWaitUntilTest_Instant()
        Dim c = New ManualClock()
        Dim f = c.AsyncWaitUntil(-1.Seconds)
        Assert.IsTrue(f.Status = TaskStatus.RanToCompletion)
    End Sub

    <TestMethod()>
    Public Sub RelativeClockTest_ElapsedTime()
        Dim c = New ManualClock()
        Dim r0 = c.Restarted()
        Assert.IsTrue(r0.ElapsedTime = 0.Seconds)
        c.Advance(5.Seconds)
        Dim r1 = c.Restarted()
        Assert.IsTrue(r0.ElapsedTime = 5.Seconds)
        Assert.IsTrue(r1.ElapsedTime = 0.Seconds)
        c.Advance(3.Seconds)
        Assert.IsTrue(r0.ElapsedTime = 8.Seconds)
        Assert.IsTrue(r1.ElapsedTime = 3.Seconds)
        Assert.IsTrue(r0.StartingTimeOnParentClock = 0.Seconds)
        Assert.IsTrue(r1.StartingTimeOnParentClock = 5.Seconds)
    End Sub
    <TestMethod()>
    Public Sub RelativeClockTest_AsyncWaitUntil()
        Dim c = New ManualClock()

        Dim r0 = c.Restarted()
        Dim t = r0.AsyncWaitUntil(1.Seconds)
        ExpectTaskToIdle(t)
        c.Advance(2.Seconds)
        WaitForTaskToSucceed(t)

        Dim r1 = c.Restarted()
        Dim t2 = r1.AsyncWaitUntil(1.Seconds)
        ExpectTaskToIdle(t2)
        c.Advance(2.Seconds)
        WaitForTaskToSucceed(t)
    End Sub
    <TestMethod()>
    Public Sub RelativeClockTest_Nested()
        Dim c = New ManualClock()
        c.Advance(3.Seconds)
        Dim r0 = c.Restarted().Restarted().Restarted().Restarted()
        c.Advance(5.Seconds)
        Assert.IsTrue(r0.ElapsedTime = 5.Seconds)
    End Sub
    <TestMethod()>
    Public Sub RelativeClockTest_NestedAsyncWaitUntil()
        Dim c = New ManualClock()
        c.Advance(3.Seconds)
        Dim r0 = c.Restarted()
        c.Advance(3.Seconds)

        Dim r1 = r0.Restarted()
        Dim t = r1.AsyncWaitUntil(1.Seconds)
        ExpectTaskToIdle(t)
        c.Advance(2.Seconds)
        WaitForTaskToSucceed(t)
    End Sub

    <TestMethod()>
    Public Sub SystemAsyncWaitUntilTest_Positive()
        Dim c = New SystemClock()
        Dim f = c.AsyncWaitUntil(100.Milliseconds)
        ExpectTaskToIdle(f, timeoutMilliseconds:=50) '[safety margin of 50ms; might still fail sometimes due to bad luck]
        WaitForTaskToSucceed(f)
    End Sub
    <TestMethod()>
    Public Sub SystemAsyncWaitUntilTest_Instant()
        Dim c = New ManualClock()
        Dim f = c.AsyncWaitUntil(-1.Seconds)
        Assert.IsTrue(f.Status = TaskStatus.RanToCompletion)
    End Sub
    <TestMethod()>
    Public Sub SystemTimeTest()
        Dim c = New SystemClock()
        Threading.Thread.Sleep(50)
        Dim m = c.ElapsedTime
        Assert.IsTrue(m > 0.Milliseconds)
    End Sub

    <TestMethod()>
    Public Sub AsyncRepeatTest_Partial()
        Dim c = New ManualClock()
        Dim lock = New Threading.AutoResetEvent(initialState:=False)
        c.AsyncRepeat(period:=2.Seconds, action:=AddressOf lock.Set)

        c.Advance(1.Seconds)
        Assert.IsTrue(Not lock.WaitOne(millisecondsTimeout:=10))

        c.Advance(1.Seconds)
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=10000))
    End Sub
    <TestMethod()>
    Public Sub AsyncRepeatTest_Multi()
        Dim c = New ManualClock()
        Dim t = 0
        Dim locks = New List(Of Threading.AutoResetEvent)()
        For i = 0 To 5 - 1
            locks.Add(New Threading.AutoResetEvent(initialState:=False))
        Next i
        c.AsyncRepeat(period:=2.Seconds, action:=Sub()
                                                     locks(t).Set()
                                                     t += 1
                                                 End Sub)
        c.Advance(10.Seconds)
        For Each e In locks
            Assert.IsTrue(e.WaitOne(millisecondsTimeout:=10000))
        Next e
        Assert.IsTrue(t = 5)
    End Sub
    <TestMethod()>
    Public Sub AsyncRepeatTest_Dispose()
        Dim c = New ManualClock()
        Dim lock = New Threading.AutoResetEvent(initialState:=False)
        Dim d = c.AsyncRepeat(period:=2.Seconds, action:=AddressOf lock.Set)
        d.Dispose()
        c.Advance(2.Seconds)
        Assert.IsTrue(Not lock.WaitOne(millisecondsTimeout:=10))
    End Sub
    <TestMethod()>
    Public Sub AsyncWaitTest()
        Dim c = New ManualClock()
        c.Advance(5.Seconds)
        Dim t = c.AsyncWait(3.Seconds)
        c.Advance(2.Seconds)
        ExpectTaskToIdle(t)
        c.Advance(2.Seconds)
        WaitForTaskToSucceed(t)
    End Sub
End Class
