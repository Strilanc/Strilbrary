Imports Strilbrary.Values

Namespace Collections
    Public Module CompatibilityExtensions
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IList(Of T))().IsReadOnly")>
        Public Function AsList(Of T)(ByVal this As IRist(Of T)) As IList(Of T)
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))().Count = this.Count)
            Contract.Ensures(Contract.Result(Of IList(Of T))().IsReadOnly)
            Dim list = New ReadableListToListBridge(Of T)(this)
            'Contract.Assume(list.IsReadOnly) '[would fix the unproven postcondition, but causes a can-be-proven-warning!]
            Return list
        End Function
        <Extension()> <Pure()>
        Public Function AsReadableList(Of T)(ByVal this As IList(Of T)) As IRist(Of T)
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = this.Count)
            Return New ListToReadableListBridge(Of T)(this)
        End Function
        <Extension()> <Pure()>
        Public Function ToReadableList(Of T)(ByVal sequence As IEnumerable(Of T)) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Return If(TryCast(sequence, IRist(Of T)), sequence.ToArray.AsReadableList)
        End Function

        <DebuggerDisplay("{ToString()}")>
        Private Class ReadableListToListBridge(Of T)
            Implements IList(Of T)

            Private ReadOnly _subList As IRist(Of T)

            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_subList IsNot Nothing)
            End Sub

            Public Sub New(ByVal subList As IRist(Of T))
                Contract.Requires(subList IsNot Nothing)
                Contract.Ensures(Me.Count = subList.Count)
                Me._subList = subList
            End Sub

            Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
                For i = 0 To _subList.Count - 1
                    array(i + arrayIndex) = _subList(i)
                Next i
            End Sub
            Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
                Get
                    Contract.Ensures(Contract.Result(Of Boolean)())
                    Return True
                End Get
            End Property

            <SuppressMessage("Microsoft.Contracts", "Ensures-25-12")>
            Public Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
                Return _subList.Contains(item)
            End Function
            Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count
                Get
                    Contract.Ensures(Contract.Result(Of Integer)() = Me._subList.Count)
                    Return _subList.Count
                End Get
            End Property
            Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
                Return _subList.GetEnumerator()
            End Function
            Public Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
                Return _subList.GetEnumerator()
            End Function
            <SuppressMessage("Microsoft.Contracts", "Ensures-43-51")>
            Public Function IndexOf(ByVal item As T) As Integer Implements IList(Of T).IndexOf
                For i = 0 To _subList.Count - 1
                    If item.Equals(_subList.Item(i)) Then
                        Return i
                    End If
                Next i
                Return -1
            End Function
            Default Public Property Item(ByVal index As Integer) As T Implements IList(Of T).Item
                <SuppressMessage("Microsoft.Contracts", "Requires-21-7")>
                Get
                    Return _subList.Item(index)
                End Get
                Set(ByVal value As T)
                    Throw New NotSupportedException()
                End Set
            End Property

            Public Sub Insert(ByVal index As Integer, ByVal item As T) Implements IList(Of T).Insert
                Throw New NotSupportedException
            End Sub
            Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of T).RemoveAt
                Throw New NotSupportedException()
            End Sub
            Public Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
                Throw New NotSupportedException
            End Function
            Public Sub Add(ByVal item As T) Implements ICollection(Of T).Add
                Throw New NotSupportedException
            End Sub
            Public Sub Clear() Implements ICollection(Of T).Clear
                Throw New NotSupportedException
            End Sub
            Public Overrides Function ToString() As String
                Return _subList.ToString
            End Function
        End Class
        <DebuggerDisplay("{ToString()}")>
        Private Class ListToReadableListBridge(Of T)
            Implements IRist(Of T)

            Private ReadOnly _subList As IList(Of T)

            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_subList IsNot Nothing)
            End Sub

            Public Sub New(ByVal subList As IList(Of T))
                Contract.Requires(subList IsNot Nothing)
                Contract.Ensures(Me.Count = subList.Count)
                Me._subList = subList
            End Sub

            Public ReadOnly Property Count As Integer Implements IRist(Of T).Count
                Get
                    Contract.Ensures(Contract.Result(Of Integer)() = Me._subList.Count)
                    Return _subList.Count
                End Get
            End Property
            Public ReadOnly Property Item(ByVal index As Integer) As T Implements IRist(Of T).Item
                Get
                    Return _subList.Item(index)
                End Get
            End Property
            Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
                Return _subList.GetEnumerator
            End Function
            Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements IEnumerable.GetEnumerator
                Return _subList.GetEnumerator
            End Function

            Public Overrides Function ToString() As String
                Const MaxItems As Integer = 10
                Return "Count: {0}, Items: [{1}{2}".Frmt(Me.Count,
                                                         Me.Take(MaxItems).StringJoin(", "),
                                                         If(Me.Count <= MaxItems, "]", ", ..."))
            End Function
        End Class
    End Module
End Namespace
