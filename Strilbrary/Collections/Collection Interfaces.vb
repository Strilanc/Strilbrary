Namespace Collections
    '''<summary>An IEnumerable with a specified size.</summary>
    <ContractClass(GetType(ISizedEnumerableContractClass(Of )))>
    Public Interface ISizedEnumerable(Of Out T)
        Inherits IEnumerable(Of T)
        ReadOnly Property Count As Integer
    End Interface

    '''<summary>An IEnumerable with a specified method to get items at given offsets.</summary>
    <ContractClass(GetType(IIndexedEnumerableContractClass(Of )))>
    Public Interface IIndexedEnumerable(Of Out T)
        Inherits ISizedEnumerable(Of T)
        Default ReadOnly Property Item(ByVal index As Integer) As T
    End Interface

    '''<summary>A readable collection of items.</summary>
    <ContractClass(GetType(IReadableCollectionContractClass(Of )))>
    Public Interface IReadableCollection(Of T)
        Inherits ISizedEnumerable(Of T)
        Function Contains(ByVal item As T) As Boolean
    End Interface

    '''<summary>A readable list of items.</summary>
    <ContractClass(GetType(IReadableListContractClass(Of )))>
    Public Interface IReadableList(Of T)
        Inherits IReadableCollection(Of T)
        Inherits IIndexedEnumerable(Of T)
        Function IndexOf(ByVal item As T) As Integer
    End Interface

    <ContractClassFor(GetType(ISizedEnumerable(Of )))>
    Public MustInherit Class ISizedEnumerableContractClass(Of T)
        Implements ISizedEnumerable(Of T)
        Public ReadOnly Property Count As Integer Implements ISizedEnumerable(Of T).Count
            Get
                Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                Throw New NotSupportedException
            End Get
        End Property
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Throw New NotSupportedException
        End Function
        Public Function GetEnumeratorObj() As System.Collections.IEnumerator Implements IEnumerable.GetEnumerator
            Throw New NotSupportedException
        End Function
    End Class
    <ContractClassFor(GetType(IIndexedEnumerable(Of )))>
    Public MustInherit Class IIndexedEnumerableContractClass(Of T)
        Inherits ISizedEnumerableContractClass(Of T)
        Implements IIndexedEnumerable(Of T)
        Default Public ReadOnly Property Item(ByVal index As Integer) As T Implements IIndexedEnumerable(Of T).Item
            Get
                Contract.Requires(index >= 0)
                Contract.Requires(index < DirectCast(Me, IIndexedEnumerable(Of T)).Count)
                Throw New NotSupportedException
            End Get
        End Property
    End Class
    <ContractClassFor(GetType(IReadableCollection(Of )))>
    Public MustInherit Class IReadableCollectionContractClass(Of T)
        Inherits ISizedEnumerableContractClass(Of T)
        Implements IReadableCollection(Of T)
        Public Function Contains(ByVal item As T) As Boolean Implements IReadableCollection(Of T).Contains
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse DirectCast(Me, IReadableCollection(Of T)).Count > 0)
            Throw New NotSupportedException
        End Function
    End Class
    <ContractClassFor(GetType(IReadableList(Of )))>
    Public MustInherit Class IReadableListContractClass(Of T)
        Inherits IIndexedEnumerableContractClass(Of T)
        Implements IReadableList(Of T)
        Public Function IndexOf(ByVal item As T) As Integer Implements IReadableList(Of T).IndexOf
            Contract.Ensures(Contract.Result(Of Integer)() < DirectCast(Me, IReadableList(Of T)).Count)
            Contract.Ensures(Contract.Result(Of Integer)() >= -1)
            Throw New NotSupportedException
        End Function
        Public Function Contains(ByVal item As T) As Boolean Implements IReadableCollection(Of T).Contains
            Throw New NotSupportedException
        End Function
    End Class
End Namespace
