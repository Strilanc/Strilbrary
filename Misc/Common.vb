Imports System.Net.NetworkInformation
Imports System.Runtime.CompilerServices
Imports System.IO.Path
Imports System.Text
Imports System.Net
Imports System.IO

'''<summary>A smattering of functions and other stuff that hasn't been placed in more reasonable groups yet.</summary>
Public Module PoorlyCategorizedFunctions
#Region "Numbers"
    <Extension()> <Pure()>
    Public Function ReversedByteOrder(ByVal value As UInteger) As UInteger
        Dim reversedValue = 0UI
        For i = 0 To 3
            reversedValue <<= 8
            reversedValue = reversedValue Or (value And CUInt(&HFF))
            value >>= 8
        Next i
        Return reversedValue
    End Function
    <Extension()> <Pure()>
    Public Function ReversedByteOrder(ByVal value As ULong) As ULong
        Dim reversedValue = 0UL
        For i = 0 To 7
            reversedValue <<= 8
            reversedValue = reversedValue Or (value And CULng(&HFF))
            value >>= 8
        Next i
        Return reversedValue
    End Function

    '''<summary>Returns the smallest multiple of n that is not less than i. Formally: min {x in Z | x = 0 (mod n), x >= i}</summary>
    <Pure()>
    Public Function ModCeiling(ByVal value As Integer, ByVal divisor As Integer) As Integer
        Contract.Requires(divisor > 0)
        If value Mod divisor = 0 Then Return value
        Dim m = (value \ divisor) * divisor
        If value < 0 Then Return m
        If m > Integer.MaxValue - divisor Then
            Throw New InvalidOperationException("The result will not fit into an Int32.")
        End If
        Return m + divisor
    End Function

    <Pure()> <Extension()>
    Public Function Between(Of T As IComparable(Of T))(ByVal value1 As T,
                                                       ByVal value2 As T,
                                                       ByVal value3 As T) As T
        Contract.Requires(value1.IsNotNullReference())
        Contract.Requires(value2.IsNotNullReference())
        Contract.Requires(value3.IsNotNullReference())
        Contract.Ensures(Contract.Result(Of T).IsNotNullReference)
        'recursive sort
        If value2.CompareTo(value1) > 0 Then Return Between(value2, value1, value3)
        If value2.CompareTo(value3) < 0 Then Return Between(value1, value3, value2)
        'median
        Return value2
    End Function
#End Region

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
        Return String.Format(format, args)
    End Function
#End Region

#Region "Enums"
    <Pure()> <Extension()>
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
            If String.Compare(value, e.ToString(), ignoreCase, Globalization.CultureInfo.InvariantCulture) = 0 Then
                ret = e
                Return True
            End If
        Next e
        Return False
    End Function
    <Pure()> <Extension()>
    Public Function EnumValueIsDefined(Of T)(ByVal value As T) As Boolean
        Return EnumValues(Of T).Contains(value)
    End Function
#End Region

#Region "Arrays"
    <Pure()> <Extension()>
    Public Function ArraysEqual(Of T As IComparable(Of T))(ByVal array1() As T, ByVal array2() As T) As Boolean
        Contract.Requires(array1 IsNot Nothing)
        Contract.Requires(array2 IsNot Nothing)
        If array1.Length <> array2.Length Then Return False
        For i = 0 To array1.Length - 1
            If array1(i).CompareTo(array2(i)) <> 0 Then Return False
        Next i
        Return True
    End Function
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
        Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
        If offset + length > array.Length Then Throw New ArgumentOutOfRangeException("offset + length")

        Dim new_array(0 To length - 1) As T
        System.Array.Copy(array, offset, new_array, 0, new_array.Length)
        Return new_array
    End Function
    <Pure()>
    Public Function Concat(Of T)(ByVal ParamArray arrays As T()()) As T()
        Contract.Requires(arrays IsNot Nothing)
        Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
        Return Concat(CType(arrays, IEnumerable(Of T())))
    End Function
    <Pure()>
    Public Function Concat(Of T)(ByVal arrays As IEnumerable(Of T())) As T()
        Contract.Requires(arrays IsNot Nothing)
        Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
        If (From array In arrays Where array Is Nothing).Any Then Throw New ArgumentNullException("array is null", "arrays")

        Dim totalLength = 0
        For Each array In arrays
            totalLength += array.Length
        Next array

        Dim flattenedArray(0 To totalLength - 1) As T
        Dim processingOffset = 0
        For Each array In arrays
            System.Array.Copy(array, 0, flattenedArray, processingOffset, array.Length)
            processingOffset += array.Length
        Next array

        Return flattenedArray
    End Function
#End Region

    Public Sub Swap(Of T)(ByRef value1 As T, ByRef value2 As T)
        Dim vt = value1
        value1 = value2
        value2 = vt
    End Sub

    Public Sub RunWithDebugTrap(ByVal action As Action, ByVal context As String)
        Contract.Requires(action IsNot Nothing)
        Contract.Requires(context IsNot Nothing)

        If My.Settings.IsDebugMode Then
            Call action()
        Else
            Try
                Call action()
            Catch e As Exception
                LogUnexpectedException("{0} threw an unhandled exception.".Frmt(context), e)
            End Try
        End If
    End Sub
    <Extension()> <Pure()>
    Public Function StringJoin(Of T)(ByVal this As IEnumerable(Of T), ByVal separator As String) As String
        Contract.Requires(this IsNot Nothing)
        Contract.Requires(separator IsNot Nothing)
        Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
        Dim this_ = this
        Return String.Join(separator, (From arg In this_ Select (arg.ToString)).ToArray)
    End Function
    <Extension()> <Pure()>
    Public Function ToView(Of T)(ByVal this As IEnumerable(Of T)) As ViewableList(Of T)
        Contract.Requires(this IsNot Nothing)
        Contract.Ensures(Contract.Result(Of ViewableList(Of T))() IsNot Nothing)
        Return New ViewableList(Of T)(this.ToArray)
    End Function

    <Extension()>
    Public Function ReadBytes(ByVal stream As IO.Stream, ByVal length As Integer) As Byte()
        Contract.Requires(stream IsNot Nothing)
        Contract.Requires(length >= 0)
        Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
        Dim buffer(0 To length - 1) As Byte
        length = stream.Read(buffer, 0, length)
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
        Return New ViewableList(Of T)(list)
    End Function

    Private Delegate Function RecursiveFunction(Of TArg1, TReturn)(ByVal self As RecursiveFunction(Of TArg1, TReturn)) As Func(Of TArg1, TReturn)
    <Pure()>
    Public Function YCombinator(Of TArg1, TReturn)(ByVal recursor As Func(Of Func(Of TArg1, TReturn), Func(Of TArg1, TReturn))) As Func(Of TArg1, TReturn)
        Contract.Requires(recursor IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Func(Of TArg1, TReturn))() IsNot Nothing)
        Dim recursor_ = recursor 'avoids hoisted argument contract verification flaw
        Dim rec As RecursiveFunction(Of TArg1, TReturn) = Function(self) Function(arg1) recursor_(self(self))(arg1)
        Dim ret = rec(rec)
        Contract.Assume(ret IsNot Nothing)
        Return ret
    End Function
    Private Delegate Function RecursiveAction(ByVal self As RecursiveAction) As Action
    <Pure()>
    Public Function YCombinator(ByVal recursor As Func(Of Action, Action)) As Action
        Contract.Requires(recursor IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Action)() IsNot Nothing)
        Dim recursor_ = recursor 'avoids hoisted argument contract verification flaw
        Dim rec As RecursiveAction = Function(self) Sub() recursor_(self(self))()
        Dim ret = rec(rec)
        Contract.Assume(ret IsNot Nothing)
        Return ret
    End Function
    Private Delegate Function RecursiveAction(Of TArg1)(ByVal self As RecursiveAction(Of TArg1)) As Action(Of TArg1)
    <Pure()>
    Public Function YCombinator(Of TArg1)(ByVal recursor As Func(Of Action(Of TArg1), Action(Of TArg1))) As Action(Of TArg1)
        Contract.Requires(recursor IsNot Nothing)
        Contract.Ensures(Contract.Result(Of Action(Of TArg1))() IsNot Nothing)
        Dim recursor_ = recursor 'avoids hoisted argument contract verification flaw
        Dim rec As RecursiveAction(Of TArg1) = Function(self) Sub(arg1) recursor_(self(self))(arg1)
        Dim ret = rec(rec)
        Contract.Assume(ret IsNot Nothing)
        Return ret
    End Function

    Public Sub FutureIterate(Of T)(ByVal producer As Func(Of IFuture(Of T)),
                                   ByVal consumer As Func(Of T, IFuture(Of Boolean)))
        producer().CallWhenValueReady(YCombinator(Of T)(
            Function(self) Sub(result)
                               consumer(result).CallWhenValueReady(
                                   Sub([continue])
                                       If [continue] Then
                                           producer().CallWhenValueReady(self)
                                       End If
                                   End Sub)
                           End Sub))
    End Sub

    '''<summary>Returns true if T is a class type and arg is nothing.</summary>
    <Extension()> <Pure()>
    Public Function IsNullReferenceGeneric(Of T)(ByVal arg As T) As Boolean
        Contract.Ensures(Contract.Result(Of Boolean)() = Object.ReferenceEquals(arg, Nothing))
        Return Object.ReferenceEquals(arg, Nothing)
    End Function
    '''<summary>Returns true if T is a structure type or arg is not nothing.</summary>
    <Extension()> <Pure()>
    Public Function IsNotNullReference(Of T)(ByVal arg As T) As Boolean
        Contract.Ensures(Contract.Result(Of Boolean)() = Not Object.ReferenceEquals(arg, Nothing))
        Return Not Object.ReferenceEquals(arg, Nothing)
    End Function

    <Extension()>
    Public Function FutureRead(ByVal stream As IO.Stream,
                               ByVal buffer() As Byte,
                               ByVal offset As Integer,
                               ByVal count As Integer) As IFuture(Of PossibleException(Of Integer, Exception))
        Contract.Requires(stream IsNot Nothing)
        Contract.Ensures(Contract.Result(Of IFuture(Of PossibleException(Of Integer, Exception)))() IsNot Nothing)
        Dim stream_ = stream
        Dim f = New Future(Of PossibleException(Of Integer, Exception))
        Try
            stream.BeginRead(buffer, offset, count, Sub(ar)
                                                        Contract.Requires(ar IsNot Nothing)
                                                        Try
                                                            f.SetValue(stream_.EndRead(ar))
                                                        Catch e As Exception
                                                            f.SetValue(e)
                                                        End Try
                                                    End Sub, Nothing)
        Catch e As Exception
            f.SetValue(e)
        End Try
        Return f
    End Function
End Module

Public Class ExpensiveValue(Of T)
    Private func As Func(Of T)
    Private _value As T
    Private computed As Boolean
    Public Sub New(ByVal func As Func(Of T))
        Me.func = func
    End Sub
    Public Sub New(ByVal value As T)
        Me._value = value
        Me.computed = True
    End Sub
    Public ReadOnly Property Value As T
        Get
            If Not computed Then
                computed = True
                _value = func()
            End If
            Return _value
        End Get
    End Property
    Public Shared Widening Operator CType(ByVal func As Func(Of T)) As ExpensiveValue(Of T)
        Return New ExpensiveValue(Of T)(func)
    End Operator
    Public Shared Widening Operator CType(ByVal value As T) As ExpensiveValue(Of T)
        Return New ExpensiveValue(Of T)(value)
    End Operator
End Class

Public Class KeyPair
    Private ReadOnly _value1 As Byte()
    Private ReadOnly _value2 As Byte()
    Public ReadOnly Property Value1 As Byte()
        Get
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Return _value1
        End Get
    End Property
    Public ReadOnly Property Value2 As Byte()
        Get
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Return _value2
        End Get
    End Property

    <ContractInvariantMethod()> Protected Sub Invariant()
        Contract.Invariant(_value1 IsNot Nothing)
        Contract.Invariant(_value2 IsNot Nothing)
    End Sub

    Public Sub New(ByVal value1 As Byte(), ByVal value2 As Byte())
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Me._value1 = value1
        Me._value2 = value2
    End Sub
End Class
