Imports Strilbrary.Values
Imports Strilbrary.Exceptions
Imports System.Threading

Namespace Threading
    '''<summary>A thread-safe queue for running actions in order.</summary>
    Public NotInheritable Class CallQueue
        Inherits TaskScheduler

        Private ReadOnly _consumerQueue As LockFreeConsumer(Of Task)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_consumerQueue IsNot Nothing)
        End Sub

        Public Sub New(ByVal context As SynchronizationContext)
            Contract.Requires(context IsNot Nothing)
            Me._consumerQueue = New LockFreeConsumer(Of Task)(
                context:=context,
                consumer:=Sub(task)
                              Dim executed = TryExecuteTask(task)
                              Contract.Assume(executed)
                          End Sub)
        End Sub

        Protected Overrides Function GetScheduledTasks() As IEnumerable(Of Task)
            Return _consumerQueue.AsEnumerable
        End Function
        Protected Overrides Sub QueueTask(ByVal task As Task)
            _consumerQueue.EnqueueConsume(task)
        End Sub
        Protected Overrides Function TryExecuteTaskInline(ByVal task As Task, ByVal taskWasPreviouslyQueued As Boolean) As Boolean
            Return False
        End Function

        '''<summary>Enqueues an action to be run and exposes it as a task.</summary>
        Public Function QueueAction(ByVal action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of task)() IsNot Nothing)
            Dim task = New Task(action)
            task.Start(Me)
            Return task
        End Function
        '''<summary>Enqueues a function to be run and exposes it as a task.</summary>
        Public Function QueueFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim task = New Task(Of TReturn)(func)
            task.Start(Me)
            Return task
        End Function
    End Class
End Namespace
