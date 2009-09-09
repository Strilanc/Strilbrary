Imports System.Threading

Namespace Threading
    Public Enum OnetimeLockState
        '''<summary>The lock was not acquired, but now may or may not be acquired.</summary>
        Unknown
        '''<summary>The lock is definitely and permanently acquired.</summary>
        Acquired
    End Enum

    '''<summary>A thread-safe lock which can be acquired once, and never released.</summary>
    Public NotInheritable Class OnetimeLock
        Private acquired As Integer

        Public Function TryAcquire() As Boolean
            Return Interlocked.Exchange(acquired, 1) = 0
        End Function

        ''' <summary>Determines if the lock has been acquired.</summary>
        Public ReadOnly Property State As OnetimeLockState
            Get
                Return If(acquired <> 0, OnetimeLockState.Acquired, OnetimeLockState.Unknown)
            End Get
        End Property
    End Class
End Namespace
