Namespace Values
    '''<summary>A 16-bit integer which explicitely allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString} (mod 2^16)")>
    Public Structure ModInt16
        Implements IEquatable(Of ModInt16)
        Private ReadOnly _value As UInt16

        Public Sub New(ByVal value As UInt16)
            Me._value = value
        End Sub
        Public Sub New(ByVal value As Int16)
            Me._value = CUShort(value)
        End Sub

        Public Shared Operator *(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(ByVal value As ModInt16) As ModInt16
            Return Not value._value
        End Operator
        Public Shared Operator >>(ByVal value As ModInt16, ByVal offset As Integer) As ModInt16
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(ByVal value As ModInt16, ByVal offset As Integer) As ModInt16
            Return value._value << offset
        End Operator
        Public Shared Operator =(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As Boolean
            Return value1._value <> value2._value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModInt16
            offset = offset And (16 - 1)
            Return (_value << offset) Or (_value >> (16 - offset))
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModInt16
            offset = offset And (16 - 1)
            Return (_value >> offset) Or (_value << (16 - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (_value.GetHashCode)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is ModInt16 Then Return False
            Return Me._value = CType(obj, ModInt16)._value
        End Function
        Public Overloads Function Equals(ByVal other As ModInt16) As Boolean Implements IEquatable(Of ModInt16).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Narrowing Operator CType(ByVal value As UInt64) As ModInt16
            Return New ModInt16(CUShort(value))
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As UInt32) As ModInt16
            Return New ModInt16(CUShort(value))
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt16) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Byte) As ModInt16
            Return New ModInt16(value)
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As Int64) As ModInt16
            Return New ModInt16(CUShort(value))
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Int32) As ModInt16
            Return New ModInt16(CUShort(value))
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int16) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As SByte) As ModInt16
            Return New ModInt16(value)
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As ModInt16) As Byte
            Return CByte(value._value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As UInt16
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As UInt32
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As UInt64
            Return value._value
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As ModInt16) As SByte
            Return CSByte(value._value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As Int16
            Return CShort(value._value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As Int32
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As Int64
            Return value._value
        End Operator
    End Structure
End Namespace
