Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading

<TestClass()>
Public Class FutureActionTest
    <TestMethod()>
    Public Sub SucceedTest()
        Dim target = New FutureAction()
        target.SetSucceeded()
        Assert.IsTrue(target.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub TrySucceedTest()
        Dim target = New FutureAction()
        Assert.IsTrue(target.TrySetSucceeded())
        Assert.IsTrue(target.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(InvalidOperationException))>
    Public Sub OverSucceedTest()
        Dim target = New FutureAction()
        target.SetSucceeded()
        target.SetSucceeded()
    End Sub
    <TestMethod()>
    Public Sub TryOverSucceedTest()
        Dim target = New FutureAction()
        target.TrySetSucceeded()
        Assert.IsTrue(Not target.TrySetSucceeded())
        Assert.IsTrue(target.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub TrySucceedFailTest()
        Dim target = New FutureAction()
        target.TrySetSucceeded()
        Assert.IsTrue(Not target.TrySetFailed(New InvalidOperationException("Mock Exception")))
        Assert.IsTrue(target.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    Public Sub TryFailSucceedTest()
        Dim target = New FutureAction()
        target.TrySetFailed(New InvalidOperationException("Mock Exception"))
        Assert.IsTrue(Not target.TrySetSucceeded())
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub SetByCallingSucceed()
        Dim target = New FutureAction()
        target.SetByCalling(Sub()
                            End Sub)
        Assert.IsTrue(target.State = FutureState.Succeeded)
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(InvalidOperationException))>
    Public Sub OverSetByCalling()
        Dim target = New FutureAction()
        target.SetByCalling(Sub()
                            End Sub)
        target.SetByCalling(Sub()
                            End Sub)
    End Sub
    <TestMethod()>
    Public Sub SetByCallingFail()
        Dim target = New FutureAction()
        target.SetByCalling(Sub()
                                Throw New InvalidOperationException("Mock exception")
                            End Sub)
        Assert.IsTrue(target.State = FutureState.Failed)
        Assert.IsTrue(TypeOf target.Exception Is InvalidOperationException)
    End Sub
End Class
