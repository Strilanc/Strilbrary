Imports Strilbrary.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Time

<TestClass()>
Public Class ThreadingExtensionsTest
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
    Public Sub AwaitableEntranceTest()
        Dim q1 = MakeThreadPooledCallQueue()
        Dim q2 = MakeThreadPooledCallQueue()
        Dim r = Async Function()
                    Try
                        Dim e = Sub()
                                End Sub
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is Nothing)

                        Dim t = q1.QueueAction(e)
                        WaitForTaskToSucceed(t)
                        Await q1.AwaitableEntrance(forceReentry:=False)
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is q1)

                        t = q1.QueueAction(e)
                        ExpectTaskToIdle(t)
                        Await q1.AwaitableEntrance(forceReentry:=False)
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is q1)
                        ExpectTaskToIdle(t)
                        Await q1.AwaitableEntrance(forceReentry:=True)
                        WaitForTaskToSucceed(t)

                        t = q1.QueueAction(e)
                        ExpectTaskToIdle(t)
                        Await q2.AwaitableEntrance(forceReentry:=False)
                        Assert.IsTrue(Threading.SynchronizationContext.Current Is q2)
                        WaitForTaskToSucceed(t)
                        Return Nothing
                    Catch ex As Exception
                        Return ex
                    End Try
                End Function().Result
        If r IsNot Nothing Then Throw New AssertFailedException("", r)
    End Sub
End Class
