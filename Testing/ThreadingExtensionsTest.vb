Imports System.Threading
Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StrilbraryTests.IFutureExtensionsTest
Imports Strilbrary

<TestClass()>
Public Class ThreadingExtensionsTest
    <TestMethod()>
    Public Sub FutureWaitTest()
        Dim result = 2.Seconds.futurewait
        BlockOnFuture(result, timeout:=1.Seconds)
        Assert.IsTrue(result.State = FutureState.Unknown)
        BlockOnFuture(result, timeout:=2.Seconds)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub

#Region "Async Eval"
    <TestMethod()>
    Public Sub ThreadedActionTest()
        Dim flag = False
        Dim result = ThreadedAction(Sub() flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ThreadedActionTest_Fail()
        Dim result = ThreadedAction(Sub() Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub ThreadedFuncTest()
        Dim result = ThreadedFunc(Function() True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = True)
    End Sub
    <TestMethod()>
    Public Sub ThreadedFuncTest_Fail()
        Dim result = ThreadedFunc(Function() As Object
                                      Throw New InvalidOperationException("Mock Exception")
                                  End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub ThreadPooledActionTest()
        Dim flag = False
        Dim result = ThreadPooledAction(Sub() flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ThreadPooledActionTest_Fail()
        Dim result = ThreadPooledAction(Sub() Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub ThreadPooledFuncTest()
        Dim result = ThreadPooledFunc(Function() True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = True)
    End Sub
    <TestMethod()>
    Public Sub ThreadPooledFuncTest_Fail()
        Dim result = ThreadPooledFunc(Function() As Object
                                          Throw New InvalidOperationException("Mock Exception")
                                      End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub TaskedActionTest()
        Dim flag = False
        Dim result = TaskedAction(Sub() flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub TaskedActionTest_Fail()
        Dim result = TaskedAction(Sub() Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub

    <TestMethod()>
    Public Sub TaskedFuncTest()
        Dim result = TaskedFunc(Function() True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = True)
    End Sub
    <TestMethod()>
    Public Sub TaskedFuncTest_Fail()
        Dim result = TaskedFunc(Function() As Object
                                    Throw New InvalidOperationException("Mock Exception")
                                End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
#End Region

#Region "WhenReady"
    <TestMethod()>
    Public Sub QueueCallWhenReadyTest()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        Dim result = future.QueueCallWhenReady(q, Sub(exception) flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded()
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCallWhenReadyTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.QueueCallWhenReady(q, Sub(exception) Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueCallWhenReadyTest_Fail2()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueCallWhenReady(q, Sub(exception) flag = exception IsNot Nothing)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueCallWhenValueReadyTest()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)
        Dim result = future.QueueCallWhenValueReady(q, Sub(value, exception) flag = value)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded(True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCallWhenValueReadyTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetSucceeded(True)
        Dim result = future.QueueCallWhenValueReady(q, Sub(value, exception) Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueCallWhenValueReadyTest_Fail2()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueCallWhenValueReady(q, Sub(value, exception) flag = exception IsNot Nothing)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueEvalWhenReadyTest()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        Dim result = future.QueueEvalWhenReady(q, Function(exception) True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded()
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalWhenReadyTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.QueueEvalWhenReady(q, Function(exception) As Boolean
                                                      Throw New InvalidOperationException("Mock Exception")
                                                  End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalWhenReadyTest_Fail2()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueEvalWhenReady(q, Function(exception) exception IsNot Nothing)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value)
    End Sub

    <TestMethod()>
    Public Sub QueueEvalWhenValueReadyTest()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        Dim result = future.QueueEvalWhenValueReady(q, Function(value, exception) value)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded(True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalWhenValueReadyTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetSucceeded(True)
        Dim result = future.QueueEvalWhenValueReady(q, Function(value, exception) As Boolean
                                                           Throw New InvalidOperationException("Mock Exception")
                                                       End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalWhenValueReadyTest_Fail2()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueEvalWhenValueReady(q, Function(value, exception) exception IsNot Nothing)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value)
    End Sub
#End Region

#Region "OnSuccess"
    <TestMethod()>
    Public Sub QueueCallOnSuccessTest()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        Dim result = future.QueueCallOnSuccess(q, Sub() flag = True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded()
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnSuccessTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.QueueCallOnSuccess(q, Sub() Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnSuccessTest_Fail2()
        Dim flag = True
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueCallOnSuccess(q, Sub() flag = False)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueCallOnValueSuccessTest()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)
        Dim result = future.QueueCallOnValueSuccess(q, Sub(value) flag = value)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded(True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnValueSuccessTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetSucceeded(True)
        Dim result = future.QueueCallOnValueSuccess(q, Sub(value) Throw New InvalidOperationException("Mock Exception"))
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueCallOnValueSuccessTest_Fail2()
        Dim flag = True
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueCallOnValueSuccess(q, Sub(value) flag = False)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueEvalOnSuccessTest()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        Dim result = future.QueueEvalOnSuccess(q, Function() True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded()
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnSuccessTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.QueueEvalOnSuccess(q, Function() As Boolean
                                                      Throw New InvalidOperationException("Mock Exception")
                                                  End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnSuccessTest_Fail2()
        Dim flag = True
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueEvalOnSuccess(q, Function()
                                                      flag = False
                                                      Return True
                                                  End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(flag)
    End Sub

    <TestMethod()>
    Public Sub QueueEvalOnValueSuccessTest()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        Dim result = future.QueueEvalOnValueSuccess(q, Function(value) value)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Unknown)
        future.SetSucceeded(True)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnValueSuccessTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetSucceeded(True)
        Dim result = future.QueueEvalOnValueSuccess(q, Function(value) As Boolean
                                                           Throw New InvalidOperationException("Mock Exception")
                                                       End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
    <TestMethod()>
    Public Sub QueueEvalOnValueSuccessTest_Fail2()
        Dim flag = True
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureFunction(Of Boolean)()
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueEvalOnValueSuccess(q, Function(value)
                                                           flag = False
                                                           Return True
                                                       End Function)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(flag)
    End Sub
#End Region

    <TestMethod()>
    Public sub QueueCatchTest()
        Dim flag = False
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueCatch(q, Sub(exception)
                                              flag = TypeOf exception Is InvalidOperationException
                                          End Sub)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCatchTest_Miss()
        Dim flag = True
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction
        future.SetSucceeded()
        Dim result = future.QueueCatch(q, Sub(exception)
                                              flag = False
                                          End Sub)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub QueueCatchTest_Fail()
        Dim q = New ThreadPooledCallQueue()
        Dim future = New FutureAction
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Dim result = future.QueueCatch(q, Sub(exception)
                                              Throw New InvalidOperationException("Mock Exception 2")
                                          End Sub)
        BlockOnFuture(result)
        Assert.IsTrue(result.State = FutureState.Failed)
    End Sub
End Class
