Imports System.Threading

Namespace Threading
    '''<summary>Runs specified actions while respecting a minimum cooldown.</summary>
    Public NotInheritable Class Throttle
        Private ReadOnly cooldown As TimeSpan
        Private nextAction As Action
        Private running As Boolean
        Private ReadOnly ref As ICallQueue = New ThreadPooledCallQueue()

        <ContractInvariantMethod()>
        Private Sub ObjectInvariant()
            Contract.Invariant(ref IsNot Nothing)
        End Sub

        Public Sub New(ByVal cooldown As TimeSpan)
            Me.cooldown = cooldown
        End Sub

        '''<summary>Sets the action to run when the cooldown finishes.</summary>
        Public Sub SetActionToRun(ByVal action As Action)
            ref.QueueAction(Sub()
                                nextAction = action
                                If Not running Then
                                    running = True
                                    RunNextAction()
                                End If
                            End Sub)
        End Sub

        '''<summary>Runs the next action and resets the cooldown.</summary>
        Private Sub RunNextAction()
            ref.QueueAction(Sub()
                                Dim action = nextAction
                                nextAction = Nothing
                                If action IsNot Nothing Then
                                    Call ThreadPooledAction(action)
                                    FutureWait(cooldown).CallOnSuccess(AddressOf RunNextAction)
                                Else
                                    running = False
                                End If
                            End Sub)
        End Sub
    End Class
End Namespace
