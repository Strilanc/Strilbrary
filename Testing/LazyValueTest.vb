Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Misc

<TestClass()>
Public Class LazyValueTest
    <TestMethod()>
    Public Sub ValueTest()
        Assert.IsTrue(New LazyValue(Of Integer)(1).Value = 1)
        Assert.IsTrue(New LazyValue(Of Integer)(2).Value = 2)
        Assert.IsTrue(CType(3, LazyValue(Of Integer)).Value = 3)
        Assert.IsTrue(CType(4, LazyValue(Of Integer)).Value = 4)
    End Sub

    <TestMethod()>
    Public Sub FuncTest()
        Assert.IsTrue(New LazyValue(Of Integer)(Function() 1).Value = 1)
        Assert.IsTrue(New LazyValue(Of Integer)(Function() 2).Value = 2)
        Dim f3 As Func(Of Integer) = Function() 3
        Dim f4 As Func(Of Integer) = Function() 4
        Assert.IsTrue(CType(f3, LazyValue(Of Integer)).Value = 3)
        Assert.IsTrue(CType(f4, LazyValue(Of Integer)).Value = 4)
    End Sub

    <TestMethod()>
    Public Sub FuncTestIdempotent()
        Dim state = 0
        Dim val = New LazyValue(Of Integer)(Function() As Integer
                                                state += 1
                                                Return state
                                            End Function)
        Assert.IsTrue(state = 0)
        Assert.IsTrue(val.Value = 1)
        Assert.IsTrue(state = 1)
        Assert.IsTrue(val.Value = 1)
        Assert.IsTrue(state = 1)
    End Sub
End Class
