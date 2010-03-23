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

    Private Sub TestCallQueue(ByVal q As CallQueue)
        Dim t = 0
        Dim flag = True
        For i = 0 To 1000 - 1
            Dim i_ = i
            q.QueueAction(Sub()
                              If i_ <> t Then flag = False
                              t += 1
                          End Sub)
        Next i
        WaitForTaskToSucceed(q.QueueAction(Sub()
                                           End Sub))
        Assert.IsTrue(t = 1000)
    End Sub
    <TestMethod()>
    Public Sub CallQueueTypeTests()
        TestCallQueue(New ThreadPooledCallQueue(initiallyStarted:=True))
        TestCallQueue(New TaskedCallQueue(initiallyStarted:=True))
        TestCallQueue(New ThreadedCallQueue(initiallyStarted:=True))
    End Sub
End Class
