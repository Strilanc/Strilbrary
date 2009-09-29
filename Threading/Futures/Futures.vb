Imports System.Threading

Namespace Threading
    Public Enum FutureState
        '''<summary>The future was not ready, but now may or may not be ready.</summary>
        Unknown
        '''<summary>The future is ready and does not contain an exception.</summary>
        Succeeded
        '''<summary>The future is ready and contains an exception.</summary>
        Failed
    End Enum

    '''<summary>Represents an action which can finish in the future.</summary>
    <ContractClass(GetType(ContractClassForIFuture))>
    Public Interface IFuture
        '''<summary>Raised when the future becomes ready.</summary>
        Event Ready()
        '''<summary>Returns the future's state.</summary>
        ReadOnly Property State() As FutureState
        ''' <summary>
        ''' Returns the exception stored in the future.
        ''' Throws an InvalidOperationException if there is no such exception.
        ''' </summary>
        ReadOnly Property Exception() As Exception
        '''<summary>Stops the future from logging its stored exception when finalized.</summary>
        Sub MarkAnyExceptionAsHandled()
    End Interface

    '''<summary>Represents a function which can finish in the future.</summary>
    Public Interface IFuture(Of Out TValue)
        Inherits IFuture
        ''' <summary>
        ''' Returns the value stored in the future.
        ''' Throws an InvalidOperationException if the value isn't ready yet.
        ''' </summary>
        ReadOnly Property Value() As TValue
    End Interface

    <ContractClassFor(GetType(IFuture))>
    Public Class ContractClassForIFuture
        Implements IFuture
        Public Event Ready() Implements IFuture.Ready
        Public ReadOnly Property Exception As System.Exception Implements IFuture.Exception
            Get
                Contract.Ensures(Contract.Result(Of Exception)() IsNot Nothing)
                Throw New NotSupportedException
            End Get
        End Property
        Public Sub MarkAnyExceptionAsHandled() Implements IFuture.MarkAnyExceptionAsHandled
        End Sub
        Public ReadOnly Property State As FutureState Implements IFuture.State
            Get
                Throw New NotSupportedException
            End Get
        End Property
    End Class

    '''<summary>Represents something which can finish in the future.</summary>
    Public MustInherit Class FutureBase
        Implements IFuture
        Protected ReadOnly lockCanSet As New OnetimeLock
        Protected ReadOnly lockIsSet As New OnetimeLock
        Protected _exception As Exception
        '''<summary>Raised when the future becomes ready.</summary>
        Public Event Ready() Implements IFuture.Ready

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(lockCanSet IsNot Nothing)
            Contract.Invariant(lockIsSet IsNot Nothing)
        End Sub

        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")>
        Protected Overrides Sub Finalize()
            Select Case Me.State
                Case FutureState.Failed
                    LogUnexpectedException("Future exception ignored.", Me.Exception)
                Case FutureState.Unknown
                    LogUnexpectedException("Future expired without being set.", New InvalidStateException("Dangling Future."))
            End Select
            MyBase.Finalize()
        End Sub

        ''' <summary>
        ''' Returns the exception stored in the future.
        ''' Throws an InvalidOperationException if there is no such exception.
        ''' </summary>
        Public ReadOnly Property Exception() As Exception Implements IFuture.Exception
            Get
                If State <> FutureState.Failed Then
                    Throw New InvalidOperationException("Future doesn't contain an exception.")
                End If
                Return _exception
            End Get
        End Property

        '''<summary>Returns the future's state.</summary>
        Public ReadOnly Property State() As FutureState Implements IFuture.State
            Get
                Contract.Ensures(Contract.Result(Of FutureState)() <> FutureState.Failed OrElse _exception IsNot Nothing)
                If lockIsSet.State = OnetimeLockState.Unknown Then Return FutureState.Unknown
                If _exception Is Nothing Then Return FutureState.Succeeded
                Return FutureState.Failed
            End Get
        End Property

        '''<summary>Stops the future from logging its stored exception when finalized.</summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")>
        Public Sub MarkAnyExceptionAsHandled() Implements IFuture.MarkAnyExceptionAsHandled
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' Sets the future's state to failed and stores the provided exception.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetFailed(ByVal exception As Exception)
            Contract.Requires(exception IsNot Nothing)
            If Not TrySetFailed(exception) Then
                Throw New InvalidOperationException("Future was already set.")
            End If
        End Sub

        ''' <summary>
        ''' Sets the future's state to failed and stores the provided exception.
        ''' Returns false if the future was already ready.
        ''' </summary>
        Public Function TrySetFailed(ByVal exception As Exception) As Boolean
            Contract.Requires(exception IsNot Nothing)

            If Not lockCanSet.TryAcquire Then Return False
            Me._exception = exception
            If Not lockIsSet.TryAcquire() Then Throw New UnreachableException()

            RaiseEvent Ready()
            Return True
        End Function

        Protected Function TrySetSucceededBase(ByVal action As action) As Boolean
            Contract.Requires(action IsNot Nothing)

            If Not lockCanSet.TryAcquire Then Return False
            Call action()
            If Not lockIsSet.TryAcquire() Then Throw New UnreachableException()

            MarkAnyExceptionAsHandled()
            RaiseEvent Ready()
            Return True
        End Function
    End Class

    '''<summary>An action which can finish in the future.</summary>
    <DebuggerDisplay("{ToString}")>
    Public NotInheritable Class FutureAction
        Inherits FutureBase

        ''' <summary>
        ''' Sets the future's state based on the outcome of an action.
        ''' Runs the action whether or not the future was already ready.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public Sub SetByCalling(ByVal action As action)
            Contract.Requires(action IsNot Nothing)
            Try
                Call action()
                SetSucceeded()
            Catch ex As Exception
                SetFailed(ex)
            End Try
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetSucceeded()
            If Not TrySetSucceeded() Then
                Throw New InvalidOperationException("Future readied more than once.")
            End If
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded, unless the future was already ready.
        ''' Returns false if the future was already ready.
        ''' </summary>
        Public Function TrySetSucceeded() As Boolean
            Return TrySetSucceededBase(Sub()
                                       End Sub)
        End Function

        Public Overrides Function ToString() As String
            Select Case State
                Case FutureState.Failed : Return "FutureAction Failed: {0}".Frmt(Exception.Message)
                Case FutureState.Succeeded : Return "FutureAction Succeeded"
                Case FutureState.Unknown : Return "FutureAction Not Ready"
                Case Else : Throw State.MakeImpossibleValueException()
            End Select
        End Function
    End Class

    '''<summary>A function which can finish in the future.</summary>
    <DebuggerDisplay("{ToString}")>
    Public NotInheritable Class FutureFunction(Of TValue)
        Inherits FutureBase
        Implements IFuture(Of TValue)
        Private _value As TValue

        ''' <summary>
        ''' Returns the value stored in the future.
        ''' Throws an InvalidOperationException if the value isn't ready yet.
        ''' </summary>
        Public ReadOnly Property Value() As TValue Implements IFuture(Of TValue).Value
            Get
                If State <> FutureState.Succeeded Then Throw New InvalidOperationException("Attempted to get a future value before it was ready.")
                Return _value
            End Get
        End Property

        ''' <summary>
        ''' Sets the future's state based on the outcome of a function.
        ''' Evalutes the function whether or not the future was already ready.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public Sub SetByEvaluating(ByVal func As Func(Of TValue))
            Contract.Requires(func IsNot Nothing)
            Try
                SetSucceeded(func())
            Catch ex As Exception
                SetFailed(ex)
            End Try
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded and stores the provided value.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetSucceeded(ByVal value As TValue)
            If Not TrySetSucceeded(value) Then
                Throw New InvalidOperationException("Future readied more than once.")
            End If
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded and stores the provided value.
        ''' Returns false if the future was already ready.
        ''' </summary>
        Public Function TrySetSucceeded(ByVal value As TValue) As Boolean
            Return TrySetSucceededBase(Sub()
                                           Contract.Assume(Me IsNot Nothing)
                                           Me._value = value
                                       End Sub)
        End Function

        Public Overrides Function ToString() As String
            Select Case State
                Case FutureState.Failed : Return "FutureFunction Failed: {0}".Frmt(Exception.Message)
                Case FutureState.Succeeded : Return "FutureFunction = {0}".Frmt(Value)
                Case FutureState.Unknown : Return "FutureFunction Not Ready"
                Case Else : Throw State.MakeImpossibleValueException()
            End Select
        End Function
    End Class

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
            handler = Sub() System.Threading.ThreadPool.QueueUserWorkItem(
                Sub()
                    Contract.Assume(result IsNot Nothing)
                    Contract.Assume(action IsNot Nothing)
                    Contract.Assume(future IsNot Nothing)
                    Contract.Assume(lock IsNot Nothing)
                    If lock.TryAcquire Then 'ensure only run once
                        RemoveHandler future.Ready, handler
                        result.SetByCalling(Sub()
                                                Contract.Assume(action IsNot Nothing)
                                                Contract.Assume(future IsNot Nothing)
                                                Call action(future.TryGetException)
                                            End Sub)
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

            Return future.CallWhenReady(Sub(exception)
                                            Contract.Assume(action IsNot Nothing)
                                            Contract.Assume(future IsNot Nothing)
                                            action(future.TryGetValue, exception)
                                        End Sub)
        End Function

        '''<summary>Determines the future result of running a function after the future is ready.</summary>
        <Extension()>
        Public Function EvalWhenReady(Of TReturn)(ByVal future As IFuture,
                                                  ByVal func As Func(Of Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)

            Dim result = New FutureFunction(Of TReturn)
            future.CallWhenReady(Sub(exception)
                                     Contract.Assume(func IsNot Nothing)
                                     Contract.Assume(result IsNot Nothing)
                                     result.SetByEvaluating(Function()
                                                                Contract.Assume(func IsNot Nothing)
                                                                Return func(exception)
                                                            End Function)
                                 End Sub).MarkAnyExceptionAsHandled()
            Return result
        End Function

        '''<summary>Determines the future result of applying a function to the future value.</summary>
        <Extension()>
        Public Function EvalWhenValueReady(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                             ByVal func As Func(Of TArg, Exception, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)

            Return future.EvalWhenReady(Function(exception)
                                            Contract.Assume(func IsNot Nothing)
                                            Return func(future.TryGetValue, exception)
                                        End Function)
        End Function
#End Region

#Region "OnSuccess/Linq"
        '''<summary>Determines the future result of running an action after the future succeeds while propagating failures.</summary>
        <Extension()>
        Public Function CallOnSuccess(ByVal future As IFuture,
                                      ByVal action As Action) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim result = New FutureAction
            future.CallWhenReady(Sub(exception)
                                     Contract.Assume(result IsNot Nothing)
                                     Contract.Assume(action IsNot Nothing)
                                     If exception IsNot Nothing Then
                                         result.SetFailed(exception)
                                     Else
                                         Contract.Assume(action IsNot Nothing)
                                         result.SetByCalling(action)
                                     End If
                                 End Sub).MarkAnyExceptionAsHandled()
            Return result
        End Function

        '''<summary>Determines the future result of applying an action to the future value while propagating failures.</summary>
        <Extension()>
        Public Function CallOnValueSuccess(Of TArg)(ByVal future As IFuture(Of TArg),
                                                    ByVal action As Action(Of TArg)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Return future.CallOnSuccess(Sub()
                                            Contract.Assume(future IsNot Nothing)
                                            Contract.Assume(action IsNot Nothing)
                                            Call action(future.Value)
                                        End Sub)
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
                                     Contract.Assume(result IsNot Nothing)
                                     Contract.Assume(func IsNot Nothing)
                                     If exception IsNot Nothing Then
                                         result.SetFailed(exception)
                                     Else
                                         result.SetByEvaluating(func)
                                     End If
                                 End Sub).MarkAnyExceptionAsHandled()
            Return result
        End Function

        '''<summary>Determines the future result of applying a function to the future value while propagating failures.</summary>
        <Extension()>
        Public Function [Select](Of TArg, TResult)(ByVal future As IFuture(Of TArg),
                                                   ByVal func As Func(Of TArg, TResult)) As IFuture(Of TResult)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TResult))() IsNot Nothing)

            Return future.EvalOnSuccess(Function()
                                            Contract.Assume(future IsNot Nothing)
                                            Contract.Assume(func IsNot Nothing)
                                            Return func(future.TryGetValue)
                                        End Function)
        End Function

        '''<summary>Determines the future result of applying two successive functions to the future value while propagating failures.</summary>
        <Extension()>
        Public Function SelectMany(Of TArg, TMid, TReturn)(ByVal future As IFuture(Of TArg),
                                                           ByVal projection1 As Func(Of TArg, IFuture(Of TMid)),
                                                           ByVal projection2 As Func(Of TArg, TMid, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(projection1 IsNot Nothing)
            Contract.Requires(projection2 IsNot Nothing)
            Return future.Select(Function(value1)
                                     Contract.Assume(projection1 IsNot Nothing)
                                     Contract.Assume(projection2 IsNot Nothing)
                                     Dim f = projection1(value1)
                                     Contract.Assume(f IsNot Nothing)
                                     Return f.Select(Function(value2)
                                                         Contract.Assume(projection2 IsNot Nothing)
                                                         Return projection2(value1, value2)
                                                     End Function)
                                 End Function).Defuturized
        End Function
#End Region

#Region "Transforms"
        '''<summary>Wraps a normal value as an instantly ready future.</summary>
        <Extension()>
        Public Function Futurized(Of TValue)(ByVal value As TValue) As IFuture(Of TValue)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))() IsNot Nothing)
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
                    Contract.Assume(result IsNot Nothing)
                    If exception1 IsNot Nothing Then
                        result.SetFailed(exception1)
                        Return
                    End If

                    Contract.Assume(futureValue IsNot Nothing)
                    futureValue.CallWhenReady(
                        Sub(exception2)
                            Contract.Assume(result IsNot Nothing)
                            If exception2 IsNot Nothing Then
                                result.SetFailed(exception2)
                            Else
                                result.SetSucceeded()
                            End If
                        End Sub
                    ).MarkAnyExceptionAsHandled()
                End Sub
            ).MarkAnyExceptionAsHandled()
            Return result
        End Function

        '''<summary>Determines the future value of the future of a future value.</summary>
        <Extension()>
        Public Function Defuturized(Of TValue)(ByVal futureFutureValue As IFuture(Of IFuture(Of TValue))) As IFuture(Of TValue)
            Contract.Requires(futureFutureValue IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))() IsNot Nothing)

            Return CType(futureFutureValue, IFuture(Of IFuture)).Defuturized().EvalOnSuccess(
                Function()
                    Contract.Assume(futureFutureValue IsNot Nothing)
                    Contract.Assume(futureFutureValue.Value IsNot Nothing)
                    Return futureFutureValue.Value.Value
                End Function)
        End Function

        '''<summary>
        ''' Returns a future for a list of futures.
        ''' Fails if any of the future values fail.
        '''</summary>
        <Extension()>
        Public Function Defuturized(ByVal sequence As IEnumerable(Of IFuture)) As IFuture
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim items = sequence.ToList
            Contract.Assume(items IsNot Nothing)
            Dim result = New FutureAction()
            Dim numReady = 0
            Dim failed As Exception = Nothing
            Dim numFutures = items.Count

            'Become ready once all input futures are ready
            Dim notify = Sub(exception As Exception)
                             If exception IsNot Nothing Then  failed = exception
                             If Interlocked.Increment(numReady) >= numFutures Then
                                 Contract.Assume(result IsNot Nothing)
                                 If failed IsNot Nothing Then
                                     result.SetFailed(failed)
                                 Else
                                     result.SetSucceeded()
                                 End If
                             End If
                         End Sub
            For Each future In items
                Contract.Assume(future IsNot Nothing)
                future.CallWhenReady(notify).MarkAnyExceptionAsHandled()
            Next future
            If numFutures = 0 Then result.SetSucceeded()

            Return result
        End Function

        '''<summary>
        ''' Returns a future list for a list of future values.
        ''' Fails if any of the future values fail.
        '''</summary>
        <Extension()>
        Public Function Defuturized(Of TValue)(ByVal sequence As IEnumerable(Of IFuture(Of TValue))) As IFuture(Of IEnumerable(Of TValue))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of IEnumerable(Of TValue)))() IsNot Nothing)
            Dim items = sequence.ToList
            Contract.Assume(items IsNot Nothing)
            Return CType(items, IEnumerable(Of IFuture)).Defuturized.EvalOnSuccess(
                Function()
                    Contract.Assume(items IsNot Nothing)
                    Dim values = (From future In items
                            Select Function()
                                       Contract.Assume(future IsNot Nothing)
                                       Return future.Value
                                   End Function())
                    Contract.Assume(values IsNot Nothing)
                    Return values.ToList
                End Function)
        End Function
#End Region
    End Module
End Namespace
