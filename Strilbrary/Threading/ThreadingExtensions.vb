Imports System.Threading
Imports Strilbrary.Values

Namespace Threading
    'Verification disabled due to missing task contracts
    <ContractVerification(False)>
    Public Module ThreadingExtensions
        '''<summary>Determines a task for running an action in a new thread.</summary>
        Public Function ThreadedAction(ByVal action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            Call New Thread(Sub() result.SetByCalling(action)).Start()
            Return result.Task
        End Function
        '''<summary>Determines a task value for running a function in a new thread.</summary>
        Public Function ThreadedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            Call New Thread(Sub() result.SetByEvaluating(func)).Start()
            Return result.Task
        End Function

        '''<summary>Determines a task for running an action as a task.</summary>
        Public Function TaskedAction(ByVal action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return Task.Factory.StartNew(action)
        End Function
        '''<summary>Determines a task value for running a function as a task.</summary>
        Public Function TaskedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            Contract.Assume(Tasks.Task.Factory IsNot Nothing)
            Tasks.Task.Factory.StartNew(Sub() result.SetByEvaluating(func))
            Return result.Task
        End Function

        '''<summary>Determines a task for running an action in the thread pool.</summary>
        Public Function ThreadPooledAction(ByVal action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            ThreadPool.QueueUserWorkItem(Sub() result.SetByCalling(action))
            Return result.Task
        End Function
        '''<summary>Determines a task value for running a function in the thread pool.</summary>
        Public Function ThreadPooledFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            ThreadPool.QueueUserWorkItem(Sub() result.SetByEvaluating(func))
            Return result.Task
        End Function

        '''<summary>Determines a task for invoking an action on a control's thread.</summary>
        <Extension()>
        Public Function AsyncInvokedAction(ByVal control As Control, ByVal action As Action) As Task
            Contract.Requires(control IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            result.DependentCall(Sub() control.BeginInvoke(Sub() result.SetByCalling(action)))
            Return result.Task
        End Function
        '''<summary>Determines a task value for invoking a function on a control's thread.</summary>
        <Extension()>
        Public Function AsyncInvokedFunc(Of TReturn)(ByVal control As Control, ByVal func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(control IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            result.DependentCall(Sub() control.BeginInvoke(Sub() result.SetByEvaluating(func)))
            Return result.Task
        End Function

        '''<summary>Creates a continuation which executes on a queue if a task succeeds.</summary>
        <Extension()>
        Public Function QueueContinueWithAction(ByVal task As Task,
                                                ByVal queue As CallQueue,
                                                ByVal action As Action) As Task
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return task.ContinueWithFunc(Function() queue.QueueAction(action)).Unwrap
        End Function
        '''<summary>Creates a continuation which executes on a queue if a task succeeds.</summary>
        <Extension()>
        Public Function QueueContinueWithAction(Of TArg)(ByVal task As Task(Of TArg),
                                                         ByVal queue As CallQueue,
                                                         ByVal action As Action(Of TArg)) As Task
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return task.ContinueWithFunc(Function(result) queue.QueueAction(Sub() action(result))).Unwrap
        End Function
        '''<summary>Creates a continuation which executes on a queue if a task succeeds.</summary>
        <Extension()>
        Public Function QueueContinueWithFunc(Of TReturn)(ByVal task As Task,
                                                          ByVal queue As CallQueue,
                                                          ByVal func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Return task.ContinueWithFunc(Function() queue.QueueFunc(func)).Unwrap
        End Function
        '''<summary>Creates a continuation which executes on a queue if a task succeeds.</summary>
        <Extension()>
        Public Function QueueContinueWithFunc(Of TArg, TReturn)(ByVal task As Task(Of TArg),
                                                                ByVal queue As CallQueue,
                                                                ByVal func As Func(Of TArg, TReturn)) As Task(Of TReturn)
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Return task.ContinueWithFunc(Function(result) queue.QueueFunc(Function() func(result))).Unwrap
        End Function
        '''<summary>Creates a continuation which executes on a queue if a task faults.</summary>
        <Extension()>
        Public Function QueueCatch(ByVal task As Task,
                                   ByVal queue As CallQueue,
                                   ByVal action As Action(Of AggregateException)) As Task
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return task.ContinueWith(Function(t) queue.QueueAction(Sub() If t.Status = TaskStatus.Faulted Then action(t.Exception)),
                                     TaskContinuationOptions.NotOnCanceled).Unwrap
        End Function
    End Module
End Namespace
