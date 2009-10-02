Imports System.Collections.Generic
Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading

<TestClass()>
Public Class IFutureExtensionsTest
    Private Shared Function BlockOnFuture(ByVal future As IFuture) As Boolean
        Return BlockOnFuture(future, New TimeSpan(0, 0, seconds:=1))
    End Function
    Private Shared Function BlockOnFuture(ByVal future As IFuture,
                                          ByVal timeout As TimeSpan) As Boolean
        Dim waitHandle = New System.Threading.ManualResetEvent(initialState:=False)
        AddHandler future.Ready, Sub() waitHandle.Set()
        If future.State <> FutureState.Unknown Then waitHandle.Set()
        Return waitHandle.WaitOne(timeout)
    End Function

    <TestMethod()>
    Public Sub TryGetTest_Fail()
        Dim future = New FutureFunction(Of Object)
        Assert.IsTrue(future.TryGetValue Is Nothing)
        Assert.IsTrue(future.TryGetException Is Nothing)
        future.SetFailed(New InvalidOperationException("Mock Exception"))
        Assert.IsTrue(future.TryGetValue Is Nothing)
        Assert.IsTrue(future.TryGetException IsNot Nothing)
    End Sub
    <TestMethod()>
    Public Sub TryGetTest_Success()
        Dim future = New FutureFunction(Of Object)
        Assert.IsTrue(future.TryGetValue Is Nothing)
        Assert.IsTrue(future.TryGetException Is Nothing)
        future.SetSucceeded(New Object())
        Assert.IsTrue(future.TryGetValue IsNot Nothing)
        Assert.IsTrue(future.TryGetException Is Nothing)
    End Sub

#Region "WhenReady"
    <TestMethod()>
    Public Sub CallWhenReadyTest_Dangle()
        Dim flag = True
        Dim future = New FutureAction()
        Dim result = future.CallWhenReady(Sub(exception)
                                              flag = False
                                          End Sub)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub CallWhenReadyTest_Succeed()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.CallWhenReady(Sub(exception)
                                              flag = exception Is Nothing
                                          End Sub)
        future.SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallWhenReadyTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.CallWhenReady(Sub(exception)
                                              flag = exception Is Nothing
                                          End Sub)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallWhenReadyTest_Fail()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.CallWhenReady(Sub(exception)
                                              flag = exception IsNot Nothing
                                          End Sub)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallWhenReadyTest_Throw()
        Dim future = New FutureAction()
        Dim result = future.CallWhenReady(Sub(exception)
                                              Throw New InvalidOperationException("Mock Failure")
                                          End Sub)
        future.SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub EvalWhenReadyTest_Dangle()
        Dim flag = True
        Dim future = New FutureAction()
        Dim result = future.EvalWhenReady(Function(exception)
                                              flag = False
                                              Return 2
                                          End Function)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenReadyTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.EvalWhenReady(Function(exception)
                                              flag = exception Is Nothing
                                              Return 3
                                          End Function)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 3)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenReadyTest_Succeed()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.EvalWhenReady(Function(exception)
                                              flag = exception Is Nothing
                                              Return 3
                                          End Function)
        future.SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 3)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenReadyTest_Fail()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.EvalWhenReady(Function(exception)
                                              flag = exception IsNot Nothing
                                              Return 4
                                          End Function)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 4)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenReadyTest_Throw()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.EvalWhenReady(Function(exception) As Integer
                                              Throw New InvalidOperationException("Mock Exception")
                                          End Function)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub CallWhenValueReadyTest_Dangle()
        Dim flag = True
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallWhenValueReady(Sub(value, exception)
                                                   flag = False
                                               End Sub)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub CallWhenValueReadyTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        future.SetSucceeded(11)
        Dim result = future.CallWhenValueReady(Sub(value, exception)
                                                   flag = exception Is Nothing AndAlso value = 11
                                               End Sub)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallWhenValueReadyTest_Succeed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallWhenValueReady(Sub(value, exception)
                                                   flag = exception Is Nothing AndAlso value = 11
                                               End Sub)
        future.SetSucceeded(11)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallWhenValueReadyTest_Fail()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallWhenValueReady(Sub(value, exception)
                                                   flag = exception IsNot Nothing
                                               End Sub)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallWhenValueReadyTest_Throw()
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallWhenValueReady(Sub(value, exception)
                                                   Throw New InvalidOperationException("Mock Failure")
                                               End Sub)
        future.SetSucceeded(12)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub EvalWhenValueReadyTest_Dangle()
        Dim flag = True
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.EvalWhenValueReady(Function(value, exception)
                                                   flag = False
                                                   Return -1
                                               End Function)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenValueReadyTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        future.SetSucceeded(27)
        Dim result = future.EvalWhenValueReady(Function(value, exception)
                                                   flag = exception Is Nothing AndAlso value = 27
                                                   Return -13
                                               End Function)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = -13)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenValueReadyTest_Succeed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.EvalWhenValueReady(Function(value, exception)
                                                   flag = exception Is Nothing AndAlso value = 21
                                                   Return -2
                                               End Function)
        future.SetSucceeded(21)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = -2)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenValueReadyTest_Fail()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.EvalWhenValueReady(Function(value, exception)
                                                   flag = exception IsNot Nothing
                                                   Return -3
                                               End Function)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = -3)
    End Sub
    <TestMethod()>
    Public Sub EvalWhenValueReadyTest_Throw()
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.EvalWhenValueReady(Function(value, exception) As Integer
                                                   Throw New InvalidOperationException("Mock Failure")
                                               End Function)
        future.SetSucceeded(22)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub
#End Region

#Region "OnSuccess/Linq"
    <TestMethod()>
Public Sub CallOnSuccessTest_Dangle()
        Dim flag = True
        Dim future = New FutureAction()
        Dim result = future.CallOnSuccess(Sub()
                                              flag = False
                                          End Sub)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub CallOnSuccessTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.CallOnSuccess(Sub()
                                              flag = True
                                          End Sub)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallOnSuccessTest_Succeed()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.CallOnSuccess(Sub()
                                              flag = True
                                          End Sub)
        future.SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallOnSuccessTest_Fail()
        Dim flag = True
        Dim future = New FutureAction()
        Dim result = future.CallOnSuccess(Sub()
                                              flag = False
                                          End Sub)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub CallOnSuccessTest_Throw()
        Dim future = New FutureAction()
        Dim result = future.CallOnSuccess(Sub()
                                              Throw New InvalidOperationException("Mock Failure")
                                          End Sub)
        future.SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub EvalOnSuccessTest_Dangle()
        Dim flag = True
        Dim future = New FutureAction()
        Dim result = future.EvalOnSuccess(Function()
                                              flag = False
                                              Return 2
                                          End Function)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub EvalOnSuccessTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureAction()
        future.SetSucceeded()
        Dim result = future.EvalOnSuccess(Function()
                                              flag = True
                                              Return 3
                                          End Function)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 3)
    End Sub
    <TestMethod()>
    Public Sub EvalOnSuccessTest_Succeed()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.EvalOnSuccess(Function()
                                              flag = True
                                              Return 3
                                          End Function)
        future.SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 3)
    End Sub
    <TestMethod()>
    Public Sub EvalOnSuccessTest_Fail()
        Dim flag = True
        Dim future = New FutureAction()
        Dim result = future.EvalOnSuccess(Function()
                                              flag = False
                                              Return 4
                                          End Function)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub EvalOnSuccessTest_Throw()
        Dim flag = False
        Dim future = New FutureAction()
        Dim result = future.EvalOnSuccess(Function() As Integer
                                              Throw New InvalidOperationException("Mock Exception")
                                          End Function)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub CallOnValueSuccessTest_Dangle()
        Dim flag = True
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallOnValueSuccess(Sub(value)
                                                   flag = False
                                               End Sub)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub CallOnValueSuccessTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        future.SetSucceeded(11)
        Dim result = future.CallOnValueSuccess(Sub(value)
                                                   flag = value = 11
                                               End Sub)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallOnValueSuccessTest_Succeed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallOnValueSuccess(Sub(value)
                                                   flag = value = 11
                                               End Sub)
        future.SetSucceeded(11)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub CallOnValueSuccessTest_Fail()
        Dim flag = True
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallOnValueSuccess(Sub(value)
                                                   flag = False
                                               End Sub)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub CallOnValueSuccessTest_Throw()
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.CallOnValueSuccess(Sub(value)
                                                   Throw New InvalidOperationException("Mock Failure")
                                               End Sub)
        future.SetSucceeded(12)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub SelectTest_Dangle()
        Dim flag = True
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.Select(Function(value)
                                       flag = False
                                       Return -1
                                   End Function)
        Assert.IsTrue(Not BlockOnFuture(result, New TimeSpan(0, 0, 0, 0, milliseconds:=100)))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub SelectTest_PreSucceed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        future.SetSucceeded(21)
        Dim result = future.Select(Function(value)
                                       flag = value = 21
                                       Return -2
                                   End Function)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = -2)
    End Sub
    <TestMethod()>
    Public Sub SelectTest_Succeed()
        Dim flag = False
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.Select(Function(value)
                                       flag = value = 21
                                       Return -2
                                   End Function)
        future.SetSucceeded(21)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = -2)
    End Sub
    <TestMethod()>
    Public Sub SelectTest_Fail()
        Dim flag = True
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.Select(Function(value)
                                       flag = False
                                       Return -3
                                   End Function)
        future.SetFailed(New InvalidOperationException("Mock Failure"))
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(flag)
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub SelectTest_Throw()
        Dim future = New FutureFunction(Of Integer)()
        Dim result = future.Select(Function(value) As Integer
                                       Throw New InvalidOperationException("Mock Failure")
                                   End Function)
        future.SetSucceeded(22)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Failed)
        Assert.IsTrue(TypeOf result.Exception Is InvalidOperationException)
    End Sub
#End Region

#Region "Transform"
    <TestMethod()>
    Public Sub DefuturizedTest_action()
        Dim future = New FutureFunction(Of IFuture)
        Dim result = future.Defuturized
        Assert.IsTrue(result.State = FutureState.Unknown)
        Dim future2 = New FutureAction
        future2.SetSucceeded()
        future.SetSucceeded(future2)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub DefuturizedTest_function()
        Dim future = New FutureFunction(Of IFuture(Of Integer))
        Dim result = future.Defuturized
        Assert.IsTrue(result.State = FutureState.Unknown)
        Dim future2 = New FutureFunction(Of Integer)
        future.SetSucceeded(future2)
        future2.SetSucceeded(3)
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 3)
    End Sub
    <TestMethod()>
    Public Sub DefuturizedTest_list()
        Dim futures As IList(Of FutureAction) = {New FutureAction, New FutureAction}
        Dim result = futures.Defuturized
        Assert.IsTrue(result.State = FutureState.Unknown)
        futures(0).SetSucceeded()
        Assert.IsTrue(result.State = FutureState.Unknown)
        futures(1).SetSucceeded()
        Assert.IsTrue(BlockOnFuture(result))
        Assert.IsTrue(result.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub FuturizedTest_value()
        Dim result = 1.Futurized
        Assert.IsTrue(result.State = FutureState.Succeeded)
        Assert.IsTrue(result.Value = 1)
    End Sub
#End Region
End Class
