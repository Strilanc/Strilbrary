Namespace Values
    '''<summary>A 16-bit integer which explicitely allows overflow and underflow.</summary>
    <DebuggerDisplay("{ToString} (mod 2^16)")>
    Public Structure ModInt16
        Implements IEquatable(Of ModInt16)
        Private ReadOnly value As UInt16

#Region "Constructors"
        Public Sub New(ByVal value As UShort)
            Me.value = value
        End Sub
        Private Sub New(ByVal value As UInt32)
            Me.value = CUShort(value And UInt16.MaxValue)
        End Sub
        Private Sub New(ByVal value As UInt64)
            Me.value = CUShort(value And CULng(UInt16.MaxValue))
        End Sub

        Public Sub New(ByVal value As Int16)
            Me.value = CUShort(value + If(value < 0, &H10000, 0))
        End Sub
        Private Sub New(ByVal value As Int32)
            Me.value = CUShort(value And CInt(UInt16.MaxValue))
        End Sub
        Private Sub New(ByVal value As Int64)
            Me.value = CUShort(value And CLng(UInt16.MaxValue))
        End Sub
#End Region

#Region "Operators"
        Public Shared Operator *(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return New ModInt16(CUInt(value1.value) * CUInt(value2.value))
        End Operator
        Public Shared Operator +(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return New ModInt16(CUInt(value1.value) + CUInt(value2.value))
        End Operator
        Public Shared Operator -(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return New ModInt16(CInt(value1.value) - CInt(value2.value))
        End Operator
        Public Shared Operator And(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return New ModInt16(value1.value And value2.value)
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return New ModInt16(value1.value Xor value2.value)
        End Operator
        Public Shared Operator Or(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As ModInt16
            Return New ModInt16(value1.value Or value2.value)
        End Operator
        Public Shared Operator Not(ByVal value As ModInt16) As ModInt16
            Return New ModInt16(Not value.value)
        End Operator
        Public Shared Operator >>(ByVal value As ModInt16, ByVal offset As Integer) As ModInt16
            Return New ModInt16(value.value >> offset)
        End Operator
        Public Shared Operator <<(ByVal value As ModInt16, ByVal offset As Integer) As ModInt16
            Return New ModInt16(value.value << offset)
        End Operator
        Public Shared Operator =(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As Boolean
            Return value1.value = value2.value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModInt16, ByVal value2 As ModInt16) As Boolean
            Return value1.value <> value2.value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModInt16
            Return value.ShiftRotateLeft(offset)
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModInt16
            Return value.ShiftRotateRight(offset)
        End Function
#End Region

#Region "Methods"
        Public Overrides Function GetHashCode() As Integer
            Return (value.GetHashCode)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is ModInt16 Then Return False
            Return Me.value = CType(obj, ModInt16).value
        End Function
        Public Function EqualsProper(ByVal other As ModInt16) As Boolean Implements IEquatable(Of ModInt16).Equals
            Return Me.value = other.value
        End Function
        Public Overrides Function ToString() As String
            Return value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function
#End Region

#Region " -> ModInt16"
        Public Shared Narrowing Operator CType(ByVal value As UInt64) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As UInt32) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt16) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Byte) As ModInt16
            Return New ModInt16(value)
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As Int64) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Int32) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int16) As ModInt16
            Return New ModInt16(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As SByte) As ModInt16
            Return New ModInt16(value)
        End Operator
#End Region

#Region "ModInt16 -> "
        Public Shared Narrowing Operator CType(ByVal value As ModInt16) As Byte
            Return CByte(value.value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As UInt16
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As UInt32
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As UInt64
            Return value.value
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As ModInt16) As SByte
            Return CSByte(value.value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As Int16
            Return CShort(value.value - If(value.value > Short.MaxValue, &H10000, 0))
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As Int32
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModInt16) As Int64
            Return value.value
        End Operator
#End Region
    End Structure
End Namespace
