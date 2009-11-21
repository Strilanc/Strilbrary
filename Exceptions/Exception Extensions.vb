Namespace Exceptions
    Public Module ExceptionExtensions
        Public Event UnexpectedException(ByVal exception As Exception, ByVal context As String)

        <Extension()>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")>
        Public Sub RaiseAsUnexpected(ByVal exception As Exception,
                                     ByVal context As String)
            Contract.Requires(context IsNot Nothing)
            Contract.Requires(exception IsNot Nothing)
            ThreadPooledAction(Sub() RaiseEvent UnexpectedException(exception, context)).SetHandled()
        End Sub

        <Extension()>
        Public Function MakeImpossibleValueException(Of T)(ByVal this As T) As ImpossibleValueException(Of T)
            Contract.Ensures(Contract.Result(Of ImpossibleValueException(Of T))() IsNot Nothing)
            Return New ImpossibleValueException(Of T)(this)
        End Function

        <Extension()>
        Public Function MakeArgumentValueException(Of T)(ByVal this As T, ByVal parameterName As String) As ArgumentValueException(Of T)
            Contract.Requires(parameterName IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ArgumentValueException(Of T))() IsNot Nothing)
            Return New ArgumentValueException(Of T)(this, parameterName)
        End Function
    End Module
End Namespace
