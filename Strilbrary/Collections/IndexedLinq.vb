Imports Strilbrary.Values

Namespace Collections
    '''<summary>Augments an IEnumerable with a count.</summary>
    Friend Class SizedEnumerable(Of T)
        Implements ISizedEnumerable(Of T)

        Private ReadOnly _count As Int32
        Private ReadOnly _iterator As IEnumerable(Of T)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_count >= 0)
            Contract.Invariant(_iterator IsNot Nothing)
        End Sub

        Public Sub New(ByVal iterator As IEnumerable(Of T),
                       ByVal count As Int32)
            Contract.Requires(iterator IsNot Nothing)
            Contract.Requires(count >= 0)
            Me._iterator = iterator
            Me._count = count
        End Sub

        Public ReadOnly Property Count As Integer Implements ICounted.Count
            Get
                Return _count
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Dim iterCount = 0
            For Each item In _iterator
                iterCount += 1
                If iterCount > _count Then Exit For
                Yield item
            Next item
            If iterCount <> _count Then
                Throw New Exceptions.InvalidStateException("Number of items in sequence doesn't match count.")
            End If
        End Function
        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class

    '''<summary>Implements a readable list based on given delegates.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public Class Rist(Of T)
        Implements IRist(Of T)

        Private ReadOnly _counter As Func(Of Int32)
        Private ReadOnly _getter As Func(Of Int32, T)
        Private ReadOnly _iterator As IEnumerable(Of T)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_counter IsNot Nothing)
            Contract.Invariant(_getter IsNot Nothing)
            Contract.Invariant(_iterator IsNot Nothing)
        End Sub

        Public Sub New(ByVal getter As Func(Of Int32, T),
                       ByVal counter As Func(Of Int32),
                       Optional ByVal efficientIterator As IEnumerable(Of T) = Nothing)
            Contract.Requires(getter IsNot Nothing)
            Contract.Requires(counter IsNot Nothing)
            Me._getter = getter
            Me._counter = counter
            Me._iterator = If(efficientIterator, Iterator Function()
                                                     For i = 0 To _counter() - 1
                                                         Yield _getter(i)
                                                     Next i
                                                 End Function())
            Contract.Assume(_iterator IsNot Nothing)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICounted.Count
            Get
                Dim result = _counter()
                If result < 0 Then Throw New Exceptions.InvalidStateException("Invalid count.")
                Return result
            End Get
        End Property
        Default Public ReadOnly Property Item(ByVal index As Integer) As T Implements IRist(Of T).Item
            Get
                Return _getter(index)
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return _iterator.GetEnumerator()
        End Function

        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Overrides Function ToString() As String
            Const MaxItems As Integer = 10
            Return "Count: {0}, Items: [{1}{2}".Frmt(Me.Count,
                                                     Me.Take(MaxItems).StringJoin(", "),
                                                     If(Me.Count <= MaxItems, "]", ", ..."))
        End Function
    End Class

    Public Module IndexedLinqExtensions
        '''<summary>Exposes a list as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function AsRist(Of T)(ByVal list As IList(Of T)) As IRist(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = list.Count)
            Return New Rist(Of T)(
                getter:=Function(i) list(i),
                counter:=Function() list.Count,
                efficientIterator:=list)
        End Function

        '''<summary>Creates a copy of the given sequence and exposes it as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function ToRist(Of T)(ByVal sequence As IEnumerable(Of T)) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Return If(TryCast(sequence, IRist(Of T)), sequence.ToArray().AsRist())
        End Function
    End Module
End Namespace
