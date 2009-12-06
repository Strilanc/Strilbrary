Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading
Imports System.Threading

<TestClass()>
Public Class ThrottleTest
    <TestMethod()>
    Public Sub RunTest()
        Dim lock = New ManualResetEvent(initialState:=False)
        Dim throttle = New Throttle(New TimeSpan(0, 0, 0, 0, milliseconds:=1000))
        throttle.SetActionToRun(Sub()
                                    lock.Set()
                                End Sub)
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=50))
    End Sub

    <TestMethod()>
    Public Sub HoldTest()
        Dim lock = New ManualResetEvent(initialState:=False)
        Dim throttle = New Throttle(New TimeSpan(0, 0, 0, 0, milliseconds:=100))

        throttle.SetActionToRun(Sub()
                                    lock.Set()
                                End Sub)
        lock.WaitOne(millisecondsTimeout:=50)

        lock.Reset()
        throttle.SetActionToRun(Sub()
                                    lock.Set()
                                End Sub)
        Assert.IsTrue(Not lock.WaitOne(millisecondsTimeout:=50))
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=1000))
    End Sub

    <TestMethod()>
    Public Sub ReentrantTest()
        Dim lock = New ManualResetEvent(initialState:=False)
        Dim throttle = New Throttle(New TimeSpan(0, 0, 0, 0, milliseconds:=100))

        throttle.SetActionToRun(Sub()
                                    throttle.SetActionToRun(Sub()
                                                                lock.Set()
                                                            End Sub)
                                End Sub)
        Assert.IsTrue(Not lock.WaitOne(millisecondsTimeout:=50))
        Assert.IsTrue(lock.WaitOne(millisecondsTimeout:=1000))
    End Sub
End Class
