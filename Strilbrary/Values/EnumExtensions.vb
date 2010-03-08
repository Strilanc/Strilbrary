Namespace Values
    Public Module EnumExtensions
        <Pure()> <Extension()>
        <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId:="Flags")>
        Public Function EnumFlagsToString(Of T)(ByVal enumValue As T) As String
            Dim v = CULng(CType(enumValue, Object))
            If v = 0 Then Return enumValue.ToString

            Dim words As New List(Of String)
            For i = 0 To 64 - 1
                Dim u = 1UL << i
                If u > v Then Exit For
                If (u And v) = 0 Then Continue For
                Dim e = CType(CType(u, Object), T)
                words.Add(If(e.EnumValueIsDefined(), e.ToString, "1<<{0}".Frmt(i)))
            Next i

            Return String.Join(", ", words.ToArray)
        End Function

        <Pure()>
        Public Function EnumValues(Of T)() As IEnumerable(Of T)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return CType([Enum].GetValues(GetType(T)), IEnumerable(Of T))
        End Function

        <Pure()> <Extension()>
        Public Function EnumParse(Of T)(ByVal value As String, ByVal ignoreCase As Boolean) As T
            Dim ret As T
            If Not EnumTryParse(Of T)(value, ignoreCase, ret) Then
                Throw New ArgumentException("""{0}"" can't be parsed to an enum of type {1}.".Frmt(value, GetType(T).ToString))
            End If
            Return ret
        End Function

        <Extension()>
        Public Function EnumTryParse(Of T)(ByVal value As String, ByVal ignoreCase As Boolean, ByRef result As T) As Boolean
            For Each e In EnumValues(Of T)()
                If String.Compare(value, e.ToString(), If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal)) = 0 Then
                    result = e
                    Return True
                End If
            Next e
            Return False
        End Function

        <Pure()> <Extension()>
        Public Function EnumValueIsDefined(Of T)(ByVal value As T) As Boolean
            Return [Enum].IsDefined(GetType(T), value)
        End Function
    End Module
End Namespace
