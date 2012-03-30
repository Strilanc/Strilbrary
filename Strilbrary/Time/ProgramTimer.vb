Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>
    ''' An <see cref="ITimer" /> that advances in real time while the program is running, but does not advance while execution is halted.
    ''' For example, the timer will stop advancing during debugger breakpoints and during system hibernation.
    ''' </summary>
    ''' <remarks>
    ''' Instances share a cached <see cref="PauseSkippingTimer" /> to prevent unexpected high overhead if many are allocated.
    ''' The shared cached <see cref="PauseSkippingTimer" /> instance can be garbage collected when no <see cref="ProgramTimer" /> instances exist.
    ''' </remarks>
    Public NotInheritable Class ProgramTimer
        Implements ITimer

        Private Shared ReadOnly _backingTimer As New WeakReference(Of ITimer)(Nothing)
        Private Shared ReadOnly _backingTimerLock As New Object()
        Private ReadOnly _clock As ITimer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_clock IsNot Nothing)
        End Sub

        Public Sub New()
            Dim clock As ITimer = Nothing
            SyncLock _backingTimerLock
                If Not _backingTimer.TryGetTarget(clock) Then
                    clock = New PauseSkippingTimer(New SystemTimer())
                    _backingTimer.SetTarget(clock)
                End If
            End SyncLock
            Contract.Assume(clock IsNot Nothing)
            Me._clock = clock.Restarted()
        End Sub

        Public Function At(time As TimeSpan) As Task Implements ITimer.At
            Return _clock.At(time)
        End Function

        Public ReadOnly Property Time As TimeSpan Implements ITimer.Time
            Get
                Return _clock.Time
            End Get
        End Property
    End Class
End Namespace
