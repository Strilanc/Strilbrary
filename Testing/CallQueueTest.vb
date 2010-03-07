Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StrilbraryTests.TaskExtensionsTest
Imports Strilbrary.Time

<TestClass()>
Public Class CallQueueTest
    <TestMethod()>
    Public Sub QueueActionTest_Succeed()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueAction(Sub() flag = True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueActionTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueAction(Sub() Throw New InvalidOperationException("Test"))
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub QueueFuncTest_Succeed()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueFunc(Function()
                                     flag = True
                                     Return 5
                                 End Function)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(result.Result = 5)
    End Sub
    <TestMethod()>
    Public Sub QueueFuncTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim result = q.QueueFunc(Function() As Object
                                     Throw New InvalidOperationException("Test")
                                 End Function)
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub StartableTest_Dangle()
        Dim flag = True
        Dim q = New ThreadPooledCallQueue(initiallyStarted:=False)
        Dim result = q.QueueAction(Sub() flag = False)
        ExpectTaskToIdle(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub StartableTest_StartAfter()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue(initiallyStarted:=False)
        Dim result = q.QueueAction(Sub() flag = True)
        q.Start()
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub StartableTest_StartBefore()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue(initiallyStarted:=False)
        q.Start()
        Dim result = q.QueueAction(Sub() flag = True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
End Class
