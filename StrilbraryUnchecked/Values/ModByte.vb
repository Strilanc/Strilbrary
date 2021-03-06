﻿Namespace Values
    '''<summary>A 8-bit integer which explicitly allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString()} (mod 2^8)")>
    Public Structure ModByte
        Implements IEquatable(Of ModByte)
        Private Const BitCount As Int32 = 8
        Private ReadOnly _value As Byte

        Public Sub New(value As Byte)
            Me._value = value
        End Sub
        Public Sub New(value As SByte)
            Me._value = CByte(value)
        End Sub

        <SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId:="unsigned")>
        Public ReadOnly Property UnsignedValue As Byte
            Get
                Return _value
            End Get
        End Property
        <SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId:="signed")>
        Public ReadOnly Property SignedValue As SByte
            Get
                Return CSByte(_value)
            End Get
        End Property

        Public Shared Operator *(value1 As ModByte, value2 As ModByte) As ModByte
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(value1 As ModByte, value2 As ModByte) As ModByte
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(value1 As ModByte, value2 As ModByte) As ModByte
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(value1 As ModByte, value2 As ModByte) As ModByte
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(value1 As ModByte, value2 As ModByte) As ModByte
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(value1 As ModByte, value2 As ModByte) As ModByte
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(value As ModByte) As ModByte
            Return Not value._value
        End Operator
        Public Shared Operator >>(value As ModByte, offset As Integer) As ModByte
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(value As ModByte, offset As Integer) As ModByte
            Return value._value << offset
        End Operator
        Public Shared Operator =(value1 As ModByte, value2 As ModByte) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(value1 As ModByte, value2 As ModByte) As Boolean
            Return value1._value <> value2._value
        End Operator
        <SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId:="8-offset")>
        Public Function ShiftRotateLeft(offset As Integer) As ModByte
            offset = offset And (BitCount - 1)
            Return (_value << offset) Or (_value >> (BitCount - offset))
        End Function
        <SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId:="8-offset")>
        Public Function ShiftRotateRight(offset As Integer) As ModByte
            offset = offset And (BitCount - 1)
            Return (_value >> offset) Or (_value << (BitCount - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _value.GetHashCode
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is ModByte AndAlso Me._value = DirectCast(obj, ModByte)._value
        End Function
        Public Overloads Function Equals(other As ModByte) As Boolean Implements IEquatable(Of ModByte).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Widening Operator CType(value As Byte) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Widening Operator CType(value As SByte) As ModByte
            Return New ModByte(value)
        End Operator
    End Structure
End Namespace
