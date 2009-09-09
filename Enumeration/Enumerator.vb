Namespace Enumeration
    ''' <summary>
    ''' Allows an IEnumerator generator function to give special commands.
    ''' All methods should be used in a return statement, eg. "Return controller.break()".
    ''' </summary>
    <ContractClass(GetType(ContractClassForIEnumeratorController(Of )))>
    Public Interface IEnumeratorController(Of T)
        '''<summary>No value is enumerated, but the generator is called again. Similar to a loop continue.</summary>
        Function Repeat() As T
        '''<summary>No value is enumerated, and the sequence ends. Similar to a loop break.</summary>
        Function Break() As T
        '''<summary>Multiple values are enumerated before the generator is called again.</summary>
        Function Sequence(ByVal enumerator As IEnumerator(Of T)) As T
    End Interface

    <ContractClassFor(GetType(IEnumeratorController(Of )))>
    Public NotInheritable Class ContractClassForIEnumeratorController(Of T)
        Implements IEnumeratorController(Of T)
        Public Function Break() As T Implements IEnumeratorController(Of T).Break
        End Function
        Public Function Repeat() As T Implements IEnumeratorController(Of T).Repeat
        End Function
        Public Function Sequence(ByVal enumerator As System.Collections.Generic.IEnumerator(Of T)) As T Implements IEnumeratorController(Of T).Sequence
            Contract.Requires(enumerator IsNot Nothing)
        End Function
    End Class

    Public Module ExtensionsForIEnumeratorController
        '''<summary>Multiple values are enumerated before the generator is called again.</summary>
        <Extension()>
        Public Function Sequence(Of T)(ByVal controller As IEnumeratorController(Of T),
                                       ByVal enumerable As IEnumerable(Of T)) As T
            Contract.Requires(controller IsNot Nothing)
            Contract.Requires(enumerable IsNot Nothing)
            Return controller.Sequence(enumerable.GetEnumerator())
        End Function
    End Module

    ''' <summary>
    ''' Uses a lambda expression to enumerate elements.
    ''' </summary>
    Public NotInheritable Class Enumerator(Of T)
        Implements IEnumerator(Of T)
        Implements IEnumeratorController(Of T)

        Private ReadOnly generator As Func(Of IEnumeratorController(Of T), T)
        Private cur As T
        Private break As Boolean
        Private repeat As Boolean
        Private curSequence As IEnumerator(Of T)
        Public Sub New(ByVal generator As Func(Of IEnumeratorController(Of T), T))
            Contract.Requires(generator IsNot Nothing)
            Me.generator = generator
        End Sub

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(generator IsNot Nothing)
        End Sub

        Public Function MoveNext() As Boolean Implements IEnumerator(Of T).MoveNext
            Dim keepGoing = True
            Do
                If Me.break Then Return False

                If curSequence IsNot Nothing Then
                    If curSequence.MoveNext Then
                        Me.cur = curSequence.Current
                        Return True
                    Else
                        keepGoing = True
                        curSequence = Nothing
                    End If
                End If

                If Not keepGoing Then Exit Do
                Me.cur = generator(Me)

                keepGoing = Me.repeat
                Me.repeat = False
            Loop

            Return True
        End Function

        Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
            Get
                Return Me.cur
            End Get
        End Property

        Private ReadOnly Property CurrentObj As Object Implements System.Collections.IEnumerator.Current
            Get
                Return Me.cur
            End Get
        End Property
        Private Sub Reset() Implements System.Collections.IEnumerator.Reset
            Throw New NotSupportedException()
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            GC.SuppressFinalize(Me)
        End Sub

        Private Function ControllerRepeat() As T Implements IEnumeratorController(Of T).Repeat
            Me.repeat = True
            Return Nothing
        End Function
        Private Function ControllerBreak() As T Implements IEnumeratorController(Of T).Break
            Me.break = True
            Return Nothing
        End Function
        Private Function ControllerSequence(ByVal enumerator As IEnumerator(Of T)) As T Implements IEnumeratorController(Of T).Sequence
            Me.curSequence = enumerator
            Return Nothing
        End Function
    End Class
End Namespace
