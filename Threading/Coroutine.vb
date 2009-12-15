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
        FinishedAndDisposed
    End Enum

    Public Class CoroutineException
        Inherits Exception
        Public Sub New(Optional ByVal message As String = Nothing,
                       Optional ByVal innerException As Exception = Nothing)
            MyBase.New(message, innerException)
        End Sub
    End Class

    ''' <summary>
    ''' Allows action which effectively yield control at multiple points in their execution.
    ''' </summary>
    ''' <remarks>Uses locks and an alternate thread. Not very lightweight.</remarks>
    Public NotInheritable Class Coroutine
        Inherits FutureDisposable
        Implements ICoroutineController
        Private started As Boolean
        Private finished As Boolean
        Private coexception As Exception
        Private ReadOnly lockJoined As New ManualResetEvent(initialState:=False)
        Private ReadOnly lockProducer As New ManualResetEvent(initialState:=True)
        Private ReadOnly lockConsumer As New ManualResetEvent(initialState:=False)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(coexception Is Nothing OrElse finished)
            Contract.Invariant(lockJoined IsNot Nothing)
            Contract.Invariant(lockProducer IsNot Nothing)
            Contract.Invariant(lockConsumer IsNot Nothing)
        End Sub

        <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public Sub New(ByVal coroutineAction As Action(Of ICoroutineController))
            Contract.Requires(coroutineAction IsNot Nothing)

            Call ThreadedAction(
                Sub()
                    Me.lockJoined.WaitOne()

                    Try
                        coroutineAction(Me)
                    Catch ex As Exception
                        coexception = ex
                    End Try

                    If FutureDisposed.State <> FutureState.Unknown Then
                        coexception = New ObjectDisposedException(Me.GetType.Name, coexception)
                    End If
                    finished = True
                    lockProducer.Set()
                    lockConsumer.Set()
                End Sub
            )
        End Sub

        Public Function [Continue]() As CoroutineOutcome
            CheckNotDisposed()

            lockProducer.Reset()
            lockConsumer.Set()
            If Not started Then
                lockJoined.Set()
                started = True
            End If
            lockProducer.WaitOne()

            If coexception IsNot Nothing Then Throw New CoroutineException("Coroutine threw an exception.", coexception)
            If finished Then
                Dispose()
                Return CoroutineOutcome.FinishedAndDisposed
            Else
                CheckNotDisposed()
                Return CoroutineOutcome.Continuing
            End If
        End Function
        Private Sub [Yield]() Implements ICoroutineController.Yield
            CheckNotDisposed()

            lockConsumer.Reset()
            lockProducer.Set()
            lockConsumer.WaitOne()

            CheckNotDisposed()
        End Sub

        Private Sub CheckNotDisposed()
            If FutureDisposed.State <> FutureState.Unknown Then Throw New ObjectDisposedException(Me.GetType.Name)
        End Sub

        Protected Overrides Function PerformDispose(ByVal finalizing As Boolean) As IFuture
            If finalizing Then Return Nothing
            lockProducer.Dispose()
            lockConsumer.Dispose()
            lockJoined.Dispose()
            Return Nothing
        End Function
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

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(coroutineContinuer IsNot Nothing)
        End Sub

        Public Sub New(ByVal coroutineFunction As Action(Of ICoroutineController(Of TReturn)))
            Contract.Requires(coroutineFunction IsNot Nothing)
            Me.coroutineContinuer = New Coroutine(Sub(yielder)
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
