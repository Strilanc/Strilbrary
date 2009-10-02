Public NotInheritable Class ViewableList(Of T)
    Implements IEnumerable(Of T)
    Implements IList(Of T)

    Private ReadOnly items As IList(Of T)
    Private ReadOnly offset As Integer
    Private ReadOnly _length As Integer

    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(offset >= 0)
        Contract.Invariant(_length >= 0)
        Contract.Invariant(offset + _length <= items.Count)
    End Sub

    Public Sub New(ByVal items As IList(Of T))
        Me.New(items, 0, items.Count, 0, items.Count)
        Contract.Requires(items IsNot Nothing)
        Contract.Ensures(Me.Length = items.Count)
    End Sub

    Private Sub New(ByVal items As IList(Of T),
                    ByVal relOffset As Integer,
                    ByVal relLength As Integer,
                    ByVal baseOffset As Integer,
                    ByVal baseLength As Integer)
        Contract.Requires(items IsNot Nothing)
        Contract.Requires(baseOffset >= 0)
        Contract.Requires(baseLength >= 0)
        Contract.Requires(relOffset >= 0)
        Contract.Requires(relLength >= 0)
        Contract.Requires(relOffset + relLength <= baseLength)
        Contract.Requires(baseOffset + baseLength <= items.Count)
        Contract.Ensures(Me.Length = relLength)

        Me.items = items
        Me.offset = baseOffset + relOffset
        Me._length = relLength
    End Sub

    Default Public ReadOnly Property Item(ByVal index As Integer) As T
        Get
            Contract.Requires(index >= 0)
            Contract.Requires(index < Length)
            Return items(index + offset)
        End Get
    End Property

    Public ReadOnly Property Length As Integer
        Get
            Contract.Ensures(Contract.Result(Of Integer)() >= 0)
            Contract.Ensures(Contract.Result(Of Integer)() = Me._length)
            Return _length
        End Get
    End Property

    <Pure()>
    Public Function SubView(ByVal relativeOffset As Integer) As ViewableList(Of T)
        Contract.Requires(relativeOffset >= 0)
        Contract.Requires(relativeOffset <= Length)
        Contract.Ensures(Contract.Result(Of ViewableList(Of T))() IsNot Nothing)
        Return SubView(relativeOffset, Length - relativeOffset)
    End Function
    <Pure()>
    Public Function SubView(ByVal relativeOffset As Integer, ByVal relativeLength As Integer) As ViewableList(Of T)
        Contract.Requires(relativeOffset >= 0)
        Contract.Requires(relativeLength >= 0)
        Contract.Requires(relativeOffset + relativeLength <= Me.Length)
        Contract.Ensures(Contract.Result(Of ViewableList(Of T))() IsNot Nothing)
        Return New ViewableList(Of T)(items, relativeOffset, relativeLength, offset, Me.Length)
    End Function

    Private Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Dim nextIndex = 0
        Return New Enumerator(Of T)(Function(controller)
                                        Contract.Requires(controller IsNot Nothing)
                                        Contract.Assume(controller IsNot Nothing)
                                        Contract.Assume(Me IsNot Nothing)
                                        Contract.Assume(nextIndex >= 0)
                                        If nextIndex >= Me.Length Then  Return controller.Break()
                                        Dim e = Item(nextIndex)
                                        nextIndex += 1
                                        Return e
                                    End Function)
    End Function
    Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

#Region "IList<T>"
    Private Sub _Add(ByVal item As T) Implements System.Collections.Generic.ICollection(Of T).Add
        Throw New NotSupportedException
    End Sub
    Private Sub _Clear() Implements System.Collections.Generic.ICollection(Of T).Clear
        Throw New NotSupportedException
    End Sub
    Private Function _Contains(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
        For i = 0 To Length - 1
            If item.Equals(Me.Item(i)) Then Return True
        Next i
        Return False
    End Function
    Private Sub _CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
        Contract.Assume(array.Length - arrayIndex >= Length)
        For i = 0 To Length - 1
            array(i + arrayIndex) = Me.Item(i)
        Next i
    End Sub
    Private ReadOnly Property _Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count
        Get
            Contract.Ensures(Contract.Result(Of Integer)() = Length)
            Return Me.Length
        End Get
    End Property
    Private ReadOnly Property _IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
        Get
            Return True
        End Get
    End Property
    Private Function _Remove(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
        Throw New NotSupportedException
    End Function
    Private Function _IndexOf(ByVal item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
        For i = 0 To Length - 1
            If item.Equals(Me.Item(i)) Then
                'contract verifier 1.2.20902.10 claims this is redundant, but removing it causes:
                '   ensures unproven: Contract.Result<int>() < @this.Count
                Contract.Assume(i < _Count)
                Return i
            End If
        Next i
        Return -1
    End Function
    Private Sub _Insert(ByVal index As Integer, ByVal item As T) Implements System.Collections.Generic.IList(Of T).Insert
        Throw New NotSupportedException
    End Sub
    Private Property _Item(ByVal index As Integer) As T Implements System.Collections.Generic.IList(Of T).Item
        Get
            Contract.Assume(index < Length)
            Return Item(index)
        End Get
        Set(ByVal value As T)
            Throw New NotSupportedException
        End Set
    End Property
    Private Sub _RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of T).RemoveAt
        Throw New NotSupportedException
    End Sub
#End Region
End Class
