Imports Strilbrary.Values
Imports Strilbrary.Exceptions
Imports System.Threading

Namespace Threading
    '''<summary>Runs queued actions in order within a synchronization context, exposing the results as tasks.</summary>
    Public NotInheritable Class CallQueue
        Inherits SynchronizationContext

        Private ReadOnly _consumerQueue As LockFreeConsumer(Of Action)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_consumerQueue IsNot Nothing)
        End Sub

        Public Sub New(context As SynchronizationContext)
            Contract.Requires(context IsNot Nothing)
            Me._consumerQueue = New LockFreeConsumer(Of Action)(
                context:=context,
                consumer:=Sub(action)
                              SynchronizationContext.SetSynchronizationContext(Me)
                              action()
                          End Sub)
        End Sub

        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            _consumerQueue.EnqueueConsume(Sub() d(state))
        End Sub
        Public Overrides Function CreateCopy() As SynchronizationContext
            Return Me
        End Function

        '''<summary>Enqueues an action to be run and exposes it as a task.</summary>
        Public Function QueueAction(action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim r = New TaskCompletionSource(Of NoValue)
            Post(Sub()
                     action()
                     r.SetResult(Nothing)
                 End Sub, Nothing)
            Contract.Assume(r.Task IsNot Nothing)
            Return r.Task
        End Function
        '''<summary>Enqueues a function to be run and exposes it as a task.</summary>
        Public Function QueueFunc(Of TReturn)(func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim r = New TaskCompletionSource(Of TReturn)
            Post(Sub()
                     r.SetResult(func())
                 End Sub, Nothing)
            Contract.Assume(r.Task IsNot Nothing)
            Return r.Task
        End Function
    End Class
End Namespace
