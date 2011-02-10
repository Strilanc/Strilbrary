Imports Strilbrary.Values

Namespace Collections
    ''' <summary>
    ''' Exposes a contiguous subset of a readable list as a readable list.
    ''' </summary>
    <DebuggerDisplay("{ToString()}")>
    Friend NotInheritable Class ListView(Of T)
        Implements IRist(Of T)

        Private ReadOnly _items As IRist(Of T)
        Private ReadOnly _offset As Integer
        Private ReadOnly _length As Integer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_items IsNot Nothing)
            Contract.Invariant(_offset >= 0)
            Contract.Invariant(_length >= 0)
            Contract.Invariant(_offset + _length <= _items.Count)
        End Sub

        Public Sub New(ByVal items As IRist(Of T),
                       ByVal offset As Integer,
                       ByVal length As Integer)
            Contract.Requires(items IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(offset + length <= items.Count)
            Contract.Ensures(Me.Count = length)

            Me._items = items
            Me._offset = offset
            Me._length = length
        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As T Implements IRist(Of T).Item
            Get
                Return _items(_offset + index)
            End Get
        End Property
        Public ReadOnly Property Count As Integer Implements IRist(Of T).Count
            Get
                Contract.Ensures(Contract.Result(Of Integer)() = _length)
                Return _length
            End Get
        End Property

        <Pure()>
        Public Function SubView(ByVal relativeOffset As Integer) As ListView(Of T)
            Contract.Requires(relativeOffset >= 0)
            Contract.Requires(relativeOffset <= Count)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = Me.Count - relativeOffset)
            Return SubView(relativeOffset, Count - relativeOffset)
        End Function
        <Pure()>
        Public Function SubView(ByVal relativeOffset As Integer, ByVal relativeLength As Integer) As ListView(Of T)
            Contract.Requires(relativeOffset >= 0)
            Contract.Requires(relativeLength >= 0)
            Contract.Requires(relativeOffset + relativeLength <= Me.Count)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = relativeLength)
            Return New ListView(Of T)(_items, Me._offset + relativeOffset, relativeLength)
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For i = _offset To _offset + _length - 1
                Yield _items(i)
            Next i
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
End Namespace
