Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class ModInt16Test
    <TestMethod()>
    Public Sub ConstructionTest()
        Assert.IsTrue(New ModInt16(3US).SignedValue = 3)
        Assert.IsTrue(New ModInt16(3US).UnsignedValue = 3)
        Assert.IsTrue(New ModInt16(UInt16.MinValue).SignedValue = 0)
        Assert.IsTrue(New ModInt16(UInt16.MinValue).UnsignedValue = 0)
        Assert.IsTrue(New ModInt16(UInt16.MaxValue).SignedValue = -1)
        Assert.IsTrue(New ModInt16(UInt16.MaxValue).UnsignedValue = &HFFFF)

        Assert.IsTrue(New ModInt16(-3S).SignedValue = -3)
        Assert.IsTrue(New ModInt16(-3S).UnsignedValue = &HFFFD)
        Assert.IsTrue(New ModInt16(Int16.MinValue).SignedValue = -&H8000)
        Assert.IsTrue(New ModInt16(Int16.MinValue).UnsignedValue = &H8000)
        Assert.IsTrue(New ModInt16(Int16.MaxValue).SignedValue = &H7FFF)
        Assert.IsTrue(New ModInt16(Int16.MaxValue).UnsignedValue = &H7FFF)
    End Sub
    <TestMethod()>
    Public Sub op_ConversionTest()
        Assert.IsTrue(CType(3US, ModInt16).SignedValue = 3)
        Assert.IsTrue(CType(3US, ModInt16).UnsignedValue = 3)
        Assert.IsTrue(CType(UInt16.MinValue, ModInt16).SignedValue = 0)
        Assert.IsTrue(CType(UInt16.MinValue, ModInt16).UnsignedValue = 0)
        Assert.IsTrue(CType(UInt16.MaxValue, ModInt16).SignedValue = -1)
        Assert.IsTrue(CType(UInt16.MaxValue, ModInt16).UnsignedValue = UInt16.MaxValue)

        Assert.IsTrue(CType(-3S, ModInt16).SignedValue = -3)
        Assert.IsTrue(CType(-3S, ModInt16).UnsignedValue = &HFFFDUS)
        Assert.IsTrue(CType(Int16.MinValue, ModInt16).SignedValue = Int16.MinValue)
        Assert.IsTrue(CType(Int16.MinValue, ModInt16).UnsignedValue = &H8000US)
        Assert.IsTrue(CType(Int16.MaxValue, ModInt16).SignedValue = Int16.MaxValue)
        Assert.IsTrue(CType(Int16.MaxValue, ModInt16).UnsignedValue = &H7FFFUS)
    End Sub

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
