Namespace Collections
    <ContractClass(GetType(IReadableList(Of ).ContractClass))>
    Public Interface IReadableList(Of T)
        Inherits IReadableCollection(Of T)

        Function IndexOf(ByVal item As T) As Integer
        Default ReadOnly Property Item(ByVal index As Integer) As T

        <ContractClassFor(GetType(IReadableList(Of )))>
        NotInheritable Shadows Class ContractClass
            Implements IReadableList(Of T)
            Public Function IndexOf(ByVal item As T) As Integer Implements IReadableList(Of T).IndexOf
                Contract.Ensures(Contract.Result(Of Integer)() < Me.Count)
                Contract.Ensures(Contract.Result(Of Integer)() >= -1)
                Throw New NotSupportedException
            End Function
            Public ReadOnly Property Item(ByVal index As Integer) As T Implements IReadableList(Of T).Item
                Get
                    Contract.Requires(index >= 0)
                    Contract.Requires(index < Me.Count)
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
    End Interface
End Namespace
