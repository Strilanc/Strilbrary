Imports System.Threading

Public NotInheritable Class Throttle
    Private ReadOnly cooldown As TimeSpan
    Private nextAction As Action
    Private readyTime As Date
    Private running As Boolean
    Private ReadOnly lock As New Object()

    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(nextAction IsNot Nothing)
        Contract.Invariant(cooldown.Ticks >= 0)
    End Sub

    Public Sub New(ByVal cooldown As TimeSpan)
        Contract.Requires(cooldown.Ticks >= 0)
        Me.cooldown = cooldown
        Me.readyTime = Now
        Me.nextAction = Sub()
                        End Sub
    End Sub
    Public Sub SetActionToRun(ByVal action As Action)
        Contract.Requires(action IsNot Nothing)

        Dim t = Now()
        SyncLock lock
            nextAction = action
            If running Then Return
            running = True
        End SyncLock

        If t >= readyTime Then
            Execute()
        Else
            FutureWait(readyTime - t).CallWhenReady(AddressOf Execute)
        End If
    End Sub
    Private Sub Execute()
        Dim action As action
        SyncLock lock
            action = nextAction
            readyTime = Now() + cooldown
            running = False
        End SyncLock
        Call action()
    End Sub
End Class