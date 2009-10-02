Imports System.Threading

Namespace Threading
    Public Module ThreadingExtensions
        '''<summary>Returns a future which is ready after a specified amount of time.</summary>
        Public Function FutureWait(ByVal dt As TimeSpan) As IFuture
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            If dt.Ticks > Int32.MaxValue Then Throw New ArgumentOutOfRangeException("dt", "Can't wait that long.")

            Dim result = New FutureAction
            Dim ds = dt.TotalMilliseconds
            If ds <= 0 Then
                result.SetSucceeded()
            Else
                Dim timer = New Timers.Timer(ds)
                AddHandler timer.Elapsed, Sub()
                                              Contract.Assume(result IsNot Nothing)
                                              Contract.Assume(timer IsNot Nothing)
                                              timer.Dispose()
                                              result.SetSucceeded()
                                          End Sub
                timer.AutoReset = False
                timer.Start()
            End If
            Return result
        End Function

        '''<summary>Determines a future for running an action in a new thread.</summary>
        Public Function ThreadedAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim result = New FutureAction
            Call New Thread(Sub()
                                Contract.Assume(action IsNot Nothing)
                                Contract.Assume(result IsNot Nothing)
                                result.SetByCalling(action)
                            End Sub).Start()
            Return result
        End Function

        '''<summary>Determines the future value of running a function in a new thread.</summary>
        Public Function ThreadedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim result = New FutureFunction(Of TReturn)
            Call New Thread(Sub()
                                Contract.Assume(func IsNot Nothing)
                                Contract.Assume(result IsNot Nothing)
                                result.SetByEvaluating(func)
                            End Sub).Start()
            Return result
        End Function

        '''<summary>Determines a future for running an action in the thread pool.</summary>
        Public Function ThreadPooledAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim result = New FutureAction
            ThreadPool.QueueUserWorkItem(Sub()
                                             Contract.Assume(action IsNot Nothing)
                                             Contract.Assume(result IsNot Nothing)
                                             result.SetByCalling(action)
                                         End Sub)
            Return result
        End Function

        '''<summary>Determines the future value of running a function in the thread pool.</summary>
        Public Function ThreadPooledFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim result = New FutureFunction(Of TReturn)
            ThreadPool.QueueUserWorkItem(Sub()
                                             Contract.Assume(func IsNot Nothing)
                                             Contract.Assume(result IsNot Nothing)
                                             result.SetByEvaluating(func)
                                         End Sub)
            Return result
        End Function

#Region "WhenReady"
        <Extension()>
        Public Function QueueCallWhenReady(ByVal future As IFuture,
                                           ByVal queue As ICallQueue,
                                           ByVal action As Action(Of Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenReady(Function(exception)
                                            Contract.Assume(queue IsNot Nothing)
                                            Return queue.QueueAction(Sub()
                                                                         Contract.Assume(action IsNot Nothing)
                                                                         action(exception)
                                                                     End Sub)
                                        End Function).Defuturized
        End Function

        <Extension()>
        Public Function QueueCallWhenValueReady(Of TArg)(ByVal future As IFuture(Of TArg),
                                                         ByVal queue As ICallQueue,
                                                         ByVal action As Action(Of TArg, Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenValueReady(Function(value, valueException)
                                                 Contract.Assume(queue IsNot Nothing)
                                                 Return queue.QueueAction(Sub()
                                                                              Contract.Assume(action IsNot Nothing)
                                                                              action(value, valueException)
                                                                          End Sub)
                                             End Function).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalWhenReady(Of TReturn)(ByVal future As IFuture,
                                                       ByVal queue As ICallQueue,
                                                       ByVal func As Func(Of Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenReady(Function(exception)
                                            Contract.Assume(queue IsNot Nothing)
                                            Return queue.QueueFunc(Function()
                                                                       Contract.Assume(func IsNot Nothing)
                                                                       Return func(exception)
                                                                   End Function)
                                        End Function).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalWhenValueReady(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                                  ByVal queue As ICallQueue,
                                                                  ByVal func As Func(Of TArg, Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenValueReady(Function(value, valueException)
                                                 Contract.Assume(queue IsNot Nothing)
                                                 Return queue.QueueFunc(Function()
                                                                            Contract.Assume(func IsNot Nothing)
                                                                            Return func(value, valueException)
                                                                        End Function)
                                             End Function).Defuturized
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
            Return future.EvalOnSuccess(Function()
                                            Contract.Assume(queue IsNot Nothing)
                                            Contract.Assume(action IsNot Nothing)
                                            Return queue.QueueAction(action)
                                        End Function).Defuturized
        End Function

        <Extension()>
        Public Function QueueCallOnValueSuccess(Of TArg)(ByVal future As IFuture(Of TArg),
                                                         ByVal queue As ICallQueue,
                                                         ByVal action As Action(Of TArg)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.Select(Function(result)
                                     Contract.Assume(queue IsNot Nothing)
                                     Return queue.QueueAction(Sub()
                                                                  Contract.Assume(action IsNot Nothing)
                                                                  Call action(result)
                                                              End Sub)
                                 End Function).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalOnSuccess(Of TReturn)(ByVal future As IFuture,
                                                       ByVal queue As ICallQueue,
                                                       ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalOnSuccess(Function()
                                            Contract.Assume(queue IsNot Nothing)
                                            Contract.Assume(func IsNot Nothing)
                                            Return queue.QueueFunc(func)
                                        End Function).Defuturized
        End Function

        <Extension()>
        Public Function QueueEvalOnValueSuccess(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                                  ByVal queue As ICallQueue,
                                                                  ByVal func As Func(Of TArg, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.Select(Function(result)
                                     Contract.Assume(queue IsNot Nothing)
                                     Return queue.QueueFunc(Function()
                                                                Contract.Assume(func IsNot Nothing)
                                                                Return func(result)
                                                            End Function)
                                 End Function).Defuturized
        End Function
#End Region
    End Module
End Namespace
