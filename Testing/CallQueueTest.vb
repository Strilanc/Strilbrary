Imports System.Threading
Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StrilbraryTests.IFutureExtensionsTest
Imports Strilbrary

<TestClass()>
Public Class CallQueueTest
    <TestMethod()>
    Public Sub QueueActionTest_Succeed()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueAction(Sub() flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueActionTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueAction(Sub() Throw New InvalidOperationException("Test"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub QueueFuncTest_Succeed()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueFunc(Function()
                                     flag = True
                                     Return 5
                                 End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
        Assert.IsTrue(result.Value = 5)
    End Sub
    <TestMethod()>
    Public Sub QueueFuncTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueFunc(Function() As Object
                                     Throw New InvalidOperationException("Test")
                                 End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub StartableTest_Dangle()
        Dim flag = True
        Dim q = New StartableCallQueue(subQueue:=New ThreadPooledCallQueue())
        Dim result = q.QueueAction(Sub() flag = False)
        BlockOnFuture(result, timeout:=100.Milliseconds)
        Assert.IsTrue(result.State = FutureState.Unknown)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub StartableTest_StartAfter()
        Dim flag = False
        Dim q = New StartableCallQueue(subQueue:=New ThreadPooledCallQueue())
        Dim result = q.QueueAction(Sub() flag = True)
        q.Start()
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub StartableTest_StartBefore()
        Dim flag = False
        Dim q = New StartableCallQueue(subQueue:=New ThreadPooledCallQueue())
        q.Start()
        Dim result = q.QueueAction(Sub() flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
End Class
