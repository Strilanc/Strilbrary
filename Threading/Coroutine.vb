Imports System.Threading

Namespace Threading
    Public Interface ICoroutineController
        Sub Yield()
    End Interface
    Public Interface ICoroutineController(Of TReturn)
        Sub Yield(ByVal value As TReturn)
    End Interface
    Public Enum CoroutineOutcome
        Continuing
        Finished
    End Enum

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

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(lockDisposed IsNot Nothing)
            Contract.Invariant(lockJoined IsNot Nothing)
            Contract.Invariant(lockProducer IsNot Nothing)
            Contract.Invariant(lockConsumer IsNot Nothing)
        End Sub

        Public Sub New(ByVal coroutineAction As Action(Of ICoroutineController))
            Contract.Assume(coroutineAction IsNot Nothing)

            Call ThreadedAction(
                Sub()
                    Contract.Assume(Me IsNot Nothing)
                    Contract.Assume(lockJoined IsNot Nothing)
                    Contract.Assume(coroutineAction IsNot Nothing)
                    Me.lockJoined.WaitOne()

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

        Public Function [Continue]() As CoroutineOutcome
            If lockDisposed.WasAcquired Then Throw New ObjectDisposedException(Me.GetType.Name)

            lockProducer.Reset()
            lockConsumer.Set()
            If Not started Then
                lockJoined.Set()
                started = True
            End If
            lockProducer.WaitOne()

            If exception IsNot Nothing Then Throw New InvalidOperationException("Coroutine threw an exception.", exception)
            If finished Then Return CoroutineOutcome.Finished
            If lockDisposed.WasAcquired Then Throw New ObjectDisposedException(Me.GetType.Name)
            Return CoroutineOutcome.Continuing
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
    Public NotInheritable Class Coroutine(Of TReturn)
        Implements ICoroutineController(Of TReturn)
        Implements IEnumerator(Of TReturn)
        Private coroutineContinuer As Coroutine
        Private coroutineYielder As ICoroutineController
        Private cur As TReturn

        <ContractInvariantMethod()> Protected Sub ObjectInvariant()
            Contract.Invariant(coroutineContinuer IsNot Nothing)
        End Sub

        Public Sub New(ByVal coroutineFunction As Action(Of ICoroutineController(Of TReturn)))
            'Contract.Requires(coroutineFunction IsNot Nothing)
            Me.coroutineContinuer = New Coroutine(Sub(yielder)
                                                      Contract.Assume(Me IsNot Nothing)
                                                      Contract.Assume(coroutineFunction IsNot Nothing)
                                                      Me.coroutineYielder = yielder
                                                      Call coroutineFunction(Me)
                                                  End Sub)
        End Sub

        Private Sub [Yield](ByVal value As TReturn) Implements ICoroutineController(Of TReturn).Yield
            Contract.Assume(Me.coroutineYielder IsNot Nothing)
            cur = value
            coroutineYielder.Yield()
        End Sub

        Public Function [Continue]() As CoroutineOutcome
            Return coroutineContinuer.Continue()
        End Function
        Public ReadOnly Property Current() As TReturn Implements IEnumerator(Of TReturn).Current
            Get
                Return cur
            End Get
        End Property

        Private ReadOnly Property CurrentObj As Object Implements System.Collections.IEnumerator.Current
            Get
                Return cur
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator(Of TReturn).MoveNext
            Return Me.Continue() = CoroutineOutcome.Continuing
        End Function

        Public Sub Reset() Implements IEnumerator(Of TReturn).Reset
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
