Imports System.Threading

Namespace Threading
    Public Interface ICoroutineController
        Sub Yield()
    End Interface
    Public Interface ICoroutineController(Of R)
        Sub Yield(ByVal value As R)
    End Interface

    ''' <summary>
    ''' Allows action which effectively yield control at multiple points in their execution.
    ''' </summary>
    ''' <remarks>Uses locks and an alternate thread. Not very lightweight.</remarks>
    Public NotInheritable Class Coroutine
        Implements IDisposable
        Implements ICoroutineController
        Private started As Boolean
        Private finished As Boolean
        Private exception As Exception
        Private ReadOnly lockDisposed As New OnetimeLock()
        Private ReadOnly lockJoined As New ManualResetEvent(initialState:=False)
        Private ReadOnly lockProducer As New ManualResetEvent(initialState:=True)
        Private ReadOnly lockConsumer As New ManualResetEvent(initialState:=False)

        <ContractInvariantMethod()> Protected Sub Invariant()
            Contract.Invariant(lockDisposed IsNot Nothing)
            Contract.Invariant(lockJoined IsNot Nothing)
            Contract.Invariant(lockProducer IsNot Nothing)
            Contract.Invariant(lockConsumer IsNot Nothing)
        End Sub

        Public Sub New(ByVal coroutineAction As Action(Of ICoroutineController))
            Contract.Assume(coroutineAction IsNot Nothing) 'changing to requires hit some sort of bug in 1.2 contracts

            Call ThreadedAction(
                Sub()
                    lockJoined.WaitOne()

                    Try
                        coroutineAction(Me)
                    Catch ex As Exception
                        If lockDisposed.WasAcquired Then  LogUnexpectedException("Coroutine threw an exception after being disposed.", ex)
                        exception = ex
                    End Try

                    finished = True
                    Dispose()
                End Sub
            )
        End Sub

        Public Enum ContinueOutcome
            Continuing
            Finished
        End Enum
        Public Function [Continue]() As ContinueOutcome
            If lockDisposed.WasAcquired Then Throw New ObjectDisposedException(Me.GetType.Name)

            lockProducer.Reset()
            lockConsumer.Set()
            If Not started Then
                lockJoined.Set()
                started = True
            End If
            lockProducer.WaitOne()

            If exception IsNot Nothing Then Throw New Exception("Coroutine threw an exception.", exception)
            If finished Then Return ContinueOutcome.Finished
            If lockDisposed.WasAcquired Then Throw New ObjectDisposedException(Me.GetType.Name)
            Return ContinueOutcome.Continuing
        End Function
        Private Sub [Yield]() Implements ICoroutineController.Yield
            If lockDisposed.WasAcquired Then Throw New ObjectDisposedException(Me.GetType.Name)

            lockConsumer.Reset()
            lockProducer.Set()
            lockConsumer.WaitOne()

            If lockDisposed.WasAcquired Then Throw New ObjectDisposedException(Me.GetType.Name)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If lockDisposed.TryAcquire Then
                lockProducer.Set()
                lockConsumer.Set()
                GC.SuppressFinalize(Me)
            End If
        End Sub

        Protected Overrides Sub Finalize()
            Dispose()
        End Sub
    End Class

    ''' <summary>
    ''' Allows functions which effectively yield values at multiple points in their execution.
    ''' </summary>
    Public NotInheritable Class Coroutine(Of R)
        Implements ICoroutineController(Of R)
        Implements IEnumerator(Of R)
        Private coroutineContinuer As Coroutine
        Private coroutineYielder As ICoroutineController
        Private cur As R

        Public Sub New(ByVal coroutineFunction As Action(Of ICoroutineController(Of R)))
            Contract.Requires(coroutineFunction IsNot Nothing)
            Dim cofunction_ = coroutineFunction
            Me.coroutineContinuer = New Coroutine(Sub(yielder)
                                                      Me.coroutineYielder = yielder
                                                      Call cofunction_(Me)
                                                  End Sub)
        End Sub

        Private Sub [Yield](ByVal value As R) Implements ICoroutineController(Of R).Yield
            cur = value
            coroutineYielder.Yield()
        End Sub

        Public Enum ContinueOutcome
            ProducedValue
            Finished
        End Enum
        Public Function [Continue]() As ContinueOutcome
            Dim c = coroutineContinuer.Continue()
            Select Case c
                Case Coroutine.ContinueOutcome.Continuing
                    Return ContinueOutcome.ProducedValue
                Case Coroutine.ContinueOutcome.Finished
                    Return ContinueOutcome.Finished
                Case Else
                    Throw c.ValueShouldBeImpossibleException()
            End Select
        End Function
        Public ReadOnly Property Current() As R Implements IEnumerator(Of R).Current
            Get
                Return cur
            End Get
        End Property

        Private ReadOnly Property CurrentObj As Object Implements System.Collections.IEnumerator.Current
            Get
                Return cur
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator(Of R).MoveNext
            Return Me.Continue = ContinueOutcome.ProducedValue
        End Function

        Public Sub Reset() Implements IEnumerator(Of R).Reset
            Throw New NotSupportedException()
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            coroutineContinuer.Dispose()
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            Dispose()
        End Sub
    End Class
End Namespace
