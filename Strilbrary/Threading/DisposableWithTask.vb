Imports Strilbrary.Values

Namespace Threading
    '''<summary>Represents an object which makes its future thread-safe disposal available as a future.</summary>
    <ContractClass(GetType(IDisposableWithTask.ContractClass))>
    Public Interface IDisposableWithTask
        Inherits IDisposable
        ReadOnly Property DisposalTask As Task

        <ContractClassFor(GetType(IDisposableWithTask))>
        <SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        MustInherit Class ContractClass
            Implements IDisposableWithTask
            Public ReadOnly Property DisposalTask As Task Implements IDisposableWithTask.DisposalTask
                Get
                    Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
                    Throw New NotSupportedException
                End Get
            End Property
            <SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
            <SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")>
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A class which makes its future disposal available as a future.</summary>
    <SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
    Public Class DisposableWithTask
        Implements IDisposableWithTask

        Private ReadOnly _disposeLock As New OnetimeLock
        Private ReadOnly _disposalTask As New TaskCompletionSource(Of NoValue)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_disposeLock IsNot Nothing)
            Contract.Invariant(_disposalTask IsNot Nothing)
        End Sub

        '''<summary>Becomes ready once disposal has completed.</summary>
        Public ReadOnly Property DisposalTask As Task Implements IDisposableWithTask.DisposalTask
            Get
                Contract.Assume(_disposalTask.Task IsNot Nothing)
                Return _disposalTask.Task
            End Get
        End Property

        Protected Overridable Function PerformDispose(ByVal finalizing As Boolean) As Task
            Return Nothing
        End Function

        Public ReadOnly Property IsDisposed As Boolean
            Get
                Return _disposeLock.State <> OnetimeLockState.Unknown
            End Get
        End Property

        Private Sub Dispose(ByVal finalizing As Boolean)
            If Not _disposeLock.TryAcquire Then Return

            Dim result = PerformDispose(finalizing)
            If result Is Nothing Then
                _disposalTask.SetResult(Nothing)
            Else
                result.ContinueWithAction(Sub() _disposalTask.SetResult(Nothing))
            End If
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(finalizing:=False)
            GC.SuppressFinalize(Me)
        End Sub
        '''<remarks>It is possible for outside references to still exist to _disposalTask.Task when finalization occurs.</remarks>
        <SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        Protected NotOverridable Overrides Sub Finalize()
            Dispose(finalizing:=True)
        End Sub
    End Class
End Namespace
