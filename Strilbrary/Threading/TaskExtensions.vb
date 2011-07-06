Imports Strilbrary.Values
Imports Strilbrary.Exceptions
Imports System.Threading

Namespace Threading
    Public Module TaskExtensions
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
            Catch ex As Exception
                taskSource.SetException(ex)
            End Try
        End Sub

        '''<summary>Creates a continuation which executes if a task succeeds, and propagates exceptions if it faults.</summary>
        '''<remarks>Linq provider for tasks.</remarks>
        <Extension()>
        Public Async Function [Select](Of TArg, TResult)(task As Task(Of TArg),
                                                         func As Func(Of TArg, TResult)) As Task(Of TResult)
            Contract.Assume(task IsNot Nothing)
            Contract.Assume(func IsNot Nothing)
            'Contract.Ensures(Contract.Result(Of Task(Of TResult))() IsNot Nothing)
            Dim arg = Await task
            Return func(arg)
        End Function
        '''<summary>Creates a chain of continuations which execute if a task succeeds, and propagates exceptions if it faults.</summary>
        '''<remarks>Linq provider for tasks.</remarks>
        <Extension()>
        Public Async Function SelectMany(Of TArg, TMid, TReturn)(task As Task(Of TArg),
                                                                 projection1 As Func(Of TArg, Task(Of TMid)),
                                                                 projection2 As Func(Of TArg, TMid, TReturn)) As Task(Of TReturn)
            Contract.Assume(task IsNot Nothing)
            Contract.Assume(projection1 IsNot Nothing)
            Contract.Assume(projection2 IsNot Nothing)
            'Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim arg1 = Await task
            Dim arg2 = Await projection1(arg1)
            Return projection2(arg1, arg2)
        End Function
    End Module
End Namespace
