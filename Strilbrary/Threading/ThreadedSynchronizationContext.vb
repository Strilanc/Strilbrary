Namespace Threading
    '''<summary>Passes posted asynchronous calls to a new thread.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public NotInheritable Class ThreadedSynchronizationContext
        Inherits SynchronizationContext
        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            Call New Thread(Sub() d(state)).Start()
        End Sub
        Public Overrides Sub Send(d As SendOrPostCallback, state As Object)
            If d Is Nothing Then Throw New ArgumentNullException("d")
            d(state)
        End Sub
        Public Overrides Function CreateCopy() As SynchronizationContext
            Return Me
        End Function
        Public Overrides Function ToString() As String
            Return "ThreadedSynchronizationContext"
        End Function
    End Class
End Namespace
