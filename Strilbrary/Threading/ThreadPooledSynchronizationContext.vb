Namespace Threading
    '''<summary>Passes posted asynchronous calls to the thread pool.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public NotInheritable Class ThreadPooledSynchronizationContext
        Inherits SynchronizationContext

        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            ThreadPool.QueueUserWorkItem(Sub() d(state))
        End Sub
        Public Overrides Sub Send(d As SendOrPostCallback, state As Object)
            If d Is Nothing Then Throw New ArgumentNullException("d")
            d(state)
        End Sub
        Public Overrides Function CreateCopy() As SynchronizationContext
            Return Me
        End Function
        Public Overrides Function ToString() As String
            Return "ThreadPooledSynchronizationContext"
        End Function
    End Class
End Namespace
