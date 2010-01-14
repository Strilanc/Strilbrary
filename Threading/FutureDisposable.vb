Namespace Threading
    '''<summary>Represents an object which makes its future thread-safe disposal available as a future.</summary>
    <ContractClass(GetType(IFutureDisposable.ContractClass))>
    Public Interface IFutureDisposable
        Inherits IDisposable
        ReadOnly Property FutureDisposed As IFuture

        <ContractClassFor(GetType(IFutureDisposable))>
        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        Class ContractClass
            Implements IFutureDisposable
            Public ReadOnly Property FutureDisposed As IFuture Implements IFutureDisposable.FutureDisposed
                Get
                    Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
                    Throw New NotSupportedException
                End Get
            End Property
            <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
            <CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")>
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A class which makes its future disposal available as a future.</summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
    Public Class FutureDisposable
        Implements IFutureDisposable

        Private ReadOnly lock As New OnetimeLock
        Private ReadOnly _futureDisposed As New FutureAction()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(lock IsNot Nothing)
            Contract.Invariant(_futureDisposed IsNot Nothing)
        End Sub

        Public Sub New()
            _futureDisposed.SetHandled() 'in case _futureDisposed were finalized before this class and complained about not being set
        End Sub

        '''<summary>Becomes ready once disposal has completed.</summary>
        Public ReadOnly Property FutureDisposed As IFuture Implements IFutureDisposable.FutureDisposed
            Get
                Return _futureDisposed
            End Get
        End Property

        Protected Overridable Function PerformDispose(ByVal finalizing As Boolean) As IFuture
            Return Nothing
        End Function

        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not lock.TryAcquire Then Return
            GC.SuppressFinalize(Me)

            Dim result = PerformDispose(finalizing:=False)
            If result Is Nothing Then
                _futureDisposed.SetSucceeded()
            Else
                result.CallWhenReady(Sub() _futureDisposed.SetSucceeded())
            End If
        End Sub

        '''<remarks>It is possible for references to still exist to _futureDisposed when finalization occurs.</remarks>
        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        Protected NotOverridable Overrides Sub Finalize()
            If Not lock.TryAcquire Then Return

            Dim result = PerformDispose(finalizing:=True)
            If result Is Nothing Then
                _futureDisposed.SetSucceeded()
            Else
                result.CallWhenReady(Sub() _futureDisposed.SetSucceeded())
            End If
        End Sub
    End Class
End Namespace
