Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    ''' <summary>
    ''' An <see cref="ITimer" /> that advances relative to another <see cref="ITimer" /> instance.
    ''' </summary>
    Public NotInheritable Class RelativeTimer
        Implements ITimer
        Private ReadOnly _baseTimer As ITimer
        Private ReadOnly _timeOffsetFromBase As TimeSpan
        Private ReadOnly _timeOffsetFromParent As TimeSpan

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_baseTimer IsNot Nothing)
        End Sub

        Public Sub New(parentTimer As ITimer, timeOffsetFromParent As TimeSpan)
            Contract.Requires(parentTimer IsNot Nothing)
            Contract.Ensures(Me.StartingTimeOnParentTimer = -timeOffsetFromParent)
            If parentTimer.Time < -timeOffsetFromParent Then
                Throw New ArgumentException("Negative initial time.")
            End If

            Me._timeOffsetFromParent = timeOffsetFromParent
            Me._timeOffsetFromBase = timeOffsetFromParent
            Me._baseTimer = parentTimer

            'Avoid creating doubly-relative clocks
            Dim relativeParentClock = TryCast(parentTimer, RelativeTimer)
            If relativeParentClock IsNot Nothing Then
                Contract.Assume(relativeParentClock._baseTimer IsNot Nothing)
                Me._baseTimer = relativeParentClock._baseTimer
                Me._timeOffsetFromBase += relativeParentClock._timeOffsetFromBase
            End If
            Contract.Assume(Me.StartingTimeOnParentTimer = -timeOffsetFromParent)
        End Sub

        Public ReadOnly Property StartingTimeOnParentTimer As TimeSpan
            Get
                Return -_timeOffsetFromParent
            End Get
        End Property

        Public Function At(time As TimeSpan) As Task Implements ITimer.At
            Return _baseTimer.At(time - _timeOffsetFromBase)
        End Function

        Public ReadOnly Property Time As TimeSpan Implements ITimer.Time
            Get
                Dim result = _baseTimer.Time + _timeOffsetFromBase
                Contract.Assume(result.Ticks >= 0)
                Return result
            End Get
        End Property
    End Class
End Namespace
