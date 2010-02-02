Imports Strilbrary.Threading
Imports Strilbrary.Values
Imports Strilbrary.Collections

Namespace Time
    ''' <summary>
    ''' A clock which advances manually.
    ''' </summary>
    Public Class ManualClock
        Implements IClock
        Private _time As New TimeSpan(ticks:=0)
        Private ReadOnly _asyncWaits As New PriorityQueue(Of Tuple(Of TimeSpan, FutureAction))(Function(x, y) -x.item1.compareto(y.item1))
        Private ReadOnly _lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_time.Ticks >= 0)
            Contract.Invariant(_asyncWaits IsNot Nothing)
            Contract.Invariant(_lock IsNot Nothing)
        End Sub

        Public Sub New()
            Contract.Assume(_time.Ticks = 0)
        End Sub

        'verification disabled due to stupid verifier (1.2.30118.5)
        <ContractVerification(False)>
        Public Sub Advance(ByVal dt As TimeSpan)
            Contract.Requires(dt.Ticks >= 0)
            Contract.Ensures(Me.ElapsedTime = Contract.OldValue(Me.ElapsedTime) + dt)
            SyncLock _lock
                _time += dt
                While _asyncWaits.Count > 0 AndAlso _asyncWaits.Peek.Item1 <= ElapsedTime
                    Dim futureAction = _asyncWaits.Dequeue.Item2
                    Contract.Assume(futureAction IsNot Nothing)
                    futureAction.SetSucceeded()
                End While
            End SyncLock
        End Sub

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            'verification disabled due to stupid verifier (1.2.30118.5)
            <ContractVerification(False)>
            Get
                SyncLock _lock
                    Return _time
                End SyncLock
            End Get
        End Property

        Public Function AsyncWait(ByVal dt As TimeSpan) As IFuture Implements IClock.AsyncWait
            Dim result = New FutureAction()
            If dt.Ticks <= 0 Then
                result.SetSucceeded()
            Else
                SyncLock _lock
                    _asyncWaits.Enqueue(New Tuple(Of TimeSpan, FutureAction)(ElapsedTime + dt, result))
                End SyncLock
            End If
            Return result
        End Function
    End Class
End Namespace
