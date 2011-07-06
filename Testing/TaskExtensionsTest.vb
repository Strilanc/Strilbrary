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
    Public Sub AsTaskTest()
        Dim result = 1.AsTask
        Assert.IsTrue(result.Status = TaskStatus.RanToCompletion)
        Assert.IsTrue(result.Result = 1)
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
