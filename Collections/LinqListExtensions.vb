Imports Strilbrary.Enumeration

Namespace Collections
    Public Module LinqListExtensions
        '''<summary>Creates a list containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function ToList(Of T)(ByVal list As IList(Of T)) As List(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of List(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of List(Of T)).Count = list.Count)
            Dim newList As New List(Of T)(list.Count)
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

        '''<summary>Creates an array containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function SubToArray(Of T)(ByVal list As IList(Of T), ByVal offset As Integer) As T()
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(offset <= list.Count)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T()).Length = list.Count - offset)
            Return list.SubToArray(offset, list.Count - offset)
        End Function

        '''<summary>Creates an array containing a contiguous subset of the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function SubToArray(Of T)(ByVal list As IList(Of T), ByVal offset As Integer, ByVal count As Integer) As T()
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(count >= 0)
            Contract.Requires(offset <= list.Count - count)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T()).Length = count)
            Dim ret(0 To count - 1) As T
            For i = 0 To count - 1
                ret(i) = list(i + offset)
            Next i
            Return ret
        End Function

        '''<summary>Creates an IList with the same elements as the given IList, but in reversed order.</summary>
        <Extension()> <Pure()>
        Public Function Reverse(Of T)(ByVal list As IList(Of T)) As IList(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))().Count = list.Count)
            Dim n = list.Count
            Dim ret = New List(Of T)(n)
            For i = 0 To n - 1
                ret.Add(list(n - i - 1))
            Next i
            Contract.Assume(ret.Count = list.Count)
            Return ret
        End Function

        '''<summary>Determines if the items in two lists are equivalent and in the same order.</summary>
        <Pure()> <Extension()>
        Public Function HasSameItemsAs(Of T As IEquatable(Of T))(ByVal this As IList(Of T), ByVal list As IList(Of T)) As Boolean
            Contract.Requires(this IsNot Nothing)
            Contract.Requires(list IsNot Nothing)

            If this.Count <> list.Count Then Return False
            For i = 0 To this.Count - 1
                'Compare element
                Dim e1 = this(i), e2 = list(i)
                If e1 Is Nothing Then
                    If e2 IsNot Nothing Then Return False
                Else
                    If Not e1.Equals(e2) Then Return False
                End If
            Next i

            Return True
        End Function

        '''<summary>Creates a read-only view of a list.</summary>
        <Extension()> <Pure()>
        Public Function ToView(Of T)(ByVal list As IReadableList(Of T)) As ListView(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = list.Count)
            Dim result = TryCast(list, ListView(Of T))
            If result Is Nothing Then
                result = New ListView(Of T)(list)
            Else
                Contract.Assume(result.Count = list.Count)
            End If
            Return result
        End Function
        '''<summary>Determines a read-only view of a contiguous subset of the list starting at the given offset.</summary>
        <Extension()> <Pure()>
        Public Function SubView(Of T)(ByVal list As IReadableList(Of T), ByVal offset As Integer) As ListView(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(offset <= list.Count)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = list.Count - offset)
            Return list.ToView.SubView(offset)
        End Function
        '''<summary>Determines a read-only view of a contiguous subset of the list starting at the given offset and running for the given length.</summary>
        <Extension()> <Pure()>
        Public Function SubView(Of T)(ByVal list As IReadableList(Of T), ByVal offset As Integer, ByVal length As Integer) As ListView(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(offset + length <= list.Count)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = length)
            Return list.ToView.SubView(offset, length)
        End Function
    End Module
End Namespace
