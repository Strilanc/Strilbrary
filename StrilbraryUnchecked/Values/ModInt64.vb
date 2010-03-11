Namespace Values
    '''<summary>A 64-bit integer which explicitly allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString} (mod 2^64)")>
    Public Structure ModInt64
        Implements IEquatable(Of ModInt64)
        Private ReadOnly _value As UInt64

        Public Sub New(ByVal value As UInt64)
            Me._value = value
        End Sub
        Public Sub New(ByVal value As Int64)
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

        Public Shared Operator *(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As ModInt64
            Return value1._value * value2._value
        End Operator
        Public Shared Operator +(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As ModInt64
            Return value1._value + value2._value
        End Operator
        Public Shared Operator -(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As ModInt64
            Return value1._value - value2._value
        End Operator
        Public Shared Operator And(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As ModInt64
            Return value1._value And value2._value
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As ModInt64
            Return value1._value Xor value2._value
        End Operator
        Public Shared Operator Or(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As ModInt64
            Return value1._value Or value2._value
        End Operator
        Public Shared Operator Not(ByVal value As ModInt64) As ModInt64
            Return Not value._value
        End Operator
        Public Shared Operator >>(ByVal value As ModInt64, ByVal offset As Integer) As ModInt64
            Return value._value >> offset
        End Operator
        Public Shared Operator <<(ByVal value As ModInt64, ByVal offset As Integer) As ModInt64
            Return value._value << offset
        End Operator
        Public Shared Operator =(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As Boolean
            Return value1._value = value2._value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModInt64, ByVal value2 As ModInt64) As Boolean
            Return value1._value <> value2._value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModInt64
            offset = offset And (64 - 1)
            Return (_value << offset) Or (_value >> (64 - offset))
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModInt64
            offset = offset And (64 - 1)
            Return (_value >> offset) Or (_value << (64 - offset))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (_value.GetHashCode)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is ModInt64 Then Return False
            Return Me._value = CType(obj, ModInt64)._value
        End Function
        Public Overloads Function Equals(ByVal other As ModInt64) As Boolean Implements IEquatable(Of ModInt64).Equals
            Return Me._value = other._value
        End Function
        Public Overrides Function ToString() As String
            Return _value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Shared Widening Operator CType(ByVal value As UInt64) As ModInt64
            Return New ModInt64(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int64) As ModInt64
            Return New ModInt64(value)
        End Operator
    End Structure
End Namespace
