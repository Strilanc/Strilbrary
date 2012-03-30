Namespace Values
    '''<summary>A 32-bit integer which explicitly allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString()} (mod 2^32)")>
    Public Structure ModInt32
        Implements IEquatable(Of ModInt32)
        Private Const BitCount As Int32 = 32
        Private ReadOnly _value As UInt32

        Public Sub New(value As UInt32)
            Me._value = value
        End Sub
        Public Sub New(value As Int32)
            Me._value = CUInt(value)
        End Sub

        <SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId:="unsigned")>
        Public ReadOnly Property UnsignedValue As UInt32
            Get
                Return _value
            End Get
        End Property
        <SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId:="signed")>
        Public ReadOnly Property SignedValue As Int32
            Get
                Return CInt(_value)
            End Get
        End Property

        Public Shared Operator *(value1 As ModInt32, value2 As ModInt32) As ModInt32
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(value1 As ModInt32, value2 As ModInt32) As ModInt32
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(value1 As ModInt32, value2 As ModInt32) As ModInt32
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(value1 As ModInt32, value2 As ModInt32) As ModInt32
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(value1 As ModInt32, value2 As ModInt32) As ModInt32
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(value1 As ModInt32, value2 As ModInt32) As ModInt32
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(value As ModInt32) As ModInt32
            Return Not value._value
        End Operator
        Public Shared Operator >>(value As ModInt32, offset As Integer) As ModInt32
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(value As ModInt32, offset As Integer) As ModInt32
            Return value._value << offset
        End Operator
        Public Shared Operator =(value1 As ModInt32, value2 As ModInt32) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(value1 As ModInt32, value2 As ModInt32) As Boolean
            Return value1._value <> value2._value
        End Operator
        <SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId:="32-offset")>
        Public Function ShiftRotateLeft(offset As Integer) As ModInt32
            offset = offset And (BitCount - 1)
            Return (_value << offset) Or (_value >> (BitCount - offset))
        End Function
        <SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId:="32-offset")>
        Public Function ShiftRotateRight(offset As Integer) As ModInt32
            offset = offset And (BitCount - 1)
            Return (_value >> offset) Or (_value << (BitCount - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _value.GetHashCode
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is ModInt32 AndAlso Me._value = DirectCast(obj, ModInt32)._value
        End Function
        Public Overloads Function Equals(other As ModInt32) As Boolean Implements IEquatable(Of ModInt32).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Widening Operator CType(value As UInt32) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(value As Int32) As ModInt32
            Return New ModInt32(value)
        End Operator
    End Structure
End Namespace
