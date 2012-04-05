Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading
Imports Strilbrary.Time
Imports System.Threading

<TestClass()>
Public Class ThrottleTest
    <TestMethod()>
    Public Sub RunTest()
        Dim c = New ManualTimer()
        Dim throttle = New Throttle(1.Milliseconds, c, New ThreadedSynchronizationContext())

        Dim lock = New ManualResetEvent(initialState:=False)
        throttle.SetActionToRun(Sub() lock.Set())
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=50))
    End Sub

    <TestMethod()>
    Public Sub ThrottleTest()
        Dim c = New ManualTimer()
        Dim throttle = New Throttle(1.Milliseconds, c, New ThreadedSynchronizationContext())

        Dim lock = New ManualResetEvent(initialState:=False)
        throttle.SetActionToRun(Sub() lock.Set())
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=50))

        lock.Reset()
        throttle.SetActionToRun(Sub() lock.Set())
        Assert.IsTrue(Not lock.WaitOne(millisecondsTimeout:=50))
        c.Advance(2.Milliseconds)
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=10000))
    End Sub

    <TestMethod()>
    Public Sub ReentrantTest()
        Dim c = New ManualTimer()
        Dim throttle = New Throttle(1.Milliseconds, c, New ThreadedSynchronizationContext())

        Dim lock = New ManualResetEvent(initialState:=False)
        throttle.SetActionToRun(Sub() throttle.SetActionToRun(Sub() lock.Set()))
        Assert.IsTrue(Not lock.WaitOne(millisecondsTimeout:=50))
        c.Advance(2.Milliseconds)
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=10000))
    End Sub
End Class
