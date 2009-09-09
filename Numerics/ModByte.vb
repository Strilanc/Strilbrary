Namespace Numerics
    <DebuggerDisplay("{ToString} (mod 2^8)")>
    Public Structure ModByte
        Implements IEquatable(Of ModByte)
        Private ReadOnly value As Byte

#Region "Constructors"
        Private Sub New(ByVal value As Byte)
            Me.value = value
        End Sub
        Private Sub New(ByVal value As UInt16)
            Me.value = CByte(value And Byte.MaxValue)
        End Sub
        Private Sub New(ByVal value As UInt32)
            Me.value = CByte(value And Byte.MaxValue)
        End Sub
        Private Sub New(ByVal value As UInt64)
            Me.value = CByte(value And Byte.MaxValue)
        End Sub

        Private Sub New(ByVal value As SByte)
            Me.value = CByte(value + If(value < 0, &H100, 0))
        End Sub
        Private Sub New(ByVal value As Int16)
            Me.value = CByte(value And Byte.MaxValue)
        End Sub
        Private Sub New(ByVal value As Int32)
            Me.value = CByte(value And Byte.MaxValue)
        End Sub
        Private Sub New(ByVal value As Int64)
            Me.value = CByte(value And Byte.MaxValue)
        End Sub
#End Region

#Region "Operators"
        Public Shared Operator *(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return New ModByte(CUShort(value1.value) * CUShort(value2.value))
        End Operator
        Public Shared Operator +(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return New ModByte(CUShort(value1.value) + CUShort(value2.value))
        End Operator
        Public Shared Operator -(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return New ModByte(CShort(value1.value) - CShort(value2.value))
        End Operator
        Public Shared Operator And(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return New ModByte(value1.value And value2.value)
        End Operator
        Public Shared Operator Xor(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return New ModByte(value1.value Xor value2.value)
        End Operator
        Public Shared Operator Or(ByVal value1 As ModByte, ByVal value2 As ModByte) As ModByte
            Return New ModByte(value1.value Or value2.value)
        End Operator
        Public Shared Operator Not(ByVal value As ModByte) As ModByte
            Return New ModByte(Not value.value)
        End Operator
        Public Shared Operator >>(ByVal value As ModByte, ByVal offset As Integer) As ModByte
            Return New ModByte(value.value >> offset)
        End Operator
        Public Shared Operator <<(ByVal value As ModByte, ByVal offset As Integer) As ModByte
            Return New ModByte(value.value << offset)
        End Operator
        Public Shared Operator =(ByVal value1 As ModByte, ByVal value2 As ModByte) As Boolean
            Return value1.value = value2.value
        End Operator
        Public Shared Operator <>(ByVal value1 As ModByte, ByVal value2 As ModByte) As Boolean
            Return value1.value <> value2.value
        End Operator
        Public Function ShiftRotateLeft(ByVal offset As Integer) As ModByte
            offset = offset And &H7
            Return New ModByte((value << offset) Or (value >> 8 - offset))
        End Function
        Public Function ShiftRotateRight(ByVal offset As Integer) As ModByte
            offset = offset And &H7
            Return New ModByte((value >> offset) Or (value << 8 - offset))
        End Function
#End Region

#Region "Methods"
        Public Overrides Function GetHashCode() As Integer
            Return (value.GetHashCode)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is ModByte Then Return False
            Return Me.value = CType(obj, ModByte).value
        End Function
        Public Function EqualsProper(ByVal other As ModByte) As Boolean Implements IEquatable(Of ModByte).Equals
            Return Me.value = other.value
        End Function
#End Region

#Region " -> ModByte"
        Public Shared Narrowing Operator CType(ByVal value As UInt64) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As UInt32) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As UInt16) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Byte) As ModByte
            Return New ModByte(value)
        End Operator

        Public Shared Narrowing Operator CType(ByVal value As Int64) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Int32) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Int16) As ModByte
            Return New ModByte(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As SByte) As ModByte
            Return New ModByte(value)
        End Operator
#End Region

#Region "ModByte -> "
        Public Shared Widening Operator CType(ByVal value As ModByte) As Byte
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As UInt16
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As UInt32
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As UInt64
            Return value.value
        End Operator

        Public Shared Widening Operator CType(ByVal value As ModByte) As SByte
            Return CSByte(value.value - If(value.value > SByte.MaxValue, &H100, 0))
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As Int16
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As Int32
            Return value.value
        End Operator
        Public Shared Widening Operator CType(ByVal value As ModByte) As Int64
            Return value.value
        End Operator
#End Region
    End Structure
End Namespace
