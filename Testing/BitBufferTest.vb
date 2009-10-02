Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Numerics

<TestClass()>
Public Class BitBufferTest
    <TestMethod()>
    Public Sub StackTest()
        Dim bb = New BitBuffer()
        bb.Stack(1, 4)
        Assert.IsTrue(bb.BufferedBitCount = 4)
        Assert.IsTrue(bb.Peek(4) = 1)
        bb.Stack(3, 8)
        Assert.IsTrue(bb.BufferedBitCount = 12)
        Assert.IsTrue(bb.Peek(4) = 3)
        Assert.IsTrue(bb.Peek(8) = 3)
        Assert.IsTrue(bb.Peek(12) = 3 + (1 << 8))
    End Sub

    <TestMethod()>
    Public Sub QueueTest()
        Dim bb = New BitBuffer()
        bb.Queue(1, 4)
        Assert.IsTrue(bb.BufferedBitCount = 4)
        Assert.IsTrue(bb.Peek(4) = 1)
        bb.Queue(3, 8)
        Assert.IsTrue(bb.BufferedBitCount = 12)
        Assert.IsTrue(bb.Peek(4) = 1)
        Assert.IsTrue(bb.Peek(8) = 1 + (3 << 4))
        Assert.IsTrue(bb.Peek(12) = 1 + (3 << 4))
    End Sub

    <TestMethod()>
    Public Sub PeekTest()
        Dim bb = New BitBuffer()
        bb.Stack(&H123456789ABCDEF, 64)
        Assert.IsTrue(bb.Peek(4) = &HF)
        Assert.IsTrue(bb.Peek(4) = &HF)
        Assert.IsTrue(bb.Peek(8) = &HEF)
        Assert.IsTrue(bb.Peek(16) = &HCDEF)
        Assert.IsTrue(bb.Peek(32) = &H89ABCDEFUI)
        Assert.IsTrue(bb.Peek(64) = &H123456789ABCDEFUL)
    End Sub

    <TestMethod()>
    Public Sub TakeTest()
        Dim bb = New BitBuffer()
        bb.Stack(&H123456789ABCDEF, 64)
        Assert.IsTrue(bb.BufferedBitCount = 64)
        Assert.IsTrue(bb.Take(4) = &HF)
        Assert.IsTrue(bb.BufferedBitCount = 60)
        Assert.IsTrue(bb.Take(4) = &HE)
        Assert.IsTrue(bb.BufferedBitCount = 56)
        Assert.IsTrue(bb.Take(8) = &HCD)
        Assert.IsTrue(bb.BufferedBitCount = 48)
        Assert.IsTrue(bb.Take(16) = &H89AB)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.Take(32) = &H1234567)
        Assert.IsTrue(bb.BufferedBitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub ClearTest()
        Dim bb = New BitBuffer()
        bb.Stack(1, 3)
        bb.Clear()
        Assert.IsTrue(bb.BufferedBitCount = 0)
        bb.Stack(&H123456789ABCDEF, 64)
        bb.Clear()
        Assert.IsTrue(bb.BufferedBitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub BufferedByteCountTest()
        Dim bb = New BitBuffer()
        bb.Stack(1, 3)
        Assert.IsTrue(bb.BufferedByteCount = 0)
        bb.Stack(1, 3)
        Assert.IsTrue(bb.BufferedByteCount = 0)
        bb.Stack(1, 3)
        Assert.IsTrue(bb.BufferedByteCount = 1)
        bb.Stack(1, 32)
        Assert.IsTrue(bb.BufferedByteCount = 5)
    End Sub

#Region "Types"
    <TestMethod()>
    Public Sub StackBitTest()
        Dim bb = New BitBuffer()
        bb.StackBit(True)
        bb.StackBit(False)
        bb.StackBit(True)
        Assert.IsTrue(bb.BufferedBitCount = 3)
        Assert.IsTrue(bb.Peek(3) = 5UL)
        bb.StackBit(True)
        Assert.IsTrue(bb.BufferedBitCount = 4)
        Assert.IsTrue(bb.Peek(4) = 11UL)
    End Sub
    <TestMethod()>
    Public Sub QueueBitTest()
        Dim bb = New BitBuffer()
        bb.QueueBit(True)
        bb.QueueBit(False)
        bb.QueueBit(True)
        Assert.IsTrue(bb.BufferedBitCount = 3)
        Assert.IsTrue(bb.Peek(3) = 5UL)
        bb.QueueBit(True)
        Assert.IsTrue(bb.BufferedBitCount = 4)
        Assert.IsTrue(bb.Peek(4) = 13UL)
    End Sub
    <TestMethod()>
    Public Sub PeekBitTest()
        Dim bb = New BitBuffer()
        bb.Stack(1, 1)
        Assert.IsTrue(bb.PeekBit)
        bb.Stack(1, 1)
        Assert.IsTrue(bb.PeekBit)
        bb.Stack(0, 1)
        Assert.IsTrue(Not bb.PeekBit)
    End Sub
    <TestMethod()>
    Public Sub TakeBitTest()
        Dim bb = New BitBuffer()
        bb.Stack(&H4, 4)
        Assert.IsTrue(bb.BufferedBitCount = 4)
        Assert.IsTrue(Not bb.TakeBit)
        Assert.IsTrue(bb.BufferedBitCount = 3)
        Assert.IsTrue(Not bb.TakeBit)
        Assert.IsTrue(bb.BufferedBitCount = 2)
        Assert.IsTrue(bb.TakeBit)
        Assert.IsTrue(bb.BufferedBitCount = 1)
        Assert.IsTrue(Not bb.TakeBit)
        Assert.IsTrue(bb.BufferedBitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackByteTest()
        Dim bb = New BitBuffer()
        bb.StackByte(1)
        bb.StackByte(2)
        bb.StackByte(3)
        Assert.IsTrue(bb.BufferedBitCount = 24)
        Assert.IsTrue(bb.Peek(24) = &H10203UL)
        bb.StackByte(4)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.Peek(32) = &H1020304UL)
    End Sub
    <TestMethod()>
    Public Sub QueueByteTest()
        Dim bb = New BitBuffer()
        bb.QueueByte(1)
        bb.QueueByte(2)
        bb.QueueByte(3)
        Assert.IsTrue(bb.BufferedBitCount = 24)
        Assert.IsTrue(bb.Peek(24) = &H30201UL)
        bb.QueueByte(4)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.Peek(32) = &H4030201UL)
    End Sub
    <TestMethod()>
    Public Sub PeekByteTest()
        Dim bb = New BitBuffer()
        bb.Stack(1, 8)
        Assert.IsTrue(bb.PeekByte = 1)
        bb.Stack(1, 2)
        Assert.IsTrue(bb.PeekByte = 5)
        bb.Stack(0, 1)
        Assert.IsTrue(bb.PeekByte = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeByteTest()
        Dim bb = New BitBuffer()
        bb.Stack(&H12345678UI, 32)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.TakeByte = &H78)
        Assert.IsTrue(bb.BufferedBitCount = 24)
        Assert.IsTrue(bb.TakeByte = &H56)
        Assert.IsTrue(bb.BufferedBitCount = 16)
        Assert.IsTrue(bb.TakeByte = &H34)
        Assert.IsTrue(bb.BufferedBitCount = 8)
        Assert.IsTrue(bb.TakeByte = &H12)
        Assert.IsTrue(bb.BufferedBitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackUInt16Test()
        Dim bb = New BitBuffer()
        bb.StackUInt16(1)
        bb.StackUInt16(2)
        bb.StackUInt16(3)
        Assert.IsTrue(bb.BufferedBitCount = 48)
        Assert.IsTrue(bb.Peek(48) = &H100020003UL)
        bb.StackUInt16(4)
        Assert.IsTrue(bb.BufferedBitCount = 64)
        Assert.IsTrue(bb.Peek(64) = &H1000200030004UL)
    End Sub
    <TestMethod()>
    Public Sub QueueUInt16Test()
        Dim bb = New BitBuffer()
        bb.QueueUInt16(1)
        bb.QueueUInt16(2)
        bb.QueueUInt16(3)
        Assert.IsTrue(bb.BufferedBitCount = 48)
        Assert.IsTrue(bb.Peek(48) = &H300020001UL)
        bb.QueueUInt16(4)
        Assert.IsTrue(bb.BufferedBitCount = 64)
        Assert.IsTrue(bb.Peek(64) = &H4000300020001UL)
    End Sub
    <TestMethod()>
    Public Sub PeekUInt16Test()
        Dim bb = New BitBuffer()
        bb.Stack(1, 16)
        Assert.IsTrue(bb.PeekUInt16 = 1)
        bb.Stack(1, 2)
        Assert.IsTrue(bb.PeekUInt16 = 5)
        bb.Stack(0, 1)
        Assert.IsTrue(bb.PeekUInt16 = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeUInt16Test()
        Dim bb = New BitBuffer()
        bb.Stack(&H12345678UI, 32)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.TakeUInt16 = &H5678US)
        Assert.IsTrue(bb.BufferedBitCount = 16)
        Assert.IsTrue(bb.TakeUInt16 = &H1234US)
        Assert.IsTrue(bb.BufferedBitCount = 0)
    End Sub

    <TestMethod()>
    Public Sub StackUInt32Test()
        Dim bb = New BitBuffer()
        bb.StackUInt32(1)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.Peek(32) = 1)
        bb.StackUInt32(4)
        Assert.IsTrue(bb.BufferedBitCount = 64)
        Assert.IsTrue(bb.Peek(64) = &H100000004UL)
    End Sub
    <TestMethod()>
    Public Sub QueueUInt32Test()
        Dim bb = New BitBuffer()
        bb.QueueUInt32(1)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.Peek(32) = 1)
        bb.QueueUInt32(4)
        Assert.IsTrue(bb.BufferedBitCount = 64)
        Assert.IsTrue(bb.Peek(64) = &H400000001UL)
    End Sub
    <TestMethod()>
    Public Sub PeekUInt32Test()
        Dim bb = New BitBuffer()
        bb.Stack(1, 32)
        Assert.IsTrue(bb.PeekUInt32 = 1)
        bb.Stack(1, 2)
        Assert.IsTrue(bb.PeekUInt32 = 5)
        bb.Stack(0, 1)
        Assert.IsTrue(bb.PeekUInt32 = 10)
    End Sub
    <TestMethod()>
    Public Sub TakeUInt32Test()
        Dim bb = New BitBuffer()
        bb.Stack(&H123456789ABCDEFUL, 64)
        Assert.IsTrue(bb.BufferedBitCount = 64)
        Assert.IsTrue(bb.TakeUInt32 = &H89ABCDEFUI)
        Assert.IsTrue(bb.BufferedBitCount = 32)
        Assert.IsTrue(bb.TakeUInt32 = &H1234567UI)
        Assert.IsTrue(bb.BufferedBitCount = 0)
    End Sub
#End Region
End Class
