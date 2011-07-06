Imports System.Threading
Imports Strilbrary.Values

Namespace Threading
    '''<remarks>Verification disabled due to missing task contracts</remarks>
    Public Module ThreadingExtensions
        '''<summary>Determines a task for running an action in a new thread.</summary>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task)() IsNot Nothing")>
        Public Function ThreadedAction(action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            Call New Thread(Sub() result.SetByCalling(action)).Start()
            Return result.Task
        End Function
        '''<summary>Determines a task value for running a function in a new thread.</summary>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task(Of TReturn))() IsNot Nothing")>
        Public Function ThreadedFunc(Of TReturn)(func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            Call New Thread(Sub() result.SetByEvaluating(func)).Start()
            Return result.Task
        End Function

        '''<summary>Determines a task for running an action as a task.</summary>
        Public Function TaskedAction(action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return Task.Factory.StartNew(action)
        End Function
        '''<summary>Determines a task value for running a function as a task.</summary>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task(Of TReturn))() IsNot Nothing")>
        Public Function TaskedFunc(Of TReturn)(func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            Task.Factory.StartNew(Sub() result.SetByEvaluating(func))
            Return result.Task
        End Function

        '''<summary>Determines a task for running an action in the thread pool.</summary>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task)() IsNot Nothing")>
        Public Function ThreadPooledAction(action As Action) As Task
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            ThreadPool.QueueUserWorkItem(Sub() result.SetByCalling(action))
            Return result.Task
        End Function
        '''<summary>Determines a task value for running a function in the thread pool.</summary>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task(Of TReturn))() IsNot Nothing")>
        Public Function ThreadPooledFunc(Of TReturn)(func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            ThreadPool.QueueUserWorkItem(Sub() result.SetByEvaluating(func))
            Return result.Task
        End Function

        '''<summary>Determines a task for invoking an action on a control's thread.</summary>
        <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task)() IsNot Nothing")>
        Public Function AsyncInvokedAction(control As Control, action As Action) As Task
            Contract.Requires(control IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of NoValue)
            result.DependentCall(Sub() control.BeginInvoke(Sub() result.SetByCalling(action)))
            Return result.Task
        End Function
        '''<summary>Determines a task value for invoking a function on a control's thread.</summary>
        <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Task(Of TReturn))() IsNot Nothing")>
        Public Function AsyncInvokedFunc(Of TReturn)(control As Control, func As Func(Of TReturn)) As Task(Of TReturn)
            Contract.Requires(control IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of TReturn))() IsNot Nothing)
            Dim result = New TaskCompletionSource(Of TReturn)
            result.DependentCall(Sub() control.BeginInvoke(Sub() result.SetByEvaluating(func)))
            Return result.Task
        End Function

        <Pure()>
        Public Function MakeThreadPoolSynchronizationContext() As SynchronizationContext
            Return New RunnerSynchronizationContext(Sub(e) ThreadPooledAction(e))
        End Function

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
        Public Function MakeThreadedCallQueue() As CallQueue
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(New RunnerSynchronizationContext(AddressOf ThreadedAction))
        End Function
        Public Function MakeThreadPooledCallQueue() As CallQueue
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(New RunnerSynchronizationContext(AddressOf ThreadPooledAction))
        End Function
        Public Function MakeTaskedCallQueue() As CallQueue
            Contract.Ensures(Contract.Result(Of CallQueue)() IsNot Nothing)
            Return New CallQueue(New RunnerSynchronizationContext(AddressOf TaskedAction))
        End Function
    End Module
End Namespace
