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
            Dim _queue = queue 'avoids contract verification problems with hoisted arguments
            Dim _action = action
            Return future.EvalWhenReady(Function() _queue.QueueAction(_action)).Defuturize
        End Function

        <Extension()>
        Public Function QueueCallWhenValueReady(Of A1)(ByVal future As IFuture(Of A1),
                                                       ByVal queue As ICallQueue,
                                                       ByVal action As Action(Of A1)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim _queue = queue 'avoids contract verification problems with hoisted arguments
            Dim _action = action
            Return future.EvalWhenValueReady(Function(result) _queue.QueueAction(Sub() _action(result))).Defuturize
        End Function

        <Extension()>
        Public Function QueueEvalWhenReady(Of R)(ByVal future As IFuture,
                                                 ByVal queue As ICallQueue,
                                                 ByVal func As Func(Of R)) As IFuture(Of R)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of R))() IsNot Nothing)
            Dim _queue = queue 'avoids contract verification problems with hoisted arguments
            Dim _func = func
            Return future.EvalWhenReady(Function() _queue.QueueFunc(_func)).Defuturize
        End Function

        <Extension()>
        Public Function QueueEvalWhenValueReady(Of A1, R)(ByVal future As IFuture(Of A1),
                                                          ByVal queue As ICallQueue,
                                                          ByVal func As Func(Of A1, R)) As IFuture(Of R)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of R))() IsNot Nothing)
            Dim _queue = queue 'avoids contract verification problems with hoisted arguments
            Dim _func = func
            Return future.EvalWhenValueReady(Function(result) _queue.QueueFunc(Function() _func(result))).Defuturize
        End Function
    End Module
End Namespace
