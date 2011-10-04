Imports Strilbrary.Values

Namespace Exceptions
    '''<summary>Indicates a valid operation failed to be performed.</summary>
    Public Class OperationFailedException
        Inherits Exception
        Public Sub New(Optional message As String = Nothing,
                       Optional innerException As Exception = Nothing)
            MyBase.New(message, innerException)
        End Sub
    End Class

    '''<summary>Indicates an invalid program state has been detected (eg. due to a fault).</summary>
    Public Class InvalidStateException
        Inherits InvalidOperationException
        Public Sub New(Optional message As String = Nothing,
                       Optional innerException As Exception = Nothing)
            MyBase.New(If(message, "Reached an unexpected state."), innerException)
        End Sub
    End Class

    '''<summary>Indicates a path expected to be unreachable has been executed.</summary>
    Public Class UnreachableException
        Inherits InvalidStateException
        Public Sub New(Optional message As String = Nothing,
                       Optional innerException As Exception = Nothing)
            MyBase.New(If(message, "Reached a state which was expected to not be reachable."), innerException)
        End Sub
    End Class

    '''<summary>Indicates an internal value expected to be impossible has been encountered.</summary>
    Public Class ImpossibleValueException(Of T)
        Inherits InvalidStateException
        Public ReadOnly Value As T
        Public Sub New(value As T,
                       Optional message As String = Nothing,
                       Optional innerException As Exception = Nothing)
            MyBase.new(If(message, "The {0} value ""{1}"" was not expected.".Frmt(GetType(T).Name, String.Concat(value))),
                       innerException)
            Me.Value = value
        End Sub
    End Class

    '''<summary>Indicates the program entered an infinite loop but the problem was caught and the loop aborted.</summary>
    Public Class InfiniteLoopException
        Inherits InvalidStateException
        Public Sub New(Optional message As String = Nothing,
                       Optional innerException As Exception = Nothing)
            MyBase.New(If(message, "Detected and aborted an infinite loop."), innerException)
        End Sub
    End Class

    '''<summary>An argument exception which includes the invalid argument.</summary>
    Public Class ArgumentValueException(Of T)
        Inherits ArgumentException
        Public ReadOnly Value As T
        Public Sub New(value As T,
                       parameterName As String)
            MyBase.new(ParamName:=parameterName,
                       Message:="The {0} value ""{1}"" is not a valid argument.".Frmt(GetType(T).Name, String.Concat(value)))
            Contract.Assume(parameterName IsNot Nothing)
            Me.Value = value
        End Sub
    End Class
End Namespace
