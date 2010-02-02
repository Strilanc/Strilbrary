Imports Strilbrary.Threading
Imports Strilbrary.Values
Imports Strilbrary.Collections

Namespace Time
    ''' <summary>
    ''' A clock which advances relative to another clock.
    ''' </summary>
    Public Class RelativeClock
        Implements IClock
        Private ReadOnly _parentClock As IClock
        Private ReadOnly _timeOffset As TimeSpan
        Private ReadOnly _startTime As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_parentClock IsNot Nothing)
            Contract.Invariant(_startTime.Ticks >= 0)
            Contract.Invariant(_timeOffset.Ticks >= 0)
            Contract.Invariant(_timeOffset.Ticks >= _startTime.Ticks)
        End Sub

        'verification disabled due to stupid verifier (1.2.30118.5)
        <ContractVerification(False)>
        Public Sub New(ByVal parentClock As IClock, ByVal startingTime As TimeSpan)
            Contract.Requires(parentClock IsNot Nothing)
            Contract.Requires(startingTime.Ticks >= 0)
            Contract.Ensures(Me.StartingTimeOnParentClock = startingTime)
            If startingTime > parentClock.ElapsedTime Then Throw New ArgumentException("The starting time must not be ahead of the clock's current time.", "startingTime")

            Me._startTime = startingTime
            Me._timeOffset = Me._startTime
            Me._parentClock = parentClock

            'Avoid creating doubly-relative clocks
            Dim relativeParentClock = TryCast(parentClock, RelativeClock)
            If relativeParentClock IsNot Nothing Then
                Me._parentClock = relativeParentClock._parentClock
                Me._timeOffset += relativeParentClock._timeOffset
            End If
        End Sub

        Public ReadOnly Property StartingTimeOnParentClock As TimeSpan
            'verification disabled due to stupid verifier (1.2.30118.5)
            <ContractVerification(False)>
            Get
                Contract.Ensures(Contract.Result(Of TimeSpan)().Ticks >= 0)
                Return _startTime
            End Get
        End Property

        Public Function AsyncWait(ByVal dt As TimeSpan) As IFuture Implements IClock.AsyncWait
            Return _parentClock.AsyncWait(dt)
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            <ContractVerification(False)>
            Get
                Return _parentClock.ElapsedTime - _timeOffset
            End Get
        End Property
    End Class
End Namespace
