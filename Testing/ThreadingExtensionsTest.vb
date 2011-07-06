Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StrilbraryTests.TaskExtensionsTest
Imports Strilbrary.Time

<TestClass()>
Public Class ThreadingExtensionsTest
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
End Class
