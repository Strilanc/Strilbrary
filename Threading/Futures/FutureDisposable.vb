Namespace Threading
    '''<summary>Represents an object which makes its future thread-safe disposal available as a future.</summary>
    <ContractClass(GetType(ContractClassForIFutureDisposable))>
        Public Interface IFutureDisposable
        Inherits IDisposable
        ReadOnly Property FutureDisposed As IFuture
    End Interface

    <ContractClassFor(GetType(IFutureDisposable))>
    Public NotInheritable Class ContractClassForIFutureDisposable
        Implements IFutureDisposable
        Public ReadOnly Property FutureDisposed As IFuture Implements IFutureDisposable.FutureDisposed
            Get
                Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
                Throw New InvalidOperationException
            End Get
        End Property
        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub
    End Class

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
