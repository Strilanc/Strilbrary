Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' A clock which advances relative to another clock.
    ''' </summary>
    Public Class RelativeClock
        Implements IClock
        Private ReadOnly _baseClock As IClock
        Private ReadOnly _timeOffsetFromBase As TimeSpan
        Private ReadOnly _timeOffsetFromParent As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_baseClock IsNot Nothing)
        End Sub

        Public Sub New(ByVal parentClock As IClock,
                       ByVal timeOffsetFromParent As TimeSpan)
            Contract.Requires(parentClock IsNot Nothing)
            Contract.Ensures(Me.StartingTimeOnParentClock = -timeOffsetFromParent)
            If parentClock.ElapsedTime < -timeOffsetFromParent Then
                Throw New ArgumentException("Negative initial time.")
            End If

            Me._timeOffsetFromParent = timeOffsetFromParent
            Me._timeOffsetFromBase = timeOffsetFromParent
            Me._baseClock = parentClock

            'Avoid creating doubly-relative clocks
            Dim relativeParentClock = TryCast(parentClock, RelativeClock)
            If relativeParentClock IsNot Nothing Then
                Contract.Assume(relativeParentClock._baseClock IsNot Nothing)
                Me._baseClock = relativeParentClock._baseClock
                Me._timeOffsetFromBase += relativeParentClock._timeOffsetFromBase
            End If
            Contract.Assume(Me.StartingTimeOnParentClock = -timeOffsetFromParent)
        End Sub

        Public ReadOnly Property StartingTimeOnParentClock As TimeSpan
            Get
                Return -_timeOffsetFromParent
            End Get
        End Property

        Public Function AsyncWaitUntil(ByVal time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Return _baseClock.AsyncWaitUntil(time - _timeOffsetFromBase)
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            Get
                Dim result = _baseClock.ElapsedTime + _timeOffsetFromBase
                Contract.Assume(result.Ticks >= 0)
                Return result
            End Get
        End Property
    End Class
End Namespace
