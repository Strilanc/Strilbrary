Namespace Threading
    '''<summary>Runs specified actions while respecting a minimum cooldown.</summary>
    Public NotInheritable Class Throttle
        Private ReadOnly _cooldown As TimeSpan
        Private _nextAction As Action
        Private _running As Boolean
        Private ReadOnly inQueue As ICallQueue = New ThreadPooledCallQueue()
        Private ReadOnly _clock As Time.IClock

        <ContractInvariantMethod()>
        Private Sub ObjectInvariant()
            Contract.Invariant(inQueue IsNot Nothing)
            Contract.Invariant(_clock IsNot Nothing)
            Contract.Invariant(_cooldown.Ticks >= 0)
        End Sub

        Public Sub New(ByVal cooldown As TimeSpan, ByVal clock As Time.IClock)
            Contract.Requires(cooldown.Ticks >= 0)
            Contract.Requires(clock IsNot Nothing)
            Me._cooldown = cooldown
            Me._clock = clock
        End Sub

        '''<summary>Sets the action to run when the cooldown finishes, or right away if not coolding down.</summary>
        Public Sub SetActionToRun(ByVal action As Action)
            inQueue.QueueAction(
                Sub()
                    _nextAction = action
                    If Not _running Then
                        _running = True
                        OnReadyToRun()
                    End If
                End Sub)
        End Sub

        '''<summary>Runs the next action and resets the cooldown.</summary>
        Private Sub OnReadyToRun()
            inQueue.QueueAction(
                Sub()
                    Dim actionToRun = _nextAction
                    _nextAction = Nothing
                    If actionToRun IsNot Nothing Then
                        Call ThreadPooledAction(actionToRun)
                        _clock.AsyncWait(_cooldown).CallOnSuccess(AddressOf OnReadyToRun)
                    Else
                        _running = False
                    End If
                End Sub)
        End Sub
    End Class
End Namespace
