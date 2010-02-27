Namespace Collections
    Public Module LinqListExtensions
        '''<summary>Creates a list containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function ToList(Of T)(ByVal list As IList(Of T)) As List(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of List(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of List(Of T)).Count = list.Count)
            Dim newList As New List(Of T)(capacity:=list.Count)
            For i = 0 To list.Count - 1
                newList.Add(list(i))
            Next i
            Contract.Assume(newList.Count = list.Count)
            Return newList
        End Function

        '''<summary>Creates an array containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function ToArray(Of T)(ByVal list As IList(Of T)) As T()
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T()).Length = list.Count)
            Dim newArray(0 To list.Count - 1) As T
            For i = 0 To list.Count - 1
                newArray(i) = list(i)
            Next i
            Return newArray
        End Function

        '''<summary>Creates an IList with the same elements as the given IList, but in reversed order.</summary>
        <Extension()> <Pure()>
        Public Function Reverse(Of T)(ByVal list As IList(Of T)) As IList(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))().Count = list.Count)
            Dim n = list.Count
            Dim ret = New List(Of T)(capacity:=n)
            For i = 0 To n - 1
                ret.Add(list(n - i - 1))
            Next i
            Contract.Assume(ret.Count = list.Count)
            Return ret
        End Function

        '''<summary>Wraps a readable list in a list view.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Private Function ToView(Of T)(ByVal list As IReadableList(Of T)) As ListView(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = list.Count)
            Return If(TryCast(list, ListView(Of T)), New ListView(Of T)(list))
        End Function
        '''<summary>Returns a list containing a contiguous subset of the given list starting at the given offset.</summary>
        <Extension()> <Pure()>
        Public Function SubView(Of T)(ByVal list As IReadableList(Of T), ByVal offset As Integer) As IReadableList(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(offset <= list.Count)
            Contract.Ensures(Contract.Result(Of IReadableList(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IReadableList(Of T))().Count = list.Count - offset)
            Return list.ToView.SubView(offset)
        End Function
        '''<summary>Returns a list containing a contiguous subset of the given list starting at the given offset and running for the given length.</summary>
        <Extension()> <Pure()>
        Public Function SubView(Of T)(ByVal list As IReadableList(Of T), ByVal offset As Integer, ByVal length As Integer) As IReadableList(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(offset + length <= list.Count)
            Contract.Ensures(Contract.Result(Of IReadableList(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IReadableList(Of T))().Count = length)
            Return list.ToView.SubView(offset, length)
        End Function
    End Module
End Namespace
