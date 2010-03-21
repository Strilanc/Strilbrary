Namespace Values
    '''<summary>Stores bits and provides methods to add and remove the bits of common numeric types.</summary>
    <DebuggerDisplay("{ToString}")>
    Public NotInheritable Class BitBuffer
        Private _words As New LinkedList(Of BitWord64)()
        Private _bitCount As Integer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_bitCount >= 0)
            Contract.Invariant(_words IsNot Nothing)
        End Sub

        '''<summary>Determines the number of bits currently stored in the BitBuffer.</summary>
        Public ReadOnly Property BitCount() As Integer
            Get
                Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                Contract.Ensures(Contract.Result(Of Integer)() = Me._bitCount)
                Return _bitCount
            End Get
        End Property

#Region "Base Operations"
        Public Sub Queue(ByVal word As BitWord64)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + word.BitCount)

            Dim tail = word
            If _words.Count > 0 AndAlso _words.Last.Value.BitCount + tail.BitCount <= BitWord64.MaxSize Then
                tail = _words.Last.Value + tail
                _words.RemoveLast()
            End If
            _words.AddLast(tail)
            _bitCount += word.BitCount
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub Stack(ByVal word As BitWord64)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + word.BitCount)
            Contract.Ensures(Me.Peek(word.BitCount) = word)

            Dim head = word
            If _words.Count > 0 AndAlso _words.First.Value.BitCount + head.BitCount <= BitWord64.MaxSize Then
                head += _words.First.Value
                _words.RemoveFirst()
            End If

            _words.AddFirst(head)
            _bitCount += word.BitCount
        End Sub
        Public Sub Skip(ByVal skippedBitCount As Integer)
            Contract.Requires(skippedBitCount >= 0)
            Contract.Requires(skippedBitCount <= Me.BitCount)
            Contract.Requires(skippedBitCount <= BitWord64.MaxSize)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - skippedBitCount)

            Dim remainingBits = skippedBitCount
            While remainingBits > 0
                Dim n = _words.First
                Contract.Assume(n IsNot Nothing)
                If n.Value.BitCount > remainingBits Then
                    Dim remainder = n.Value.HighPart(splitIndex:=remainingBits)
                    remainingBits = 0
                    _words.RemoveFirst()
                    _words.AddFirst(remainder)
                Else
                    remainingBits -= n.Value.BitCount
                    _words.RemoveFirst()
                End If
            End While

            _bitCount -= skippedBitCount
        End Sub
        Public Function Take(ByVal resultBitCount As Integer) As BitWord64
            Contract.Requires(resultBitCount >= 0)
            Contract.Requires(resultBitCount <= Me.BitCount)
            Contract.Requires(resultBitCount <= BitWord64.MaxSize)
            Contract.Ensures(Contract.Result(Of BitWord64)() = Contract.OldValue(Me.Peek(resultBitCount)))
            Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = resultBitCount)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - resultBitCount)

            Dim result = Peek(resultBitCount)
            Skip(resultBitCount)
            Return result
        End Function
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        <Pure()>
        Public Function Peek(ByVal resultBitCount As Integer) As BitWord64
            Contract.Requires(resultBitCount >= 0)
            Contract.Requires(resultBitCount <= Me.BitCount)
            Contract.Requires(resultBitCount <= BitWord64.MaxSize)
            Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = resultBitCount)

            Dim n = _words.First
            Dim result = New BitWord64(0, 0)
            While result.BitCount < resultBitCount
                Contract.Assume(n IsNot Nothing)
                If result.BitCount + n.Value.BitCount > resultBitCount Then
                    result += n.Value.LowPart(splitIndex:=resultBitCount - result.BitCount)
                Else
                    result += n.Value
                End If
                n = n.Next
                Contract.Assert(result.BitCount <= resultBitCount)
            End While
            Return result
        End Function

        Public Sub Clear()
            Contract.Ensures(Me.BitCount = 0)
            _bitCount = 0
            _words.Clear()
        End Sub
#End Region

#Region "Derived Operations"
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub StackBit(ByVal value As Boolean)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 1)
            Stack(New BitWord64(If(value, 1UL, 0UL), 1))
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub StackByte(ByVal value As Byte)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 8)
            Stack(New BitWord64(value, 8))
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub StackUInt16(ByVal value As UShort)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 16)
            Stack(New BitWord64(value, 16))
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub StackUInt32(ByVal value As UInteger)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 32)
            Stack(New BitWord64(value, 32))
        End Sub
        Public Sub StackUInt64(ByVal value As UInt64)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 64)
            Stack(New BitWord64(value, 64))
        End Sub

        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub QueueBit(ByVal value As Boolean)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 1)
            Queue(New BitWord64(If(value, 1UL, 0UL), 1))
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub QueueByte(ByVal value As Byte)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 8)
            Queue(New BitWord64(value, 8))
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub QueueUInt16(ByVal value As UInt16)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 16)
            Queue(New BitWord64(value, 16))
        End Sub
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub QueueUInt32(ByVal value As UInt32)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 32)
            Queue(New BitWord64(value, 32))
        End Sub
        Public Sub QueueUInt64(ByVal value As UInt64)
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) + 64)
            Queue(New BitWord64(value, 64))
        End Sub

        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Function TakeBit() As Boolean
            Contract.Requires(Me.BitCount >= 1)
            Contract.Ensures(Contract.Result(Of Boolean)() = Contract.OldValue(Me.PeekBit))
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - 1)
            Return Take(1).Bits <> 0
        End Function
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Function TakeByte() As Byte
            Contract.Requires(Me.BitCount >= 8)
            Contract.Ensures(Contract.Result(Of Byte)() = Contract.OldValue(Me.PeekByte))
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - 8)
            Return CByte(Take(8).Bits)
        End Function
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Function TakeUInt16() As UInt16
            Contract.Requires(Me.BitCount >= 16)
            Contract.Ensures(Contract.Result(Of UInt16)() = Contract.OldValue(Me.PeekUInt16))
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - 16)
            Return CUShort(Take(16).Bits)
        End Function
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Function TakeUInt32() As UInt32
            Contract.Requires(Me.BitCount >= 32)
            Contract.Ensures(Contract.Result(Of UInt32)() = Contract.OldValue(Me.PeekUInt32))
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - 32)
            Return CUInt(Take(32).Bits)
        End Function
        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Function TakeUInt64() As UInt64
            Contract.Requires(Me.BitCount >= 64)
            Contract.Ensures(Contract.Result(Of UInt64)() = Contract.OldValue(Me.PeekUInt64))
            Contract.Ensures(Me.BitCount = Contract.OldValue(Me.BitCount) - 64)
            Return Take(64).Bits
        End Function

        <Pure()>
        Public Function PeekBit() As Boolean
            Contract.Requires(Me.BitCount >= 1)
            Return Peek(1).Bits <> 0
        End Function
        <Pure()>
        Public Function PeekByte() As Byte
            Contract.Requires(Me.BitCount >= 8)
            Return CByte(Peek(8).Bits)
        End Function
        <Pure()>
        Public Function PeekUInt16() As UInt16
            Contract.Requires(Me.BitCount >= 16)
            Return CUShort(Peek(16).Bits)
        End Function
        <Pure()>
        Public Function PeekUInt32() As UInt32
            Contract.Requires(Me.BitCount >= 32)
            Return CUInt(Peek(32).Bits)
        End Function
        <Pure()>
        Public Function PeekUInt64() As UInt64
            Contract.Requires(Me.BitCount >= 64)
            Return Peek(64).Bits
        End Function
#End Region

        Public Overrides Function ToString() As String
            If Me.BitCount > BitWord64.MaxSize Then
                Return "{0}: {1}...".Frmt(BitCount, Peek(BitWord64.MaxSize))
            Else
                Return "{0}: {1}".Frmt(BitCount, Peek(BitCount))
            End If
        End Function
    End Class
End Namespace
