Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading

<TestClass()>
Public Class FutureBaseTest
    <TestMethod()>
    Public Sub IdleTest()
        Dim target = New FutureAction()
        Assert.IsTrue(target.State = FutureState.Unknown)
    End Sub
    <TestMethod()>
    Public Sub FailTest()
        Dim target As FutureBase = New FutureAction()
        target.SetFailed(New InvalidOperationException("Mock exception"))
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub TryFailTest()
        Dim target As FutureBase = New FutureAction()
        Assert.IsTrue(target.TrySetFailed(New InvalidOperationException("Mock exception")))
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub

    <TestMethod()>
    <ExpectedException(GetType(InvalidOperationException))>
    Public Sub OverFailTest()
        Dim target As FutureBase = New FutureAction()
        target.SetFailed(New IO.IOException("Mock exception"))
        target.SetFailed(New IO.IOException("Mock exception"))
    End Sub

    <TestMethod()>
    Public Sub TryOverFailTest()
        Dim target As FutureBase = New FutureAction()
        Assert.IsTrue(target.TrySetFailed(New InvalidOperationException("Mock exception")))
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
        Assert.IsTrue(Not target.TrySetFailed(New IO.IOException("Mock exception")))
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub
End Class
