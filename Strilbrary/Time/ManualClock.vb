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
        Private ReadOnly _asyncWaits As New PriorityQueue(Of Tuple(Of TimeSpan, TaskCompletionSource(Of NoValue)))(Function(x, y) -x.item1.compareto(y.item1))
        Private ReadOnly _lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_time.Ticks >= 0)
            Contract.Invariant(_asyncWaits IsNot Nothing)
            Contract.Invariant(_lock IsNot Nothing)
        End Sub

        Public Sub New()
            Contract.Assume(_time.Ticks = 0)
        End Sub

        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Me.ElapsedTime = Contract.OldValue(Me.ElapsedTime) + dt")>
        Public Sub Advance(ByVal dt As TimeSpan)
            Contract.Requires(dt.Ticks >= 0)
            Contract.Ensures(Me.ElapsedTime = Contract.OldValue(Me.ElapsedTime) + dt)
            SyncLock _lock
                _time += dt
                While _asyncWaits.Count > 0
                    Dim pair = _asyncWaits.Peek()
                    Contract.Assume(pair IsNot Nothing)
                    If pair.Item1 > ElapsedTime Then Exit While
                    Dim futureAction = pair.Item2
                    Contract.Assume(futureAction IsNot Nothing)
                    futureAction.SetResult(Nothing)
                End While
            End SyncLock
            Contract.Assume(_time.Ticks >= 0)
        End Sub

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            Get
                SyncLock _lock
                    Return _time
                End SyncLock
            End Get
        End Property

        Public Function AsyncWaitUntil(ByVal time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Dim result = New TaskCompletionSource(Of NoValue)
            SyncLock _lock
                If time <= ElapsedTime Then
                    result.SetResult(Nothing)
                Else
                    _asyncWaits.Enqueue(Tuple.Create(time, result))
                End If
            End SyncLock
            Contract.Assume(result.Task IsNot Nothing)
            Return result.Task
        End Function
    End Class
End Namespace
