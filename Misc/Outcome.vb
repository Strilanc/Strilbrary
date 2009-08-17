'''<summary>Stores the outcome of an operation that doesn't produce a value.</summary>
Public Structure Outcome
    Public ReadOnly succeeded As Boolean
    Private ReadOnly _message As String
    Public ReadOnly Property Message As String
        Get
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Contract.Assume(_message IsNot Nothing) 'I hope you didn't use default(Outcome)!
            Return _message
        End Get
    End Property

    <ContractInvariantMethod()> Private Sub Invariant()
        Contract.Invariant(_message IsNot Nothing)
    End Sub

    Public Sub New(ByVal succeeded As Boolean, ByVal message As String)
        Contract.Requires(message IsNot Nothing)
        Me._message = message
        Me.succeeded = succeeded
    End Sub
End Structure

'''<summary>Stores the outcome of an operation that produces a value.</summary>
'''<typeparam name="R">The type of value produced by the operation</typeparam>
'''<remarks>Doesn't inherit from Outcome to allow implicit conversions from Outcome to Outcome(Of R), using CType.</remarks>
Public Structure Outcome(Of R)
    Public ReadOnly succeeded As Boolean
    Public ReadOnly Value As R
    Private ReadOnly _message As String
    Public ReadOnly Property Message As String
        Get
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Contract.Assume(_message IsNot Nothing) 'I hope you didn't use default(Outcome)!
            Return _message
        End Get
    End Property
    <ContractInvariantMethod()> Private Sub Invariant()
        Contract.Invariant(_message IsNot Nothing)
    End Sub
    Public ReadOnly Property Outcome As Outcome
        Get
            Return CType(Me, Outcome)
        End Get
    End Property

    Public Sub New(ByVal value As R, ByVal succeeded As Boolean, Optional ByVal message As String = Nothing)
        Contract.Requires(message IsNot Nothing)
        If message Is Nothing Then Throw New ArgumentNullException("message")
        Me.succeeded = succeeded
        Me._message = message
        Me.Value = value
    End Sub

    Public Shared Widening Operator CType(ByVal out As Outcome(Of R)) As Outcome
        Return New Outcome(out.succeeded, out.Message)
    End Operator
    Public Shared Widening Operator CType(ByVal out As Outcome) As Outcome(Of R)
        Return New Outcome(Of R)(Nothing, out.succeeded, out.Message)
    End Operator
End Structure

Public Module OutcomeCommon
    Public Function Success(ByVal message As String) As Outcome
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(succeeded:=True, message:=message)
    End Function
    Public Function Success(Of R)(ByVal value As R, ByVal message As String) As Outcome(Of R)
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(Of R)(value:=value, succeeded:=True, message:=message)
    End Function
    Public Function Failure(ByVal message As String) As Outcome
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(succeeded:=False, message:=message)
    End Function
    Public Function Failure(Of R)(ByVal message As String) As Outcome(Of R)
        Contract.Requires(message IsNot Nothing)
        Return Failure(Of R)(Nothing, message)
    End Function
    Public Function Failure(Of R)(ByVal value As R, ByVal message As String) As Outcome(Of R)
        Contract.Requires(message IsNot Nothing)
        Return New Outcome(Of R)(value:=value, succeeded:=False, message:=message)
    End Function

    '''<summary>Casts a future of an outcome with a value  to a future of an outcome without a value.</summary>
    '''<typeparam name="R">The type returned by the outcome with value.</typeparam>
    <Pure()> <Extension()>
    Public Function StripFutureOutcome(Of R)(ByVal future As IFuture(Of Outcome(Of R))) As IFuture(Of Outcome)
        Contract.Requires(future IsNot Nothing)
        Contract.Ensures(Contract.Result(Of IFuture(Of Outcome))() IsNot Nothing)
        Dim future_ = future
        Return future_.EvalWhenReady(Function() CType(future_.Value(), Outcome))
    End Function
End Module