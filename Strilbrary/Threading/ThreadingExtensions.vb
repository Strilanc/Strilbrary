Imports Strilbrary.Values

Namespace Threading
    '''<remarks>Verification disabled due to missing task contracts</remarks>
    Public Module ThreadingExtensions
        ''' <summary>
        ''' Returns the eventual synchronization context of a control.
        ''' The context is available once the control has a handle.
        ''' The context will hang if the control will never have a handle (eg. the handle has been destroyed).
        ''' </summary>
        ''' <remarks>
        ''' Must not be called while the control handle is being created or destroyed.
        ''' </remarks>
        <Extension()>
        Public Function EventualSynchronizationContext(control As Control) As SynchronizationContext
            Contract.Requires(control IsNot Nothing)
            Contract.Ensures(Contract.Result(Of SynchronizationContext)() IsNot Nothing)

            Dim result = New TaskCompletionSource(Of SynchronizationContext)

            If Not control.IsHandleCreated Then
                AddHandler control.HandleCreated, Sub() result.TrySetResult(SynchronizationContext.Current)
            ElseIf control.InvokeRequired Then
                control.BeginInvoke(Sub() result.SetResult(SynchronizationContext.Current))
            Else
                Contract.Assume(SynchronizationContext.Current IsNot Nothing)
                Return SynchronizationContext.Current
            End If

            Contract.Assume(result.Task IsNot Nothing)
            Return New EventualSynchronizationContext(result.Task)
        End Function

        Public Function MakeControlCallQueue(control As Control) As CallQueue
            Contract.Requires(control IsNot Nothing)
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(control.EventualSynchronizationContext())
        End Function
        <Pure()>
        Public Function MakeThreadedCallQueue() As CallQueue
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(New ThreadedSynchronizationContext())
        End Function
        <Pure()>
        Public Function MakeTaskedCallQueue() As CallQueue
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(New TaskedSynchronizationContext())
        End Function
        <Pure()>
        Public Function MakeThreadPooledCallQueue() As CallQueue
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(New ThreadPooledSynchronizationContext())
        End Function

        '''<summary>Returns a Task object which has already RanToCompletion.</summary>
        <Pure()>
        Public Function CompletedTask() As Task
            Dim result = New TaskCompletionSource(Of NoValue)
            result.SetResult(Nothing)
            Contract.Assume(result.Task IsNot Nothing)
            Return result.Task
        End Function

        '''<summary>Wraps a value in an instantly completed task.</summary>
        <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task(Of TValue))().Status = TaskStatus.RanToCompletion")>
        Public Function AsTask(Of TValue)(value As TValue) As Task(Of TValue)
            Contract.Ensures(Contract.Result(Of Task(Of TValue))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TValue))().Status = TaskStatus.RanToCompletion)
            Dim result = New TaskCompletionSource(Of TValue)
            result.SetResult(value)
            Contract.Assume(result.Task IsNot Nothing)
            Return result.Task
        End Function

        '''<summary>Causes a task completion source to succeed with the result of a function, or to fault if the function throws an exception.</summary>
        <Extension()>
        Public Sub SetByEvaluating(Of T)(taskSource As TaskCompletionSource(Of T), func As Func(Of T))
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            taskSource.DependentCall(Sub() taskSource.SetResult(func()))
        End Sub
        '''<summary>Causes a task completion source to succeed if an action runs, or to fault if the action throws an exception.</summary>
        <Extension()>
        Public Sub SetByCalling(taskSource As TaskCompletionSource(Of NoValue), action As action)
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            taskSource.DependentCall(Sub()
                                         Call action()
                                         taskSource.SetResult(Nothing)
                                     End Sub)
        End Sub
        '''<summary>Causes a task completion source to fault if running an action throws an exception.</summary>
        <Extension()>
        <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="The exception is propagated into the task.")>
        Public Sub DependentCall(Of T)(taskSource As TaskCompletionSource(Of T), action As action)
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Try
                Call action()
            Catch ex As AggregateException
                If ex.InnerExceptions.Count <> 1 Then
                    taskSource.SetException(ex.InnerExceptions)
                ElseIf TypeOf ex.InnerExceptions(0) Is TaskCanceledException Then
                    taskSource.SetCanceled()
                Else
                    taskSource.SetException(ex.InnerExceptions(0))
                End If
            Catch ex As TaskCanceledException
                taskSource.SetCanceled()
            Catch ex As Exception
                taskSource.SetException(ex)
            End Try
        End Sub

        '''<summary>Enqueues an action to be run and exposes it as a task.</summary>
        <Extension()>
        Public Function QueueAction(this As CallQueue, action As Action) As Task
            Contract.Requires(this IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim r = New TaskCompletionSource(Of NoValue)
            this.Post(Sub() r.SetByCalling(action), Nothing)
            Contract.Assume(r.Task IsNot Nothing)
            Return r.Task
        End Function
        '''<summary>Enqueues a function to be run and exposes it as a task.</summary>
        <Extension()>
        Public Function QueueFunc(Of TReturn)(this As CallQueue, func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(this IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim r = New TaskCompletionSource(Of TReturn)
            this.Post(Sub() r.SetByEvaluating(func), Nothing)
            Contract.Assume(r.Task IsNot Nothing)
            Return r.Task
        End Function

        Public Structure ContextAwaiter
            Implements INotifyCompletion
            Private ReadOnly _context As SynchronizationContext
            Private ReadOnly _forceReentry As Boolean
            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_context IsNot Nothing)
            End Sub
            Public Sub New(context As SynchronizationContext, forceReentry As Boolean)
                Contract.Requires(context IsNot Nothing)
                Me._context = context
                Me._forceReentry = forceReentry
            End Sub
            <SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification:="Required to be an awaitable type.")>
            Public Function GetAwaiter() As ContextAwaiter
                Return Me
            End Function
            Public ReadOnly Property IsCompleted As Boolean
                Get
                    Return Not _forceReentry AndAlso SynchronizationContext.Current Is _context
                End Get
            End Property
            Public Sub OnCompleted(continuation As Action) Implements INotifyCompletion.OnCompleted
                _context.Post(Sub() continuation(), Nothing)
            End Sub
            <SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification:="Required to be an awaiter type.")>
            Public Sub GetResult()
            End Sub
        End Structure
        '''<summary>Returns an awaitable object that, when await-ed, resumes execution within the given context.</summary>        
        '''<param name="forceReentry">Determines if awaiting the result completes immediately when within the desired context.</param>
        <Extension()> <Pure()>
        Public Function AwaitableEntrance(context As SynchronizationContext, Optional forceReentry As Boolean = True) As ContextAwaiter
            Contract.Requires(context IsNot Nothing)
            Return New ContextAwaiter(context, forceReentry)
        End Function
    End Module
End Namespace
