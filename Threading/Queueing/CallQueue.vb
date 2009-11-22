Namespace Threading
    '''<summary>Represents a thread-safe call queue.</summary>
    <ContractClass(GetType(ICallQueue.ContractClass))>
    Public Interface ICallQueue
        '''<summary>Queues an action to be run and returns a future for the action's eventual completion.</summary>
        Function QueueAction(ByVal action As Action) As IFuture

        <ContractClassFor(GetType(ICallQueue))>
        Class ContractClass
            Implements ICallQueue
            Public Function QueueAction(ByVal action As Action) As IFuture Implements ICallQueue.QueueAction
                Contract.Requires(action IsNot Nothing)
                Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
                Throw New NotSupportedException
            End Function
        End Class
    End Interface

    Public Module ExtensionsForICallQueue
        '''<summary>Queues a function to be run and returns a future for the function's eventual output.</summary>
        <Extension()>
        Public Function QueueFunc(Of TReturn)(ByVal queue As ICallQueue,
                                              ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)

            Dim result = New FutureFunction(Of TReturn)
            queue.QueueAction(Sub() result.SetByEvaluating(func))
            Return result
        End Function
    End Module

    '''<summary>A thread-safe queue for running actions in order.</summary>
    Public MustInherit Class BaseCallQueue
        Inherits BaseLockFreeConsumer(Of Node)
        Implements ICallQueue
        Public NotInheritable Class Node
            Private ReadOnly _action As Action
            Private ReadOnly _future As New FutureAction

            Public ReadOnly Property Action As Action
                Get
                    Contract.Ensures(Contract.Result(Of Action)() IsNot Nothing)
                    Return _action
                End Get
            End Property
            Public ReadOnly Property Future As FutureAction
                Get
                    Contract.Ensures(Contract.Result(Of FutureAction)() IsNot Nothing)
                    Return _future
                End Get
            End Property
            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_action IsNot Nothing)
                Contract.Invariant(_future IsNot Nothing)
            End Sub

            Public Sub New(ByVal action As Action)
                Contract.Requires(action IsNot Nothing)
                Me._action = action
            End Sub
        End Class

        '''<summary>Enqueues an action to be run and returns a future for the action's eventual completion.</summary>
        Public Function QueueAction(ByVal action As Action) As IFuture Implements ICallQueue.QueueAction
            Dim item = New Node(action)
            EnqueueConsume(item)
            Return item.Future
        End Function

        '''<summary>Processes a queued action.</summary>
        Protected NotOverridable Overrides Sub Consume(ByVal item As Node)
            Contract.Assume(item IsNot Nothing)
            item.Future.SetByCalling(item.Action)
        End Sub
    End Class

    '''<summary>Runs queued calls on a control's thread.</summary>
    Public NotInheritable Class InvokedCallQueue
        Inherits BaseCallQueue
        Private ReadOnly control As Control

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(control IsNot Nothing)
        End Sub

        Public Sub New(ByVal control As Control)
            Contract.Requires(control IsNot Nothing)
            Me.control = control
        End Sub
        Protected Overrides Sub StartRunning()
            Try
                control.BeginInvoke(Sub() Run())
            Catch ex As InvalidOperationException
                ex.RaiseAsUnexpected("Invalid Invoke from {0}.StartRunning() ({1}, {2})".
                                        Frmt(Me.GetType.Name, control.GetType.Name, control.Name))
            End Try
        End Sub
    End Class

    '''<summary>Runs queued calls on an independent thread.</summary>
    Public NotInheritable Class ThreadedCallQueue
        Inherits BaseCallQueue
        Protected Overrides Sub StartRunning()
            ThreadedAction(Sub() Run())
        End Sub
    End Class

    '''<summary>Runs queued calls on the thread pool.</summary>
    Public NotInheritable Class ThreadPooledCallQueue
        Inherits BaseCallQueue
        Protected Overrides Sub StartRunning()
            ThreadPooledAction(Sub() Run())
        End Sub
    End Class

    '''<summary>Runs queued calls as a task.</summary>
    Public NotInheritable Class TaskedCallQueue
        Inherits BaseCallQueue
        Protected Overrides Sub StartRunning()
            Contract.Assume(System.Threading.Tasks.Task.Factory IsNot Nothing)
            System.Threading.Tasks.Task.Factory.StartNew(Sub() Run())
        End Sub
    End Class

    '''<summary>Queues calls on a subQueue, but prevents actions from running until the queue is started.</summary>
    Public Class StartableCallQueue
        Inherits BaseCallQueue
        Private ReadOnly subQueue As ICallQueue
        Private ReadOnly lock As New OnetimeLock

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(subQueue IsNot Nothing)
            Contract.Invariant(lock IsNot Nothing)
        End Sub

        Public Sub New(ByVal subQueue As ICallQueue)
            Contract.Requires(subQueue IsNot Nothing)
            Me.subQueue = subQueue
        End Sub

        '''<summary>Allows queued actions to begin to run.</summary>
        Public Sub Start()
            If lock.TryAcquire Then Return
            StartRunning()
        End Sub

        Protected Overrides Sub StartRunning()
            If lock.TryAcquire Then Return
            subQueue.QueueAction(Sub() Me.Run())
        End Sub
    End Class
End Namespace
