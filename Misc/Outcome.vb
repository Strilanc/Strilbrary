'''<summary>Stores the outcome of an operation that doesn't produce a value.</summary>
Public Structure Outcome
    Public ReadOnly succeeded As Boolean
    Private ReadOnly _message As String
    Public ReadOnly Property Message As String
        Get
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            If _message Is Nothing Then Throw New InvalidStateException("Outcome not initialized")
            Return _message
        End Get
    End Property

    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(_message IsNot Nothing)
    End Sub

    Public Sub New(ByVal succeeded As Boolean, ByVal message As String)
        Contract.Requires(message IsNot Nothing)
        Me._message = message
        Me.succeeded = succeeded
    End Sub
End Structure

'''<summary>Stores the outcome of an operation that produces a value.</summary>
'''<typeparam name="TValue">The type of value produced by the operation</typeparam>
'''<remarks>Doesn't inherit from Outcome to allow implicit conversions from Outcome to Outcome(Of R), using CType.</remarks>
Public Structure Outcome(Of TValue)
    Public ReadOnly succeeded As Boolean
    Public ReadOnly Value As TValue
    Private ReadOnly _message As String
    Public ReadOnly Property Message As String
        Get
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            If _message Is Nothing Then Throw New InvalidStateException("Outcome not initialized")
            Return _message
        End Get
    End Property
    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(_message IsNot Nothing)
    End Sub
    Public ReadOnly Property Outcome As Outcome
        Get
            Return CType(Me, Outcome)
        End Get
    End Property

    Public Sub New(ByVal value As TValue, ByVal succeeded As Boolean, Optional ByVal message As String = Nothing)
        Contract.Requires(message IsNot Nothing)
        Me.succeeded = succeeded
        Me._message = message
        Me.Value = value
    End Sub

    Public Shared Widening Operator CType(ByVal out As Outcome(Of TValue)) As Outcome
        Return New Outcome(out.succeeded, out.Message)
    End Operator
    Public Shared Widening Operator CType(ByVal out As Outcome) As Outcome(Of TValue)
        Return New Outcome(Of TValue)(Nothing, out.succeeded, out.Message)
    End Operator
End Structure

Public Module OutcomeCommon
    Public Function Success(ByVal message As String) As Outcome
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(succeeded:=True, message:=message)
    End Function
    Public Function Success(Of TValue)(ByVal value As TValue, ByVal message As String) As Outcome(Of TValue)
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(Of TValue)(value:=value, succeeded:=True, message:=message)
    End Function
    Public Function Failure(ByVal message As String) As Outcome
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(succeeded:=False, message:=message)
    End Function
    Public Function Failure(Of TValue)(ByVal message As String) As Outcome(Of TValue)
        Contract.Requires(message IsNot Nothing)
        Return Failure(Of TValue)(Nothing, message)
    End Function
    Public Function Failure(Of TValue)(ByVal value As TValue, ByVal message As String) As Outcome(Of TValue)
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(Of TValue)(value:=value, succeeded:=False, message:=message)
    End Function

    '''<summary>Casts a future of an outcome with a value  to a future of an outcome without a value.</summary>
    '''<typeparam name="TValue">The type returned by the outcome with value.</typeparam>
    <Pure()> <Extension()>
    Public Function StripFutureOutcome(Of TValue)(ByVal future As IFuture(Of Outcome(Of TValue))) As IFuture(Of Outcome)
        Contract.Requires(future IsNot Nothing)
        Contract.Ensures(Contract.Result(Of IFuture(Of Outcome))() IsNot Nothing)
        Return future.EvalWhenReady(Function()
                                        Contract.Assume(future IsNot Nothing)
                                        Return CType(future.Value(), Outcome)
                                    End Function)
    End Function
End Module