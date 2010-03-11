Imports Strilbrary.Values
Imports Strilbrary.Exceptions

Namespace Threading
    '''<summary>A thread-safe queue for running actions in order.</summary>
    Public Class CallQueue
        Inherits TaskScheduler

        Private ReadOnly _consumerQueue As LockFreeConsumer(Of Task)
        Private ReadOnly _startedLock As New OnetimeLock
        Private ReadOnly _runner As Action(Of Action)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_consumerQueue IsNot Nothing)
            Contract.Invariant(_startedLock IsNot Nothing)
            Contract.Invariant(_runner IsNot Nothing)
        End Sub

        Public Sub New(ByVal runner As Action(Of Action), ByVal initiallyStarted As Boolean)
            Contract.Requires(runner IsNot Nothing)
            If initiallyStarted Then _startedLock.TryAcquire()
            Me._runner = runner
            Me._consumerQueue = New LockFreeConsumer(Of Task)(runner:=AddressOf Start,
                                                              consumer:=Sub(task)
                                                                            Dim executed = TryExecuteTask(task)
                                                                            Contract.Assume(executed)
                                                                        End Sub)
        End Sub
        Public Sub Start()
            '[Ignores the first call, which is either consumption ready before user start or user start before consumption ready]
            If _startedLock.TryAcquire Then Return

            Call _runner(AddressOf _consumerQueue.Run)
        End Sub

        Protected NotOverridable Overrides Function GetScheduledTasks() As IEnumerable(Of Task)
            Return _consumerQueue.AsEnumerable
        End Function
        Protected NotOverridable Overrides Sub QueueTask(ByVal task As Task)
            _consumerQueue.EnqueueConsume(task)
        End Sub
        Protected NotOverridable Overrides Function TryExecuteTaskInline(ByVal task As Task, ByVal taskWasPreviouslyQueued As Boolean) As Boolean
            Return False
        End Function
        Protected NotOverridable Overrides Function TryDequeue(ByVal task As Task) As Boolean
            Return MyBase.TryDequeue(task)
        End Function
        Public NotOverridable Overrides ReadOnly Property MaximumConcurrencyLevel As Integer
            Get
                Return MyBase.MaximumConcurrencyLevel
            End Get
        End Property

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

    '''<summary>Runs queued calls on a control's thread.</summary>
    Public NotInheritable Class InvokedCallQueue
        Inherits CallQueue
        Public Sub New(ByVal control As Control, ByVal initiallyStarted As Boolean)
            MyBase.New(Sub(action) control.AsyncInvokedAction(action).Catch(
                    Sub(ex) ex.RaiseAsUnexpected("Invalid Invoke from {0}.StartRunning() ({1}, {2})".Frmt(GetType(InvokedCallQueue).Name,
                                                                                                          control.GetType.Name,
                                                                                                          control.Name))), initiallyStarted)
            Contract.Requires(control IsNot Nothing)
        End Sub
    End Class
    '''<summary>Runs queued calls on an independent thread.</summary>
    Public NotInheritable Class ThreadedCallQueue
        Inherits CallQueue
        Public Sub New(Optional ByVal initiallyStarted As Boolean = True)
            MyBase.New(Sub(action) ThreadedAction(action), initiallyStarted)
        End Sub
    End Class
    '''<summary>Runs queued calls on the thread pool.</summary>
    Public NotInheritable Class ThreadPooledCallQueue
        Inherits CallQueue
        Public Sub New(Optional ByVal initiallyStarted As Boolean = True)
            MyBase.New(Sub(action) ThreadPooledAction(action), initiallyStarted)
        End Sub
    End Class
    '''<summary>Runs queued calls as a task.</summary>
    Public NotInheritable Class TaskedCallQueue
        Inherits CallQueue
        Public Sub New(Optional ByVal initiallyStarted As Boolean = True)
            MyBase.New(Sub(action) TaskedAction(action), initiallyStarted)
        End Sub
    End Class
End Namespace