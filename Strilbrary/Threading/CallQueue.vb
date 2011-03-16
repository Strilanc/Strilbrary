Imports Strilbrary.Values
Imports Strilbrary.Exceptions
Imports System.Threading

Namespace Threading
    '''<summary>Runs queued actions in order within a synchronization context, exposing the results as tasks.</summary>
    Public NotInheritable Class CallQueue
        Inherits TaskScheduler

        Private ReadOnly _consumerQueue As LockFreeConsumer(Of Task)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_consumerQueue IsNot Nothing)
        End Sub

        Public Sub New(context As SynchronizationContext)
            Contract.Requires(context IsNot Nothing)
            Me._consumerQueue = New LockFreeConsumer(Of Task)(
                context:=context,
                consumer:=Sub(task)
                              Dim executed = TryExecuteTask(task)
                              Contract.Assume(executed)
                          End Sub)
        End Sub

        Protected Overrides Function GetScheduledTasks() As IEnumerable(Of Task)
            Return _consumerQueue
        End Function
        Protected Overrides Sub QueueTask(task As Task)
            _consumerQueue.EnqueueConsume(task)
        End Sub
        Protected Overrides Function TryExecuteTaskInline(task As Task, taskWasPreviouslyQueued As Boolean) As Boolean
            Return False
        End Function

        '''<summary>Enqueues an action to be run and exposes it as a task.</summary>
        Public Function QueueAction(action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim task = New Task(action)
            task.Start(Me)
            Return task
        End Function
        '''<summary>Enqueues a function to be run and exposes it as a task.</summary>
        Public Function QueueFunc(Of TReturn)(func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim task = New Task(Of TReturn)(func)
            task.Start(Me)
            Return task
        End Function
    End Class
End Namespace
