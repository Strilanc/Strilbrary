Namespace Values
    '''<summary>A 32-bit integer which explicitly allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString} (mod 2^32)")>
    Public Structure ModInt32
        Implements IEquatable(Of ModInt32)
        Private Const BitCount As Int32 = 32
        Private ReadOnly _value As UInt32

        Public Sub New(ByVal value As UInt32)
            Me._value = value
        End Sub
        Public Sub New(ByVal value As Int32)
            Me._value = CUInt(value)
        End Sub

        Public ReadOnly Property UnsignedValue As UInt32
            Get
                Return _value
            End Get
        End Property
        Public ReadOnly Property SignedValue As Int32
            Get
                Return CInt(_value)
            End Get
        End Property

        Public Shared Operator *(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(ByVal value As ModInt32) As ModInt32
            Return Not value._value
        End Operator
        Public Shared Operator >>(ByVal value As ModInt32, ByVal offset As Integer) As ModInt32
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(ByVal value As ModInt32, ByVal offset As Integer) As ModInt32
            Return value._value << offset
        End Operator
        Public Shared Operator =(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As Boolean
            Return value1._value <> value2._value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModInt32
            offset = offset And (BitCount - 1)
            Return (_value << offset) Or (_value >> (BitCount - offset))
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModInt32
            offset = offset And (BitCount - 1)
            Return (_value >> offset) Or (_value << (BitCount - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _value.GetHashCode
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return TypeOf obj Is ModInt32 AndAlso Me._value = DirectCast(obj, ModInt32)._value
        End Function
        Public Overloads Function Equals(ByVal other As ModInt32) As Boolean Implements IEquatable(Of ModInt32).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Widening Operator CType(ByVal value As UInt32) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int32) As ModInt32
            Return New ModInt32(value)
        End Operator
    End Structure
End Namespace
