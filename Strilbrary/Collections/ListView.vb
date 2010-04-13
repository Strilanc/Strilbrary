Imports Strilbrary.Values

Namespace Collections
    ''' <summary>
    ''' Exposes a contiguous subset of a readable list as a readable list.
    ''' </summary>
    <DebuggerDisplay("{ToString}")>
    Friend NotInheritable Class ListView(Of T)
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

        Public Sub New(ByVal items As IReadableList(Of T),
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
            'verification disabled due to stupid verifier (1.2.30312.0)
            <ContractVerification(False)>
            Get
                Return _items(_offset + index)
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
            Return New Enumerator(Me)
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

        Private Class Enumerator
            Implements IEnumerator(Of T)

            Private _index As Int32 = -1
            Private ReadOnly _list As IReadableList(Of T)

            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_list IsNot Nothing)
            End Sub

            Public Sub New(ByVal list As IReadableList(Of T))
                Contract.Requires(list IsNot Nothing)
                Me._list = list
            End Sub

            Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
                Get
                    If _index < 0 Then Throw New InvalidOperationException("Enumerator not started.")
                    If _index >= _list.Count Then Throw New InvalidOperationException("Enumerator finished.")
                    Return _list(_index)
                End Get
            End Property

            Private ReadOnly Property CurrentObj As Object Implements System.Collections.IEnumerator.Current
                Get
                    Return Current
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
                _index += 1
                Return _index < _list.Count
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset
                Throw New NotSupportedException
            End Sub

            Private Sub Dispose() Implements IDisposable.Dispose
                _index = _list.Count
            End Sub
        End Class
    End Class
End Namespace
