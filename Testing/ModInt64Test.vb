Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class ModInt64Test
    <TestMethod()>
    Public Sub op_SubtractionTest()
        Assert.IsTrue(New ModInt64(0) - 1 = UInt64.MaxValue)
        Assert.IsTrue(New ModInt64(UInt64.MaxValue) - -1 = 0)
        Assert.IsTrue(New ModInt64(Int64.MaxValue) - -1 = Int64.MinValue)
        Assert.IsTrue(New ModInt64(Int64.MinValue) - 1 = Int64.MaxValue)
        Assert.IsTrue(New ModInt64(1) - 1 = 0)
        Assert.IsTrue(New ModInt64(3) - 2 = 1)
    End Sub

    <TestMethod()>
    Public Sub op_ShiftTest()
        Assert.IsTrue(New ModInt64(0) << 12 = 0)
        Assert.IsTrue(New ModInt64(3) << 2 = 12)
        Assert.IsTrue(New ModInt64(3) >> 2 = 0)
        Assert.IsTrue(New ModInt64(3) >> 1 = 1)
        Assert.IsTrue(New ModInt64(&HFFFFFFFFFFFFFFFFUL) >> 2 = &H3FFFFFFFFFFFFFFFUL)
    End Sub

    <TestMethod()>
    Public Sub op_NotTest()
        Assert.IsTrue(Not New ModInt64(0) = &HFFFFFFFFFFFFFFFFUL)
        Assert.IsTrue(Not New ModInt64(1) = &HFFFFFFFFFFFFFFFEUL)
        Assert.IsTrue(Not New ModInt64(-1) = 0)
        Assert.IsTrue(Not New ModInt64(UInt64.MaxValue) = 0)
    End Sub

    <TestMethod()>
    Public Sub op_MultiplyTest()
        Assert.IsTrue(New ModInt64(&HFFFFFFFFFFFFFFFFUL) * 2 = &HFFFFFFFFFFFFFFFEUL)
        Assert.IsTrue(New ModInt64(3) * -2 = -6)
    End Sub

    <TestMethod()>
    Public Sub op_EqualityTest()
        Assert.IsTrue(New ModInt64(UInt64.MaxValue) = UInt64.MaxValue)
        Assert.IsTrue(New ModInt64(-1) = UInt64.MaxValue)
        Assert.IsTrue(New ModInt64(UInt64.MaxValue) = -1)
        Assert.IsTrue(New ModInt64(-1) = -1)
        Assert.IsTrue(New ModInt64(1) = 1)
        Assert.IsTrue(Not New ModInt64(1) = 2)
        Assert.IsTrue(New ModInt64(2) <> 1)
        Assert.IsTrue(New ModInt64(2) <> -1)
        Assert.IsTrue(New ModInt64(UInt64.MaxValue).Equals(New ModInt64(-1)))
    End Sub

    <TestMethod()>
    Public Sub op_Conversion()
        Assert.IsTrue(CULng(CType(&H100000005UL, ModInt64)) = &H100000005UL)
        Assert.IsTrue(CULng(CType(5, ModInt64)) = 5)
        Assert.IsTrue(CULng(CType(-1, ModInt64)) = UInt64.MaxValue)
        Assert.IsTrue(CLng(CType(UInt64.MaxValue, ModInt64)) = -1)
        Assert.IsTrue(CLng(CType(UInt32.MaxValue, ModInt64)) = UInt32.MaxValue)
        Assert.IsTrue(CLng(CType(4, ModInt64)) = 4)
    End Sub

    <TestMethod()>
    Public Sub op_ExclusiveOrTest()
        Assert.IsTrue((New ModInt64(1) Xor 1) = 0)
        Assert.IsTrue((New ModInt64(2) Xor 1) = 3)
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseOrTest()
        Assert.IsTrue((New ModInt64(1) Or 1) = 1)
        Assert.IsTrue((New ModInt64(2) Or 1) = 3)
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseAndTest()
        Assert.IsTrue((New ModInt64(1) And 1) = 1)
        Assert.IsTrue((New ModInt64(2) And 1) = 0)
    End Sub

    <TestMethod()>
    Public Sub op_AdditionTest()
        Assert.IsTrue(New ModInt64(1) + 1 = 2)
        Assert.IsTrue(New ModInt64(UInt64.MaxValue) + 1 = 0)
        Assert.IsTrue(New ModInt64(-1) + 1 = 0)
        Assert.IsTrue(New ModInt64(Int64.MaxValue) + 1 = Int64.MinValue)
        Assert.IsTrue(New ModInt64(Int64.MinValue) + -1 = Int64.MaxValue)
    End Sub

    <TestMethod()>
    Public Sub ShiftRotateTest()
        Assert.IsTrue(New ModInt64(2).ShiftRotateLeft(63) = 1)
        Assert.IsTrue(New ModInt64(0).ShiftRotateLeft(12) = 0)
        Assert.IsTrue(New ModInt64(3).ShiftRotateLeft(2) = 12)
        Assert.IsTrue(New ModInt64(3).ShiftRotateRight(2) = &HC000000000000000UL)
        Assert.IsTrue(New ModInt64(3).ShiftRotateRight(1) = &H8000000000000001UL)
        Assert.IsTrue(New ModInt64(&HFFFFFFFFFFFFFFFFUL).ShiftRotateRight(2) = &HFFFFFFFFFFFFFFFFUL)
    End Sub
End Class
