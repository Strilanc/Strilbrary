Imports System.Threading

Namespace Threading
    Public Enum OnetimeLockState
        '''<summary>The lock was not acquired, but now may or may not be acquired.</summary>
        Unknown
        '''<summary>The lock is definitely and permanently acquired.</summary>
        Acquired
    End Enum

    '''<summary>A thread-safe lock which can be acquired once, and never released.</summary>
    <DebuggerDisplay("{ToString}")>
    Public NotInheritable Class OnetimeLock
        Private _acquired As Integer

        '''<summary>Tries to permanently acquire the lock. Returns true exactly once, then always returns false.</summary>
        Public Function TryAcquire() As Boolean
            Contract.Ensures(Me.State = OnetimeLockState.Acquired)
            Dim result = Interlocked.Exchange(_acquired, 1) = 0
            Contract.Assume(_acquired = 1)
            Return result
        End Function

        '''<summary>Determines if the lock has been acquired.</summary>
        Public ReadOnly Property State As OnetimeLockState
            Get
                Contract.Ensures(Contract.Result(Of OnetimeLockState)() = If(_acquired <> 0,
                                                                             OnetimeLockState.Acquired,
                                                                             OnetimeLockState.Unknown))
                Return If(_acquired <> 0,
                          OnetimeLockState.Acquired,
                          OnetimeLockState.Unknown)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Select Case State
                Case OnetimeLockState.Acquired : Return "Acquired"
                Case OnetimeLockState.Unknown : Return "Not Acquired"
                Case Else : Return "?"
            End Select
        End Function
    End Class
End Namespace
