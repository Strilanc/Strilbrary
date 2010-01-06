Imports Strilbrary.Values
Imports Strilbrary.Exceptions

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
    <ContractClass(GetType(IFuture.ContractClass))>
    Public Interface IFuture
        '''<summary>Raised when the future becomes ready.</summary>
        Event Ready()
        '''<summary>Determines the future's state.</summary>
        ReadOnly Property State() As FutureState
        ''' <summary>
        ''' Returns the exception stored in the future.
        ''' Throws an InvalidOperationException if there is no such exception.
        ''' </summary>
        ReadOnly Property Exception() As Exception
        '''<summary>Stops the future from complaining about unhandled exceptions.</summary>
        Sub SetHandled()

        <ContractClassFor(GetType(IFuture))>
        Class ContractClass
            Implements IFuture
            Public Event Ready() Implements IFuture.Ready
            Public ReadOnly Property Exception As System.Exception Implements IFuture.Exception
                Get
                    Contract.Ensures(Contract.Result(Of Exception)() IsNot Nothing)
                    Throw New NotSupportedException
                End Get
            End Property
            Public Sub SetHandled() Implements IFuture.SetHandled
                Throw New NotSupportedException
            End Sub
            Public ReadOnly Property State As FutureState Implements IFuture.State
                Get
                    Throw New NotSupportedException
                End Get
            End Property
        End Class
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

    '''<summary>Represents something which can finish in the future.</summary>
    Public MustInherit Class FutureBase
        Implements IFuture
        Private ReadOnly lockCanSet As New OnetimeLock
        Private ReadOnly lockIsSet As New OnetimeLock
        Private _exception As Exception
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
                    Call Me.Exception.RaiseAsUnexpected("Future exception ignored.")
                Case FutureState.Unknown
                    Call New InvalidStateException("Dangling Future.").RaiseAsUnexpected("Dangling Future.")
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

        '''<summary>Determines the future's state.</summary>
        Public ReadOnly Property State() As FutureState Implements IFuture.State
            Get
                Contract.Ensures(Contract.Result(Of FutureState)() <> FutureState.Failed OrElse _exception IsNot Nothing)
                If lockIsSet.State = OnetimeLockState.Unknown Then Return FutureState.Unknown
                Return If(_exception Is Nothing, FutureState.Succeeded, FutureState.Failed)
            End Get
        End Property

        '''<summary>Stops the future from complaining about unhandled exceptions.</summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")>
        Public Sub SetHandled() Implements IFuture.SetHandled
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' Sets the future's state to failed and stores the provided exception.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetFailed(ByVal exception As Exception)
            Contract.Requires(exception IsNot Nothing)
            Contract.Ensures(Me.State = FutureState.Failed)
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
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.State = FutureState.Failed)

            If Not lockCanSet.TryAcquire() Then Return False
            Me._exception = exception
            If Not lockIsSet.TryAcquire() Then Throw New UnreachableException()

            RaiseEvent Ready()
            Return True
        End Function

        ''' <summary>
        ''' Causes the future to fail if running the given action throws an exception.
        ''' Throws an InvalidOperationException if the action fails and the future was already ready.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public Sub DependentCall(ByVal action As action)
            Contract.Requires(action IsNot Nothing)
            Try
                Call action()
            Catch ex As Exception
                SetFailed(ex)
            End Try
        End Sub

        Protected Function TrySetSucceededBase(ByVal action As action) As Boolean
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.State = FutureState.Succeeded)

            If Not lockCanSet.TryAcquire() Then Return False
            Call action()
            If Not lockIsSet.TryAcquire() Then Throw New UnreachableException()

            Me.SetHandled()
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
        ''' Throws an InvalidOperationException if the future was already ready, but will still evaluate the subroutine.
        ''' </summary>
        Public Sub SetByCalling(ByVal action As action)
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Me.State <> FutureState.Unknown)
            MyBase.DependentCall(Sub()
                                     Call action()
                                     SetSucceeded()
                                 End Sub)
            Contract.Assume(Me.State <> FutureState.Unknown)
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetSucceeded()
            Contract.Ensures(Me.State = FutureState.Succeeded)
            If Not TrySetSucceeded() Then
                Throw New InvalidOperationException("Future readied more than once.")
            End If
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded, unless the future was already ready.
        ''' Returns false if the future was already ready.
        ''' </summary>
        Public Function TrySetSucceeded() As Boolean
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.State = FutureState.Succeeded)
            Return TrySetSucceededBase(Sub()
                                       End Sub)
        End Function

        Public Overrides Function ToString() As String
            Select Case State
                Case FutureState.Unknown : Return "Not Ready"
                Case FutureState.Succeeded : Return "Succeeded"
                Case FutureState.Failed : Return "Failed: {0}".Frmt(Exception.Message)
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
                If State <> FutureState.Succeeded Then Throw New InvalidOperationException("Attempted to get a premature or failed future value.")
                Return _value
            End Get
        End Property

        ''' <summary>
        ''' Sets the future's state based on the outcome of a function.
        ''' Throws an InvalidOperationException if the future was already ready, but will still evaluate the function.
        ''' </summary>
        Public Sub SetByEvaluating(ByVal func As Func(Of TValue))
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Me.State <> FutureState.Unknown)
            MyBase.DependentCall(Sub() SetSucceeded(func()))
            Contract.Assume(Me.State <> FutureState.Unknown)
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded and stores the provided value.
        ''' Throws an InvalidOperationException if the future was already ready.
        ''' </summary>
        Public Sub SetSucceeded(ByVal value As TValue)
            Contract.Ensures(Me.State = FutureState.Succeeded)
            If Not TrySetSucceeded(value) Then
                Throw New InvalidOperationException("Future readied more than once.")
            End If
        End Sub

        ''' <summary>
        ''' Sets the future's state to succeeded and stores the provided value.
        ''' Returns false if the future was already ready.
        ''' </summary>
        Public Function TrySetSucceeded(ByVal value As TValue) As Boolean
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.State = FutureState.Succeeded)
            Return TrySetSucceededBase(Sub() Me._value = value)
        End Function

        Public Overrides Function ToString() As String
            Select Case State
                Case FutureState.Unknown : Return "Not Ready"
                Case FutureState.Succeeded : Return "Succeeded: {0}".Frmt(Value)
                Case FutureState.Failed : Return "Failed: {0}".Frmt(Exception.Message)
                Case Else : Throw State.MakeImpossibleValueException()
            End Select
        End Function
    End Class
End Namespace
