Imports Strilbrary.Values

Namespace Collections
    ''' <summary>
    ''' A queue which returns higher-priority items first.
    ''' </summary>
    Public Class PriorityQueue(Of TValue)
        Private ReadOnly _items As List(Of TValue)
        Private ReadOnly _comparer As Func(Of TValue, TValue, Integer)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_items IsNot Nothing)
            Contract.Invariant(_comparer IsNot Nothing)
        End Sub

        Public Sub New(ByVal comparer As Func(Of TValue, TValue, Integer))
            Contract.Requires(comparer IsNot Nothing)
            Me._comparer = comparer
            Me._items = New List(Of TValue)()
        End Sub
        Public Sub New(ByVal comparer As Func(Of TValue, TValue, Integer),
                       ByVal capacity As Integer)
            Contract.Requires(comparer IsNot Nothing)
            Contract.Requires(capacity >= 0)
            Me._comparer = comparer
            Me._items = New List(Of TValue)(capacity:=capacity)
        End Sub

        '''<summary>Adds an item to the priority queue.</summary>
        Public Sub Enqueue(ByVal item As TValue)
            Dim curIndex = _items.Count
            _items.Add(item)
            While curIndex > 0
                Dim parentIndex = (curIndex - 1) \ 2
                Dim parentItem = _items(parentIndex)
                If _comparer(item, parentItem) <= 0 Then Exit While

                Contract.Assume(curIndex < _items.Count)
                _items(curIndex) = parentItem
                _items(parentIndex) = item
                curIndex = parentIndex
            End While
        End Sub

        '''<summary>Retrieves the highest-priority item in the priority queue.</summary>
        <Pure()>
        Public Function Peek() As TValue
            Contract.Requires(Count > 0)
            Return _items(0)
        End Function

        '''<summary>Retrieves the highest-priority item in the priority queue, and removes it from the queue.</summary>
        Public Function Dequeue() As TValue
            Contract.Requires(Count > 0)
            Dim result = _items(0)

            Dim curIndex = 0
            Do
                Dim childIndex1 = curIndex * 2 + 1
                Dim childIndex2 = curIndex * 2 + 2
                If childIndex2 >= Count Then Exit Do

                Dim childIndex = If(_comparer(_items(childIndex1), _items(childIndex2)) > 0, childIndex1, childIndex2)
                _items(curIndex) = _items(childIndex)
                curIndex = childIndex
            Loop

            Contract.Assert(curIndex < Count)
            _items(curIndex) = _items(Count - 1)
            _items.RemoveAt(Count - 1)

            Return result
        End Function

        '''<summary>Determines the number of items in the priority queue.</summary>
        Public ReadOnly Property Count As Integer
            Get
                Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                Contract.Ensures(Contract.Result(Of Integer)() = _items.Count)
                Return _items.Count
            End Get
        End Property
    End Class
End Namespace
