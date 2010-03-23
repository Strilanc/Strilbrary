Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class ModByteTest
    <TestMethod()>
    Public Sub ConstructionTest()
        Assert.IsTrue(New ModByte(CByte(3)).SignedValue = 3)
        Assert.IsTrue(New ModByte(CByte(3)).UnsignedValue = 3)
        Assert.IsTrue(New ModByte(Byte.MinValue).SignedValue = 0)
        Assert.IsTrue(New ModByte(Byte.MinValue).UnsignedValue = 0)
        Assert.IsTrue(New ModByte(Byte.MaxValue).SignedValue = -1)
        Assert.IsTrue(New ModByte(Byte.MaxValue).UnsignedValue = &HFF)

        Assert.IsTrue(New ModByte(CSByte(-3)).SignedValue = -3)
        Assert.IsTrue(New ModByte(CSByte(-3)).UnsignedValue = &HFD)
        Assert.IsTrue(New ModByte(SByte.MinValue).SignedValue = -&H80)
        Assert.IsTrue(New ModByte(SByte.MinValue).UnsignedValue = &H80)
        Assert.IsTrue(New ModByte(SByte.MaxValue).SignedValue = &H7F)
        Assert.IsTrue(New ModByte(SByte.MaxValue).UnsignedValue = &H7F)
    End Sub
    <TestMethod()>
    Public Sub op_ConversionTest()
        Assert.IsTrue(CType(CByte(3), ModByte).SignedValue = 3)
        Assert.IsTrue(CType(CByte(3), ModByte).UnsignedValue = 3)
        Assert.IsTrue(CType(Byte.MinValue, ModByte).SignedValue = 0)
        Assert.IsTrue(CType(Byte.MinValue, ModByte).UnsignedValue = 0)
        Assert.IsTrue(CType(Byte.MaxValue, ModByte).SignedValue = -1)
        Assert.IsTrue(CType(Byte.MaxValue, ModByte).UnsignedValue = Byte.MaxValue)

        Assert.IsTrue(CType(CSByte(-3), ModByte).SignedValue = -3)
        Assert.IsTrue(CType(CSByte(-3), ModByte).UnsignedValue = &HFD)
        Assert.IsTrue(CType(SByte.MinValue, ModByte).SignedValue = SByte.MinValue)
        Assert.IsTrue(CType(SByte.MinValue, ModByte).UnsignedValue = &H80)
        Assert.IsTrue(CType(SByte.MaxValue, ModByte).SignedValue = SByte.MaxValue)
        Assert.IsTrue(CType(SByte.MaxValue, ModByte).UnsignedValue = &H7F)
    End Sub

    <TestMethod()>
    Public Sub op_SubtractionTest()
        Assert.IsTrue(New ModByte(0) - CByte(1) = Byte.MaxValue)
        Assert.IsTrue(New ModByte(Byte.MaxValue) - CSByte(-1) = CByte(0))
        Assert.IsTrue(New ModByte(SByte.MaxValue) - CSByte(-1) = SByte.MinValue)
        Assert.IsTrue(New ModByte(SByte.MinValue) - CByte(1) = SByte.MaxValue)
        Assert.IsTrue(New ModByte(1) - CByte(1) = CByte(0))
        Assert.IsTrue(New ModByte(3) - CByte(2) = CByte(1))
    End Sub

    <TestMethod()>
    Public Sub op_ShiftTest()
        Assert.IsTrue(New ModByte(0) << 12 = CByte(0))
        Assert.IsTrue(New ModByte(3) << 2 = CByte(12))
        Assert.IsTrue(New ModByte(3) >> 2 = CByte(0))
        Assert.IsTrue(New ModByte(3) >> 1 = CByte(1))
        Assert.IsTrue(New ModByte(Byte.MaxValue) >> 2 = CByte(&H3F))
    End Sub

    <TestMethod()>
    Public Sub op_NotTest()
        Assert.IsTrue((Not New ModByte(0)) = CByte(&HFF))
        Assert.IsTrue((Not New ModByte(1)) = CByte(&HFE))
        Assert.IsTrue((Not New ModByte(-1)) = CByte(0))
        Assert.IsTrue((Not New ModByte(Byte.MaxValue)) = CByte(0))
    End Sub

    <TestMethod()>
    Public Sub op_MultiplyTest()
        Assert.IsTrue(New ModByte(&HFFFFFFFF) * CByte(2) = CByte(&HFE))
        Assert.IsTrue(New ModByte(3) * CSByte(-2) = CSByte(-6))
    End Sub

    <TestMethod()>
    Public Sub op_EqualityTest()
        Assert.IsTrue(New ModByte(Byte.MaxValue) = Byte.MaxValue)
        Assert.IsTrue(New ModByte(-1) = Byte.MaxValue)
        Assert.IsTrue(New ModByte(Byte.MaxValue) = CSByte(-1))
        Assert.IsTrue(New ModByte(-1) = CSByte(-1))
        Assert.IsTrue(New ModByte(1) = CByte(1))
        Assert.IsTrue(Not New ModByte(1) = CByte(2))
        Assert.IsTrue(New ModByte(2) <> CByte(1))
        Assert.IsTrue(New ModByte(2) <> CSByte(-1))
        Assert.IsTrue(New ModByte(Byte.MaxValue).Equals(New ModByte(-1)))

        Assert.IsTrue(New ModByte(1).Equals(CType(New ModByte(1), Object)))
        Assert.IsTrue(Not New ModByte(1).Equals(CType(CByte(1), Object)))
        Assert.IsTrue(New ModByte(1).GetHashCode = New ModByte(1).GetHashCode)
    End Sub

    <TestMethod()>
    Public Sub op_ExclusiveOrTest()
        Assert.IsTrue((New ModByte(1) Xor CByte(1)) = CByte(0))
        Assert.IsTrue((New ModByte(2) Xor CByte(1)) = CByte(3))
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseOrTest()
        Assert.IsTrue((New ModByte(1) Or CByte(1)) = CByte(1))
        Assert.IsTrue((New ModByte(2) Or CByte(1)) = CByte(3))
    End Sub

    <TestMethod()>
    Public Sub op_BitwiseAndTest()
        Assert.IsTrue((New ModByte(1) And CByte(1)) = CByte(1))
        Assert.IsTrue((New ModByte(2) And CByte(1)) = CByte(0))
    End Sub

    <TestMethod()>
    Public Sub op_AdditionTest()
        Assert.IsTrue(New ModByte(1) + CByte(1) = CByte(2))
        Assert.IsTrue(New ModByte(Byte.MaxValue) + CByte(1) = CByte(0))
        Assert.IsTrue(New ModByte(-1) + CByte(1) = CByte(0))
        Assert.IsTrue(New ModByte(SByte.MaxValue) + CByte(1) = SByte.MinValue)
        Assert.IsTrue(New ModByte(SByte.MinValue) + CSByte(-1) = SByte.MaxValue)
    End Sub

    <TestMethod()>
    Public Sub ShiftRotateTest()
        Assert.IsTrue(New ModByte(2).ShiftRotateLeft(31) = CByte(1))
        Assert.IsTrue(New ModByte(0).ShiftRotateLeft(12) = CByte(0))
        Assert.IsTrue(New ModByte(3).ShiftRotateLeft(2) = CByte(12))
        Assert.IsTrue(New ModByte(3).ShiftRotateRight(2) = CByte(&HC0))
        Assert.IsTrue(New ModByte(3).ShiftRotateRight(1) = CByte(&H81))
        Assert.IsTrue(New ModByte(Byte.MaxValue).ShiftRotateRight(2) = Byte.MaxValue)
    End Sub

    <TestMethod()>
    Public Sub ToStringTest()
        Assert.IsTrue(New ModByte(2).ToString = "2")
    End Sub
End Class
