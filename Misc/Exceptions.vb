<Serializable()>
Public Class OperationFailedException
    Inherits Exception
    Public Sub New(Optional ByVal message As String = Nothing,
                   Optional ByVal innerException As Exception = Nothing)
        MyBase.New(message, innerException)
    End Sub
End Class

<Serializable()>
Public Class InvalidStateException
    Inherits Exception
    Public Sub New(Optional ByVal message As String = Nothing,
                   Optional ByVal innerException As Exception = Nothing)
        MyBase.New(If(message, "Reached an invalid state."), innerException)
    End Sub
End Class

<Serializable()>
Public Class UnreachableException
    Inherits InvalidStateException
    Public Sub New(Optional ByVal message As String = Nothing,
                   Optional ByVal innerException As Exception = Nothing)
        MyBase.New(If(message, "Reached a state which was expected to not be reachable."), innerException)
    End Sub
End Class

Public Class ImpossibleValueException(Of E)
    Inherits UnreachableException
    Public ReadOnly Value As E
    Public Sub New(ByVal value As E,
                   Optional ByVal message As String = Nothing,
                   Optional ByVal innerException As Exception = Nothing)
        MyBase.new(If(message, "The {0} value ""{1}"" was not expected.".Frmt(GetType(E).Name,
                                                                              If(value Is Nothing, "Nothing", value.ToString))),
                   innerException)
        Me.Value = value
    End Sub
End Class

Public Module ExceptionExtensions
    <Extension()>
    Public Function ValueShouldBeImpossibleException(Of E)(ByVal this As E) As ImpossibleValueException(Of E)
        Return New ImpossibleValueException(Of E)(this)
    End Function
End Module

<DebuggerDisplay("{ToString}")>
Public Structure PossibleException(Of T, E As Exception)
    Public ReadOnly Exception As E
    Public ReadOnly Value As T
    Public Sub New(ByVal value As T)
        Me.Value = value
    End Sub
    Public Sub New(ByVal exception As E)
        If exception Is Nothing Then Throw New ArgumentException("exception")
        Me.Exception = exception
    End Sub
    Public Sub New(ByVal partialValue As T, ByVal exception As E)
        If exception Is Nothing Then Throw New ArgumentException("exception")
        Me.Value = partialValue
        Me.Exception = exception
    End Sub
    Public Shared Widening Operator CType(ByVal value As T) As PossibleException(Of T, E)
        Return New PossibleException(Of T, E)(value)
    End Operator
    Public Shared Widening Operator CType(ByVal exception As E) As PossibleException(Of T, E)
        Return New PossibleException(Of T, E)(exception)
    End Operator
    Public Overrides Function ToString() As String
        Return "Value: {0}{1}Exception: {2}".Frmt(If(Value.IsNullReferenceGeneric, "Null", Value.ToString),
                                                  Environment.NewLine,
                                                  If(Exception Is Nothing, "Null", Exception.ToString))
    End Function
End Structure
<DebuggerDisplay("{ToString}")>
Public Structure PossibleException(Of T)
    Public ReadOnly Exception As Exception
    Public ReadOnly Value As T
    Public Sub New(ByVal value As T)
        Me.Value = value
    End Sub
    Public Sub New(ByVal exception As Exception)
        If exception Is Nothing Then Throw New ArgumentException("exception")
        Me.Exception = exception
    End Sub
    Public Sub New(ByVal partialValue As T, ByVal exception As Exception)
        If exception Is Nothing Then Throw New ArgumentException("exception")
        Me.Value = partialValue
        Me.Exception = exception
    End Sub
    Public Shared Widening Operator CType(ByVal value As T) As PossibleException(Of T)
        Return New PossibleException(Of T)(value)
    End Operator
    Public Shared Widening Operator CType(ByVal exception As Exception) As PossibleException(Of T)
        Return New PossibleException(Of T)(exception)
    End Operator
    Public Overrides Function ToString() As String
        Return "Value: {0}{1}Exception: {2}".Frmt(If(Value.IsNullReferenceGeneric, "Null", Value.ToString),
                                                  Environment.NewLine,
                                                  If(Exception Is Nothing, "Null", Exception.ToString))
    End Function
End Structure
