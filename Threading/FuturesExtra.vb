Imports System.Threading

Namespace Threading.Futures
    Public Module FutureCommon
        '''<summary>Returns a future which is ready after a specified amount of time.</summary>
        Public Function FutureWait(ByVal dt As TimeSpan) As IFuture
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            If dt.Ticks > Int32.MaxValue Then Throw New ArgumentOutOfRangeException("dt", "Can't wait that long")

            Dim f As New Future
            Dim ds = dt.TotalMilliseconds
            If ds <= 0 Then
                f.SetReady()
            Else
                Dim timer = New Timers.Timer(ds)
                AddHandler timer.Elapsed, Sub()
                                              Contract.Assume(f IsNot Nothing)
                                              Contract.Assume(timer IsNot Nothing)
                                              timer.Dispose()
                                              f.SetReady()
                                          End Sub
                timer.AutoReset = False
                timer.Start()
            End If
            Return f
        End Function

        '''<summary>Returns a future which is ready once all members of a sequence of futures is ready.</summary>
        <Extension()>
        Public Function FutureCompress(ByVal futures As IEnumerable(Of IFuture)) As IFuture
            Contract.Requires(futures IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim f As New Future()
            Dim numReady = 0
            Dim numFutures = futures.Count

            Dim notify = Sub()
                             If Interlocked.Increment(numReady) >= numFutures Then
                                 Contract.Assume(f IsNot Nothing)
                                 Call f.SetReady()
                             End If
                         End Sub

            For Each future In futures
                Contract.Assume(future IsNot Nothing)
                future.CallWhenReady(notify)
            Next future
            If numFutures = 0 Then f.SetReady()

            Return f
        End Function

        <Extension()>
        Public Function QueueCallWhenReady(ByVal future As IFuture,
                                           ByVal queue As ICallQueue,
                                           ByVal action As Action) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenReady(Function()
                                            Contract.Assume(queue IsNot Nothing)
                                            Contract.Assume(action IsNot Nothing)
                                            Return queue.QueueAction(action)
                                        End Function).Defuturize
        End Function

        <Extension()>
        Public Function QueueCallWhenValueReady(Of TArg)(ByVal future As IFuture(Of TArg),
                                                         ByVal queue As ICallQueue,
                                                         ByVal action As Action(Of TArg)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.EvalWhenValueReady(Function(result)
                                                 Contract.Assume(queue IsNot Nothing)
                                                 Return queue.QueueAction(Sub()
                                                                              Contract.Assume(action IsNot Nothing)
                                                                              action(result)
                                                                          End Sub)
                                             End Function).Defuturize
        End Function

        <Extension()>
        Public Function QueueEvalWhenReady(Of TReturn)(ByVal future As IFuture,
                                                       ByVal queue As ICallQueue,
                                                       ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenReady(Function()
                                            Contract.Assume(queue IsNot Nothing)
                                            Contract.Assume(func IsNot Nothing)
                                            Return queue.QueueFunc(func)
                                        End Function).Defuturize
        End Function

        <Extension()>
        Public Function QueueEvalWhenValueReady(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                                  ByVal queue As ICallQueue,
                                                                  ByVal func As Func(Of TArg, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenValueReady(Function(result)
                                                 Contract.Assume(queue IsNot Nothing)
                                                 Return queue.QueueFunc(Function()
                                                                            Contract.Assume(func IsNot Nothing)
                                                                            Return func(result)
                                                                        End Function)
                                             End Function).Defuturize
        End Function
    End Module
End Namespace
