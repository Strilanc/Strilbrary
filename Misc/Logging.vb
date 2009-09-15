Namespace Logging
    Public Enum LogMessageType
        Typical
        DataEvent
        DataParsed
        DataRaw
        Problem
        Negative
        Positive
    End Enum

    Public NotInheritable Class Logger
        Public Event LoggedMessage(ByVal type As LogMessageType, ByVal message As ExpensiveValue(Of String))
        Public Event LoggedFutureMessage(ByVal placeholder As String, ByVal message As IFuture(Of Outcome))
        Private ReadOnly ref As ICallQueue

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(ref IsNot Nothing)
        End Sub

        Public Sub New(Optional ByVal ref As ICallQueue = Nothing)
            Me.ref = If(ref, New ThreadPooledCallQueue())
        End Sub

        Public Sub FutureLog(ByVal placeholder As String, ByVal message As IFuture(Of Outcome))
            ref.QueueAction(Sub()
                                Contract.Assume(Me IsNot Nothing)
                                RaiseEvent LoggedFutureMessage(placeholder, message)
                            End Sub)
        End Sub
        Public Sub Log(ByVal message As ExpensiveValue(Of String), ByVal messageType As LogMessageType)
            Contract.Requires(message IsNot Nothing)
            ref.QueueAction(Sub()
                                Contract.Assume(Me IsNot Nothing)
                                RaiseEvent LoggedMessage(messageType, message)
                            End Sub)
        End Sub
        Public Sub Log(ByVal message As Func(Of String), ByVal messageType As LogMessageType)
            Contract.Requires(message IsNot Nothing)
            Log(New ExpensiveValue(Of String)(message), messageType)
        End Sub
    End Class

    '''<summary>Implements a simple way to log unexpected exceptions.</summary>
    '''<remarks>One of those rare cases where a global is appropriate.</remarks>
    Public Module UnexpectedExceptionLogging
        Public Event CaughtUnexpectedException(ByVal context As String, ByVal exception As Exception)
        Private ReadOnly ref As ICallQueue = New ThreadPooledCallQueue

        Public Function GenerateUnexpectedExceptionDescription(ByVal context As String, ByVal exception As Exception) As String
            If context Is Nothing Then Throw New ArgumentNullException("context")
            If exception Is Nothing Then Throw New ArgumentNullException("exception")

            'Generate Message
            Dim message As String
            message = "Context: " + context
            'exception information
            For inner_recurse = 0 To 10
                'info
                Dim type = exception.GetType
                Contract.Assume(type IsNot Nothing)
                message += Environment.NewLine + "Exception Type: {0}".Frmt(type.Name)
                message += Environment.NewLine + "Exception Message: {0}".Frmt(exception.Message)
                If exception.StackTrace IsNot Nothing Then
                    message += Environment.NewLine + "Stack Trace: " + Environment.NewLine + indent(exception.StackTrace.ToString())
                Else
                    message += Environment.NewLine + "Stack Trace: None"
                End If
                'next
                exception = exception.InnerException
                If exception Is Nothing Then Exit For
                message += Environment.NewLine + "[Inner Exception]"
            Next inner_recurse

            Return New String("!"c, 20) + Environment.NewLine +
                   "UNEXPECTED EXCEPTION:" + Environment.NewLine +
                   Indent(message) + Environment.NewLine +
                   New String("!"c, 20)
        End Function

        Public Sub LogUnexpectedException(ByVal context As String, ByVal exception As Exception)
            If context Is Nothing Then Throw New ArgumentNullException("context")
            If exception Is Nothing Then Throw New ArgumentNullException("exception")
            ref.QueueAction(Sub()
                                RaiseEvent CaughtUnexpectedException(context, exception)
                            End Sub)
        End Sub
    End Module
End Namespace
