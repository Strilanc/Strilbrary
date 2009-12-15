Namespace Values
    '''<summary>A 32-bit integer which explicitely allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString} (mod 2^32)")>
    Public Structure ModInt32
        Implements IEquatable(Of ModInt32)
        Private ReadOnly value As UInt32

#Region "Constructors"
        Public Sub New(ByVal value As UInt32)
            Me.value = value
        End Sub
        Private Sub New(ByVal value As UInt64)
            Me.value = CUInt(value And CULng(UInt32.MaxValue))
        End Sub

        Public Sub New(ByVal value As Int32)
            Me.value = CUInt(value + If(value < 0, &H100000000L, 0))
        End Sub
        Private Sub New(ByVal value As Int64)
            Me.value = CUInt(value And CLng(UInt32.MaxValue))
        End Sub
#End Region

#Region "Operators"
        Public Shared Operator *(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return New ModInt32(CULng(value1.value) * CULng(value2.value))
        End Operator
        Public Shared Operator +(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return New ModInt32(CULng(value1.value) + CULng(value2.value))
        End Operator
        Public Shared Operator -(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return New ModInt32(CLng(value1.value) - CLng(value2.value))
        End Operator
        Public Shared Operator And(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return New ModInt32(value1.value And value2.value)
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return New ModInt32(value1.value Xor value2.value)
        End Operator
        Public Shared Operator Or(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As ModInt32
            Return New ModInt32(value1.value Or value2.value)
        End Operator
        Public Shared Operator Not(ByVal value As ModInt32) As ModInt32
            Return New ModInt32(Not value.value)
        End Operator
        Public Shared Operator >>(ByVal value As ModInt32, ByVal offset As Integer) As ModInt32
            Return New ModInt32(value.value >> offset)
        End Operator
        Public Shared Operator <<(ByVal value As ModInt32, ByVal offset As Integer) As ModInt32
            Return New ModInt32(value.value << offset)
        End Operator
        Public Shared Operator =(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As Boolean
            Return value1.value = value2.value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModInt32, ByVal value2 As ModInt32) As Boolean
            Return value1.value <> value2.value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModInt32
            Return value.ShiftRotateLeft(offset)
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModInt32
            Return value.ShiftRotateRight(offset)
        End Function
#End Region

#Region "Methods"
        Public Overrides Function GetHashCode() As Integer
            Return (value.GetHashCode)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is ModInt32 Then Return False
            Return Me.value = CType(obj, ModInt32).value
        End Function
        Public Function EqualsProper(ByVal other As ModInt32) As Boolean Implements IEquatable(Of ModInt32).Equals
            Return Me.value = other.value
        End Function
        Public Overrides Function ToString() As String
            Return value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function
#End Region

#Region " -> ModInt32"
        Public Shared Narrowing Operator CType(ByVal value As UInt64) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt32) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt16) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Byte) As ModInt32
            Return New ModInt32(value)
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As Int64) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int32) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int16) As ModInt32
            Return New ModInt32(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As SByte) As ModInt32
            Return New ModInt32(value)
        End Operator
#End Region

#Region "ModInt32 -> "
        Public Shared Narrowing Operator CType(ByVal value As ModInt32) As Byte
            Return CByte(value.value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As ModInt32) As UInt16
            Return CUShort(value.value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt32) As UInt32
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt32) As UInt64
            Return value.value
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As ModInt32) As SByte
            Return CSByte(value.value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As ModInt32) As Int16
            Return CShort(value.value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt32) As Int32
            Return CInt(value.value - If(value.value > Int32.MaxValue, &H100000000L, 0))
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt32) As Int64
            Return value.value
        End Operator
#End Region
    End Structure
End Namespace
