Imports System.Threading

Namespace Threading.Futures
    Public Enum FutureState
        '''<summary>The future was not ready, but now may or may not be ready.</summary>
        Unknown
        '''<summary>The future is definitely and permanently ready.</summary>
        Ready
    End Enum

    '''<summary>Represents a thread-safe read-only class that fires an event when it becomes ready.</summary>
    Public Interface IFuture
        '''<summary>Raised when the future becomes ready.</summary>
        Event Readied()
        '''<summary>Returns the future's state.</summary>
        ReadOnly Property State() As FutureState
    End Interface

    '''<summary>Represents a thread-safe read-only class that fires an event when its value becomes ready.</summary>
    Public Interface IFuture(Of Out TValue)
        Inherits IFuture
        ''' <summary>
        ''' Returns the future's value.
        ''' Throws an InvalidOperationException if the value isn't ready yet.
        ''' </summary>
        ReadOnly Property Value() As TValue
    End Interface

    '''<summary>A thread-safe class that fires an event when it becomes ready.</summary>
    Public Class Future
        Implements IFuture
        Private ReadOnly lockReady As New OnetimeLock
        Public Event Readied() Implements IFuture.Readied

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(lockReady IsNot Nothing)
        End Sub

        '''<summary>Returns the future's state.</summary>
        Public ReadOnly Property State() As FutureState Implements IFuture.state
            Get
                Return If(lockReady.State = OnetimeLockState.Acquired, FutureState.Ready, FutureState.Unknown)
            End Get
        End Property

        ''' <summary>
        ''' Makes the future ready.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetReady()
            If Not TrySetReady() Then
                Throw New InvalidOperationException("Future readied more than once.")
            End If
        End Sub
        ''' <summary>
        ''' Makes the future ready.
        ''' Returns false if the future was already ready.
        ''' </summary>
        Public Function TrySetReady() As Boolean
            If Not lockReady.TryAcquire Then Return False
            RaiseEvent Readied()
            Return True
        End Function
    End Class

    '''<summary>A thread-safe class that fires an event when its value becomes ready.</summary>
    Public Class Future(Of TValue)
        Implements IFuture(Of TValue)
        Protected _value As TValue
        Protected ReadOnly lockReady As New OnetimeLock
        Public Event Readied() Implements IFuture.Readied

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(lockReady IsNot Nothing)
        End Sub

        '''<summary>Returns the future's state.</summary>
        Public ReadOnly Property State() As FutureState Implements IFuture.state
            Get
                Return If(lockReady.State = OnetimeLockState.Acquired, FutureState.Ready, FutureState.Unknown)
            End Get
        End Property

        '''<summary>
        '''Returns the future's value.
        '''Throws an InvalidOperationException if the value isn't ready yet.
        '''</summary>
        Public ReadOnly Property Value() As TValue Implements IFuture(Of TValue).Value
            Get
                If State <> FutureState.Ready Then Throw New InvalidOperationException("Attempted to get a future value before it was ready.")
                Return _value
            End Get
        End Property

        ''' <summary>
        ''' Sets the future's value and makes the future ready.
        ''' Throws a InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetValue(ByVal value As TValue)
            If Not TrySetValue(value) Then
                Throw New InvalidOperationException("Future readied more than once.")
            End If
        End Sub
        ''' <summary>
        ''' Sets the future's value and makes the future ready.
        ''' Fails if the future was already ready.
        ''' </summary>
        Public Function TrySetValue(ByVal value As TValue) As Boolean
            If Not lockReady.TryAcquire Then Return False
            Me._value = value
            RaiseEvent Readied()
            Return True
        End Function
    End Class

    Public Module ExtensionsForIFuture
        '''<summary>Runs an action once the future is ready, and returns a future for the action's completion.</summary>
        <Extension()>
        Public Function CallWhenReady(ByVal future As IFuture,
                                      ByVal action As Action) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)

            Dim lock = New OnetimeLock()
            Dim f = New Future()
            Dim handler As IFuture.ReadiedEventHandler
            handler = Sub() ThreadPool.QueueUserWorkItem(Sub()
                                                             Contract.Assume(f IsNot Nothing)
                                                             Contract.Assume(action IsNot Nothing)
                                                             Contract.Assume(future IsNot Nothing)
                                                             Contract.Assume(lock IsNot Nothing)
                                                             If lock.TryAcquire Then 'only run once
                                                                 RemoveHandler future.Readied, handler
                                                                 Call RunWithDebugTrap(action, "Future callback")
                                                                 f.SetReady()
                                                             End If
                                                         End Sub)

            AddHandler future.Readied, handler
            If future.State = FutureState.Ready Then
                'If the future was already ready, this will ensure the handler is called
                'If the future just became ready after adding the handler, this will be an ignored duplicated call
                Call handler()
            End If

            Return f
        End Function

        '''<summary>Passes the future's value to an action once ready, and returns a future for the action's completion.</summary>
        <Extension()>
        Public Function CallWhenValueReady(Of TArg1)(ByVal future As IFuture(Of TArg1),
                                                     ByVal action As Action(Of TArg1)) As IFuture
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return future.CallWhenReady(Sub()
                                            Contract.Assume(action IsNot Nothing)
                                            Contract.Assume(future IsNot Nothing)
                                            action(future.Value)
                                        End Sub)
        End Function

        '''<summary>Runs a function once the future is ready, and returns a future for the function's return value.</summary>
        <Extension()>
        Public Function EvalWhenReady(Of TReturn)(ByVal future As IFuture,
                                                  ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim f As New Future(Of TReturn)
            future.CallWhenReady(Sub()
                                     Contract.Assume(func IsNot Nothing)
                                     Contract.Assume(f IsNot Nothing)
                                     f.SetValue(func())
                                 End Sub)
            Return f
        End Function

        '''<summary>Passes the future's value to a function once ready, and returns a future for the function's return value.</summary>
        <Extension()>
        Public Function EvalWhenValueReady(Of TArg, TReturn)(ByVal future As IFuture(Of TArg),
                                                             ByVal func As Func(Of TArg, TReturn)) As IFuture(Of TReturn)
            Contract.Requires(future IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Return future.EvalWhenReady(Function()
                                            Contract.Assume(func IsNot Nothing)
                                            Contract.Assume(future IsNot Nothing)
                                            Return func(future.Value)
                                        End Function)
        End Function

        '''<summary>Wraps a normal value as an instantly ready future.</summary>
        <Extension()>
        Public Function Futurize(Of TValue)(ByVal value As TValue) As IFuture(Of TValue)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))() IsNot Nothing)
            Dim f = New Future(Of TValue)
            f.SetValue(value)
            Return f
        End Function

        '''<summary>Returns a future for the final value of a future of a future.</summary>
        <Extension()>
        Public Function Defuturize(Of TValue)(ByVal futureFutureValue As IFuture(Of IFuture(Of TValue))) As IFuture(Of TValue)
            Contract.Requires(futureFutureValue IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TValue))() IsNot Nothing)
            Dim f = New Future(Of TValue)
            futureFutureValue.CallWhenValueReady(Sub(futureValue)
                                                     Contract.Assume(futureValue IsNot Nothing)
                                                     futureValue.CallWhenValueReady(Sub(value)
                                                                                        Contract.Assume(f IsNot Nothing)
                                                                                        f.SetValue(value)
                                                                                    End Sub)
                                                 End Sub)
            Return f
        End Function

        '''<summary>Returns a future for the readyness of a future of a future.</summary>
        <Extension()>
        Public Function Defuturize(ByVal futureFuture As IFuture(Of IFuture)) As IFuture
            Contract.Requires(futureFuture IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim f = New Future
            futureFuture.CallWhenValueReady(Sub(future)
                                                Contract.Assume(future IsNot Nothing)
                                                future.CallWhenReady(Sub()
                                                                         Contract.Assume(f IsNot Nothing)
                                                                         f.SetReady()
                                                                     End Sub)
                                            End Sub)
            Return f
        End Function
    End Module
End Namespace
