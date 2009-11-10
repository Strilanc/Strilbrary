Imports System.Net.NetworkInformation
Imports System.Runtime.CompilerServices
Imports System.IO.Path
Imports System.Text
Imports System.Net
Imports System.IO

'''<summary>A smattering of functions and other stuff that hasn't been placed in more reasonable groups yet.</summary>
Public Module PoorlyCategorizedFunctions
#Region "Strings"
    ''' <summary>
    ''' Determines a string created by extending the given string up to the given minimum length using the given padding character.
    ''' </summary>
    <Pure()> <Extension()>
    Public Function Padded(ByVal text As String,
                           ByVal minimumLength As Integer,
                           Optional ByVal paddingCharacter As Char = " "c) As String
        Contract.Requires(text IsNot Nothing)
        Contract.Requires(minimumLength >= 0)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        'Contract.Ensures(Contract.Result(Of String)().Length = Math.Max(minimumLength, text.Length))
        'Contract.Ensures(Contract.Result(Of String)().Substring(0, text.Length) = text)
        'Contract.Ensures(Contract.Result(Of String)().Substring(text.Length) = New String(paddingCharacter, Math.Max(0, minimumLength - text.Length)))
        Return text + New String(paddingCharacter, Math.Max(0, minimumLength - text.Length))
    End Function

    ''' <summary>
    ''' Determines a string created by prefixing every line of the given string with the given prefix.
    ''' </summary>
    <Pure()> <Extension()>
    Public Function Indent(ByVal paragraph As String,
                           Optional ByVal prefix As String = vbTab) As String
        Contract.Requires(paragraph IsNot Nothing)
        Contract.Requires(prefix IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Return prefix + paragraph.Replace(Environment.NewLine, Environment.NewLine + prefix)
    End Function

    ''' <summary>
    ''' Determines a string created by replacing format items in the string with a representation of the corresponding arguments.
    ''' </summary>
    <Pure()> <Extension()>
    Public Function Frmt(ByVal format As String, ByVal ParamArray args() As Object) As String
        Contract.Requires(format IsNot Nothing)
        Contract.Requires(args IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Return String.Format(Globalization.CultureInfo.InvariantCulture, format, args)
    End Function

    ''' <summary>
    ''' Concatenates a separator string between the representations of elements in a specified sequence, yielding a single concatenated string.
    ''' </summary>
    <Extension()> <Pure()>
    Public Function StringJoin(Of T)(ByVal this As IEnumerable(Of T), ByVal separator As String) As String
        Contract.Requires(this IsNot Nothing)
        Contract.Requires(separator IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Dim words = From arg In this Select String.Concat(arg)
        Return String.Join(separator, words.ToArray)
    End Function
#End Region

#Region "Enums"
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
#End Region

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
        Return Concat(DirectCast(arrays, IEnumerable(Of T())))
    End Function
    <Pure()>
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

    <Extension()> <Pure()>
    Public Function ToView(Of T)(ByVal list As IList(Of T)) As ViewableList(Of T)
        Contract.Requires(list IsNot Nothing)
        Contract.Ensures(Contract.Result(Of ViewableList(Of T))() IsNot Nothing)
        Contract.Ensures(Contract.Result(Of ViewableList(Of T))().Length = list.Count)
        Return New ViewableList(Of T)(list)
    End Function
    <Extension()> <Pure()>
    Public Function ToView(Of T)(ByVal this As IEnumerable(Of T)) As ViewableList(Of T)
        Contract.Requires(this IsNot Nothing)
        Contract.Ensures(Contract.Result(Of ViewableList(Of T))() IsNot Nothing)
        Return New ViewableList(Of T)(this.ToArray)
    End Function
End Module
