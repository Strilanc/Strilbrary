Imports System.Collections.Generic
Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Threading
Imports System.Threading
Imports Strilbrary.Time

<TestClass()>
Public Class TaskExtensionsTest
    Public Shared Sub WaitForTaskToSucceed(task As Task, Optional timeoutMilliseconds As Integer = 10000)
        Assert.IsTrue(task.Wait(timeoutMilliseconds))
        Assert.IsTrue(task.Status = TaskStatus.RanToCompletion)
    End Sub
    Public Shared Sub WaitForTaskToFault(task As Task, Optional timeoutMilliseconds As Integer = 10000)
        Try
            Assert.IsTrue(task.Wait(timeoutMilliseconds))
        Catch ex As Exception
        End Try
        Assert.IsTrue(task.Status = TaskStatus.Faulted)
    End Sub
    Public Shared Sub ExpectTaskToIdle(task As Task, Optional timeoutMilliseconds As Integer = 10)
        Assert.IsTrue(Not task.Wait(timeoutMilliseconds))
    End Sub

    <TestMethod()>
    Public Sub AsAggregateTaskTest_Raw()
        Dim taskSources = {New TaskCompletionSource(Of Boolean), New TaskCompletionSource(Of Boolean)}
        Dim tasks = From t In taskSources Select CType(t.Task, Task)
        Dim result = tasks.AsAggregateTask
        ExpectTaskToIdle(result)
        taskSources(0).SetResult(True)
        ExpectTaskToIdle(result)
        taskSources(1).SetResult(True)
        WaitForTaskToSucceed(result)
    End Sub
    <TestMethod()>
    Public Sub AsAggregateTaskTest_Empty()
        Dim tasks = New Task() {}
        Dim result = tasks.AsAggregateTask
        WaitForTaskToSucceed(result)
    End Sub
    <TestMethod()>
    Public Sub AsAggregateTaskTest_Fault()
        Dim taskSources = {New TaskCompletionSource(Of Boolean), New TaskCompletionSource(Of Boolean), New TaskCompletionSource(Of Boolean)}
        Dim tasks = From t In taskSources Select CType(t.Task, Task)
        Dim result = tasks.AsAggregateTask
        taskSources(0).SetException(New ArgumentException())
        taskSources(1).SetResult(True)
        taskSources(2).SetException(New InvalidOperationException())
        WaitForTaskToFault(result)
        Assert.IsTrue(result.Exception.InnerExceptions.Count = 2)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions(0) Is ArgumentException)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions(1) Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub AsAggregateTaskTest_Value()
        Dim taskSources = {New TaskCompletionSource(Of Int32), New TaskCompletionSource(Of Int32)}
        Dim tasks = From t In taskSources Select t.Task
        Dim result = tasks.AsAggregateTask
        ExpectTaskToIdle(result)
        taskSources(0).SetResult(1)
        ExpectTaskToIdle(result)
        taskSources(1).SetResult(2)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result.SequenceEqual({1, 2}))
    End Sub
    <TestMethod()>
    Public Sub AsTaskTest()
        Dim result = 1.AsTask
        Assert.IsTrue(result.Status = TaskStatus.RanToCompletion)
        Assert.IsTrue(result.Result = 1)
    End Sub

    <TestMethod()>
    Public Sub CatchTest_Dangle()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.Catch(Sub(exception) flag = False)
        ExpectTaskToIdle(task.Task)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub CatchTest_PreSucceed()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.Catch(Sub(exception) flag = False)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub CatchTest_Succeed()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.Catch(Sub(exception) flag = False)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub CatchTest_Fail()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.Catch(Sub(exception) flag = True)
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub CatchTest_Throw()
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.Catch(Sub(exception) Throw New InvalidOperationException("Mock Failure"))
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToFault(result)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub ContinueWithActionNoArgTest_Dangle()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithAction(Sub() flag = False)
        ExpectTaskToIdle(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionNoArgTest_PreSucceed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.ContinueWithAction(Sub() flag = True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionNoArgTest_Succeed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithAction(Sub() flag = True)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionNoArgTest_Fail()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithAction(Sub() flag = False)
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionNoArgTest_Throw()
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithAction(Sub() Throw New InvalidOperationException("Mock Failure"))
        task.SetResult(True)
        WaitForTaskToFault(result)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub ContinueWithFuncNoArgTest_Dangle()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithFunc(Function()
                                                    flag = False
                                                    Return 2
                                                End Function)
        ExpectTaskToIdle(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncNoArgTest_PreSucceed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Boolean)()
        task.SetResult(True)
        Dim result = task.Task.ContinueWithFunc(Function()
                                                    flag = True
                                                    Return 3
                                                End Function)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(result.Result = 3)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncNoArgTest_Succeed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithFunc(Function()
                                                    flag = True
                                                    Return 3
                                                End Function)
        task.SetResult(True)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(result.Result = 3)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncNoArgTest_Fail()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithFunc(Function()
                                                    flag = False
                                                    Return 4
                                                End Function)
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncNoArgTest_Throw()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Boolean)()
        Dim result = task.Task.ContinueWithFunc(Function() As Integer
                                                    Throw New InvalidOperationException("Mock Exception")
                                                End Function)
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToFault(result)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub ContinueWithActionWithArgTest_Dangle()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithAction(Sub(value) flag = False)
        ExpectTaskToIdle(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionWithArgTest_PreSucceed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Integer)()
        task.SetResult(11)
        Dim result = task.Task.ContinueWithAction(Sub(value) flag = value = 11)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionWithArgTest_Succeed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithAction(Sub(value) flag = value = 11)
        task.SetResult(11)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionWithArgTest_Fail()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithAction(Sub(value) flag = False)
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithActionWithArgTest_Throw()
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithAction(Sub(value) Throw New InvalidOperationException("Mock Failure"))
        task.SetResult(12)
        WaitForTaskToFault(result)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub ContinueWithFuncWithArgTest_Dangle()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.Select(Function(v)
                                          flag = False
                                          Return -1
                                      End Function)
        ExpectTaskToIdle(result)
        Assert.IsTrue(flag)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncWithArgTest_PreSucceed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Integer)()
        task.SetResult(21)
        Dim result = task.Task.ContinueWithFunc(Function(v)
                                                    flag = v = 21
                                                    Return -2
                                                End Function)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(result.Result = -2)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncWithArgTest_Succeed()
        Dim flag = False
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithFunc(Function(v)
                                                    flag = v = 21
                                                    Return -2
                                                End Function)
        task.SetResult(21)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(result.Result = -2)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncWithArgTest_Fail()
        Dim flag = True
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithFunc(Function(v)
                                                    flag = False
                                                    Return -3
                                                End Function)
        task.SetException(New InvalidOperationException("Mock Failure"))
        WaitForTaskToFault(result)
        Assert.IsTrue(flag)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub
    <TestMethod()>
    Public Sub ContinueWithFuncWithArgTest_Throw()
        Dim task = New TaskCompletionSource(Of Integer)()
        Dim result = task.Task.ContinueWithFunc(Function(v) As Integer
                                                    Throw New InvalidOperationException("Mock Failure")
                                                End Function)
        task.SetResult(22)
        WaitForTaskToFault(result)
        Assert.IsTrue(TypeOf result.Exception.InnerExceptions.Single Is InvalidOperationException)
    End Sub

    <TestMethod()>
    Public Sub LinqSelectTest()
        Dim task = New TaskCompletionSource(Of Int32)()
        Dim result = From value In task.Task
                     Select value + 1
        ExpectTaskToIdle(result)
        task.SetResult(5)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result = 6)
    End Sub
    <TestMethod()>
    Public Sub LinqSelectManyTest()
        Dim task2 = New TaskCompletionSource(Of Int32)()
        Dim task1 = New TaskCompletionSource(Of Task(Of Int32))()
        Dim result = From task In task1.Task
                     From value In task
                     Select value + 5
        ExpectTaskToIdle(result)
        task1.SetResult(task2.Task)
        task2.SetResult(6)
        WaitForTaskToSucceed(result)
        Assert.IsTrue(result.Result = 11)
    End Sub
End Class
