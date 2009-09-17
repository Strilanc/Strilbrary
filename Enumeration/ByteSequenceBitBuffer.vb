Namespace Enumeration
    Public NotInheritable Class ByteSequenceBitBuffer
        Private ReadOnly buf As New BitBuffer
        Private ReadOnly sequence As IEnumerator(Of Byte)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(buf IsNot Nothing)
            Contract.Invariant(sequence IsNot Nothing)
        End Sub

        Public Sub New(ByVal sequence As IEnumerator(Of Byte))
            Contract.Requires(sequence IsNot Nothing)
            Me.sequence = sequence
        End Sub
        Public ReadOnly Property BufferedBitCount As Integer
            Get
                Return buf.BufferedBitCount
            End Get
        End Property
        Public Function TryBufferBits(ByVal bitCount As Integer) As Boolean
            Contract.Requires(bitCount >= 0)
            While buf.BufferedBitCount < bitCount
                If Not sequence.MoveNext Then Return False
                buf.QueueByte(sequence.Current())
            End While
            Return True
        End Function
        Public Function Take(ByVal bitCount As Integer) As ULong
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= BitBuffer.MaxBits)
            If Not TryBufferBits(bitCount) Then Throw New InvalidOperationException("Ran past end of sequence.")
            Return buf.Take(bitCount)
        End Function
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
    End Class
End Namespace
