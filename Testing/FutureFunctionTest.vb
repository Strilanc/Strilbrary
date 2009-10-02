Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading

<TestClass()>
Public Class FutureFunctionTest
    <TestMethod()>
    Public Sub SucceedTest()
        Dim target = New FutureFunction(Of Integer)()
        target.SetSucceeded(1)
        Assert.IsTrue(target.State = FutureState.Succeeded)
        Assert.IsTrue(target.value = 1)
    End Sub
    <TestMethod()>
    Public Sub TrySucceedTest()
        Dim target = New FutureFunction(Of Integer)()
        Assert.IsTrue(target.TrySetSucceeded(2))
        Assert.IsTrue(target.State = FutureState.Succeeded)
        Assert.IsTrue(target.value = 2)
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(InvalidOperationException))>
    Public Sub OverSucceedTest()
        Dim target = New FutureFunction(Of Integer)()
        target.SetSucceeded(3)
        target.SetSucceeded(4)
    End Sub
    <TestMethod()>
    Public Sub TryOverSucceedTest()
        Dim target = New FutureFunction(Of Integer)()
        target.TrySetSucceeded(5)
        Assert.IsTrue(Not target.TrySetSucceeded(6))
        Assert.IsTrue(target.State = FutureState.Succeeded)
        Assert.IsTrue(target.value = 5)
    End Sub
    <TestMethod()>
    Public Sub TrySucceedFailTest()
        Dim target = New FutureFunction(Of Integer)()
        target.TrySetSucceeded(6)
        Assert.IsTrue(Not target.TrySetFailed(New InvalidOperationException("Mock Exception")))
        Assert.IsTrue(target.State = FutureState.Succeeded)
        Assert.IsTrue(target.value = 6)
    End Sub
    <TestMethod()>
    Public Sub TryFailSucceedTest()
        Dim target = New FutureFunction(Of Integer)()
        target.TrySetFailed(New InvalidOperationException("Mock Exception"))
        Assert.IsTrue(Not target.TrySetSucceeded(7))
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub SetByEvaluatingSucceed()
        Dim target = New FutureFunction(Of Integer)
        target.SetByEvaluating(Function()
                                   Return -3
                               End Function)
        Assert.IsTrue(target.State = FutureState.Succeeded)
        Assert.IsTrue(target.Value = -3)
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(InvalidOperationException))>
    Public Sub OverSetByEvaluating()
        Dim target = New FutureFunction(Of Integer)
        target.SetByEvaluating(Function()
                               End Function)
        target.SetByEvaluating(Function()
                               End Function)
    End Sub
    <TestMethod()>
    Public Sub SetByEvaluatingFail()
        Dim target = New FutureFunction(Of Integer)
        target.SetByEvaluating(Function()
                                   Throw New InvalidOperationException("Mock exception")
                               End Function)
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub
End Class
