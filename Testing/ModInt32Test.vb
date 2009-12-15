Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class ModInt32Test
    <TestMethod()>
    Public Sub op_SubtractionTest()
        Assert.IsTrue(New ModInt32(0) - 1 = UInteger.MaxValue)
        Assert.IsTrue(New ModInt32(UInteger.MaxValue) - -1 = 0)
        Assert.IsTrue(New ModInt32(Integer.MaxValue) - -1 = Integer.MinValue)
        Assert.IsTrue(New ModInt32(Integer.MinValue) - 1 = Integer.MaxValue)
        Assert.IsTrue(New ModInt32(1) - 1 = 0)
        Assert.IsTrue(New ModInt32(3) - 2 = 1)
    End Sub

    <TestMethod()>
    Public Sub op_ShiftTest()
        Assert.IsTrue(New ModInt32(0) << 12 = 0)
        Assert.IsTrue(New ModInt32(3) << 2 = 12)
        Assert.IsTrue(New ModInt32(3) >> 2 = 0)
        Assert.IsTrue(New ModInt32(3) >> 1 = 1)
        Assert.IsTrue(New ModInt32(&HFFFFFFFFUI) >> 2 = &H3FFFFFFFUI)
    End Sub

    <TestMethod()>
    Public Sub op_NotTest()
        Assert.IsTrue(Not New ModInt32(0) = &HFFFFFFFF)
        Assert.IsTrue(Not New ModInt32(1) = &HFFFFFFFE)
        Assert.IsTrue(Not New ModInt32(-1) = 0)
        Assert.IsTrue(Not New ModInt32(UInteger.MaxValue) = 0)
    End Sub

    <TestMethod()>
    Public Sub op_MultiplyTest()
        Assert.IsTrue(New ModInt32(&HFFFFFFFF) * 2 = &HFFFFFFFE)
        Assert.IsTrue(New ModInt32(3) * -2 = -6)
    End Sub

    <TestMethod()>
    Public Sub op_EqualityTest()
        Assert.IsTrue(New ModInt32(UInteger.MaxValue) = UInteger.MaxValue)
        Assert.IsTrue(New ModInt32(-1) = UInteger.MaxValue)
        Assert.IsTrue(New ModInt32(UInteger.MaxValue) = -1)
        Assert.IsTrue(New ModInt32(-1) = -1)
        Assert.IsTrue(New ModInt32(1) = 1)
        Assert.IsTrue(Not New ModInt32(1) = 2)
        Assert.IsTrue(New ModInt32(2) <> 1)
        Assert.IsTrue(New ModInt32(2) <> -1)
        Assert.IsTrue(New ModInt32(UInteger.MaxValue).Equals(New ModInt32(-1)))
    End Sub

    <TestMethod()>
    Public Sub op_Conversion()
        Assert.IsTrue(CUShort(CType(&H100000005UL, ModInt16)) = 5)
        Assert.IsTrue(CUInt(CType(5, ModInt32)) = 5)
        Assert.IsTrue(CUInt(CType(-1, ModInt32)) = UInteger.MaxValue)
        Assert.IsTrue(CInt(CType(UInteger.MaxValue, ModInt32)) = -1)
        Assert.IsTrue(CInt(CType(4, ModInt32)) = 4)
    End Sub

    <TestMethod()>
    Public Sub op_ExclusiveOrTest()
        Assert.IsTrue((New ModInt32(1) Xor 1) = 0)
        Assert.IsTrue((New ModInt32(2) Xor 1) = 3)
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseOrTest()
        Assert.IsTrue((New ModInt32(1) Or 1) = 1)
        Assert.IsTrue((New ModInt32(2) Or 1) = 3)
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseAndTest()
        Assert.IsTrue((New ModInt32(1) And 1) = 1)
        Assert.IsTrue((New ModInt32(2) And 1) = 0)
    End Sub

    <TestMethod()>
    Public Sub op_AdditionTest()
        Assert.IsTrue(New ModInt32(1) + 1 = 2)
        Assert.IsTrue(New ModInt32(UInteger.MaxValue) + 1 = 0)
        Assert.IsTrue(New ModInt32(-1) + 1 = 0)
        Assert.IsTrue(New ModInt32(Integer.MaxValue) + 1 = Integer.MinValue)
        Assert.IsTrue(New ModInt32(Integer.MinValue) + -1 = Integer.MaxValue)
    End Sub

    <TestMethod()>
    Public Sub ShiftRotateTest()
        Assert.IsTrue(New ModInt32(2).ShiftRotateLeft(31) = 1)
        Assert.IsTrue(New ModInt32(0).ShiftRotateLeft(12) = 0)
        Assert.IsTrue(New ModInt32(3).ShiftRotateLeft(2) = 12)
        Assert.IsTrue(New ModInt32(3).ShiftRotateRight(2) = &HC0000000UI)
        Assert.IsTrue(New ModInt32(3).ShiftRotateRight(1) = &H80000001UI)
        Assert.IsTrue(New ModInt32(&HFFFFFFFFUI).ShiftRotateRight(2) = &HFFFFFFFFUI)
    End Sub
End Class
