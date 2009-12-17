Imports Strilbrary.Enumeration

Namespace Collections
    Public NotInheritable Class ListView(Of T)
        Implements IEnumerable(Of T)
        Implements IReadableList(Of T)

        Private ReadOnly _items As IReadableList(Of T)
        Private ReadOnly _offset As Integer
        Private ReadOnly _length As Integer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_items IsNot Nothing)
            Contract.Invariant(_offset >= 0)
            Contract.Invariant(_length >= 0)
            Contract.Invariant(_offset + _length <= _items.Count)
        End Sub

        Public Sub New(ByVal items As IReadableList(Of T))
            Me.New(items, 0, items.Count)
            Contract.Requires(items IsNot Nothing)
            Contract.Ensures(Me.Count = items.Count)
        End Sub
        Private Sub New(ByVal items As IReadableList(Of T),
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

        Default Public ReadOnly Property Item(ByVal index As Integer) As T Implements IReadableList(Of T).Item
            Get
                Return _items(index + _offset)
            End Get
        End Property
        Public ReadOnly Property Count As Integer Implements IReadableCollection(Of T).Count
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

        <Pure()>
        Public Function Contains(ByVal item As T) As Boolean Implements IReadableCollection(Of T).Contains
            Return IndexOf(item) <> -1
        End Function
        <Pure()>
        Public Function IndexOf(ByVal item As T) As Integer Implements IReadableList(Of T).IndexOf
            For i = 0 To Me.Count - 1
                If item Is Nothing Then
                    If Me.Item(i) Is Nothing Then Return i
                Else
                    If item.Equals(Me.Item(i)) Then Return i
                End If
            Next i
            Return -1
        End Function
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Dim nextIndex = 0
            Return New Enumerator(Of T)(Function(controller)
                                            If nextIndex >= Me.Count Then Return controller.Break()
                                            Dim e = Item(nextIndex)
                                            nextIndex += 1
                                            Return e
                                        End Function)
        End Function
        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace
