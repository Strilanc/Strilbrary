Imports Strilbrary.Time

Namespace Threading
    '''<summary>Runs specified actions while respecting a minimum cooldown.</summary>
    Public NotInheritable Class Throttle
        Private ReadOnly _cooldown As TimeSpan
        Private _nextAction As Action
        Private _running As Boolean
        Private ReadOnly inQueue As CallQueue
        Private ReadOnly _clock As IClock

        <ContractInvariantMethod()>
        Private Sub ObjectInvariant()
            Contract.Invariant(inQueue IsNot Nothing)
            Contract.Invariant(_clock IsNot Nothing)
            Contract.Invariant(_cooldown.Ticks >= 0)
        End Sub

        Public Sub New(cooldown As TimeSpan, clock As IClock, context As SynchronizationContext)
            Contract.Requires(cooldown.Ticks >= 0)
            Contract.Requires(clock IsNot Nothing)
            Contract.Requires(context IsNot Nothing)
            Me._cooldown = cooldown
            Me._clock = clock
            Me.inQueue = New CallQueue(context)
        End Sub

        '''<summary>Sets the action to run when the cooldown finishes, or right away if not cooling down.</summary>
        Public Async Sub SetActionToRun(action As Action)
            Await inQueue.AwaitableEntrance()

            _nextAction = action
            If _running Then Return
            _running = True
            Try
                While _nextAction IsNot Nothing
                    Dim curAction = _nextAction
                    _nextAction = Nothing
                    Dim t = _clock.AsyncWait(_cooldown)
                    curAction.Invoke()
                    Await t
                End While
            Finally
                _running = False
            End Try
        End Sub
    End Class
End Namespace
