Namespace Threading
    '''<summary>Represents an object which makes its future thread-safe disposal available as a future.</summary>
    <ContractClass(GetType(IFutureDisposable.ContractClass))>
    Public Interface IFutureDisposable
        Inherits IDisposable
        ReadOnly Property FutureDisposed As IFuture

        <ContractClassFor(GetType(IFutureDisposable))>
        Class ContractClass
            Implements IFutureDisposable
            Public ReadOnly Property FutureDisposed As IFuture Implements IFutureDisposable.FutureDisposed
                Get
                    Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
                    Throw New NotSupportedException
                End Get
            End Property
            Public Sub Dispose() Implements IDisposable.Dispose
                Throw New NotSupportedException
            End Sub
        End Class
    End Interface

    '''<summary>A class which makes its future thread-safe disposal available as a future.</summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
    Public Class FutureDisposable
        Implements IFutureDisposable

        Private ReadOnly lock As New OnetimeLock
        Private ReadOnly _futureDisposed As New FutureAction()
        Public ReadOnly Property FutureDisposed As IFuture Implements IFutureDisposable.FutureDisposed
            Get
                Return _futureDisposed
            End Get
        End Property

        Public Sub New()
            _futureDisposed.MarkAnyExceptionAsHandled()
        End Sub

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(lock IsNot Nothing)
            Contract.Invariant(_futureDisposed IsNot Nothing)
        End Sub

        Protected Overridable Sub PerformDispose(ByVal finalizing As Boolean)
        End Sub

        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not lock.TryAcquire Then Return
            PerformDispose(finalizing:=False)
            _futureDisposed.SetSucceeded()
            GC.SuppressFinalize(Me)
        End Sub

        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
        Protected NotOverridable Overrides Sub Finalize()
            If Not lock.TryAcquire Then Return
            PerformDispose(finalizing:=True)
            _futureDisposed.SetSucceeded()
        End Sub
    End Class
End Namespace
