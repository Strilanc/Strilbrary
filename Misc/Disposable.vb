﻿Public Interface INotifyingDisposable
    Inherits IDisposable
    Event Disposed()
    ReadOnly Property IsDisposed As Boolean
End Interface

Public Class NotifyingDisposable
    Implements INotifyingDisposable

    Private ReadOnly lockDisposed As New OnetimeLock

    Public Event Disposed() Implements INotifyingDisposable.Disposed

    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(lockDisposed IsNot Nothing)
    End Sub

    Public ReadOnly Property IsDisposed As Boolean Implements INotifyingDisposable.IsDisposed
        Get
            Return lockDisposed.WasAcquired
        End Get
    End Property

    Protected Overridable Sub Dispose(ByVal disposingNotFinalizing As Boolean)
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        If Not lockDisposed.TryAcquire Then Return
        Dispose(disposingNotFinalizing:=True)
        RaiseEvent Disposed()
        GC.SuppressFinalize(Me)
    End Sub
    Protected NotOverridable Overrides Sub Finalize()
        If lockDisposed.TryAcquire Then
            Dispose(disposingNotFinalizing:=False)
        End If
    End Sub
End Class
