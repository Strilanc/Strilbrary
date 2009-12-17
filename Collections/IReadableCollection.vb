Namespace Collections
    <ContractClass(GetType(IReadableCollection(Of ).ContractClass))>
    Public Interface IReadableCollection(Of T)
        Inherits IEnumerable(Of T)

        Function Contains(ByVal item As T) As Boolean
        ReadOnly Property Count As Integer

        <ContractClassFor(GetType(IReadableCollection(Of )))>
        NotInheritable Class ContractClass
            Implements IReadableCollection(Of T)
            Public ReadOnly Property Count As Integer Implements IReadableCollection(Of T).Count
                Get
                    Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                    Throw New NotSupportedException
                End Get
            End Property
            Public Function Contains(ByVal item As T) As Boolean Implements IReadableCollection(Of T).Contains
                Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.Count > 0)
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
