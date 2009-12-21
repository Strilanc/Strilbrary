Namespace Misc
    '''<summary>A smattering of functions and other stuff that hasn't been placed in more reasonable groups yet.</summary>
    Public Module PoorlyCategorizedFunctions
#Region "Arrays"
        <Extension()> <Pure()>
        Public Function SubArray(Of T)(ByVal array As T(), ByVal offset As Integer) As T()
            Contract.Requires(array IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            If offset > array.Length Then Throw New ArgumentOutOfRangeException("offset")

            Dim new_array(0 To array.Length - offset - 1) As T
            System.Array.Copy(array, offset, new_array, 0, new_array.Length)
            Return new_array
        End Function
        <Extension()> <Pure()>
        Public Function SubArray(Of T)(ByVal array As T(), ByVal offset As Integer, ByVal length As Integer) As T()
            Contract.Requires(array IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(offset <= array.Length - length)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)

            Dim newArray(0 To length - 1) As T
            System.Array.Copy(array, offset, newArray, 0, newArray.Length)
            Return newArray
        End Function
        <Pure()>
        Public Function Concat(Of T)(ByVal ParamArray arrays As T()()) As T()
            Contract.Requires(arrays IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Return arrays.AsEnumerable.Concat()
        End Function
        <Extension()> <Pure()>
        Public Function Concat(Of T)(ByVal arrays As IEnumerable(Of T())) As T()
            Contract.Requires(arrays IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            If (From array In arrays Where array Is Nothing).Any Then Throw New ArgumentNullException("arrays", "array is null")

            Dim totalLength = 0
            For Each array In arrays
                Contract.Assume(array IsNot Nothing)
                totalLength += array.Length
            Next array

            Dim flattenedArray(0 To totalLength - 1) As T
            Dim processingOffset = 0
            For Each array In arrays
                Contract.Assume(array IsNot Nothing)
                Contract.Assume(processingOffset + array.Length <= flattenedArray.Length)
                System.Array.Copy(array, 0, flattenedArray, processingOffset, array.Length)
                processingOffset += array.Length
            Next array

            Return flattenedArray
        End Function
#End Region

        <Pure()> <Extension()>
        Public Function Between(Of T As IComparable(Of T))(ByVal value1 As T,
                                                           ByVal value2 As T,
                                                           ByVal value3 As T) As T
            Contract.Requires(value1 IsNot Nothing)
            Contract.Requires(value2 IsNot Nothing)
            Contract.Requires(value3 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
            'recursive sort
            If value2.CompareTo(value1) > 0 Then Return Between(value2, value1, value3)
            If value2.CompareTo(value3) < 0 Then Return Between(value1, value3, value2)
            'median
            Return value2
        End Function

        Public Sub Swap(Of T)(ByRef value1 As T, ByRef value2 As T)
            Dim vt = value1
            value1 = value2
            value2 = vt
        End Sub

        <Extension()> <Pure()>
            Public Function Minutes(ByVal quantity As Integer) As TimeSpan
            Return New TimeSpan(0, quantity, 0)
        End Function
        <Extension()> <Pure()>
            Public Function Seconds(ByVal quantity As Integer) As TimeSpan
            Return New TimeSpan(0, 0, quantity)
        End Function
        <Extension()> <Pure()>
            Public Function Milliseconds(ByVal quantity As Integer) As TimeSpan
            Return New TimeSpan(0, 0, 0, 0, quantity)
        End Function
    End Module
End Namespace
