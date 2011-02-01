Namespace Collections
    '''<summary>An object with a specified count.</summary>
    <ContractClass(GetType(ICountedContractClass))>
    Public Interface ICounted
        ReadOnly Property Count As Integer
    End Interface

    '''<summary>An IEnumerable with a known finite count.</summary>
    Public Interface ISizedEnumerable(Of Out T)
        Inherits IEnumerable(Of T)
        Inherits ICounted
    End Interface

    '''<summary>An IEnumerable with a specified method to get items at given offsets.</summary>
    <ContractClass(GetType(IRistContractClass(Of )))>
    Public Interface IRist(Of Out T)
        Inherits ISizedEnumerable(Of T)
        Default ReadOnly Property Item(ByVal index As Integer) As T
    End Interface

    <ContractClassFor(GetType(ICounted))>
    Public MustInherit Class ICountedContractClass
        Implements ICounted
        Public ReadOnly Property Count As Integer Implements ICounted.Count
            Get
                Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                Throw New NotSupportedException
            End Get
        End Property
    End Class
    <ContractClassFor(GetType(IRist(Of )))>
    Public MustInherit Class IRistContractClass(Of T)
        Implements IRist(Of T)
        Default Public ReadOnly Property Item(ByVal index As Integer) As T Implements IRist(Of T).Item
            Get
                Contract.Requires(index >= 0)
                Contract.Requires(index < DirectCast(Me, IRist(Of T)).Count)
                Throw New NotSupportedException
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ISizedEnumerable(Of T).Count
            Get
                Throw New NotSupportedException
            End Get
        End Property
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Throw New NotSupportedException
        End Function
        Public Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Throw New NotSupportedException
        End Function
    End Class
End Namespace
