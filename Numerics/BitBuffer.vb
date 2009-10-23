Namespace Numerics
    '''<summary>Stores up to 64 bits and provides methods to add and extract bits for common types.</summary>
    Public NotInheritable Class BitBuffer
        Public Const MaxBits As Integer = 64
        Private buf As ULong 'bit storage
        Private _numBufferedBits As Integer 'number of stored bits
        Public ReadOnly Property BufferedBitCount() As Integer
            Get
                Return _numBufferedBits
            End Get
        End Property
        Public ReadOnly Property BufferedByteCount() As Integer
            Get
                Return _numBufferedBits \ 8
            End Get
        End Property

#Region "Base Operations"
        Public Sub Queue(ByVal bits As ULong, ByVal bitCount As Integer)
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= MaxBits)
            If bitCount > MaxBits - BufferedBitCount Then Throw New InvalidOperationException("Not enough capacity available.")
            buf = buf Or (bits << _numBufferedBits)
            _numBufferedBits += bitCount
        End Sub
        Public Sub Stack(ByVal bits As ULong, ByVal bitCount As Integer)
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= MaxBits)
            If bitCount > MaxBits - BufferedBitCount Then Throw New InvalidOperationException("Not enough capacity available.")
            buf <<= bitCount
            buf = buf Or bits
            _numBufferedBits += bitCount
        End Sub
        Public Function Take(ByVal bitCount As Integer) As ULong
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= MaxBits)
            Dim result = Peek(bitCount)
            buf >>= bitCount
            _numBufferedBits -= bitCount
            Return result
        End Function
        Public Function Peek(ByVal bitCount As Integer) As ULong
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= MaxBits)
            If bitCount > BufferedBitCount Then Throw New InvalidOperationException("Not enough buffered buffered bits available.")
            Dim mask = (1UL << bitCount) - 1UL
            If mask = 0 Then mask = ULong.MaxValue
            Return buf And mask
        End Function

        Public Sub Clear()
            _numBufferedBits = 0
            buf = 0
        End Sub
#End Region

#Region "Stack Types"
        Public Sub StackBit(ByVal value As Boolean)
            Stack(If(value, 1UL, 0UL), 1)
        End Sub
        Public Sub StackByte(ByVal value As Byte)
            Stack(value, 8)
        End Sub
        Public Sub StackUInt16(ByVal value As UShort)
            Stack(value, 16)
        End Sub
        Public Sub StackUInt32(ByVal value As UInteger)
            Stack(value, 32)
        End Sub
#End Region

#Region "Queue Types"
        Public Sub QueueBit(ByVal value As Boolean)
            Queue(If(value, 1UL, 0UL), 1)
        End Sub
        Public Sub QueueByte(ByVal value As Byte)
            Queue(value, 8)
        End Sub
        Public Sub QueueUInt16(ByVal value As UInt16)
            Queue(value, 16)
        End Sub
        Public Sub QueueUInt32(ByVal value As UInt32)
            Queue(value, 32)
        End Sub
#End Region

#Region "Take Types"
        Public Function TakeBit() As Boolean
            Return Take(1) <> 0
        End Function
        Public Function TakeByte() As Byte
            Return CByte(Take(8))
        End Function
        Public Function TakeUInt16() As UInt16
            Return CUShort(Take(16))
        End Function
        Public Function TakeUInt32() As UInt32
            Return CUInt(Take(32))
        End Function
#End Region

#Region "Peek Types"
        Public Function PeekBit() As Boolean
            Return Peek(1) <> 0
        End Function
        Public Function PeekByte() As Byte
            Return CByte(Peek(8))
        End Function
        Public Function PeekUInt16() As UInt16
            Return CUShort(Peek(16))
        End Function
        Public Function PeekUInt32() As UInt32
            Return CUInt(Peek(32))
        End Function
#End Region
    End Class
End Namespace