Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Misc

<TestClass()>
Public Class MiscTest
    <TestMethod()>
    Public Sub DelegatedDisposableTest()
        Dim flag = 0
        Dim d = New DelegatedDisposable(Sub() flag += 1)
        Assert.IsTrue(flag = 0)
        d.Dispose()
        Assert.IsTrue(flag = 1)
        d.Dispose()
        Assert.IsTrue(flag = 1)
    End Sub
End Class
