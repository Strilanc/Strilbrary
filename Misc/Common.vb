Imports System.Net.NetworkInformation
Imports System.Runtime.CompilerServices
Imports System.IO.Path
Imports System.Text
Imports System.Net
Imports System.IO

'''<summary>A smattering of functions and other stuff that hasn't been placed in more reasonable groups yet.</summary>
Public Module PoorlyCategorizedFunctions
#Region "Strings"
    <Pure()> <Extension()>
    Public Function Padded(ByVal text As String,
                           ByVal minChars As Integer,
                           Optional ByVal paddingCharacter As Char = " "c) As String
        Contract.Requires(text IsNot Nothing)
        Contract.Requires(minChars >= 0)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        If text.Length < minChars Then
            text += New String(paddingCharacter, minChars - text.Length)
        End If
        Return text
    End Function
    <Pure()> <Extension()>
    Public Function Indent(ByVal paragraph As String,
                           Optional ByVal prefix As String = vbTab) As String
        Contract.Requires(paragraph IsNot Nothing)
        Contract.Requires(prefix IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Return prefix + paragraph.Replace(Environment.NewLine, Environment.NewLine + prefix)
    End Function
    <Pure()> <Extension()>
    Public Function Frmt(ByVal format As String, ByVal ParamArray args() As Object) As String
        Contract.Requires(format IsNot Nothing)
        Contract.Requires(args IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Return String.Format(Globalization.CultureInfo.InvariantCulture, format, args)
    End Function
    <Extension()> <Pure()>
    Public Function StringJoin(Of T)(ByVal this As IEnumerable(Of T), ByVal separator As String) As String
        Contract.Requires(this IsNot Nothing)
        Contract.Requires(separator IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Dim words = From arg In this Select String.Concat(arg)
        Contract.Assume(words IsNot Nothing)
        Return String.Join(separator, words.ToArrayNonNull)
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
    Public Function EnumTryParse(Of T)(ByVal value As String, ByVal ignoreCase As Boolean, ByRef ret As T) As Boolean
        For Each e In EnumValues(Of T)()
            If String.Compare(value, e.ToString(), If(ignoreCase, StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase)) = 0 Then
                ret = e
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

#Region "Temporary"
    <Extension()> <Pure()>
    Friend Function SkipNonNull(Of T)(ByVal this As IEnumerable(Of T), ByVal amount As Integer) As IEnumerable(Of T)
        Contract.Requires(this IsNot Nothing)
        Contract.Requires(amount >= 0)
        Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
        Dim res = this.Skip(amount)
        Contract.Assume(res IsNot Nothing)
        Return res
    End Function
    <Extension()> <Pure()>
    Friend Function ToArrayNonNull(Of T)(ByVal this As IEnumerable(Of T)) As T()
        Contract.Requires(this IsNot Nothing)
        Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
        Dim res = this.ToArray
        Contract.Assume(res IsNot Nothing)
        Return res
    End Function
#End Region

    <Extension()>
    Public Function ReadBytes(ByVal stream As IO.Stream, ByVal length As Integer) As Byte()
        Contract.Requires(stream IsNot Nothing)
        Contract.Requires(length >= 0)
        Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
        Dim buffer(0 To length - 1) As Byte
        length = stream.Read(buffer, 0, length)
        Contract.Assume(length >= 0)
        If length < buffer.Length Then
            ReDim Preserve buffer(0 To length - 1)
            Contract.Assume(buffer IsNot Nothing)
        End If
        Return buffer
    End Function

    Public Function ReadAllStreamBytes(ByVal stream As IO.Stream) As Byte()
        Contract.Requires(stream IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
        Dim m = 1024
        Dim bb(0 To m - 1) As Byte
        Dim c = 0
        Do
            Dim n = stream.Read(bb, c, m - c)
            c += n
            If c <> m Then Exit Do
            m *= 2
            ReDim Preserve bb(0 To m - 1)
        Loop
        Contract.Assume(c >= 0)
        ReDim Preserve bb(0 To c - 1)
        Contract.Assume(bb IsNot Nothing)
        Return bb
    End Function

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
        Return New ViewableList(Of T)(this.ToArrayNonNull)
    End Function

    Private Delegate Function RecursiveFunction(Of TArg1, TReturn)(ByVal self As RecursiveFunction(Of TArg1, TReturn)) As Func(Of TArg1, TReturn)
    <Pure()>
    Public Function YCombinator(Of TArg1, TReturn)(ByVal recursor As Func(Of Func(Of TArg1, TReturn), Func(Of TArg1, TReturn))) As Func(Of TArg1, TReturn)
        Contract.Requires(recursor IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Func(Of TArg1, TReturn))() IsNot Nothing)
        Dim rec As RecursiveFunction(Of TArg1, TReturn) = Function(self)
                                                              Return Function(arg1)
                                                                         Contract.Assume(self IsNot Nothing)
                                                                         Contract.Assume(recursor IsNot Nothing)
                                                                         Dim x = recursor(self(self))
                                                                         Contract.Assume(x IsNot Nothing)
                                                                         Return x(arg1)
                                                                     End Function
                                                          End Function
        Dim ret = rec(rec)
        Contract.Assume(ret IsNot Nothing)
        Return ret
    End Function
    Private Delegate Function RecursiveAction(ByVal self As RecursiveAction) As Action
    <Pure()>
    Public Function YCombinator(ByVal recursor As Func(Of Action, Action)) As Action
        Contract.Requires(recursor IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Action)() IsNot Nothing)
        Dim rec As RecursiveAction = Function(self)
                                         Return Sub()
                                                    Contract.Assume(recursor IsNot Nothing)
                                                    Contract.Assume(self IsNot Nothing)
                                                    Dim x = recursor(self(self))
                                                    Contract.Assume(x IsNot Nothing)
                                                    Call x()
                                                End Sub
                                     End Function
        Dim ret = rec(rec)
        Contract.Assume(ret IsNot Nothing)
        Return ret
    End Function
    Private Delegate Function RecursiveAction(Of TArg1)(ByVal self As RecursiveAction(Of TArg1)) As Action(Of TArg1)
    <Pure()>
    Public Function YCombinator(Of TArg1)(ByVal recursor As Func(Of Action(Of TArg1), Action(Of TArg1))) As Action(Of TArg1)
        Contract.Requires(recursor IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Action(Of TArg1))() IsNot Nothing)
        Dim rec As RecursiveAction(Of TArg1) = Function(self)
                                                   Return Sub(arg1)
                                                              Contract.Assume(recursor IsNot Nothing)
                                                              Contract.Assume(self IsNot Nothing)
                                                              Dim x = recursor(self(self))
                                                              Contract.Assume(x IsNot Nothing)
                                                              Call x(arg1)
                                                          End Sub
                                               End Function
        Dim ret = rec(rec)
        Contract.Assume(ret IsNot Nothing)
        Return ret
    End Function
End Module
