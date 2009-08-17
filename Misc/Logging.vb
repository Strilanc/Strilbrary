Namespace Logging
    Public Enum LogMessageTypes
        Typical
        DataEvent
        DataParsed
        DataRaw
        Problem
        Negative
        Positive
    End Enum

    Public Class Logger
        Public Event LoggedMessage(ByVal type As LogMessageTypes, ByVal message As ExpensiveValue(Of String))
        Public Event LoggedFutureMessage(ByVal placeholder As String, ByVal message As IFuture(Of Outcome))
        Private ReadOnly ref As ICallQueue

        Public Sub New(Optional ByVal ref As ICallQueue = Nothing)
            Me.ref = If(ref, New ThreadPooledCallQueue())
        End Sub

        Public Sub FutureLog(ByVal placeholder As String, ByVal message As IFuture(Of Outcome))
            ref.QueueAction(Sub()
                                RaiseEvent LoggedFutureMessage(placeholder, message)
                            End Sub)
        End Sub
        Public Sub Log(ByVal message As ExpensiveValue(Of String), ByVal messageType As LogMessageTypes)
            ref.QueueAction(Sub()
                                RaiseEvent LoggedMessage(messageType, message)
                            End Sub)
        End Sub
        Public Sub Log(ByVal message As Func(Of String), ByVal messageType As LogMessageTypes)
            Log(New ExpensiveValue(Of String)(message), messageType)
        End Sub
    End Class

    '''<summary>Implements a simple way to log unexpected exceptions.</summary>
    '''<remarks>One of those rare cases where a global is appropriate.</remarks>
    Public Module UnexpectedExceptionLogging
        Public Event CaughtUnexpectedException(ByVal context As String, ByVal e As Exception)
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
                message += Environment.NewLine + "Exception Type: " + exception.GetType.Name
                message += Environment.NewLine + "Exception Message: " + exception.Message
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
            'wrapper formating
            message = "UNEXPECTED EXCEPTION:" + Environment.NewLine + indent(message)
            message = New String("!"c, 20) + Environment.NewLine + message
            message += Environment.NewLine + New String("!"c, 20)
            Return message
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
