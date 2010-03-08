Namespace Values
    '''<summary>A 8-bit integer which explicitely allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString} (mod 2^8)")>
    Public Structure ModByte
        Implements IEquatable(Of ModByte)
        Private ReadOnly _value As Byte

        Public Sub New(ByVal value As Byte)
            Me._value = value
        End Sub
        Public Sub New(ByVal value As SByte)
            Me._value = CByte(value)
        End Sub

        Public Shared Operator *(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(ByVal value As ModByte) As ModByte
            Return Not value._value
        End Operator
        Public Shared Operator >>(ByVal value As ModByte, ByVal offset As Integer) As ModByte
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(ByVal value As ModByte, ByVal offset As Integer) As ModByte
            Return value._value << offset
        End Operator
        Public Shared Operator =(ByVal value1 As ModByte, ByVal value2 As ModByte) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModByte, ByVal value2 As ModByte) As Boolean
            Return value1._value <> value2._value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModByte
            offset = offset And (8 - 1)
            Return (_value << offset) Or (_value >> (8 - offset))
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModByte
            offset = offset And (8 - 1)
            Return (_value >> offset) Or (_value << (8 - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (_value.GetHashCode)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is ModByte Then Return False
            Return Me._value = CType(obj, ModByte)._value
        End Function
        Public Overloads Function Equals(ByVal other As ModByte) As Boolean Implements IEquatable(Of ModByte).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Narrowing Operator CType(ByVal value As UInt64) As ModByte
            Return New ModByte(CByte(value))
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As UInt32) As ModByte
            Return New ModByte(CByte(value))
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As UInt16) As ModByte
            Return New ModByte(CByte(value))
        End Operator
        Public Shared Widening Operator CType(ByVal value As Byte) As ModByte
            Return New ModByte(value)
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As Int64) As ModByte
            Return New ModByte(CByte(value))
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Int32) As ModByte
            Return New ModByte(CByte(value))
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Int16) As ModByte
            Return New ModByte(CByte(value))
        End Operator
        Public Shared Widening Operator CType(ByVal value As SByte) As ModByte
            Return New ModByte(value)
        End Operator

        Public Shared Widening Operator CType(ByVal value As ModByte) As Byte
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As UInt16
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As UInt32
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As UInt64
            Return value._value
        End Operator

        Public Shared Widening Operator CType(ByVal value As ModByte) As SByte
            Return CSByte(value._value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As Int16
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As Int32
            Return value._value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As Int64
            Return value._value
        End Operator
    End Structure
End Namespace
