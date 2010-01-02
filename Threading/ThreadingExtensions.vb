﻿Imports System.Threading

Namespace Threading
    Public Module ThreadingExtensions
        ''' <summary>
        ''' Returns a future which becomes ready after the given amount of time.
        ''' The resulting future is instantly ready if the given time is non-positive.
        ''' </summary>
        <Extension()>
        Public Function AsyncWait(ByVal dt As TimeSpan) As IFuture
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            If dt.Ticks > Int32.MaxValue Then Throw New ArgumentOutOfRangeException("dt", "Can't wait that long.")

            Dim result = New FutureAction
            Dim ds = dt.TotalMilliseconds
            If ds <= 0 Then
                result.SetSucceeded()
            Else
                Dim timer = New Timers.Timer(ds)
                AddHandler timer.Elapsed, Sub()
                                              timer.Dispose()
                                              result.SetSucceeded()
                                          End Sub
                timer.AutoReset = False
                timer.Start()
            End If
            Return result
        End Function

#Region "Async Eval"
        '''<summary>Determines a future for running an action in a new thread.</summary>
        Public Function ThreadedAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim result = New FutureAction
            Call New Thread(Sub() result.SetByCalling(action)).Start()
            Return result
        End Function
        '''<summary>Determines a future value for running a function in a new thread.</summary>
        Public Function ThreadedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim result = New FutureFunction(Of TReturn)
            Call New Thread(Sub() result.SetByEvaluating(func)).Start()
            Return result
        End Function

        '''<summary>Determines a future for running an action as a task.</summary>
        Public Function TaskedAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim result = New FutureAction
            Contract.Assume(Tasks.Task.Factory IsNot Nothing)
            Tasks.Task.Factory.StartNew(Sub() result.SetByCalling(action))
            Return result
        End Function
        '''<summary>Determines a future value for running a function as a task.</summary>
        Public Function TaskedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim result = New FutureFunction(Of TReturn)
            Contract.Assume(Tasks.Task.Factory IsNot Nothing)
            Tasks.Task.Factory.StartNew(Sub() result.SetByEvaluating(func))
            Return result
        End Function

        '''<summary>Determines a future for running an action in the thread pool.</summary>
        Public Function ThreadPooledAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim result = New FutureAction
            ThreadPool.QueueUserWorkItem(Sub() result.SetByCalling(action))
            Return result
        End Function
        '''<summary>Determines a future value for running a function in the thread pool.</summary>
        Public Function ThreadPooledFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim result = New FutureFunction(Of TReturn)
            ThreadPool.QueueUserWorkItem(Sub() result.SetByEvaluating(func))
            Return result
        End Function

        '''<summary>Determines a future for invoking an action on a control's thread.</summary>
        <Extension()>
        Public Function AsyncInvokedAction(ByVal control As Control, ByVal action As Action) As IFuture
            Contract.Requires(control IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim result = New FutureAction()
            result.DependentCall(Sub() control.BeginInvoke(Sub() result.SetByCalling(action)))
            Return result
        End Function
        '''<summary>Determines a future value for invoking a function on a control's thread.</summary>
        <Extension()>
        Public Function AsyncInvokedFunc(Of T)(ByVal control As Control, ByVal func As Func(Of T)) As IFuture(Of T)
            Contract.Requires(control IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of T))() IsNot Nothing)
            Dim result = New FutureFunction(Of T)()
            result.DependentCall(Sub() control.BeginInvoke(Sub() result.SetByEvaluating(func)))
            Return result
        End Function
#End Region

#Region "WhenReady"
        <Extension()>
        Public Function QueueCallWhenReady(ByVal future As IFuture,
                                           ByVal queue As ICallQueue,
                                           ByVal action As Action(Of Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenReady(Function(exception) queue.QueueAction(
                                        Sub() action(exception))).Defuturized
        End Function

        <Extension()>
        Public Function QueueCallWhenValueReady(Of TArg)(ByVal future As IFuture(Of TArg),
                                                         ByVal queue As ICallQueue,
                                                         ByVal action As Action(Of TArg, Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenValueReady(Function(value, valueException) queue.QueueAction(
                                             Sub() action(value, valueException))).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalWhenReady(Of TReturn)(ByVal future As IFuture,
                                                       ByVal queue As ICallQueue,
                                                       ByVal func As Func(Of Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenReady(Function(exception) queue.QueueFunc(
                                        Function() func(exception))).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalWhenValueReady(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                                  ByVal queue As ICallQueue,
                                                                  ByVal func As Func(Of TArg, Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenValueReady(Function(value, valueException) queue.QueueFunc(
                                             Function() func(value, valueException))).Defuturized
        End Function
#End Region

#Region "OnSuccess"
        <Extension()>
        Public Function QueueCallOnSuccess(ByVal future As IFuture,
                                           ByVal queue As ICallQueue,
                                           ByVal action As Action) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalOnSuccess(Function() queue.QueueAction(action)).Defuturized
        End Function

        <Extension()>
        Public Function QueueCallOnValueSuccess(Of TArg)(ByVal future As IFuture(Of TArg),
                                                         ByVal queue As ICallQueue,
                                                         ByVal action As Action(Of TArg)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.Select(Function(result) queue.QueueAction(Sub() action(result))).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalOnSuccess(Of TReturn)(ByVal future As IFuture,
                                                       ByVal queue As ICallQueue,
                                                       ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalOnSuccess(Function() queue.QueueFunc(func)).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalOnValueSuccess(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                                  ByVal queue As ICallQueue,
                                                                  ByVal func As Func(Of TArg, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.Select(Function(result) queue.QueueFunc(Function() func(result))).Defuturized
        End Function
#End Region

        <Extension()>
        Public Function QueueCatch(ByVal future As IFuture,
                                   ByVal queue As ICallQueue,
                                   ByVal action As Action(Of Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenReady(Function(exception) queue.QueueAction(
                Sub()
                    If exception IsNot Nothing Then
                        Call action(exception)
                    End If
                End Sub)).Defuturized
        End Function
    End Module
End Namespace
