Imports Strilbrary.Values
Imports Strilbrary.Exceptions
Imports System.Threading

Namespace Threading
    '''<summary>A thread-safe queue for running actions in order.</summary>
    Public Class CallQueue
        Inherits TaskScheduler

        Private ReadOnly _consumerQueue As LockFreeConsumer(Of Task)
        Private ReadOnly _startedLock As New OnetimeLock
        Private ReadOnly _primedLock As New OnetimeLock

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_consumerQueue IsNot Nothing)
            Contract.Invariant(_startedLock IsNot Nothing)
            Contract.Invariant(_primedLock IsNot Nothing)
        End Sub

        Public Sub New(ByVal initiallyStarted As Boolean)
            Me._consumerQueue = New LockFreeConsumer(Of Task)(runner:=AddressOf StartOrBeginConsuming,
                                                              consumer:=Sub(task)
                                                                            Dim executed = TryExecuteTask(task)
                                                                            Contract.Assume(executed)
                                                                        End Sub)
            If initiallyStarted Then
                _startedLock.TryAcquire()
                _primedLock.TryAcquire()
            End If
        End Sub
        Public Sub Start()
            If _startedLock.TryAcquire Then StartOrBeginConsuming()
        End Sub

        Private Sub StartOrBeginConsuming()
            '[The first two calls will be from Start and from the LockFreeConsumer getting its first item.]
            '[We have to wait for both to have occurred before continuing.]
            If _primedLock.TryAcquire Then Return

            BeginConsuming(AddressOf _consumerQueue.Run)
        End Sub
        Protected Overridable Sub BeginConsuming(ByVal action As Action)
            Contract.Requires(action IsNot Nothing)
            Call action()
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

    '''<summary>Queues calls on a control's synchronization context.</summary>
    Public NotInheritable Class ControlCallQueue
        Inherits CallQueue
        Private ReadOnly _eventualContext As Task(Of SynchronizationContext)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_eventualContext IsNot Nothing)
        End Sub

        Public Sub New(ByVal control As Control, ByVal initiallyStarted As Boolean)
            MyBase.New(initiallyStarted)
            Contract.Requires(control IsNot Nothing)
            Me._eventualContext = control.EventualSynchronizationContextAsync()
        End Sub
        Protected Overrides Sub BeginConsuming(ByVal action As Action)
            _eventualContext.ContinueWithAction(Sub(context) context.Post(Sub() action(), Nothing))
        End Sub
    End Class
    '''<summary>Runs queued calls on an independent thread.</summary>
    Public NotInheritable Class ThreadedCallQueue
        Inherits CallQueue
        Public Sub New(Optional ByVal initiallyStarted As Boolean = True)
            MyBase.New(initiallyStarted)
        End Sub
        Protected Overrides Sub BeginConsuming(ByVal action As Action)
            ThreadedAction(action)
        End Sub
    End Class
    '''<summary>Runs queued calls on the thread pool.</summary>
    Public NotInheritable Class ThreadPooledCallQueue
        Inherits CallQueue
        Public Sub New(Optional ByVal initiallyStarted As Boolean = True)
            MyBase.New(initiallyStarted)
        End Sub
        Protected Overrides Sub BeginConsuming(ByVal action As Action)
            ThreadPooledAction(action)
        End Sub
    End Class
    '''<summary>Runs queued calls as a task.</summary>
    Public NotInheritable Class TaskedCallQueue
        Inherits CallQueue
        Public Sub New(Optional ByVal initiallyStarted As Boolean = True)
            MyBase.New(initiallyStarted)
        End Sub
        Protected Overrides Sub BeginConsuming(ByVal action As Action)
            TaskedAction(action)
        End Sub
    End Class
End Namespace
