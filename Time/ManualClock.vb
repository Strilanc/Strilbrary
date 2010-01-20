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
        Private ReadOnly lock As New Object()

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_time.Ticks >= 0)
            Contract.Invariant(_asyncWaits IsNot Nothing)
            Contract.Invariant(lock IsNot Nothing)
        End Sub

        Public Sub New()
            Contract.Assume(_time.Ticks = 0)
        End Sub

        'verification disabled due to stupid verifier (1.2.30118.5)
        <ContractVerification(False)>
        Public Sub Advance(ByVal dt As TimeSpan)
            Contract.Requires(dt.Ticks >= 0)
            Contract.Ensures(Me.Time = Contract.OldValue(Me.Time) + dt)
            SyncLock lock
                _time += dt
                While _asyncWaits.Count > 0 AndAlso _asyncWaits.Peek.Item1 <= Time
                    Dim futureAction = _asyncWaits.Dequeue.Item2
                    Contract.Assume(futureAction IsNot Nothing)
                    futureAction.SetSucceeded()
                End While
            End SyncLock
        End Sub
        Public ReadOnly Property Time As TimeSpan
            'verification disabled due to stupid verifier (1.2.30118.5)
            <ContractVerification(False)>
            Get
                Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks >= 0)
                Contract.Ensures(Contract.Result(Of TimeSpan)() = _time)
                SyncLock lock
                    Return _time
                End SyncLock
            End Get
        End Property

        Public Function AsyncWait(ByVal dt As TimeSpan) As IFuture Implements IClock.AsyncWait
            Dim result = New FutureAction()
            If dt.Ticks <= 0 Then
                result.SetSucceeded()
            Else
                SyncLock lock
                    _asyncWaits.Enqueue(New Tuple(Of TimeSpan, FutureAction)(Time + dt, result))
                End SyncLock
            End If
            Return result
        End Function

        <Pure()>
        Public Function StartTimer() As ITimer Implements IClock.StartTimer
            Return New ManualTimer(Me)
        End Function

        Private Class ManualTimer
            Implements ITimer
            Private ReadOnly _clock As ManualClock
            Private _startTime As TimeSpan

            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_clock IsNot Nothing)
            End Sub

            Public Sub New(ByVal clock As ManualClock)
                Contract.Requires(clock IsNot Nothing)
                Me._clock = clock
                Me._startTime = clock.Time
            End Sub

            <Pure()>
            Public Function ElapsedTime() As System.TimeSpan Implements ITimer.ElapsedTime
                Dim result = _clock.Time - Me._startTime
                Contract.Assume(result.Ticks >= 0)
                Return result
            End Function

            Public Function Reset() As TimeSpan Implements ITimer.Reset
                Dim result = ElapsedTime()
                _startTime += result
                Return result
            End Function
        End Class
    End Class
End Namespace
