Namespace Numerics
    Public Enum ByteOrder
        '''<summary>Least significant bytes first.</summary>
        LittleEndian
        '''<summary>Most significant bytes first.</summary>
        BigEndian
    End Enum

    Public Module NumericExtensions
        <Extension()> <Pure()>
        Public Function ReversedByteOrder(ByVal value As UInt32) As UInt32
            Dim reversedValue = 0UI
            For i = 0 To 3
                reversedValue <<= 8
                reversedValue = reversedValue Or (value And &HFFUI)
                value >>= 8
            Next i
            Return reversedValue
        End Function
        <Extension()> <Pure()>
        Public Function ReversedByteOrder(ByVal value As UInt64) As UInt64
            Dim reversedValue = 0UL
            For i = 0 To 7
                reversedValue <<= 8
                reversedValue = reversedValue Or (value And &HFFUL)
                value >>= 8
            Next i
            Return reversedValue
        End Function

        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function ModCeiling(ByVal value As Integer, ByVal divisor As Integer) As Integer
            'contracts 1.2.21022.12 incorrectly proves this method false
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Integer)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Integer)() >= value)
            Contract.Ensures(Contract.Result(Of Integer)() < value + divisor)

            If value Mod divisor = 0 Then
                Return value
            ElseIf value < 0 Then
                'division will truncate upwards
                Return (value \ divisor) * divisor
            Else
                'division will truncate downwards
                Return (value \ divisor) * divisor + divisor
            End If
        End Function

        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function ModFloor(ByVal value As Integer, ByVal divisor As Integer) As Integer
            'contracts 1.2.21022.12 incorrectly proves this method false
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Integer)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Integer)() <= value)
            Contract.Ensures(Contract.Result(Of Integer)() > value - divisor)

            If value Mod divisor = 0 Then
                Return value
            ElseIf value < 0 Then
                'division will truncate upwards
                Return (value \ divisor) * divisor - divisor
            Else
                'division will truncate downwards
                Return (value \ divisor) * divisor
            End If
        End Function

        '''<summary>Determines if a double is not positive infinity, negative infinity, or NaN.</summary>
        <Extension()> <Pure()>
        Public Function IsFinite(ByVal value As Double) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (Not Double.IsInfinity(value) AndAlso Not Double.IsNaN(value)))
            Return Not Double.IsInfinity(value) AndAlso Not Double.IsNaN(value)
        End Function

        <Extension()> <Pure()>
        Public Function ToUInt16(ByVal data As IEnumerable(Of Byte),
                                 Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Contract.Requires(data IsNot Nothing)
            If data.CountIsGreaterThan(2) Then Throw New ArgumentOutOfRangeException("data", "Data has too many bytes.")
            Return CUShort(data.ToUInt64(byteOrder))
        End Function
        <Extension()> <Pure()>
        Public Function ToUInt32(ByVal data As IEnumerable(Of Byte),
                                 Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(data IsNot Nothing)
            If data.CountIsGreaterThan(4) Then Throw New ArgumentOutOfRangeException("data", "Data has too many bytes.")
            Return CUInt(data.ToUInt64(byteOrder))
        End Function
        <Extension()> <Pure()>
        Public Function ToUInt64(ByVal data As IEnumerable(Of Byte),
                                 Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Contract.Requires(data IsNot Nothing)
            If data.CountIsGreaterThan(8) Then Throw New ArgumentOutOfRangeException("data", "Data has too many bytes.")
            Dim val As ULong
            Select Case byteOrder
                Case byteOrder.LittleEndian
                    data = data.Reverse
                Case byteOrder.BigEndian
                    'no change required
                Case Else
                    Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            For Each b In data
                val <<= 8
                val = val Or b
            Next b
            Return val
        End Function

        <Extension()> <Pure()>
        Public Function Bytes(ByVal value As UInt16,
                              Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian,
                              Optional ByVal size As Integer = 2) As Byte()
            Contract.Requires(size >= 0)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = size)
            Return CULng(value).Bytes(byteOrder, size)
        End Function
        <Extension()> <Pure()>
        Public Function Bytes(ByVal value As UInt32,
                              Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian,
                              Optional ByVal size As Integer = 4) As Byte()
            Contract.Requires(size >= 0)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = size)
            Return CULng(value).Bytes(byteOrder, size)
        End Function
        <Extension()> <Pure()>
        Public Function Bytes(ByVal value As UInt64,
                              Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian,
                              Optional ByVal size As Integer = 8) As Byte()
            Contract.Requires(size >= 0)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = size)
            Dim data(0 To size - 1) As Byte
            For i = 0 To size - 1
                data(i) = CByte(value And CULng(&HFF))
                value >>= 8
            Next i
            If value <> 0 Then Throw New ArgumentOutOfRangeException("value", "The specified value won't fit in the specified number of bytes.")
            Select Case byteOrder
                Case byteOrder.BigEndian
                    Return data.Reverse.ToArray
                Case byteOrder.LittleEndian
                    Return data
                Case Else
                    Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
        End Function

        <Extension()> <Pure()>
        Public Function ToAscBytes(ByVal data As String,
                                   Optional ByVal nullTerminate As Boolean = False) As Byte()
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Dim bytes As New List(Of Byte)(data.Length + 1)
            For Each c In data
                bytes.Add(CByte(Asc(c)))
            Next
            If nullTerminate Then bytes.Add(0)
            Return bytes.ToArray()
        End Function
        <Extension()> <Pure()>
        Public Function ParseChrString(ByVal data As IEnumerable(Of Byte),
                                       ByVal nullTerminated As Boolean) As String
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)

            Dim s As New System.Text.StringBuilder()
            For Each b In data
                If b = 0 AndAlso nullTerminated Then Exit For
                s.Append(Chr(b))
            Next b

            Return s.ToString
        End Function

        <Extension()> <Pure()>
        Public Function ToHexString(ByVal data As IEnumerable(Of Byte),
                                    Optional ByVal minWordLength As Byte = 2,
                                    Optional ByVal separator As String = " ") As String
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(separator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)

            Dim s As New System.Text.StringBuilder()
            For Each b In data
                If s.Length > 0 Then s.Append(separator)
                Dim h = Hex(b)
                Contract.Assume(h IsNot Nothing)
                For i = 1 To minWordLength - h.Length
                    s.Append("0"c)
                Next i
                s.Append(h)
            Next b
            Return s.ToString()
        End Function
        <Extension()> <Pure()>
        Public Function FromHexStringToBytes(ByVal data As String) As Byte()
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)

            If data Like "*[!0-9A-Fa-f ]*" Then Throw New ArgumentException("Invalid characters.")
            If data Like "*[! ][! ][! ]*" Then Throw New ArgumentException("Contains a hex value which won't fit in a byte.")
            Dim words = data.Split(" "c)
            Dim bb(0 To words.Length - 1) As Byte
            For i = 0 To words.Length - 1
                Contract.Assume(words(i) IsNot Nothing)
                bb(i) = CByte(words(i).FromHexToUInt64(ByteOrder.BigEndian))
            Next i
            Return bb
        End Function

        <Extension()> <Pure()>
        Public Function ToBinary(ByVal value As ULong,
                                 Optional ByVal minLength As Integer = 8) As String
            Contract.Requires(minLength >= 0)
            Contract.Requires(minLength <= 64)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Dim ret = ""
            While value > 0 Or minLength > 0
                ret = (value And CULng(&H1)).ToString(Globalization.CultureInfo.InvariantCulture) + ret
                value >>= 1
                minLength -= 1
            End While
            Return ret
        End Function

        Private ReadOnly HexDictionary As New Dictionary(Of Char, Byte) From {
                    {"0"c, 0}, {"1"c, 1}, {"2"c, 2}, {"3"c, 3}, {"4"c, 4},
                    {"5"c, 5}, {"6"c, 6}, {"7"c, 7}, {"8"c, 8}, {"9"c, 9},
                    {"a"c, 10}, {"A"c, 10},
                    {"b"c, 11}, {"B"c, 11},
                    {"c"c, 12}, {"C"c, 12},
                    {"d"c, 13}, {"D"c, 13},
                    {"e"c, 14}, {"E"c, 14},
                    {"f"c, 15}, {"F"c, 15}}

        <Extension()> <Pure()>
        Public Function FromHexToUInt64(ByVal chars As IEnumerable(Of Char),
                                        ByVal byteOrder As ByteOrder) As ULong
            Contract.Requires(chars IsNot Nothing)

            Select Case byteOrder
                Case byteOrder.LittleEndian
                    chars = chars.Reverse()
                Case byteOrder.BigEndian
                    'no change needed
                Case Else
                    Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            Dim val = 0UL
            For Each c In chars
                If Not HexDictionary.ContainsKey(c) Then Throw New ArgumentException("Invalid character.", "chars")
                val <<= 4
                val += HexDictionary(c)
            Next c
            Return val
        End Function
    End Module
End Namespace