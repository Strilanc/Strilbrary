Namespace Values
    '''<summary>A 64-bit integer which explicitly allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString()} (mod 2^64)")>
    Public Structure ModInt64
        Implements IEquatable(Of ModInt64)
        Private Const BitCount As Int32 = 64
        Private ReadOnly _value As UInt64

        Public Sub New(value As UInt64)
            Me._value = value
        End Sub
        Public Sub New(value As Int64)
            Me._value = CULng(value)
        End Sub

        Public ReadOnly Property UnsignedValue As UInt64
            Get
                Return _value
            End Get
        End Property
        Public ReadOnly Property SignedValue As Int64
            Get
                Return CLng(_value)
            End Get
        End Property

        Public Shared Operator *(value1 As ModInt64, value2 As ModInt64) As ModInt64
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(value1 As ModInt64, value2 As ModInt64) As ModInt64
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(value1 As ModInt64, value2 As ModInt64) As ModInt64
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(value1 As ModInt64, value2 As ModInt64) As ModInt64
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(value1 As ModInt64, value2 As ModInt64) As ModInt64
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(value1 As ModInt64, value2 As ModInt64) As ModInt64
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(value As ModInt64) As ModInt64
            Return Not value._value
        End Operator
        Public Shared Operator >>(value As ModInt64, offset As Integer) As ModInt64
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(value As ModInt64, offset As Integer) As ModInt64
            Return value._value << offset
        End Operator
        Public Shared Operator =(value1 As ModInt64, value2 As ModInt64) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(value1 As ModInt64, value2 As ModInt64) As Boolean
            Return value1._value <> value2._value
        End Operator
        Public Function ShiftRotateLeft(offset As Integer) As ModInt64
            offset = offset And (BitCount - 1)
            Return (_value << offset) Or (_value >> (BitCount - offset))
        End Function
        Public Function ShiftRotateRight(offset As Integer) As ModInt64
            offset = offset And (BitCount - 1)
            Return (_value >> offset) Or (_value << (BitCount - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _value.GetHashCode
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is ModInt64 AndAlso Me._value = DirectCast(obj, ModInt64)._value
        End Function
        Public Overloads Function Equals(other As ModInt64) As Boolean Implements IEquatable(Of ModInt64).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Widening Operator CType(value As UInt64) As ModInt64
            Return New ModInt64(value)
        End Operator
        Public Shared Widening Operator CType(value As Int64) As ModInt64
            Return New ModInt64(value)
        End Operator
    End Structure
End Namespace
