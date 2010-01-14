Imports Strilbrary.Exceptions
Imports Strilbrary.Collections

Namespace Values
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

        '''<summary>Determines the smallest non-negative remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function ProperMod(ByVal value As Integer, ByVal divisor As Integer) As Integer
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Integer)() >= 0)
            Contract.Ensures(Contract.Result(Of Integer)() < divisor)
            Contract.Ensures((value - Contract.Result(Of Integer)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result < 0 Then result += divisor
            Contract.Assume(result >= 0)
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function PositiveMod(ByVal value As Integer, ByVal divisor As Integer) As Integer
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Integer)() > 0)
            Contract.Ensures(Contract.Result(Of Integer)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of Integer)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Contract.Assume(result > 0)
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function

        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(ByVal value As Integer, ByVal divisor As Integer) As Integer
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Integer)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Integer)() >= value)
            Contract.Ensures(Contract.Result(Of Integer)() < value + divisor)
            Dim result = value - (value.PositiveMod(divisor) - divisor)
            Contract.Assume(result Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function FloorMultiple(ByVal value As Integer, ByVal divisor As Integer) As Integer
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Integer)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Integer)() <= value)
            Contract.Ensures(Contract.Result(Of Integer)() > value - divisor)
            Return value - value.ProperMod(divisor)
        End Function

        '''<summary>Determines if a double is not positive infinity, negative infinity, or NaN.</summary>
        <Extension()> <Pure()>
        Public Function IsFinite(ByVal value As Double) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (Not Double.IsInfinity(value) AndAlso Not Double.IsNaN(value)))
            Return Not Double.IsInfinity(value) AndAlso Not Double.IsNaN(value)
        End Function

#Region "Bytes"
        <Extension()> <Pure()>
        Public Function Bytes(ByVal value As UInt16,
                              Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As Byte()
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = 2)

            Dim result = BitConverter.GetBytes(value)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then result = result.Reverse.ToArray
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then result = result.Reverse.ToArray
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Contract.Assume(result.Length = 2)
            Return result
        End Function
        <Extension()> <Pure()>
        Public Function Bytes(ByVal value As UInt32,
                              Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As Byte()
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = 4)

            Dim result = BitConverter.GetBytes(value)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then result = result.Reverse.ToArray
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then result = result.Reverse.ToArray
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Contract.Assume(result.Length = 4)
            Return result
        End Function
        <Extension()> <Pure()>
        Public Function Bytes(ByVal value As UInt64,
                              Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As Byte()
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = 8)

            Dim result = BitConverter.GetBytes(value)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then result = result.Reverse.ToArray
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then result = result.Reverse.ToArray
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Contract.Assume(result.Length = 8)
            Return result
        End Function

        <Extension()> <Pure()>
        Public Function ToUInt16(ByVal data As IEnumerable(Of Byte),
                                 Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Contract.Requires(data IsNot Nothing)
            If data.CountIsGreaterThan(2) Then Throw New ArgumentOutOfRangeException("data", "Data has too many bytes.")
            Dim bytes = data.Take(2).ToArray
            Contract.Assume(bytes.Length = 2)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then data = data.Reverse.ToArray
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then data = data.Reverse.ToArray
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Return BitConverter.ToUInt16(bytes, 0)
        End Function
        <Extension()> <Pure()>
        Public Function ToUInt32(ByVal data As IEnumerable(Of Byte),
                                 Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(data IsNot Nothing)
            If data.CountIsGreaterThan(4) Then Throw New ArgumentOutOfRangeException("data", "Data has too many bytes.")
            Dim bytes = data.Take(4).ToArray
            Contract.Assume(bytes.Length = 4)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then data = data.Reverse.ToArray
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then data = data.Reverse.ToArray
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Return BitConverter.ToUInt32(bytes, 0)
        End Function
        <Extension()> <Pure()>
        Public Function ToUInt64(ByVal data As IEnumerable(Of Byte),
                                 Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Contract.Requires(data IsNot Nothing)
            If data.CountIsGreaterThan(8) Then Throw New ArgumentOutOfRangeException("data", "Data has too many bytes.")
            Dim bytes = data.Take(8).ToArray
            Contract.Assume(bytes.Length = 8)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then data = data.Reverse.ToArray
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then data = data.Reverse.ToArray
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Return BitConverter.ToUInt64(bytes, 0)
        End Function
#End Region

#Region "Bitwise Conversions"
        <Pure()> <Extension()>
        Public Function BitwiseToSByte(ByVal value As Byte) As SByte
            Dim sign = value And (CByte(1) << 7)
            If CBool(sign) Then
                Return CSByte(value And Not sign) Or (CSByte(1) << 7)
            Else
                Return CSByte(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToInt16(ByVal value As UInt16) As Int16
            Dim sign = value And (1US << 15)
            If CBool(sign) Then
                Return CShort(value And Not sign) Or (1S << 15)
            Else
                Return CShort(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToInt32(ByVal value As UInt32) As Int32
            Dim sign = value And (1UI << 31)
            If CBool(sign) Then
                Return CInt(value And Not sign) Or (1 << 31)
            Else
                Return CInt(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToInt64(ByVal value As UInt64) As Int64
            Dim sign = value And (1UL << 63)
            If CBool(sign) Then
                Return CLng(value And Not sign) Or (1L << 63)
            Else
                Return CLng(value)
            End If
        End Function

        <Pure()> <Extension()>
        Public Function BitwiseToByte(ByVal value As SByte) As Byte
            Dim sign = value And (CSByte(1) << 7)
            If CBool(sign) Then
                Return CByte(value And Not sign) Or (CByte(1) << 7)
            Else
                Return CByte(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToUInt16(ByVal value As Int16) As UInt16
            Dim sign = value And (1S << 15)
            If CBool(sign) Then
                Return CUShort(value And Not sign) Or (1US << 15)
            Else
                Return CUShort(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToUInt32(ByVal value As Int32) As UInt32
            Dim sign = value And (1 << 31)
            If CBool(sign) Then
                Return CUInt(value And Not sign) Or (1UI << 31)
            Else
                Return CUInt(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToUInt64(ByVal value As Int64) As UInt64
            Dim sign = value And (1L << 63)
            If CBool(sign) Then
                Return CULng(value And Not sign) Or (1UL << 63)
            Else
                Return CULng(value)
            End If
        End Function
#End Region

#Region "ShiftRotate"
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(ByVal value As Byte, ByVal offset As Integer) As Byte
            offset = offset.ProperMod(8)
            Return (value << offset) Or (value >> (8 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(ByVal value As UInt16, ByVal offset As Integer) As UInt16
            offset = offset.ProperMod(16)
            Return (value << offset) Or (value >> (16 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(ByVal value As UInt32, ByVal offset As Integer) As UInt32
            offset = offset.ProperMod(32)
            Return (value << offset) Or (value >> (32 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(ByVal value As UInt64, ByVal offset As Integer) As UInt64
            offset = offset.ProperMod(64)
            Return (value << offset) Or (value >> (64 - offset))
        End Function

        <Pure()> <Extension()>
        Public Function ShiftRotateRight(ByVal value As Byte, ByVal offset As Integer) As Byte
            offset = offset.ProperMod(8)
            Return (value >> offset) Or (value << (8 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateRight(ByVal value As UInt16, ByVal offset As Integer) As UInt16
            offset = offset.ProperMod(16)
            Return (value >> offset) Or (value << (16 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateRight(ByVal value As UInt32, ByVal offset As Integer) As UInt32
            offset = offset.ProperMod(32)
            Return (value >> offset) Or (value << (32 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateRight(ByVal value As UInt64, ByVal offset As Integer) As UInt64
            offset = offset.ProperMod(64)
            Return (value >> offset) Or (value << (64 - offset))
        End Function
#End Region
    End Module
End Namespace
