Imports System.Threading

Namespace Threading
    Public Module IFutureExtensions
        ''' <summary>
        ''' Returns the exception stored in the future.
        ''' Returns nothing if the future doesn't contain an exception.
        ''' </summary>
        <Extension()>
        Public Function TryGetException(ByVal future As IFuture) As Exception
            Contract.Requires(future IsNot Nothing)
            Return If(future.State = FutureState.Failed, future.Exception, Nothing)
        End Function

        ''' <summary>
        ''' Returns the value stored in the future.
        ''' Returns default(T) if the future doesn't contain a value.
        ''' </summary>
        <Extension()>
        Public Function TryGetValue(Of T)(ByVal future As IFuture(Of T)) As T
            Contract.Requires(future IsNot Nothing)
            Return If(future.State = FutureState.Succeeded, future.Value, Nothing)
        End Function

#Region "WhenReady"
        '''<summary>Determines the future result of running an action after the future is ready.</summary>
        <Extension()>
        Public Function CallWhenReady(ByVal future As IFuture,
                                      ByVal action As Action(Of Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            'Prep callback
            Dim lock = New OnetimeLock()
            Dim result = New FutureAction()
            Dim handler As IFuture.ReadyEventHandler
            handler = Sub() System.Threading.Tasks.Task.Factory.StartNew(
                Sub()
                    If lock.TryAcquire Then 'ensure only run once
                        RemoveHandler future.Ready, handler
                        result.SetByCalling(Sub() action(future.TryGetException))
                    End If
                End Sub
            )

            'Activate callback
            AddHandler future.Ready, handler
            If future.State <> FutureState.Unknown Then
                '[covers the case where the future was ready before the callback was added]
                Call handler()
            End If

            future.MarkAnyExceptionAsHandled()
            Return result
        End Function

        '''<summary>Determines the future result of applying an action to the future value.</summary>
        <Extension()>
        Public Function CallWhenValueReady(Of TArg1)(ByVal future As IFuture(Of TArg1),
                                                     ByVal action As Action(Of TArg1, Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.CallWhenReady(Sub(exception) action(future.TryGetValue, exception))
        End Function

        '''<summary>Determines the future result of running a function after the future is ready.</summary>
        <Extension()>
        Public Function EvalWhenReady(Of TReturn)(ByVal future As IFuture,
                                                  ByVal func As Func(Of Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)

            Dim result = New FutureFunction(Of TReturn)
            future.CallWhenReady(
                Sub(exception) result.SetByEvaluating(
                    Function() func(exception))
            )
            Return result
        End Function

        '''<summary>Determines the future result of applying a function to the future value.</summary>
        <Extension()>
        Public Function EvalWhenValueReady(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                             ByVal func As Func(Of TArg, Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenReady(Function(exception) func(future.TryGetValue, exception))
        End Function
#End Region

#Region "OnSuccess/Linq/Catch"
        ''' <summary>
        ''' Determines the future result of running an exception handler if a future fails.
        ''' Succeeds if the handler is not run.
        ''' </summary>
        <Extension()>
        Public Function [Catch](ByVal future As IFuture,
                                ByVal action As Action(Of Exception)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.CallWhenReady(Sub(exception)
                                            If exception IsNot Nothing Then
                                                Call action(exception)
                                            End If
                                        End Sub)
        End Function

        '''<summary>Determines the future result of running an action after the future succeeds while propagating failures.</summary>
        <Extension()>
        Public Function CallOnSuccess(ByVal future As IFuture,
                                      ByVal action As Action) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim result = New FutureAction
            future.CallWhenReady(Sub(exception)
                                     If exception IsNot Nothing Then
                                         result.SetFailed(exception)
                                     Else
                                         result.SetByCalling(action)
                                     End If
                                 End Sub)
            Return result
        End Function

        '''<summary>Determines the future result of applying an action to the future value while propagating failures.</summary>
        <Extension()>
        Public Function CallOnValueSuccess(Of TArg)(ByVal future As IFuture(Of TArg),
                                                    ByVal action As Action(Of TArg)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.CallOnSuccess(Sub() action(future.Value))
        End Function

        '''<summary>Determines the future result of running a function after the future is ready while propagating failures.</summary>
        <Extension()>
        Public Function EvalOnSuccess(Of TResult)(ByVal future As IFuture,
                                                  ByVal func As Func(Of TResult)) As IFuture(Of TResult)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TResult))() IsNot Nothing)

            Dim result = New FutureFunction(Of TResult)
            future.CallWhenReady(Sub(exception)
                                     If exception IsNot Nothing Then
                                         result.SetFailed(exception)
                                     Else
                                         result.SetByEvaluating(func)
                                     End If
                                 End Sub)
            Return result
        End Function

        '''<summary>Determines the future result of applying a function to the future value while propagating failures.</summary>
        <Extension()>
        Public Function [Select](Of TArg, TResult)(ByVal future As IFuture(Of TArg),
                                                   ByVal func As Func(Of TArg, TResult)) As IFuture(Of TResult)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TResult))() IsNot Nothing)
            Return future.EvalOnSuccess(Function() func(future.TryGetValue))
        End Function

        '''<summary>Determines the future result of applying two successive functions to the future value while propagating failures.</summary>
        <Extension()>
        Public Function SelectMany(Of TArg, TMid, TReturn)(ByVal future As IFuture(Of TArg),
                                                           ByVal projection1 As Func(Of TArg, IFuture(Of TMid)),
                                                           ByVal projection2 As Func(Of TArg, TMid, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(projection1 IsNot Nothing)
            Contract.Requires(projection2 IsNot Nothing)
            Return future.Select(Function(value1) projection1(value1).
                          Select(Function(value2) projection2(value1, value2))).Defuturized
        End Function
#End Region

#Region "Transforms"
        '''<summary>Wraps a normal value as an instantly ready future.</summary>
        <Extension()>
        Public Function Futurized(Of TValue)(ByVal value As TValue) As IFuture(Of TValue)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))().State = FutureState.Succeeded)
            Dim result = New FutureFunction(Of TValue)
            result.SetSucceeded(value)
            Return result
        End Function

        '''<summary>Determines a future for the future of a future.</summary>
        <Extension()>
        Public Function Defuturized(ByVal futureFuture As IFuture(Of IFuture)) As IFuture
            Contract.Requires(futureFuture IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim result = New FutureAction
            futureFuture.CallWhenValueReady(
                Sub(futureValue, exception1)
                    If exception1 IsNot Nothing Then
                        result.SetFailed(exception1)
                        Return
                    End If

                    futureValue.CallWhenReady(
                        Sub(exception2)
                            If exception2 IsNot Nothing Then
                                result.SetFailed(exception2)
                            Else
                                result.SetSucceeded()
                            End If
                        End Sub
                    )
                        End Sub
            )
            Return result
        End Function

        '''<summary>Determines the future value of the future of a future value.</summary>
        <Extension()>
        Public Function Defuturized(Of TValue)(ByVal futureFutureValue As IFuture(Of IFuture(Of TValue))) As IFuture(Of TValue)
            Contract.Requires(futureFutureValue IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))() IsNot Nothing)

            Return CType(futureFutureValue, IFuture(Of IFuture)).Defuturized().
                    EvalOnSuccess(Function() futureFutureValue.Value.Value)
        End Function

        '''<summary>
        ''' Determines a future sequence for a sequence of futures.
        ''' The future sequence becomes ready once all the futures are ready.
        ''' The future sequence fails if any of the futures fail.
        '''</summary>
        <Extension()>
        Public Function Defuturized(ByVal sequence As IEnumerable(Of IFuture)) As IFuture
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim items = sequence.ToList
            Dim result = New FutureAction()
            Dim numReady = 0
            Dim failed As Exception = Nothing
            Dim numFutures = items.Count

            'Become ready once all input futures are ready
            Dim notify = Sub(exception As Exception)
                             If exception IsNot Nothing Then failed = exception
                             If Interlocked.Increment(numReady) >= numFutures Then
                                 If failed IsNot Nothing Then
                                     result.SetFailed(failed)
                                 Else
                                     result.SetSucceeded()
                                 End If
                             End If
                         End Sub
            For Each future In items
                Contract.Assume(future IsNot Nothing)
                future.CallWhenReady(notify)
            Next future
            If numFutures = 0 Then result.SetSucceeded()

            Return result
        End Function

        '''<summary>
        ''' Determines a future sequence for a sequence of future values.
        ''' The future sequence becomes ready once all the future values are ready.
        ''' The future sequence fails if any of the future values fail.
        '''</summary>
        <Extension()>
        Public Function Defuturized(Of TValue)(ByVal sequence As IEnumerable(Of IFuture(Of TValue))) As IFuture(Of IEnumerable(Of TValue))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of IEnumerable(Of TValue)))() IsNot Nothing)
            Dim items = sequence.ToList
            Return CType(items, IEnumerable(Of IFuture)).Defuturized.
                    EvalOnSuccess(Function() (From future In items Select future.Value).ToList)
        End Function
#End Region
    End Module
End Namespace
