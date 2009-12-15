Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class ModInt16Test
    <TestMethod()>
    Public Sub op_SubtractionTest()
        Assert.IsTrue(New ModInt16(0) - 1S = UShort.MaxValue)
        Assert.IsTrue(New ModInt16(UShort.MaxValue) - -1S = 0S)
        Assert.IsTrue(New ModInt16(Short.MaxValue) - -1S = Short.MinValue)
        Assert.IsTrue(New ModInt16(Short.MinValue) - 1S = Short.MaxValue)
        Assert.IsTrue(New ModInt16(1) - 1S = 0S)
        Assert.IsTrue(New ModInt16(3) - 2S = 1S)
    End Sub

    <TestMethod()>
    Public Sub op_ShiftTest()
        Assert.IsTrue(New ModInt16(0) << 12 = 0S)
        Assert.IsTrue(New ModInt16(3) << 2 = 12S)
        Assert.IsTrue(New ModInt16(3) >> 2 = 0S)
        Assert.IsTrue(New ModInt16(3) >> 1 = 1S)
        Assert.IsTrue(New ModInt16(&HFFFFUS) >> 2 = &H3FFFUS)
    End Sub

    <TestMethod()>
    Public Sub op_NotTest()
        Assert.IsTrue(Not New ModInt16(0) = &HFFFFS)
        Assert.IsTrue(Not New ModInt16(1) = &HFFFES)
        Assert.IsTrue(Not New ModInt16(-1) = 0S)
        Assert.IsTrue(Not New ModInt16(UShort.MaxValue) = 0S)
    End Sub

    <TestMethod()>
    Public Sub op_MultiplyTest()
        Assert.IsTrue(New ModInt16(&HFFFF) * 2S = &HFFFES)
        Assert.IsTrue(New ModInt16(3) * -2S = -6S)
    End Sub

    <TestMethod()>
    Public Sub op_EqualityTest()
        Assert.IsTrue(New ModInt16(UShort.MaxValue) = UShort.MaxValue)
        Assert.IsTrue(New ModInt16(-1) = UShort.MaxValue)
        Assert.IsTrue(New ModInt16(UShort.MaxValue) = -1S)
        Assert.IsTrue(New ModInt16(-1) = -1S)
        Assert.IsTrue(New ModInt16(1) = 1S)
        Assert.IsTrue(Not New ModInt16(1) = 2S)
        Assert.IsTrue(New ModInt16(2) <> 1S)
        Assert.IsTrue(New ModInt16(2) <> -1S)
        Assert.IsTrue(New ModInt16(UShort.MaxValue).Equals(New ModInt16(-1)))
    End Sub

    <TestMethod()>
    Public Sub op_Conversion()
        Assert.IsTrue(CUShort(CType(&H10005, ModInt16)) = 5)
        Assert.IsTrue(CUShort(CType(5, ModInt16)) = 5)
        Assert.IsTrue(CUShort(CType(-1, ModInt16)) = UShort.MaxValue)
        Assert.IsTrue(CShort(CType(UShort.MaxValue, ModInt16)) = -1)
        Assert.IsTrue(CShort(CType(4, ModInt16)) = 4)
    End Sub

    <TestMethod()>
    Public Sub op_ExclusiveOrTest()
        Assert.IsTrue((New ModInt16(1) Xor 1S) = 0S)
        Assert.IsTrue((New ModInt16(2) Xor 1S) = 3S)
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseOrTest()
        Assert.IsTrue((New ModInt16(1) Or 1S) = 1S)
        Assert.IsTrue((New ModInt16(2) Or 1S) = 3S)
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseAndTest()
        Assert.IsTrue((New ModInt16(1) And 1S) = 1S)
        Assert.IsTrue((New ModInt16(2) And 1S) = 0S)
    End Sub

    <TestMethod()>
    Public Sub op_AdditionTest()
        Assert.IsTrue(New ModInt16(1) + 1S = 2S)
        Assert.IsTrue(New ModInt16(UShort.MaxValue) + 1S = 0S)
        Assert.IsTrue(New ModInt16(-1) + 1S = 0S)
        Assert.IsTrue(New ModInt16(Short.MaxValue) + 1S = Short.MinValue)
        Assert.IsTrue(New ModInt16(Short.MinValue) + -1S = Short.MaxValue)
    End Sub

    <TestMethod()>
    Public Sub ShiftRotateTest()
        Assert.IsTrue(New ModInt16(2).ShiftRotateLeft(15) = 1S)
        Assert.IsTrue(New ModInt16(0).ShiftRotateLeft(12) = 0S)
        Assert.IsTrue(New ModInt16(3).ShiftRotateLeft(2) = 12S)
        Assert.IsTrue(New ModInt16(3).ShiftRotateRight(2) = &HC000US)
        Assert.IsTrue(New ModInt16(3).ShiftRotateRight(1) = &H8001US)
        Assert.IsTrue(New ModInt16(&HFFFFUS).ShiftRotateRight(2) = &HFFFFUS)
    End Sub
End Class
