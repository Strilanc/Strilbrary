Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Time

<TestClass()>
Public Class ThreadingExtensionsTest
    Public Shared Sub WaitForTaskToSucceed(task As Task, Optional timeoutMilliseconds As Integer = 10000)
        Assert.IsTrue(task.Wait(timeoutMilliseconds))
        Assert.IsTrue(task.Status = TaskStatus.RanToCompletion)
    End Sub
    Public Shared Sub WaitForTaskToFault(task As Task, Optional timeoutMilliseconds As Integer = 10000)
        Try
            Assert.IsTrue(task.Wait(timeoutMilliseconds))
        Catch ex As Exception
        End Try
        Assert.IsTrue(task.Status = TaskStatus.Faulted)
    End Sub
    Public Shared Sub ExpectTaskToIdle(task As Task, Optional timeoutMilliseconds As Integer = 10)
        Assert.IsTrue(Not task.Wait(timeoutMilliseconds))
    End Sub

    <TestMethod()>
    Public Sub AsTaskTest()
        Dim result = 1.AsTask
        Assert.IsTrue(result.Status = TaskStatus.RanToCompletion)
        Assert.IsTrue(result.Result = 1)
    End Sub

    <TestMethod()>
    Public Sub ThreadedActionTest()
        Dim flag = False
        Dim result = ThreadedAction(Sub() flag = True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ThreadedActionTest_Fail()
        Dim result = ThreadedAction(Sub() Throw New InvalidOperationException("Mock Exception"))
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub ThreadedFuncTest()
        Dim result = ThreadedFunc(Function() True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result = True)
    End Sub
    <TestMethod()>
    Public Sub ThreadedFuncTest_Fail()
        Dim result = ThreadedFunc(Function() As Object
                                      Throw New InvalidOperationException("Mock Exception")
                                  End Function)
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub ThreadPooledActionTest()
        Dim flag = False
        Dim result = ThreadPooledAction(Sub() flag = True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ThreadPooledActionTest_Fail()
        Dim result = ThreadPooledAction(Sub() Throw New InvalidOperationException("Mock Exception"))
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub ThreadPooledFuncTest()
        Dim result = ThreadPooledFunc(Function() True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result = True)
    End Sub
    <TestMethod()>
    Public Sub ThreadPooledFuncTest_Fail()
        Dim result = ThreadPooledFunc(Function() As Object
                                          Throw New InvalidOperationException("Mock Exception")
                                      End Function)
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub TaskedActionTest()
        Dim flag = False
        Dim result = TaskedAction(Sub() flag = True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub TaskedActionTest_Fail()
        Dim result = TaskedAction(Sub() Throw New InvalidOperationException("Mock Exception"))
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub TaskedFuncTest()
        Dim result = TaskedFunc(Function() True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result = True)
    End Sub
    <TestMethod()>
    Public Sub TaskedFuncTest_Fail()
        Dim result = TaskedFunc(Function() As Object
                                    Throw New InvalidOperationException("Mock Exception")
                                End Function)
        WaitForTaskToFault(result)
    End Sub

    <TestMethod()>
    Public Sub AwaitableEntranceTest()
        Dim q1 = MakeThreadPooledCallQueue()
        Dim q2 = MakeThreadPooledCallQueue()
        Dim r = Async Function()
                    Try
                        Dim e = Sub()
                                End Sub
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is Nothing)

                        Dim t = q1.QueueAction(e)
                        WaitForTaskToSucceed(t)
                        Await q1.AwaitableEntrance(forceReentry:=False)
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is q1)

                        t = q1.QueueAction(e)
                        ExpectTaskToIdle(t)
                        Await q1.AwaitableEntrance(forceReentry:=False)
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is q1)
                        ExpectTaskToIdle(t)
                        Await q1.AwaitableEntrance(forceReentry:=True)
                        WaitForTaskToSucceed(t)

                        t = q1.QueueAction(e)
                        ExpectTaskToIdle(t)
                        Await q2.AwaitableEntrance(forceReentry:=False)
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is q2)
                        WaitForTaskToSucceed(t)
                        Return Nothing
                    Catch ex As Exception
                        Return ex
                    End Try
                End Function().Result
        If r IsNot Nothing Then Throw New AssertFailedException("", r)
    End Sub
End Class
