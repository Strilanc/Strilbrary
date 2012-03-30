Imports Strilbrary.Threading
Imports Strilbrary.Values
Imports Strilbrary.Collections

Namespace Time
    ''' <summary>
    ''' An <see cref="ITimer" /> that advances manually.
    ''' </summary>
    Public NotInheritable Class ManualTimer
        Implements ITimer
        Private _time As New TimeSpan(ticks:=0)
        Private ReadOnly _waitQueue As New PriorityQueue(Of Tuple(Of TimeSpan, TaskCompletionSource(Of NoValue)))(Function(x, y) y.Item1.CompareTo(x.Item1))
        Private ReadOnly _lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_time.Ticks >= 0)
            Contract.Invariant(_waitQueue IsNot Nothing)
            Contract.Invariant(_lock IsNot Nothing)
        End Sub

        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Me.ElapsedTime = Contract.OldValue(Me.ElapsedTime) + dt")>
        Public Sub Advance(duration As TimeSpan)
            Contract.Requires(duration.Ticks >= 0)
            Contract.Ensures(Me.Time = Contract.OldValue(Me.Time) + duration)
            SyncLock _lock
                _time += duration
                While _waitQueue.Count > 0
                    Dim timeTaskPair = _waitQueue.Peek()
                    Contract.Assume(timeTaskPair IsNot Nothing)
                    If timeTaskPair.Item1 > Time Then Exit While
                    _waitQueue.Dequeue()
                    Contract.Assume(timeTaskPair.Item2 IsNot Nothing)
                    timeTaskPair.Item2.SetResult(Nothing)
                End While
            End SyncLock
            Contract.Assume(_time.Ticks >= 0)
        End Sub

        Public ReadOnly Property Time As TimeSpan Implements ITimer.Time
            Get
                SyncLock _lock
                    Return _time
                End SyncLock
            End Get
        End Property

        Public Function At(time As TimeSpan) As Task Implements ITimer.At
            SyncLock _lock
                If time <= Time Then Return CompletedTask()

                Dim result = New TaskCompletionSource(Of NoValue)
                _waitQueue.Enqueue(Tuple.Create(time, result))
                Contract.Assume(result.Task IsNot Nothing)
                Return result.Task
            End SyncLock
        End Function
    End Class
End Namespace
