Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StrilbraryTests.TaskExtensionsTest
Imports Strilbrary.Time

<TestClass()>
Public Class ThreadingExtensionsTest
#Region "Async Eval"
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
#End Region

#Region "QueueContinueWith"
    <TestMethod()>
    Public Sub QueueCallOnSuccessTest()
        Dim flag = False
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.QueueContinueWithAction(q, Sub() flag = True)
        ExpectTaskToIdle(result)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnSuccessTest_Fail()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.QueueContinueWithAction(q, Sub() Throw New InvalidOperationException("Mock Exception"))
        WaitForTaskToFault(result)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnSuccessTest_Fail2()
        Dim flag = True
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetException(New InvalidOperationException("Mock Exception"))
        Dim result = task.Task.QueueContinueWithAction(q, Sub() flag = False)
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueCallOnValueSuccessTest()
        Dim flag = False
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)
        Dim result = task.Task.QueueContinueWithAction(q, Sub(value) flag = value)
        ExpectTaskToIdle(result)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnValueSuccessTest_Fail()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.QueueContinueWithAction(q, Sub(value) Throw New InvalidOperationException("Mock Exception"))
        WaitForTaskToFault(result)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnValueSuccessTest_Fail2()
        Dim flag = True
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetException(New InvalidOperationException("Mock Exception"))
        Dim result = task.Task.QueueContinueWithAction(q, Sub(value) flag = False)
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueEvalOnSuccessTest()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.QueueContinueWithFunc(q, Function() True)
        ExpectTaskToIdle(result)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnSuccessTest_Fail()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.QueueContinueWithFunc(q, Function() As Boolean
                                                            Throw New InvalidOperationException("Mock Exception")
                                                        End Function)
        WaitForTaskToFault(result)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnSuccessTest_Fail2()
        Dim flag = True
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)
        task.SetException(New InvalidOperationException("Mock Exception"))
        Dim result = task.Task.QueueContinueWithFunc(q, Function()
                                                            flag = False
                                                            Return True
                                                        End Function)
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueEvalOnValueSuccessTest()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.QueueContinueWithFunc(q, Function(value) value)
        ExpectTaskToIdle(result)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnValueSuccessTest_Fail()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.QueueContinueWithFunc(q, Function(value) As Boolean
                                                            Throw New InvalidOperationException("Mock Exception")
                                                        End Function)
        WaitForTaskToFault(result)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnValueSuccessTest_Fail2()
        Dim flag = True
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetException(New InvalidOperationException("Mock Exception"))
        Dim result = task.Task.QueueContinueWithFunc(q, Function(value)
                                                            flag = False
                                                            Return True
                                                        End Function)
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
    End Sub
#End Region

    <TestMethod()>
    Public Sub QueueCatchTest()
        Dim flag = False
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)
        task.SetException(New InvalidOperationException("Mock Exception"))
        Dim result = task.Task.QueueCatch(q, Sub(ex) flag = TypeOf ex.InnerExceptions.Single Is InvalidOperationException)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCatchTest_Miss()
        Dim flag = True
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)
        task.SetResult(True)
        Dim result = task.Task.QueueCatch(q, Sub(exception) flag = False)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCatchTest_Fail()
        Dim q = MakeThreadPooledCallQueue()
        Dim task = New TaskCompletionSource(Of Boolean)
        task.SetException(New InvalidOperationException("Mock Exception"))
        Dim result = task.Task.QueueCatch(q, Sub(exception) Throw New InvalidOperationException("Mock Exception 2"))
        WaitForTaskToFault(result)
    End Sub
End Class
