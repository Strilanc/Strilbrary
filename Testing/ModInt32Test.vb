Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class ModInt32Test
    <TestMethod()>
    Public Sub ConstructionTest()
        Assert.IsTrue(New ModInt32(3UI).SignedValue = 3)
        Assert.IsTrue(New ModInt32(3UI).UnsignedValue = 3)
        Assert.IsTrue(New ModInt32(UInt32.MinValue).SignedValue = 0)
        Assert.IsTrue(New ModInt32(UInt32.MinValue).UnsignedValue = 0)
        Assert.IsTrue(New ModInt32(UInt32.MaxValue).SignedValue = -1)
        Assert.IsTrue(New ModInt32(UInt32.MaxValue).UnsignedValue = UInt32.MaxValue)

        Assert.IsTrue(New ModInt32(-3).SignedValue = -3)
        Assert.IsTrue(New ModInt32(-3).UnsignedValue = &HFFFFFFFDUI)
        Assert.IsTrue(New ModInt32(Int32.MinValue).SignedValue = Int32.MinValue)
        Assert.IsTrue(New ModInt32(Int32.MinValue).UnsignedValue = &H80000000UI)
        Assert.IsTrue(New ModInt32(Int32.MaxValue).SignedValue = Int32.MaxValue)
        Assert.IsTrue(New ModInt32(Int32.MaxValue).UnsignedValue = &H7FFFFFFFUI)
    End Sub
    <TestMethod()>
    Public Sub op_ConversionTest()
        Assert.IsTrue(CType(3UI, ModInt32).SignedValue = 3)
        Assert.IsTrue(CType(3UI, ModInt32).UnsignedValue = 3)
        Assert.IsTrue(CType(UInt32.MinValue, ModInt32).SignedValue = 0)
        Assert.IsTrue(CType(UInt32.MinValue, ModInt32).UnsignedValue = 0)
        Assert.IsTrue(CType(UInt32.MaxValue, ModInt32).SignedValue = -1)
        Assert.IsTrue(CType(UInt32.MaxValue, ModInt32).UnsignedValue = UInt32.MaxValue)

        Assert.IsTrue(CType(-3, ModInt32).SignedValue = -3)
        Assert.IsTrue(CType(-3, ModInt32).UnsignedValue = &HFFFFFFFDUI)
        Assert.IsTrue(CType(Int32.MinValue, ModInt32).SignedValue = Int32.MinValue)
        Assert.IsTrue(CType(Int32.MinValue, ModInt32).UnsignedValue = &H80000000UI)
        Assert.IsTrue(CType(Int32.MaxValue, ModInt32).SignedValue = Int32.MaxValue)
        Assert.IsTrue(CType(Int32.MaxValue, ModInt32).UnsignedValue = &H7FFFFFFFUI)
    End Sub

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
        Assert.IsTrue((Not New ModInt32(0)) = &HFFFFFFFF)
        Assert.IsTrue((Not New ModInt32(1)) = &HFFFFFFFE)
        Assert.IsTrue((Not New ModInt32(-1)) = 0)
        Assert.IsTrue((Not New ModInt32(UInteger.MaxValue)) = 0)
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

        Assert.IsTrue(New ModInt32(1).Equals(CType(New ModInt32(1), Object)))
        Assert.IsTrue(Not New ModInt32(1).Equals(CType(1UI, Object)))
        Assert.IsTrue(New ModInt32(1).GetHashCode = New ModInt32(1).GetHashCode)
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

    <TestMethod()>
    Public Sub ToStringTest()
        Assert.IsTrue(New ModInt32(2).ToString = "2")
    End Sub
End Class
