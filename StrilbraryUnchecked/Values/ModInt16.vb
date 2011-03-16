Namespace Values
    '''<summary>A 16-bit integer which explicitly allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString()} (mod 2^16)")>
    Public Structure ModInt16
        Implements IEquatable(Of ModInt16)
        Private Const BitCount As Int32 = 16
        Private ReadOnly _value As UInt16

        Public Sub New(value As UInt16)
            Me._value = value
        End Sub
        Public Sub New(value As Int16)
            Me._value = CUShort(value)
        End Sub

        Public ReadOnly Property UnsignedValue As UInt16
            Get
                Return _value
            End Get
        End Property
        Public ReadOnly Property SignedValue As Int16
            Get
                Return CShort(_value)
            End Get
        End Property

        Public Shared Operator *(value1 As ModInt16, value2 As ModInt16) As ModInt16
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(value1 As ModInt16, value2 As ModInt16) As ModInt16
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(value1 As ModInt16, value2 As ModInt16) As ModInt16
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(value1 As ModInt16, value2 As ModInt16) As ModInt16
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(value1 As ModInt16, value2 As ModInt16) As ModInt16
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(value1 As ModInt16, value2 As ModInt16) As ModInt16
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(value As ModInt16) As ModInt16
            Return Not value._value
        End Operator
        Public Shared Operator >>(value As ModInt16, offset As Integer) As ModInt16
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(value As ModInt16, offset As Integer) As ModInt16
            Return value._value << offset
        End Operator
        Public Shared Operator =(value1 As ModInt16, value2 As ModInt16) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(value1 As ModInt16, value2 As ModInt16) As Boolean
            Return value1._value <> value2._value
        End Operator
        Public Function ShiftRotateLeft(offset As Integer) As ModInt16
            offset = offset And (BitCount - 1)
            Return (_value << offset) Or (_value >> (BitCount - offset))
        End Function
        Public Function ShiftRotateRight(offset As Integer) As ModInt16
            offset = offset And (BitCount - 1)
            Return (_value >> offset) Or (_value << (BitCount - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _value.GetHashCode
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is ModInt16 AndAlso Me._value = DirectCast(obj, ModInt16)._value
        End Function
        Public Overloads Function Equals(other As ModInt16) As Boolean Implements IEquatable(Of ModInt16).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Widening Operator CType(value As UInt16) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(value As Int16) As ModInt16
            Return New ModInt16(value)
        End Operator
    End Structure
End Namespace
