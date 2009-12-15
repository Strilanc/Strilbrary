Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Numerics

<TestClass()>
Public Class BitBufferTest
    <TestMethod()>
    Public Sub StackTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1, 4))
        Assert.IsTrue(bb.BitCount = 4)
        Assert.IsTrue(bb.Peek(4).Bits = 1)
        bb.Stack(New BitWord64(3, 8))
        Assert.IsTrue(bb.BitCount = 12)
        Assert.IsTrue(bb.Peek(4).Bits = 3)
        Assert.IsTrue(bb.Peek(8).Bits = 3)
        Assert.IsTrue(bb.Peek(12).Bits = 3 + (1 << 8))
    End Sub
    <TestMethod()>
    Public Sub QueueTest()
        Dim bb = New BitBuffer()
        bb.Queue(New BitWord64(1, 4))
        Assert.IsTrue(bb.BitCount = 4)
        Assert.IsTrue(bb.Peek(4).Bits = 1)
        bb.Queue(New BitWord64(3, 8))
        Assert.IsTrue(bb.BitCount = 12)
        Assert.IsTrue(bb.Peek(4).Bits = 1)
        Assert.IsTrue(bb.Peek(8).Bits = 1 + (3 << 4))
        Assert.IsTrue(bb.Peek(12).Bits = 1 + (3 << 4))
    End Sub
    <TestMethod()>
    Public Sub PeekTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H123456789ABCDEF, 64))
        Assert.IsTrue(bb.Peek(4).Bits = &HF)
        Assert.IsTrue(bb.Peek(4).Bits = &HF)
        Assert.IsTrue(bb.Peek(8).Bits = &HEF)
        Assert.IsTrue(bb.Peek(16).Bits = &HCDEF)
        Assert.IsTrue(bb.Peek(32).Bits = &H89ABCDEFUI)
        Assert.IsTrue(bb.Peek(64).Bits = &H123456789ABCDEFUL)
    End Sub
    <TestMethod()>
    Public Sub TakeTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H123456789ABCDEF, 64))
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Take(4).Bits = &HF)
        Assert.IsTrue(bb.BitCount = 60)
        Assert.IsTrue(bb.Take(4).Bits = &HE)
        Assert.IsTrue(bb.BitCount = 56)
        Assert.IsTrue(bb.Take(8).Bits = &HCD)
        Assert.IsTrue(bb.BitCount = 48)
        Assert.IsTrue(bb.Take(16).Bits = &H89AB)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.Take(32).Bits = &H1234567)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub
    <TestMethod()>
    Public Sub SkipTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H123456789ABCDEF, 64))
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(64).Bits = &H123456789ABCDEF)
        bb.Skip(16)
        Assert.IsTrue(bb.BitCount = 48)
        Assert.IsTrue(bb.Peek(48).Bits = &H123456789AB)
        bb.Skip(16)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.Peek(32).Bits = &H1234567)
        bb.Skip(16)
        Assert.IsTrue(bb.BitCount = 16)
        Assert.IsTrue(bb.Peek(16).Bits = &H123)
        bb.Skip(16)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub ClearTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1, 3))
        bb.Clear()
        Assert.IsTrue(bb.BitCount = 0)
        bb.Stack(New BitWord64(&H123456789ABCDEF, 64))
        bb.Clear()
        Assert.IsTrue(bb.BitCount = 0)
    End Sub

#Region "Types"
    <TestMethod()>
    Public Sub StackBitTest()
        Dim bb = New BitBuffer()
        bb.StackBit(True)
        bb.StackBit(False)
        bb.StackBit(True)
        Assert.IsTrue(bb.BitCount = 3)
        Assert.IsTrue(bb.Peek(3).Bits = 5UL)
        bb.StackBit(True)
        Assert.IsTrue(bb.BitCount = 4)
        Assert.IsTrue(bb.Peek(4).Bits = 11UL)
    End Sub
    <TestMethod()>
    Public Sub QueueBitTest()
        Dim bb = New BitBuffer()
        bb.QueueBit(True)
        bb.QueueBit(False)
        bb.QueueBit(True)
        Assert.IsTrue(bb.BitCount = 3)
        Assert.IsTrue(bb.Peek(3).Bits = 5UL)
        bb.QueueBit(True)
        Assert.IsTrue(bb.BitCount = 4)
        Assert.IsTrue(bb.Peek(4).Bits = 13UL)
    End Sub
    <TestMethod()>
    Public Sub PeekBitTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1, 1))
        Assert.IsTrue(bb.PeekBit)
        bb.Stack(New BitWord64(1, 1))
        Assert.IsTrue(bb.PeekBit)
        bb.Stack(New BitWord64(0, 1))
        Assert.IsTrue(Not bb.PeekBit)
    End Sub
    <TestMethod()>
    Public Sub TakeBitTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H4, 4))
        Assert.IsTrue(bb.BitCount = 4)
        Assert.IsTrue(Not bb.TakeBit)
        Assert.IsTrue(bb.BitCount = 3)
        Assert.IsTrue(Not bb.TakeBit)
        Assert.IsTrue(bb.BitCount = 2)
        Assert.IsTrue(bb.TakeBit)
        Assert.IsTrue(bb.BitCount = 1)
        Assert.IsTrue(Not bb.TakeBit)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackByteTest()
        Dim bb = New BitBuffer()
        bb.StackByte(1)
        bb.StackByte(2)
        bb.StackByte(3)
        Assert.IsTrue(bb.BitCount = 24)
        Assert.IsTrue(bb.Peek(24).Bits = &H10203UL)
        bb.StackByte(4)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.Peek(32).Bits = &H1020304UL)
    End Sub
    <TestMethod()>
    Public Sub QueueByteTest()
        Dim bb = New BitBuffer()
        bb.QueueByte(1)
        bb.QueueByte(2)
        bb.QueueByte(3)
        Assert.IsTrue(bb.BitCount = 24)
        Assert.IsTrue(bb.Peek(24).Bits = &H30201UL)
        bb.QueueByte(4)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.Peek(32).Bits = &H4030201UL)
    End Sub
    <TestMethod()>
    Public Sub PeekByteTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1, 8))
        Assert.IsTrue(bb.PeekByte = 1)
        bb.Stack(New BitWord64(1, 2))
        Assert.IsTrue(bb.PeekByte = 5)
        bb.Stack(New BitWord64(0, 1))
        Assert.IsTrue(bb.PeekByte = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeByteTest()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H12345678UI, 32))
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.TakeByte = &H78)
        Assert.IsTrue(bb.BitCount = 24)
        Assert.IsTrue(bb.TakeByte = &H56)
        Assert.IsTrue(bb.BitCount = 16)
        Assert.IsTrue(bb.TakeByte = &H34)
        Assert.IsTrue(bb.BitCount = 8)
        Assert.IsTrue(bb.TakeByte = &H12)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackUInt16Test()
        Dim bb = New BitBuffer()
        bb.StackUInt16(1)
        bb.StackUInt16(2)
        bb.StackUInt16(3)
        Assert.IsTrue(bb.BitCount = 48)
        Assert.IsTrue(bb.Peek(48).Bits = &H100020003UL)
        bb.StackUInt16(4)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(64).Bits = &H1000200030004UL)
    End Sub
    <TestMethod()>
    Public Sub QueueUInt16Test()
        Dim bb = New BitBuffer()
        bb.QueueUInt16(1)
        bb.QueueUInt16(2)
        bb.QueueUInt16(3)
        Assert.IsTrue(bb.BitCount = 48)
        Assert.IsTrue(bb.Peek(48).Bits = &H300020001UL)
        bb.QueueUInt16(4)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(64).Bits = &H4000300020001UL)
    End Sub
    <TestMethod()>
    Public Sub PeekUInt16Test()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1, 16))
        Assert.IsTrue(bb.PeekUInt16 = 1)
        bb.Stack(New BitWord64(1, 2))
        Assert.IsTrue(bb.PeekUInt16 = 5)
        bb.Stack(New BitWord64(0, 1))
        Assert.IsTrue(bb.PeekUInt16 = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeUInt16Test()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H12345678UI, 32))
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.TakeUInt16 = &H5678US)
        Assert.IsTrue(bb.BitCount = 16)
        Assert.IsTrue(bb.TakeUInt16 = &H1234US)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackUInt32Test()
        Dim bb = New BitBuffer()
        bb.StackUInt32(1)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.Peek(32).Bits = 1)
        bb.StackUInt32(4)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(64).Bits = &H100000004UL)
    End Sub
    <TestMethod()>
    Public Sub QueueUInt32Test()
        Dim bb = New BitBuffer()
        bb.QueueUInt32(1)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.Peek(32).Bits = 1)
        bb.QueueUInt32(4)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(64).Bits = &H400000001UL)
    End Sub
    <TestMethod()>
    Public Sub PeekUInt32Test()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1, 32))
        Assert.IsTrue(bb.PeekUInt32 = 1)
        bb.Stack(New BitWord64(1, 2))
        Assert.IsTrue(bb.PeekUInt32 = 5)
        bb.Stack(New BitWord64(0, 1))
        Assert.IsTrue(bb.PeekUInt32 = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeUInt32Test()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H123456789ABCDEFUL, 64))
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.TakeUInt32 = &H89ABCDEFUI)
        Assert.IsTrue(bb.BitCount = 32)
        Assert.IsTrue(bb.TakeUInt32 = &H1234567UI)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackUInt64Test()
        Dim bb = New BitBuffer()
        bb.StackUInt64(1)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(32).Bits = 1)
        bb.StackUInt64(4)
        Assert.IsTrue(bb.BitCount = 128)
        Assert.IsTrue(bb.Peek(64).Bits = &H4UL)
    End Sub
    <TestMethod()>
    Public Sub QueueUInt64Test()
        Dim bb = New BitBuffer()
        bb.QueueUInt64(1)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.Peek(64).Bits = 1)
        bb.QueueUInt64(4)
        Assert.IsTrue(bb.BitCount = 128)
        Assert.IsTrue(bb.Peek(64).Bits = &H1)
    End Sub
    <TestMethod()>
    Public Sub PeekUInt64Test()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(1UL, 64))
        Assert.IsTrue(bb.PeekUInt64 = 1)
        bb.Stack(New BitWord64(1UL, 2))
        Assert.IsTrue(bb.PeekUInt64 = 5)
        bb.Stack(New BitWord64(0UL, 1))
        Assert.IsTrue(bb.PeekUInt64 = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeUInt64Test()
        Dim bb = New BitBuffer()
        bb.Stack(New BitWord64(&H123456789ABCDEFUL, 64))
        bb.Stack(New BitWord64(&H213456789ABCDEFUL, 64))
        Assert.IsTrue(bb.BitCount = 128)
        Assert.IsTrue(bb.TakeUInt64 = &H213456789ABCDEFUL)
        Assert.IsTrue(bb.BitCount = 64)
        Assert.IsTrue(bb.TakeUInt64 = &H123456789ABCDEFUL)
        Assert.IsTrue(bb.BitCount = 0)
    End Sub
#End Region
End Class
