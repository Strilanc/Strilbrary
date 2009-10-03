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

            If Not lockCanSet.TryAcquire Then Return False
            Me._exception = exception
            If Not lockIsSet.TryAcquire() Then Throw New UnreachableException()

            RaiseEvent Ready()
            Return True
        End Function

        Protected Function TrySetSucceededBase(ByVal action As action) As Boolean
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.State = FutureState.Succeeded)

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
            Contract.Ensures(Me.State <> FutureState.Unknown)
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
End Namespace
