Imports Strilbrary.Threading
Imports Strilbrary.Values

Namespace Time
    '''<summary>
    ''' A clock that advances while the program is running, but does not advance while execution is halted (eg. at debugger breakpoints).
    ''' </summary>
    ''' <remarks>
    ''' Instances share a cached PauseSkippingClock to prevent unexpected high overhead if many are allocated.
    ''' The shared cached PauseSkippingClock instance may be garbage collected when no ProgramClock instances exist.
    ''' </remarks>
    Public NotInheritable Class ProgramClock
        Implements IClock

        Private Shared ReadOnly _backingClock As New WeakReference(Nothing)
        Private Shared ReadOnly _backingClockLock As New Object()
        Private ReadOnly _clock As IClock

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_clock IsNot Nothing)
        End Sub

        Public Sub New()
            Dim clock As IClock
            SyncLock _backingClockLock
                clock = DirectCast(_backingClock.Target, IClock)
                If clock Is Nothing Then
                    clock = New PauseSkippingClock(New SystemClock())
                    _backingClock.Target = clock
                End If
            End SyncLock
            Me._clock = clock.Restarted()
        End Sub

        Public Function AsyncWaitUntil(time As TimeSpan) As Task Implements IClock.AsyncWaitUntil
            Return _clock.AsyncWaitUntil(time)
        End Function

        Public ReadOnly Property ElapsedTime As TimeSpan Implements IClock.ElapsedTime
            Get
                Return _clock.ElapsedTime
            End Get
        End Property
    End Class
End Namespace
