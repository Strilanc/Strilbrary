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

            Dim result As New Future()
            Dim numReady = 0
            Dim numFutures = futures.Count

            'Become ready once all input futures are ready
            Dim notify = Sub()
                             If Interlocked.Increment(numReady) >= numFutures Then
                                 Contract.Assume(result IsNot Nothing)
                                 Call result.SetReady()
                             End If
                         End Sub
            For Each future In futures
                Contract.Assume(future IsNot Nothing)
                future.CallWhenReady(notify)
            Next future
            If numFutures = 0 Then result.SetReady()

            Return result
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

        Public Sub FutureIterate(Of T)(ByVal producer As Func(Of IFuture(Of T)),
                                       ByVal consumer As Func(Of T, IFuture(Of Boolean)))
            Contract.Requires(producer IsNot Nothing)
            Contract.Requires(consumer IsNot Nothing)

            Dim q = producer()
            Contract.Assume(q IsNot Nothing)
            q.CallWhenValueReady(YCombinator(Of T)(
                Function(self) Sub(result)
                                   Contract.Assume(consumer IsNot Nothing)
                                   Dim c = consumer(result)
                                   Contract.Assume(c IsNot Nothing)
                                   c.CallWhenValueReady(
                                       Sub([continue])
                                           Contract.Assume(self IsNot Nothing)
                                           If [continue] Then
                                               Contract.Assume(producer IsNot Nothing)
                                               Dim p = producer()
                                               Contract.Assume(p IsNot Nothing)
                                               p.CallWhenValueReady(self)
                                           End If
                                       End Sub)
                               End Sub))
        End Sub

        <Extension()>
        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public Function FutureRead(ByVal stream As IO.Stream,
                                           ByVal buffer() As Byte,
                                           ByVal offset As Integer,
                                           ByVal count As Integer) As IFuture(Of PossibleException(Of Integer, Exception))
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of PossibleException(Of Integer, Exception)))() IsNot Nothing)
            Dim f = New Future(Of PossibleException(Of Integer, Exception))
            Try
                stream.BeginRead(buffer, offset, count, Sub(ar)
                                                            Contract.Requires(ar IsNot Nothing)
                                                            Contract.Assume(f IsNot Nothing)
                                                            Contract.Assume(ar IsNot Nothing)
                                                            Contract.Assume(stream IsNot Nothing)
                                                            Try
                                                                f.SetValue(stream.EndRead(ar))
                                                            Catch e As Exception
                                                                f.SetValue(e)
                                                            End Try
                                                        End Sub, Nothing)
            Catch e As Exception
                f.SetValue(e)
            End Try
            Return f
        End Function

        Public Function ThreadedAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim f As New Future
            Call New Thread(Sub() RunWithDebugTrap(Sub()
                                                       Contract.Assume(action IsNot Nothing)
                                                       Contract.Assume(f IsNot Nothing)
                                                       Call action()
                                                       Call f.SetReady()
                                                   End Sub, "Exception rose past ThreadedAction.")).Start()
            Return f
        End Function
        Public Function ThreadedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim f As New Future(Of TReturn)
            ThreadedAction(Sub()
                               Contract.Assume(func IsNot Nothing)
                               Contract.Assume(f IsNot Nothing)
                               f.SetValue(func())
                           End Sub)
            Return f
        End Function

        Public Function ThreadPooledAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim f As New Future
            ThreadPool.QueueUserWorkItem(Sub() RunWithDebugTrap(Sub()
                                                                    Contract.Assume(action IsNot Nothing)
                                                                    Contract.Assume(f IsNot Nothing)
                                                                    Call action()
                                                                    Call f.SetReady()
                                                                End Sub, "Exception rose past ThreadPooledAction."))
            Return f
        End Function
        Public Function ThreadPooledFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim f As New Future(Of TReturn)
            ThreadPooledAction(Sub()
                                   Contract.Assume(func IsNot Nothing)
                                   Contract.Assume(f IsNot Nothing)
                                   f.SetValue(func())
                               End Sub)
            Return f
        End Function
    End Module
End Namespace
