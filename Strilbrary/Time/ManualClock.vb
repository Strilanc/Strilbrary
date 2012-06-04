Imports Strilbrary.Threading
Imports Strilbrary.Values
Imports Strilbrary.Collections

Namespace Time
    ''' <summary>
    ''' An <see cref="IClock" /> that advances manually.
    ''' </summary>
    Public NotInheritable Class ManualClock
        Implements IClock
        Private _elapsedTime As New TimeSpan(ticks:=0)
        Private ReadOnly _waitQueue As New SortedDictionary(Of Moment, List(Of TaskCompletionSource(Of NoValue)))()
        Private ReadOnly _lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_elapsedTime.Ticks >= 0)
            Contract.Invariant(_waitQueue IsNot Nothing)
            Contract.Invariant(_lock IsNot Nothing)
        End Sub

        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Me.Time = Contract.OldValue(Me.Time) + duration")>
        Public Sub Advance(duration As TimeSpan)
            Contract.Requires(duration.Ticks >= 0)
            Contract.Ensures(Me.Time = Contract.OldValue(Me.Time) + duration)
            SyncLock _lock
                _elapsedTime += duration
                While _waitQueue.Count > 0
                    Dim e = _waitQueue.First()
                    If e.Key > Time() Then Exit While
                    _waitQueue.Remove(e.Key)
                    Contract.Assume(e.Value IsNot Nothing)
                    For Each t In e.Value
                        Contract.Assume(t IsNot Nothing)
                        t.SetResult(Nothing)
                    Next
                End While
            End SyncLock
            Contract.Assume(_elapsedTime.Ticks >= 0)
        End Sub

        Public Function Time() As Moment Implements IClock.Time
            SyncLock _lock
                Return New Moment(_elapsedTime.Ticks, Me)
            End SyncLock
        End Function

        Public Function At(time As Moment) As Task Implements IClock.At
            SyncLock _lock
                If time <= Me.Time Then Return CompletedTask()

                If Not _waitQueue.ContainsKey(time) Then _waitQueue.Add(time, New List(Of TaskCompletionSource(Of NoValue))())
                Dim result = New TaskCompletionSource(Of NoValue)
                _waitQueue(time).Add(result)
                Contract.Assume(result.Task IsNot Nothing)
                Return result.Task
            End SyncLock
        End Function
    End Class
End Namespace
