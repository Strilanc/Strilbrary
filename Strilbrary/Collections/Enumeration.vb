Imports Strilbrary.Exceptions

Namespace Collections
    ''' <summary>
    ''' Allows an IEnumerator generator function to give special commands.
    ''' All methods should be used in a return statement, eg. "return controller.Break()".
    ''' </summary>
    <ContractClass(GetType(IEnumeratorControllerContractClass(Of )))>
    Public Interface IEnumeratorController(Of T)
        '''<summary>No value is enumerated, but the generator is called again. Similar to a loop continue.</summary>
        Function Repeat() As T
        '''<summary>No value is enumerated, and the sequence ends. Similar to a loop break.</summary>
        Function Break() As T
        '''<summary>Multiple values are enumerated before the generator is called again.</summary>
        Function Sequence(ByVal enumerator As IEnumerator(Of T)) As T
    End Interface

    <ContractClassFor(GetType(IEnumeratorController(Of )))>
    Public MustInherit Class IEnumeratorControllerContractClass(Of T)
        Implements IEnumeratorController(Of T)
        Public Function Break() As T Implements IEnumeratorController(Of T).Break
            Throw New NotSupportedException
        End Function
        Public Function Repeat() As T Implements IEnumeratorController(Of T).Repeat
            Throw New NotSupportedException
        End Function
        Public Function Sequence(ByVal enumerator As IEnumerator(Of T)) As T Implements IEnumeratorController(Of T).Sequence
            Contract.Requires(enumerator IsNot Nothing)
            Throw New NotSupportedException
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
        Private ReadOnly disposer As action
        Private cur As T
        Private break As Boolean
        Private repeat As Boolean
        Private curSequence As IEnumerator(Of T)
        Public Sub New(ByVal generator As Func(Of IEnumeratorController(Of T), T),
                       Optional ByVal disposer As action = Nothing)
            Contract.Requires(generator IsNot Nothing)
            Me.generator = generator
            Me.disposer = disposer
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
            If disposer IsNot Nothing Then Call disposer()
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

    ''' <summary>
    ''' Uses a lambda expression to create enumerators.
    ''' </summary>
    Public NotInheritable Class Enumerable(Of T)
        Implements IEnumerable(Of T)
        Private ReadOnly generator As Func(Of IEnumerator(Of T))

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(generator IsNot Nothing)
        End Sub

        Public Sub New(ByVal generator As Func(Of IEnumerator(Of T)))
            Contract.Requires(generator IsNot Nothing)
            Me.generator = generator
        End Sub
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Dim x = generator()
            If x Is Nothing Then Throw New InvalidStateException("The generator function returned a null value.")
            Return x
        End Function
        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace
