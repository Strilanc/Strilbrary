Namespace Collections
    <ContractClass(GetType(IReadableListContractClass(Of )))>
    Public Interface IReadableList(Of T)
        Inherits IReadableCollection(Of T)

        Function IndexOf(ByVal item As T) As Integer
        Default ReadOnly Property Item(ByVal index As Integer) As T
    End Interface

    '[This class causes a code analysis bug in code contracts 1.2.30113.1 if moved into its target interface.]
    <ContractClassFor(GetType(IReadableList(Of )))>
    Public NotInheritable Class IReadableListContractClass(Of T)
        Implements IReadableList(Of T)
        Public Function IndexOf(ByVal item As T) As Integer Implements IReadableList(Of T).IndexOf
            Contract.Ensures(Contract.Result(Of Integer)() < DirectCast(Me, IReadableList(Of T)).Count)
            Contract.Ensures(Contract.Result(Of Integer)() >= -1)
            Throw New NotSupportedException
        End Function
        Public ReadOnly Property Item(ByVal index As Integer) As T Implements IReadableList(Of T).Item
            Get
                Contract.Requires(index >= 0)
                Contract.Requires(index < DirectCast(Me, IReadableList(Of T)).Count)
                Throw New NotSupportedException
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements IReadableCollection(Of T).Count
            Get
                Throw New NotSupportedException
            End Get
        End Property
        Public Function Contains(ByVal item As T) As Boolean Implements IReadableCollection(Of T).Contains
            Throw New NotSupportedException
        End Function
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Throw New NotSupportedException
        End Function
        Public Function GetEnumeratorObj() As System.Collections.IEnumerator Implements IEnumerable.GetEnumerator
            Throw New NotSupportedException
        End Function
    End Class
End Namespace
