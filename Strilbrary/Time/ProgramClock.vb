Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>
    ''' An <see cref="IClock" /> that advances in real time while the program is running, but does not advance while execution is halted.
    ''' For example, the timer will stop advancing during debugger breakpoints and during system hibernation.
    ''' </summary>
    ''' <remarks>
    ''' Instances share a cached <see cref="PauseSkippingClock" /> to prevent unexpected high overhead if many are allocated.
    ''' The shared cached <see cref="PauseSkippingClock" /> instance can be garbage collected when no <see cref="ProgramClock" /> instances exist.
    ''' </remarks>
    Public NotInheritable Class ProgramClock
        Implements IClock

        Private Shared ReadOnly _backingClock As New WeakReference(Of IClock)(Nothing)
        Private Shared ReadOnly _backingClockLock As New Object()
        Private ReadOnly _clock As IClock

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_clock IsNot Nothing)
        End Sub

        Public Sub New()
            SyncLock _backingClockLock
                If Not _backingClock.TryGetTarget(_clock) Then
                    _clock = New PauseSkippingClock(New SystemClock())
                    _backingClock.SetTarget(_clock)
                End If
            End SyncLock
            Contract.Assume(_clock IsNot Nothing)
        End Sub

        Public Function At(time As Moment) As Task Implements IClock.At
            Return _clock.At(New Moment(time.Ticks, _clock))
        End Function

        Public Function Time() As Moment Implements IClock.Time
            Return New Moment(_clock.Time().Ticks, Me)
        End Function
    End Class
End Namespace
