Namespace Exceptions
    <DebuggerDisplay("{ToString}")>
    Public Structure PossibleException(Of TValue, TException As Exception)
        Public ReadOnly Exception As TException
        Public ReadOnly Value As TValue
        Public Sub New(ByVal value As TValue)
            Me.Value = value
        End Sub
        Public Sub New(ByVal exception As TException)
            Contract.Requires(exception IsNot Nothing)
            Me.Exception = exception
        End Sub
        Public Sub New(ByVal partialValue As TValue, ByVal exception As TException)
            Contract.Requires(exception IsNot Nothing)
            Me.Value = partialValue
            Me.Exception = exception
        End Sub
        Public Shared Widening Operator CType(ByVal value As TValue) As PossibleException(Of TValue, TException)
            Return New PossibleException(Of TValue, TException)(value)
        End Operator
        Public Shared Widening Operator CType(ByVal exception As TException) As PossibleException(Of TValue, TException)
            Contract.Requires(exception IsNot Nothing)
            Return New PossibleException(Of TValue, TException)(exception)
        End Operator
        Public Overrides Function ToString() As String
            Return "Value: {0}{1}Exception: {2}".Frmt(If(Value Is Nothing, "Null", Value.ToString),
                                                      Environment.NewLine,
                                                      If(Exception Is Nothing, "Null", Exception.ToString))
        End Function
    End Structure

    <DebuggerDisplay("{ToString}")>
        Public Structure PossibleException(Of TValue)
        Public ReadOnly Exception As Exception
        Public ReadOnly Value As TValue
        Public Sub New(ByVal value As TValue)
            Me.Value = value
        End Sub
        Public Sub New(ByVal exception As Exception)
            Contract.Requires(exception IsNot Nothing)
            Me.Exception = exception
        End Sub
        Public Sub New(ByVal partialValue As TValue, ByVal exception As Exception)
            Contract.Requires(exception IsNot Nothing)
            Me.Value = partialValue
            Me.Exception = exception
        End Sub
        Public Shared Widening Operator CType(ByVal value As TValue) As PossibleException(Of TValue)
            Return New PossibleException(Of TValue)(value)
        End Operator
        Public Shared Widening Operator CType(ByVal exception As Exception) As PossibleException(Of TValue)
            Contract.Requires(exception IsNot Nothing)
            Return New PossibleException(Of TValue)(exception)
        End Operator
        Public Overrides Function ToString() As String
            Return "Value: {0}{1}Exception: {2}".Frmt(If(Value Is Nothing, "Null", Value.ToString),
                                                      Environment.NewLine,
                                                      If(Exception Is Nothing, "Null", Exception.ToString))
        End Function
    End Structure
End Namespace
