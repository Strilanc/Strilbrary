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
            _runner(Sub() d(state))
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

        Public Overrides Sub Post(d As SendOrPostCallback, state As Object)
            _eventualContext.ContinueWithAction(Sub(c) c.Post(d, state))
        End Sub
        Public Overrides Sub Send(d As SendOrPostCallback, state As Object)
            _eventualContext.ContinueWithAction(Sub(c) c.Send(d, state)).Wait()
        End Sub
        Public Overrides Function CreateCopy() As System.Threading.SynchronizationContext
            Return New EventualSynchronizationContext(_eventualContext.ContinueWithFunc(Function(c) c.CreateCopy()))
        End Function
        Public Overrides Sub OperationCompleted()
            _eventualContext.ContinueWithAction(Sub(c) c.OperationCompleted())
        End Sub
        Public Overrides Sub OperationStarted()
            _eventualContext.ContinueWithAction(Sub(c) c.OperationStarted())
        End Sub
        <SuppressMessage("Microsoft.Contracts", "Requires-13-64")>
        Public Overrides Function Wait(waitHandles() As IntPtr, waitAll As Boolean, millisecondsTimeout As Integer) As Integer
            Dim result = -1
            _eventualContext.ContinueWithAction(
                    Sub(c) result = c.Wait(waitHandles, waitAll, millisecondsTimeout)
                ).Wait(millisecondsTimeout)
            Return result
        End Function
    End Class
End Namespace
