Namespace Enumeration
    ''' <summary>
    ''' Reverses the direction of an enumerator, allowing you to push values instead of pulling them.
    ''' </summary>
    Public NotInheritable Class PushEnumerator(Of T)
        Inherits NotifyingDisposable
        Private finished As Boolean
        Private ReadOnly sequenceQueue As New Queue(Of IEnumerator(Of T))
        Private ReadOnly coroutine As Coroutine

        <ContractInvariantMethod()> Protected overrides Sub Invariant()
            Contract.Invariant(coroutine IsNot Nothing)
            Contract.Invariant(sequenceQueue IsNot Nothing)
        End Sub

        Public Sub New(ByVal consumer As Action(Of IEnumerator(Of T)))
            Contract.Assume(consumer IsNot Nothing)
            'Contract.Requires(consumer IsNot Nothing) 'commented because events screw with Contracts and NotifyingDisposable has an event

            Dim consumer_ = consumer
            Me.coroutine = New Coroutine(
                Sub(coroutineController)
                    Contract.Requires(coroutineController IsNot Nothing)
                    Contract.Assume(consumer_ IsNot Nothing)

                    'Construct the blocking sequence
                    Dim curSubsequence As IEnumerator(Of T) = Nothing
                    Dim sequence = New Enumerator(Of T)(
                        Function(enumController)
                            Contract.Requires(enumController IsNot Nothing)
                            Contract.Assume(curSubsequence IsNot Nothing)
                            Contract.Assume(coroutineController IsNot Nothing)
                            Contract.Assume(sequenceQueue IsNot Nothing)

                            'Move to next element, and when current sequence runs out grab another one
                            While curSubsequence Is Nothing OrElse Not curSubsequence.MoveNext
                                If sequenceQueue.Count <= 0 Then
                                    'Wait for more elements
                                    Call coroutineController.Yield()

                                    'Break if there are no more elements to return
                                    If finished Then  Return enumController.Break
                                End If

                                'Grab next sequence of elements to return
                                curSubsequence = sequenceQueue.Dequeue()
                            End While

                            Return curSubsequence.Current
                        End Function
                    )

                    'Consume the sequence
                    Call consumer_(sequence)
                    'Dump any more pushed values
                    While sequence.MoveNext
                    End While
                End Sub
            )
        End Sub

        ''' <summary>Adds more elements for the consumer, and blocks until they have been consumed.</summary>
        Public Sub Push(ByVal sequence As IEnumerator(Of T))
            Contract.Requires(sequence IsNot Nothing)
            If finished Then Throw New InvalidOperationException("Can't push after PushDone.")
            sequenceQueue.Enqueue(sequence)
            coroutine.Continue()
        End Sub
        ''' <summary>Notifies the consumer that there are no elements, and blocks until the consumer finishes.</summary>
        Public Sub PushDone()
            If finished Then Throw New InvalidOperationException("Can't push after PushDone.")
            finished = True
            coroutine.Continue()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                coroutine.Dispose()
            End If
        End Sub
    End Class
End Namespace
