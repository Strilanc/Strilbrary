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
            Contract.Ensures(Me.State = OnetimeLockState.Acquired)
            Dim result = Interlocked.Exchange(acquired, 1) = 0
            Contract.Assume(acquired = 1)
            Return result
        End Function

        '''<summary>Determines if the lock has been acquired.</summary>
        Public ReadOnly Property State As OnetimeLockState
            Get
                Contract.Ensures(Contract.Result(Of OnetimeLockState)() = If(acquired <> 0,
                                                                             OnetimeLockState.Acquired,
                                                                             OnetimeLockState.Unknown))
                Return If(acquired <> 0,
                          OnetimeLockState.Acquired,
                          OnetimeLockState.Unknown)
            End Get
        End Property
    End Class
End Namespace
