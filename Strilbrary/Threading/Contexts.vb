Imports System.Threading

Namespace Threading
    '''<summary>Passes posted asynchronous calls to a runner method.</summary>
    Public NotInheritable Class RunnerSynchronizationContext
        Inherits SynchronizationContext
        Private ReadOnly _runner As Action(Of Action)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_runner IsNot Nothing)
        End Sub

        Public Sub New(runner As Action(Of Action))
            Contract.Requires(runner IsNot Nothing)
            Me._runner = runner
        End Sub

        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            _runner(Sub()
                        SynchronizationContext.SetSynchronizationContext(Me)
                        d(state)
                    End Sub)
        End Sub
        Public Overrides Function CreateCopy() As SynchronizationContext
            Return Me
        End Function
    End Class

    '''<summary>Delegates calls to a synchronization context which may not be available when this class is constructed.</summary>
    Public NotInheritable Class EventualSynchronizationContext
        Inherits SynchronizationContext
        Private ReadOnly _eventualContext As Task(Of SynchronizationContext)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_eventualContext IsNot Nothing)
        End Sub

        Public Sub New(eventualContext As Task(Of SynchronizationContext))
            Contract.Requires(eventualContext IsNot Nothing)
            Me._eventualContext = eventualContext
        End Sub

        Public Overrides Async Sub Post(d As SendOrPostCallback, state As Object)
            Dim c = Await _eventualContext
            c.Post(d, state)
        End Sub
        Public Overrides Sub Send(d As SendOrPostCallback, state As Object)
            Dim c = _eventualContext.Result
            Contract.Assume(c IsNot Nothing)
            c.Send(d, state)
        End Sub
        Public Overrides Function CreateCopy() As System.Threading.SynchronizationContext
            Return New EventualSynchronizationContext(_eventualContext.ContinueWithFunc(Function(c) c.CreateCopy()))
        End Function
        Public Overrides Async Sub OperationCompleted()
            Dim c = Await _eventualContext
            c.OperationCompleted()
        End Sub
        Public Overrides Async Sub OperationStarted()
            Dim c = Await _eventualContext
            c.OperationStarted()
        End Sub
        Public Overrides Function Wait(waitHandles() As IntPtr, waitAll As Boolean, millisecondsTimeout As Integer) As Integer
            If millisecondsTimeout < -1 Then Throw New ArgumentException("millisecondsTimeout < -1")
            If Not _eventualContext.Wait(millisecondsTimeout) Then Return -1
            Dim c = _eventualContext.Result
            Contract.Assume(c IsNot Nothing)
            Return c.Wait(waitHandles, waitAll, millisecondsTimeout)
        End Function
    End Class
End Namespace
