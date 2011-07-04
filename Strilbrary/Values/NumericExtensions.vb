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
        Public Function ReversedByteOrder(value As UInt16) As UInt16
            Dim reversedValue = 0US
            For i = 0 To 2 - 1
                reversedValue <<= 8
                reversedValue = reversedValue Or (value And &HFFUS)
                value >>= 8
            Next i
            Return reversedValue
        End Function
        <Extension()> <Pure()>
        Public Function ReversedByteOrder(value As UInt32) As UInt32
            Dim reversedValue = 0UI
            For i = 0 To 4 - 1
                reversedValue <<= 8
                reversedValue = reversedValue Or (value And &HFFUI)
                value >>= 8
            Next i
            Return reversedValue
        End Function
        <Extension()> <Pure()>
        Public Function ReversedByteOrder(value As UInt64) As UInt64
            Dim reversedValue = 0UL
            For i = 0 To 8 - 1
                reversedValue <<= 8
                reversedValue = reversedValue Or (value And &HFFUL)
                value >>= 8
            Next i
            Return reversedValue
        End Function

        '''<summary>Determines if a double is not positive infinity, negative infinity, or NaN.</summary>
        <Extension()> <Pure()>
        Public Function IsFinite(value As Double) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (Not Double.IsInfinity(value) AndAlso Not Double.IsNaN(value)))
            Return Not Double.IsInfinity(value) AndAlso Not Double.IsNaN(value)
        End Function

#Region "Mod/Multiple"
        '''<summary>Determines the smallest non-negative remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function ProperMod(value As BigInteger, divisor As BigInteger) As BigInteger
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() >= 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() < divisor)
            Contract.Ensures((value - Contract.Result(Of BigInteger)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result < 0 Then result += divisor
            Contract.Assume(result >= 0)
            Contract.Assume(result < divisor)
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest non-negative remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function ProperMod(value As Int64, divisor As Int64) As Int64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int64)() >= 0)
            Contract.Ensures(Contract.Result(Of Int64)() < divisor)
            Contract.Ensures((value - Contract.Result(Of Int64)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result < 0 Then result += divisor
            Return result
        End Function
        '''<summary>Determines the smallest non-negative remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function ProperMod(value As Int32, divisor As Int32) As Int32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int32)() >= 0)
            Contract.Ensures(Contract.Result(Of Int32)() < divisor)
            Contract.Ensures((value - Contract.Result(Of Int32)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result < 0 Then result += divisor
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest non-negative remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function ProperMod(value As Int16, divisor As Int16) As Int16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int16)() >= 0)
            Contract.Ensures(Contract.Result(Of Int16)() < divisor)
            Contract.Ensures((value - Contract.Result(Of Int16)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result < 0 Then result += divisor
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function

        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function PositiveMod(value As BigInteger, divisor As BigInteger) As BigInteger
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() > 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of BigInteger)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Contract.Assume(result > 0)
            Contract.Assume(result <= divisor)
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function PositiveMod(value As Int64, divisor As Int64) As Int64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int64)() > 0)
            Contract.Ensures(Contract.Result(Of Int64)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of Int64)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function PositiveMod(value As Int32, divisor As Int32) As Int32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int32)() > 0)
            Contract.Ensures(Contract.Result(Of Int32)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of Int32)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function PositiveMod(value As Int16, divisor As Int16) As Int16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int16)() > 0)
            Contract.Ensures(Contract.Result(Of Int16)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of Int16)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of UInt64)() > 0")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of UInt64)() <= divisor")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-(value - Contract.Result(Of UInt64)()) Mod divisor = 0")>
        Public Function PositiveMod(value As UInt64, divisor As UInt64) As UInt64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt64)() > 0)
            Contract.Ensures(Contract.Result(Of UInt64)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of UInt64)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-(value - Contract.Result(Of UInt32)()) Mod divisor = 0")>
        Public Function PositiveMod(value As UInt32, divisor As UInt32) As UInt32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt32)() > 0)
            Contract.Ensures(Contract.Result(Of UInt32)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of UInt32)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Return result
        End Function
        '''<summary>Determines the smallest positive remainder of the division of the value by the given divisor.</summary>
        <Extension()> <Pure()>
        Public Function PositiveMod(value As UInt16, divisor As UInt16) As UInt16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt16)() > 0)
            Contract.Ensures(Contract.Result(Of UInt16)() <= divisor)
            Contract.Ensures((value - Contract.Result(Of UInt16)()) Mod divisor = 0)
            Dim result = value Mod divisor
            If result <= 0 Then result += divisor
            Contract.Assume((value - result) Mod divisor = 0)
            Return result
        End Function

        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(value As BigInteger, divisor As BigInteger) As BigInteger
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() >= value)
            Contract.Ensures(Contract.Result(Of BigInteger)() < value + divisor)
            Dim result = value + divisor - value.PositiveMod(divisor)
            Contract.Assume(result Mod divisor = 0)
            Contract.Assume(result >= value)
            Contract.Assume(result < value + divisor)
            Return result
        End Function
        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(value As Int64, divisor As Int64) As Int64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int64)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Int64)() >= value)
            Contract.Ensures(Contract.Result(Of Int64)() < value + divisor)
            Return value + divisor - value.PositiveMod(divisor)
        End Function
        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(value As Int32, divisor As Int32) As Int32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int32)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Int32)() >= value)
            Contract.Ensures(Contract.Result(Of Int32)() < value + divisor)
            Dim result = value + divisor - value.PositiveMod(divisor)
            Contract.Assume(result Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(value As Int16, divisor As Int16) As Int16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int16)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Int16)() >= value)
            Contract.Ensures(Contract.Result(Of Int16)() < value + divisor)
            Dim result = value + divisor - value.PositiveMod(divisor)
            Contract.Assume(result Mod divisor = 0)
            Return result
        End Function
        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of UInt64)() Mod divisor = 0")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of UInt64)() < value + divisor")>
        <SuppressMessage("Microsoft.Contracts", "Requires-19-90")>
        Public Function CeilingMultiple(value As UInt64, divisor As UInt64) As UInt64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt64)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of UInt64)() >= value)
            Contract.Ensures(Contract.Result(Of UInt64)() < value + divisor)
            Return value + divisor - value.PositiveMod(divisor)
        End Function
        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(value As UInt32, divisor As UInt32) As UInt32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt32)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of UInt32)() >= value)
            Contract.Ensures(Contract.Result(Of UInt32)() < value + divisor)
            Return value + divisor - value.PositiveMod(divisor)
        End Function
        '''<summary>Determines the smallest multiple of the divisor greater than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function CeilingMultiple(value As UInt16, divisor As UInt16) As UInt16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt16)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of UInt16)() >= value)
            Contract.Ensures(Contract.Result(Of UInt16)() < value + divisor)
            Dim result = value + divisor - value.PositiveMod(divisor)
            Contract.Assume(result Mod divisor = 0)
            Return result
        End Function

        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of BigInteger)() Mod divisor = 0")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of BigInteger)() <= value")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of BigInteger)() > value - divisor")>
        Public Function FloorMultiple(value As BigInteger, divisor As BigInteger) As BigInteger
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of BigInteger)() <= value)
            Contract.Ensures(Contract.Result(Of BigInteger)() > value - divisor)
            Return value - value.ProperMod(divisor)
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function FloorMultiple(value As Int64, divisor As Int64) As Int64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int64)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Int64)() <= value)
            Contract.Ensures(Contract.Result(Of Int64)() > value - divisor)
            Return value - value.ProperMod(divisor)
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function FloorMultiple(value As Int32, divisor As Int32) As Int32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int32)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Int32)() <= value)
            Contract.Ensures(Contract.Result(Of Int32)() > value - divisor)
            Return value - value.ProperMod(divisor)
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function FloorMultiple(value As Int16, divisor As Int16) As Int16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of Int16)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of Int16)() <= value)
            Contract.Ensures(Contract.Result(Of Int16)() > value - divisor)
            Return value - value.ProperMod(divisor)
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of UInt64)() Mod divisor = 0")>
        Public Function FloorMultiple(value As UInt64, divisor As UInt64) As UInt64
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt64)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of UInt64)() <= value)
            Contract.Ensures(Contract.Result(Of UInt64)() + divisor > value)
            Return value - value Mod divisor
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        <Extension()> <Pure()>
        Public Function FloorMultiple(value As UInt32, divisor As UInt32) As UInt32
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt32)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of UInt32)() <= value)
            Contract.Ensures(Contract.Result(Of UInt32)() + divisor > value)
            Return value - value Mod divisor
        End Function
        '''<summary>Determines the largest multiple of the divisor less than or equal to the given value.</summary>
        '''<remarks>Verification is disabled because the verifier suggests a tautological precondition.</remarks>
        <Extension()> <Pure()>
        Public Function FloorMultiple(value As UInt16, divisor As UInt16) As UInt16
            Contract.Requires(divisor > 0)
            Contract.Ensures(Contract.Result(Of UInt16)() Mod divisor = 0)
            Contract.Ensures(Contract.Result(Of UInt16)() <= value)
            Contract.Ensures(Contract.Result(Of UInt16)() + divisor > value)
            Return value - value Mod divisor
        End Function
#End Region

#Region "Bytes"
        <Extension()> <Pure()>
        Public Function Bytes(value As UInt16,
                              Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As IRist(Of Byte)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))().Count = 2)

            Dim result = BitConverter.GetBytes(value).AsRist()
            Contract.Assume(result.Count = 2)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then Return result.Reverse()
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then Return result.Reverse()
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Return result
        End Function
        <Extension()> <Pure()>
        Public Function Bytes(value As UInt32,
                              Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As IRist(Of Byte)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))().Count = 4)

            Dim result = BitConverter.GetBytes(value).AsRist()
            Contract.Assume(result.Count = 4)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then Return result.Reverse()
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then Return result.Reverse()
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Return result
        End Function
        <Extension()> <Pure()>
        Public Function Bytes(value As UInt64,
                              Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As IRist(Of Byte)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))().Count = 8)

            Dim result = BitConverter.GetBytes(value).AsRist()
            Contract.Assume(result.Count = 8)
            Select Case byteOrder
                Case byteOrder.BigEndian : If BitConverter.IsLittleEndian Then Return result.Reverse()
                Case byteOrder.LittleEndian : If Not BitConverter.IsLittleEndian Then Return result.Reverse()
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select
            Return result
        End Function

        '''<summary>Determines the value of a sequence of 2 bytes treated as base-256 digits.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "ArrayLowerBound-138-0")>
        <SuppressMessage("Microsoft.Contracts", "ArrayUpperBound-138-0")>
        Public Function ToUInt16(data As IEnumerable(Of Byte),
                                 Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Const size As Integer = 2
            Contract.Requires(data IsNot Nothing)
            If data.LazyCount <> size Then Throw New ArgumentOutOfRangeException("data", "Incorrect number of bytes.")

            'Handle endian-ness
            Dim reverse = False
            Select Case byteOrder
                Case byteOrder.BigEndian : reverse = BitConverter.IsLittleEndian
                Case byteOrder.LittleEndian : reverse = Not BitConverter.IsLittleEndian
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            'Copy bytes into array
            Dim bytes(0 To size - 1) As Byte
            Dim i = 0
            For Each b In data
                bytes(If(reverse, size - i - 1, i)) = b
                i += 1
            Next b

            Return BitConverter.ToUInt16(bytes, 0)
        End Function
        '''<summary>Determines the value of a sequence of 4 bytes treated as base-256 digits.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "ArrayLowerBound-138-0")>
        <SuppressMessage("Microsoft.Contracts", "ArrayUpperBound-138-0")>
        Public Function ToUInt32(data As IEnumerable(Of Byte),
                                 Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Const size As Integer = 4
            Contract.Requires(data IsNot Nothing)
            If data.LazyCount <> size Then Throw New ArgumentOutOfRangeException("data", "Incorrect number of bytes.")

            'Handle endian-ness
            Dim reverse = False
            Select Case byteOrder
                Case byteOrder.BigEndian : reverse = BitConverter.IsLittleEndian
                Case byteOrder.LittleEndian : reverse = Not BitConverter.IsLittleEndian
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            'Copy bytes into array
            Dim bytes(0 To size - 1) As Byte
            Dim i = 0
            For Each b In data
                bytes(If(reverse, size - i - 1, i)) = b
                i += 1
            Next b

            Return BitConverter.ToUInt32(bytes, 0)
        End Function
        '''<summary>Determines the value of a sequence of 8 bytes treated as base-256 digits.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "ArrayLowerBound-138-0")>
        <SuppressMessage("Microsoft.Contracts", "ArrayUpperBound-138-0")>
        Public Function ToUInt64(data As IEnumerable(Of Byte),
                                 Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Const size As Integer = 8
            Contract.Requires(data IsNot Nothing)
            If data.LazyCount <> size Then Throw New ArgumentOutOfRangeException("data", "Incorrect number of bytes.")

            'Handle endian-ness
            Dim reverse = False
            Select Case byteOrder
                Case byteOrder.BigEndian : reverse = BitConverter.IsLittleEndian
                Case byteOrder.LittleEndian : reverse = Not BitConverter.IsLittleEndian
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            'Copy bytes into array
            Dim bytes(0 To size - 1) As Byte
            Dim i = 0
            For Each b In data
                bytes(If(reverse, size - i - 1, i)) = b
                i += 1
            Next b

            Return BitConverter.ToUInt64(bytes, 0)
        End Function

        ''' <summary>
        ''' Determines the value of a list of 2 bytes treated as base-256 digits.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function ToUInt16(data As IRist(Of Byte),
                                 Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Const size As Integer = 2
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(data.Count = size)

            'Handle endian-ness
            Dim reverse = False
            Select Case byteOrder
                Case byteOrder.BigEndian : reverse = BitConverter.IsLittleEndian
                Case byteOrder.LittleEndian : reverse = Not BitConverter.IsLittleEndian
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            'Pack bytes
            Dim bytes(0 To size - 1) As Byte
            For i = 0 To size - 1
                bytes(If(reverse, size - i - 1, i)) = data(i)
            Next i

            Return BitConverter.ToUInt16(bytes, 0)
        End Function
        ''' <summary>
        ''' Determines the value of a list of 4 bytes treated as base-256 digits.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function ToUInt32(data As IRist(Of Byte),
                                 Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Const size As Integer = 4
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(data.Count = size)

            'Handle endian-ness
            Dim reverse = False
            Select Case byteOrder
                Case byteOrder.BigEndian : reverse = BitConverter.IsLittleEndian
                Case byteOrder.LittleEndian : reverse = Not BitConverter.IsLittleEndian
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            'Pack bytes
            Dim bytes(0 To size - 1) As Byte
            For i = 0 To size - 1
                bytes(If(reverse, size - i - 1, i)) = data(i)
            Next i

            Return BitConverter.ToUInt32(bytes, 0)
        End Function
        ''' <summary>
        ''' Determines the value of a list of 8 bytes treated as base-256 digits.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function ToUInt64(data As IRist(Of Byte),
                                 Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Const size As Integer = 8
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(data.Count = size)

            'Handle endian-ness
            Dim reverse = False
            Select Case byteOrder
                Case byteOrder.BigEndian : reverse = BitConverter.IsLittleEndian
                Case byteOrder.LittleEndian : reverse = Not BitConverter.IsLittleEndian
                Case Else : Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            'Pack bytes
            Dim bytes(0 To size - 1) As Byte
            For i = 0 To size - 1
                bytes(If(reverse, size - i - 1, i)) = data(i)
            Next i

            Return BitConverter.ToUInt64(bytes, 0)
        End Function
#End Region

#Region "Bitwise Conversions"
        <Pure()> <Extension()>
        Public Function BitwiseToSByte(value As Byte) As SByte
            Dim sign = value And (CByte(1) << 7)
            If CBool(sign) Then
                Return CSByte(value And Not sign) Or (CSByte(1) << 7)
            Else
                Return CSByte(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToInt16(value As UInt16) As Int16
            Dim sign = value And (1US << 15)
            If CBool(sign) Then
                Return CShort(value And Not sign) Or (1S << 15)
            Else
                Return CShort(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToInt32(value As UInt32) As Int32
            Dim sign = value And (1UI << 31)
            If CBool(sign) Then
                Return CInt(value And Not sign) Or (1 << 31)
            Else
                Return CInt(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToInt64(value As UInt64) As Int64
            Dim sign = value And (1UL << 63)
            If CBool(sign) Then
                Return CLng(value And Not sign) Or (1L << 63)
            Else
                Return CLng(value)
            End If
        End Function

        <Pure()> <Extension()>
        Public Function BitwiseToByte(value As SByte) As Byte
            Dim sign = value And (CSByte(1) << 7)
            If CBool(sign) Then
                Return CByte(value And Not sign) Or (CByte(1) << 7)
            Else
                Return CByte(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToUInt16(value As Int16) As UInt16
            Dim sign = value And (1S << 15)
            If CBool(sign) Then
                Return CUShort(value And Not sign) Or (1US << 15)
            Else
                Return CUShort(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToUInt32(value As Int32) As UInt32
            Dim sign = value And (1 << 31)
            If CBool(sign) Then
                Return CUInt(value And Not sign) Or (1UI << 31)
            Else
                Return CUInt(value)
            End If
        End Function
        <Pure()> <Extension()>
        Public Function BitwiseToUInt64(value As Int64) As UInt64
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
        Public Function ShiftRotateLeft(value As Byte, offset As Integer) As Byte
            offset = offset.ProperMod(8)
            Return (value << offset) Or (value >> (8 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(value As UInt16, offset As Integer) As UInt16
            offset = offset.ProperMod(16)
            Return (value << offset) Or (value >> (16 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(value As UInt32, offset As Integer) As UInt32
            offset = offset.ProperMod(32)
            Return (value << offset) Or (value >> (32 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateLeft(value As UInt64, offset As Integer) As UInt64
            offset = offset.ProperMod(64)
            Return (value << offset) Or (value >> (64 - offset))
        End Function

        <Pure()> <Extension()>
        Public Function ShiftRotateRight(value As Byte, offset As Integer) As Byte
            offset = offset.ProperMod(8)
            Return (value >> offset) Or (value << (8 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateRight(value As UInt16, offset As Integer) As UInt16
            offset = offset.ProperMod(16)
            Return (value >> offset) Or (value << (16 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateRight(value As UInt32, offset As Integer) As UInt32
            offset = offset.ProperMod(32)
            Return (value >> offset) Or (value << (32 - offset))
        End Function
        <Pure()> <Extension()>
        Public Function ShiftRotateRight(value As UInt64, offset As Integer) As UInt64
            offset = offset.ProperMod(64)
            Return (value >> offset) Or (value << (64 - offset))
        End Function
#End Region

        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of T)().CompareTo(value1) >= 0")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of T)().CompareTo(value2) >= 0")>
        Public Function ClampAtOrAbove(Of T As IComparable(Of T))(value1 As T, value2 As T) As T
            Contract.Requires(value1 IsNot Nothing)
            Contract.Requires(value2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T)().CompareTo(value1) >= 0)
            Contract.Ensures(Contract.Result(Of T)().CompareTo(value2) >= 0)
            Return If(value1.CompareTo(value2) >= 0, value1, value2)
        End Function
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of T)().CompareTo(value1) <= 0")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of T)().CompareTo(value2) <= 0")>
        Public Function ClampAtOrBelow(Of T As IComparable(Of T))(value1 As T, value2 As T) As T
            Contract.Requires(value1 IsNot Nothing)
            Contract.Requires(value2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T)().CompareTo(value1) <= 0)
            Contract.Ensures(Contract.Result(Of T)().CompareTo(value2) <= 0)
            Return If(value1.CompareTo(value2) <= 0, value1, value2)
        End Function
        <Pure()> <Extension()>
        Public Function Between(Of T As IComparable(Of T))(value1 As T,
                                                           value2 As T,
                                                           value3 As T) As T
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
    End Module
End Namespace
