Imports Strilbrary.Values
Imports Strilbrary.Exceptions
Imports System.Threading

Namespace Threading
    Public Module TaskExtensions
        '''<summary>Wraps a value in an instantly completed task.</summary>
        <Extension()>
        <ContractVerification(False)>
        Public Function AsTask(Of TValue)(ByVal value As TValue) As Task(Of TValue)
            Contract.Ensures(Contract.Result(Of Task(Of TValue))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TValue))().Status = TaskStatus.RanToCompletion)
            Dim result = New TaskCompletionSource(Of TValue)
            result.SetResult(value)
            Return result.Task
        End Function
        '''<summary>
        ''' Returns a task which completes once all tasks in a sequence have completed.
        ''' The result faults if any of the sequence tasks fault.
        '''</summary>
        <Extension()>
        <ContractVerification(False)>
        Public Function AsAggregateTask(ByVal sequence As IEnumerable(Of Task)) As Task
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)

            Dim tasks = sequence.ToList
            Dim result = New TaskCompletionSource(Of NoValue)
            Dim readyCount = 0

            'Become ready once all input futures are ready
            Dim notify = Sub(task As task)
                             If Interlocked.Increment(readyCount) < tasks.Count Then Return

                             Dim faults = (From t In tasks
                                           Where t.Status = TaskStatus.Faulted
                                           From e In t.Exception.InnerExceptions
                                           Select e
                                           ).ToList
                             If faults.Count > 0 Then
                                 result.SetException(faults)
                             Else
                                 result.SetResult(Nothing)
                             End If
                         End Sub

            For Each task In tasks
                Contract.Assume(task IsNot Nothing)
                task.ContinueWith(notify)
            Next task
            If tasks.Count = 0 Then result.SetResult(Nothing)

            Return result.Task
        End Function
        '''<summary>
        ''' Converts a sequence of tasks into a task for the eventual complete sequence.
        ''' The result faults if any of the sequence's tasks fault.
        '''</summary>
        <Extension()>
        Public Function AsAggregateTask(Of TValue)(ByVal sequence As IEnumerable(Of Task(Of TValue))) As Task(Of IEnumerable(Of TValue))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of IEnumerable(Of TValue)))() IsNot Nothing)
            Dim tasks = sequence.ToList
            Return DirectCast(tasks, IEnumerable(Of Task)).AsAggregateTask.ContinueWithFunc(Function() (From task In tasks Select task.Result).ToList.AsEnumerable)
        End Function

        '''<summary>Causes a task completion source to succeed with the result of a function, or to fault if the function throws an exception.</summary>
        <Extension()>
        Public Sub SetByEvaluating(Of T)(ByVal taskSource As TaskCompletionSource(Of T), ByVal func As Func(Of T))
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            taskSource.DependentCall(Sub() taskSource.SetResult(func()))
        End Sub
        '''<summary>Causes a task completion source to succeed if an action runs, or to fault if the action throws an exception.</summary>
        <Extension()>
        Public Sub SetByCalling(ByVal taskSource As TaskCompletionSource(Of NoValue), ByVal action As action)
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            taskSource.DependentCall(Sub()
                                         Call action()
                                         taskSource.SetResult(Nothing)
                                     End Sub)
        End Sub
        '''<summary>Causes a task completion source to fault if running an action throws an exception.</summary>
        <Extension()>
        <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public Sub DependentCall(Of T)(ByVal taskSource As TaskCompletionSource(Of T), ByVal action As action)
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Try
                Call action()
            Catch ex As Exception
                taskSource.SetException(ex)
            End Try
        End Sub
        <Extension()>
        Private Function PropagateFaultsFrom(Of T)(ByVal taskSource As TaskCompletionSource(Of T), ByVal task As Task) As Boolean
            Contract.Requires(taskSource IsNot Nothing)
            Contract.Requires(task IsNot Nothing)
            Select Case task.Status
                Case TaskStatus.Canceled
                    taskSource.SetCanceled()
                    Return True
                Case TaskStatus.Faulted
                    Contract.Assume(task.Exception IsNot Nothing)
                    taskSource.SetException(task.Exception.InnerExceptions)
                    Return True
                Case TaskStatus.RanToCompletion
                    Return False
                Case Else
                    Throw task.Status.MakeImpossibleValueException()
            End Select
        End Function

        '''<summary>Creates a continuation which executes if a task succeeds, and propagates exceptions if it faults.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function ContinueWithAction(ByVal task As Task,
                                           ByVal action As action) As Task
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            task.ContinueWith(Sub(t) If Not result.PropagateFaultsFrom(t) Then result.SetByCalling(action))
            Return result.Task
        End Function
        '''<summary>Creates a continuation which executes if a task succeeds, and propagates exceptions if it faults.</summary>
        <Extension()> <Pure()>
        Public Function ContinueWithAction(Of TInput)(ByVal task As Task(Of TInput),
                                                      ByVal action As Action(Of TInput)) As Task
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of task)() IsNot Nothing)
            Return task.ContinueWithAction(Sub() action(task.Result))
        End Function
        '''<summary>Creates a continuation which executes if a task succeeds, and propagates exceptions if it faults.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function ContinueWithFunc(Of TResult)(ByVal task As Task,
                                                     ByVal func As Func(Of TResult)) As Task(Of TResult)
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TResult))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TResult)
            task.ContinueWith(Sub(t) If Not result.PropagateFaultsFrom(t) Then result.SetByEvaluating(func))
            Return result.Task
        End Function
        '''<summary>Creates a continuation which executes if a task succeeds, and propagates exceptions if it faults.</summary>
        <Extension()> <Pure()>
        Public Function ContinueWithFunc(Of TInput, TResult)(ByVal task As Task(Of TInput),
                                                             ByVal func As Func(Of TInput, TResult)) As Task(Of TResult)
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TResult))() IsNot Nothing)
            Return task.ContinueWithFunc(Function() func(task.Result))
        End Function
        '''<summary>Creates a continuation which executes if a task faults, and propagates success if it succeeds.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function [Catch](ByVal task As Task,
                                ByVal action As Action(Of AggregateException)) As Task
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return task.ContinueWith(Sub(t) If t.Status = TaskStatus.Faulted Then action(t.Exception), TaskContinuationOptions.NotOnCanceled)
        End Function

        '''<summary>Creates a continuation which executes if a task succeeds, and propagates exceptions if it faults.</summary>
        '''<remarks>Linq provider for tasks.</remarks>
        <Extension()>
        Public Function [Select](Of TArg, TResult)(ByVal task As Task(Of TArg),
                                                   ByVal func As Func(Of TArg, TResult)) As Task(Of TResult)
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TResult))() IsNot Nothing)
            Return task.ContinueWithFunc(func)
        End Function
        '''<summary>Creates a chain of continuations which execute if a task succeeds, and propagates exceptions if it faults.</summary>
        '''<remarks>Linq provider for tasks.</remarks>
        <Extension()>
        <ContractVerification(False)>
        Public Function SelectMany(Of TArg, TMid, TReturn)(ByVal task As Task(Of TArg),
                                                           ByVal projection1 As Func(Of TArg, Task(Of TMid)),
                                                           ByVal projection2 As Func(Of TArg, TMid, TReturn)) As Task(Of TReturn)
            Contract.Requires(task IsNot Nothing)
            Contract.Requires(projection1 IsNot Nothing)
            Contract.Requires(projection2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Return task.Select(Function(value1) projection1(value1).
                        Select(Function(value2) projection2(value1, value2))).Unwrap
        End Function
    End Module
End Namespace
