Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading

<TestClass()>
Public Class CoroutineTest
#Region "Action"
    <TestMethod()>
    Public Sub YieldTest_Idle()
        Dim coroutine = New Coroutine(Sub(controller)
                                      End Sub)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.FinishedAndDisposed)
    End Sub
    <TestMethod()>
    Public Sub YieldTest_Value()
        Dim value = 0
        Dim coroutine = New Coroutine(Sub(controller)
                                          value = 1
                                          controller.Yield()
                                          value = 2
                                          controller.Yield()
                                          value = -3
                                      End Sub)
        Assert.IsTrue(value = 0)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.Continuing)
        Assert.IsTrue(value = 1)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.Continuing)
        Assert.IsTrue(value = 2)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.FinishedAndDisposed)
        Assert.IsTrue(value = -3)
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(CoroutineException))>
    Public Sub YieldTest_CoroutineThrow()
        Dim coroutine = New Coroutine(Sub(controller)
                                          Throw New IO.IOException("Test Exception")
                                      End Sub)
        coroutine.Continue()
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(ObjectDisposedException))>
    Public Sub YieldTest_TooFar()
        Dim coroutine = New Coroutine(Sub(controller)
                                      End Sub)
        coroutine.Continue()
        coroutine.Continue()
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(ObjectDisposedException))>
    Public Sub YieldTest_EarlyDispose()
        Dim coroutine = New Coroutine(Sub(controller)
                                      End Sub)
        coroutine.Dispose()
        coroutine.Continue()
    End Sub
    <TestMethod()>
    Public Sub YieldTest_Dispose()
        Dim coroutine = New Coroutine(Sub(controller)
                                      End Sub)
        coroutine.Continue()
        coroutine.Dispose()
    End Sub
    <TestMethod()>
    Public Sub YieldTest_DisposeStart()
        Dim coroutine = New Coroutine(Sub(controller)
                                      End Sub)
        coroutine.Dispose()
    End Sub
#End Region

#Region "Function"
    <TestMethod()>
    Public Sub ValueYieldTest_Idle()
        Dim coroutine = New Coroutine(Sub(controller)
                                      End Sub)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.FinishedAndDisposed)
    End Sub
    <TestMethod()>
    Public Sub ValueYieldTest_Value()
        Dim value = 0
        Dim coroutine = New Coroutine(Of Integer)(Sub(controller)
                                                      value = 1
                                                      controller.Yield(5)
                                                      value = 2
                                                      controller.Yield(6)
                                                      value = -3
                                                  End Sub)
        Assert.IsTrue(value = 0)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.Continuing)
        Assert.IsTrue(coroutine.Current = 5)
        Assert.IsTrue(value = 1)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.Continuing)
        Assert.IsTrue(coroutine.Current = 6)
        Assert.IsTrue(value = 2)
        Assert.IsTrue(coroutine.Continue() = CoroutineOutcome.FinishedAndDisposed)
        Assert.IsTrue(value = -3)
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(CoroutineException))>
    Public Sub ValueYieldTest_CoroutineThrow()
        Dim coroutine = New Coroutine(Of Integer)(Sub(controller)
                                                      Throw New IO.IOException("Test Exception")
                                                  End Sub)
        coroutine.Continue()
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(ObjectDisposedException))>
    Public Sub ValueYieldTest_TooFar()
        Dim coroutine = New Coroutine(Of Integer)(Sub(controller)
                                                  End Sub)
        coroutine.Continue()
        coroutine.Continue()
    End Sub
    <TestMethod()>
    <ExpectedException(GetType(ObjectDisposedException))>
    Public Sub ValueYieldTest_EarlyDispose()
        Dim coroutine = New Coroutine(Of Integer)(Sub(controller)
                                                  End Sub)
        coroutine.Dispose()
        coroutine.Continue()
    End Sub
    <TestMethod()>
    Public Sub ValueYieldTest_Dispose()
        Dim coroutine = New Coroutine(Of Integer)(Sub(controller)
                                                  End Sub)
        coroutine.Continue()
        coroutine.Dispose()
    End Sub
    <TestMethod()>
    Public Sub ValueYieldTest_DisposeStart()
        Dim coroutine = New Coroutine(Of Integer)(Sub(controller)
                                                  End Sub)
        coroutine.Dispose()
    End Sub
#End Region
End Class
