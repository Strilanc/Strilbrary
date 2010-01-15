Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values
Imports Strilbrary.Collections

<TestClass()>
Public Class ValueExtensionTest
    <TestMethod()>
    Public Sub ReversedByteOrderUInt32Test()
        Assert.IsTrue(0UI.ReversedByteOrder() = 0UI)
        Assert.IsTrue(UInt32.MaxValue.ReversedByteOrder() = UInt32.MaxValue)
        Assert.IsTrue(&H5UI.ReversedByteOrder() = &H5000000)
        Assert.IsTrue(&H12345678UI.ReversedByteOrder() = &H78563412UI)
    End Sub
    <TestMethod()>
    Public Sub ReversedByteOrderUInt64Test()
        Assert.IsTrue(0UL.ReversedByteOrder() = 0UL)
        Assert.IsTrue(UInt64.MaxValue.ReversedByteOrder() = UInt64.MaxValue)
        Assert.IsTrue(&H5UL.ReversedByteOrder() = &H500000000000000)
        Assert.IsTrue(&H123456789ABCDEFUL.ReversedByteOrder() = &HEFCDAB8967452301UL)
    End Sub

    <TestMethod()>
    Public Sub ProperModTest()
        Assert.IsTrue(27.ProperMod(27) = 0)
        Assert.IsTrue(16.ProperMod(15) = 1)
        Assert.IsTrue(15.ProperMod(16) = 15)
        Assert.IsTrue(6.ProperMod(7) = 6)
        Assert.IsTrue(18.ProperMod(9) = 0)
        Assert.IsTrue((-5).ProperMod(10) = 5)
        Assert.IsTrue((-5).ProperMod(9) = 4)
        Assert.IsTrue(23.ProperMod(11) = 1)
        Assert.IsTrue((-9).ProperMod(10) = 1)
        Assert.IsTrue((-10).ProperMod(10) = 0)
        Assert.IsTrue((-11).ProperMod(10) = 9)
        Assert.IsTrue((9).ProperMod(10) = 9)
        Assert.IsTrue((10).ProperMod(10) = 0)
        Assert.IsTrue((11).ProperMod(10) = 1)
    End Sub
    <TestMethod()>
    Public Sub PositiveModTest()
        Assert.IsTrue(27.PositiveMod(27) = 27)
        Assert.IsTrue(16.PositiveMod(15) = 1)
        Assert.IsTrue(15.PositiveMod(16) = 15)
        Assert.IsTrue(6.PositiveMod(7) = 6)
        Assert.IsTrue(18.PositiveMod(9) = 9)
        Assert.IsTrue((-5).PositiveMod(10) = 5)
        Assert.IsTrue((-5).PositiveMod(9) = 4)
        Assert.IsTrue(23.PositiveMod(11) = 1)
        Assert.IsTrue((-9).PositiveMod(10) = 1)
        Assert.IsTrue((-10).PositiveMod(10) = 10)
        Assert.IsTrue((-11).PositiveMod(10) = 9)
        Assert.IsTrue((9).PositiveMod(10) = 9)
        Assert.IsTrue((10).PositiveMod(10) = 10)
        Assert.IsTrue((11).PositiveMod(10) = 1)
    End Sub

    <TestMethod()>
    Public Sub CeilingMultipleTest()
        Assert.IsTrue(6.CeilingMultiple(7) = 7)
        Assert.IsTrue(18.CeilingMultiple(9) = 18)
        Assert.IsTrue((-5).CeilingMultiple(10) = 0)
        Assert.IsTrue(23.CeilingMultiple(11) = 33)
        Assert.IsTrue((-9).CeilingMultiple(10) = 0)
        Assert.IsTrue((-10).CeilingMultiple(10) = -10)
        Assert.IsTrue((-11).CeilingMultiple(10) = -10)
        Assert.IsTrue((9).CeilingMultiple(10) = 10)
        Assert.IsTrue((10).CeilingMultiple(10) = 10)
        Assert.IsTrue((11).CeilingMultiple(10) = 20)
    End Sub
    <TestMethod()>
    Public Sub FloorMultipleTest()
        Assert.IsTrue(6.FloorMultiple(7) = 0)
        Assert.IsTrue(18.FloorMultiple(9) = 18)
        Assert.IsTrue((-5).FloorMultiple(10) = -10)
        Assert.IsTrue(23.FloorMultiple(11) = 22)
        Assert.IsTrue((-9).FloorMultiple(10) = -10)
        Assert.IsTrue((-10).FloorMultiple(10) = -10)
        Assert.IsTrue((-11).FloorMultiple(10) = -20)
        Assert.IsTrue((9).FloorMultiple(10) = 0)
        Assert.IsTrue((10).FloorMultiple(10) = 10)
        Assert.IsTrue((11).FloorMultiple(10) = 10)
    End Sub

    <TestMethod()>
    Public Sub IsFiniteTest()
        Assert.IsTrue(2.0.IsFinite)
        Assert.IsTrue(2.5.IsFinite)
        Assert.IsTrue(CDbl(2).IsFinite)
        Assert.IsTrue(Double.MaxValue.IsFinite)
        Assert.IsTrue(Double.MinValue.IsFinite)
        Assert.IsTrue(Double.Epsilon.IsFinite)
        Assert.IsTrue(Not Double.NaN.IsFinite)
        Assert.IsTrue(Not Double.PositiveInfinity.IsFinite)
        Assert.IsTrue(Not Double.NegativeInfinity.IsFinite)
    End Sub

    <TestMethod()>
    Public Sub ToUInt16Test()
        Assert.IsTrue(New Byte() {}.ToUInt16(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1}.ToUInt16(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2}.ToUInt16(ByteOrder.LittleEndian) = &H201US)
        Assert.IsTrue(New Byte() {}.ToUInt16(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {1}.ToUInt16(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2}.ToUInt16(ByteOrder.BigEndian) = &H102US)
        Assert.IsTrue(New Byte() {3, 4}.ToUInt16(ByteOrder.BigEndian) = &H304US)
    End Sub
    <TestMethod()>
    Public Sub ToUInt32Test()
        Assert.IsTrue(New Byte() {}.ToUInt32(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1}.ToUInt32(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4}.ToUInt32(ByteOrder.LittleEndian) = &H4030201UI)
        Assert.IsTrue(New Byte() {}.ToUInt32(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {1}.ToUInt32(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4}.ToUInt32(ByteOrder.BigEndian) = &H1020304UI)
        Assert.IsTrue(New Byte() {5, 6, 7, 8}.ToUInt32(ByteOrder.BigEndian) = &H5060708UI)
    End Sub
    <TestMethod()>
    Public Sub ToUInt64Test()
        Assert.IsTrue(New Byte() {}.ToUInt64(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1}.ToUInt64(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}.ToUInt64(ByteOrder.LittleEndian) = &H807060504030201UL)
        Assert.IsTrue(New Byte() {}.ToUInt64(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {1}.ToUInt64(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}.ToUInt64(ByteOrder.BigEndian) = &H102030405060708UL)
        Assert.IsTrue(New Byte() {9, 10, 11, 12, 13, 14, 15, 16}.ToUInt64(ByteOrder.BigEndian) = &H90A0B0C0D0E0F10UL)
    End Sub

    <TestMethod()>
    Public Sub BytesUInt16Test()
        Assert.IsTrue(&H201US.Bytes(ByteOrder.LittleEndian).HasSameItemsAs({1, 2}))
        Assert.IsTrue(&H201US.Bytes(ByteOrder.BigEndian).HasSameItemsAs({2, 1}))
        Assert.IsTrue(&H304US.Bytes(ByteOrder.BigEndian).HasSameItemsAs({3, 4}))
    End Sub
    <TestMethod()>
    Public Sub BytesUInt32Test()
        Assert.IsTrue(&H4030201UI.Bytes(ByteOrder.LittleEndian).HasSameItemsAs({1, 2, 3, 4}))
        Assert.IsTrue(&H1020304UI.Bytes(ByteOrder.BigEndian).HasSameItemsAs({1, 2, 3, 4}))
        Assert.IsTrue(&H5060708UI.Bytes(ByteOrder.BigEndian).HasSameItemsAs({5, 6, 7, 8}))
    End Sub
    <TestMethod()>
    Public Sub BytesUInt64Test()
        Assert.IsTrue(&H807060504030201UL.Bytes(ByteOrder.LittleEndian).HasSameItemsAs({1, 2, 3, 4, 5, 6, 7, 8}))
        Assert.IsTrue(&H102030405060708UL.Bytes(ByteOrder.BigEndian).HasSameItemsAs({1, 2, 3, 4, 5, 6, 7, 8}))
        Assert.IsTrue(&H90A0B0C0D0E0F10UL.Bytes(ByteOrder.BigEndian).HasSameItemsAs({9, 10, 11, 12, 13, 14, 15, 16}))
    End Sub

    <TestMethod()>
    Public Sub ToAscBytesTest()
        Assert.IsTrue(" 0a".ToAscBytes.HasSameItemsAs({&H20, &H30, &H61}))
        Assert.IsTrue("".ToAscBytes.HasSameItemsAs({}))
        Assert.IsTrue("A".ToAscBytes.HasSameItemsAs({&H41}))
        Assert.IsTrue(("B" + Chr(0) + "C").ToAscBytes(nullTerminate:=True).HasSameItemsAs({&H42, 0, &H43, 0}))
    End Sub
    <TestMethod()>
    Public Sub ParseChrStringTest()
        Assert.IsTrue(New Byte() {&H20, &H30, &H61}.ParseChrString(nullTerminated:=False) = " 0a")
        Assert.IsTrue(New Byte() {}.ParseChrString(nullTerminated:=False) = "")
        Assert.IsTrue(New Byte() {&H41}.ParseChrString(nullTerminated:=False) = "A")
        Assert.IsTrue(New Byte() {&H42, 0, &H43, 0}.ParseChrString(nullTerminated:=True) = "B")
    End Sub

    <TestMethod()>
    Public Sub ToHexStringTest()
        Assert.IsTrue(New Byte() {&HFF, &H10, &HAB, &H24, &H5}.ToHexString.ToUpperInvariant = "FF 10 AB 24 05")
        Assert.IsTrue(New Byte() {&HFF, &H10, &HAB, &H24, &H5}.ToHexString(minWordLength:=1, separator:=", ").ToUpperInvariant = "FF, 10, AB, 24, 5")
        Assert.IsTrue(New Byte() {1, 2, 3}.ToHexString(minWordLength:=3, separator:=":").ToUpperInvariant = "001:002:003")
    End Sub
    <TestMethod()>
    Public Sub FromHexStringToBytesTest()
        Assert.IsTrue("FF 10 AB 24 05".FromHexStringToBytes.HasSameItemsAs({&HFF, &H10, &HAB, &H24, &H5}))
        Assert.IsTrue("ff 10 ab 24 5".FromHexStringToBytes.HasSameItemsAs({&HFF, &H10, &HAB, &H24, &H5}))
        Assert.IsTrue("1 2 3".FromHexStringToBytes.HasSameItemsAs({1, 2, 3}))
    End Sub

    <TestMethod()>
    Public Sub ToBinaryTest()
        Assert.IsTrue(5UL.ToBinary() = "00000101")
        Assert.IsTrue(256UL.ToBinary(minLength:=0) = "100000000")
        Assert.IsTrue(257UL.ToBinary(minLength:=16) = "0000000100000001")
    End Sub

    <TestMethod()>
    Public Sub FromHexToUInt64Test()
        Assert.IsTrue("0123456789ABCdef".FromHexToUInt64(ByteOrder.LittleEndian) = &HFEDCBA9876543210UL)
        Assert.IsTrue("0123456789ABCdef".FromHexToUInt64(ByteOrder.BigEndian) = &H123456789ABCDEFUL)
    End Sub

    <TestMethod()>
    Public Sub BitwiseToSByteTest()
        Assert.IsTrue(CByte(&H0).BitwiseToSByte() = CSByte(&H0))
        Assert.IsTrue(CByte(&H1).BitwiseToSByte() = CSByte(&H1))
        Assert.IsTrue(CByte(&HFF).BitwiseToSByte() = CSByte(&HFF - 256))
        Assert.IsTrue(CByte(&H12).BitwiseToSByte() = CSByte(&H12))
        Assert.IsTrue(CByte(&HAB).BitwiseToSByte() = CSByte(&HAB - 256))
    End Sub
    <TestMethod()>
    Public Sub BitwiseToInt16Test()
        Assert.IsTrue(&H0US.BitwiseToInt16() = &H0S)
        Assert.IsTrue(&H1US.BitwiseToInt16() = &H1S)
        Assert.IsTrue(&HFFFFUS.BitwiseToInt16() = &HFFFFS)
        Assert.IsTrue(&H1234US.BitwiseToInt16() = &H1234S)
        Assert.IsTrue(&HABCDUS.BitwiseToInt16() = &HABCDS)
    End Sub
    <TestMethod()>
    Public Sub BitwiseToInt32Test()
        Assert.IsTrue(&H0UI.BitwiseToInt32() = &H0)
        Assert.IsTrue(&H1UI.BitwiseToInt32() = &H1)
        Assert.IsTrue(&HFFFFFFFFUI.BitwiseToInt32() = &HFFFFFFFF)
        Assert.IsTrue(&H12345678UI.BitwiseToInt32() = &H12345678)
        Assert.IsTrue(&HABCDEF01UI.BitwiseToInt32() = &HABCDEF01)
    End Sub
    <TestMethod()>
    Public Sub BitwiseToInt64Test()
        Assert.IsTrue(&H0UL.BitwiseToInt64() = &H0L)
        Assert.IsTrue(&H1UL.BitwiseToInt64() = &H1L)
        Assert.IsTrue(&HFFFFFFFFFFFFFFFFUL.BitwiseToInt64() = &HFFFFFFFFFFFFFFFFL)
        Assert.IsTrue(&H1234567890ABCDEFUL.BitwiseToInt64() = &H1234567890ABCDEFL)
        Assert.IsTrue(&HABCDEF0123456789UL.BitwiseToInt64() = &HABCDEF0123456789L)
    End Sub

    <TestMethod()>
    Public Sub BitwiseToByteTest()
        Assert.IsTrue(CSByte(&H0).BitwiseToByte() = CByte(&H0))
        Assert.IsTrue(CSByte(&H1).BitwiseToByte() = CByte(&H1))
        Assert.IsTrue(CSByte(&HFF - 256).BitwiseToByte() = CByte(&HFF))
        Assert.IsTrue(CSByte(&H12).BitwiseToByte() = CByte(&H12))
        Assert.IsTrue(CSByte(&HAB - 256).BitwiseToByte() = CByte(&HAB))
    End Sub
    <TestMethod()>
    Public Sub BitwiseToUInt16Test()
        Assert.IsTrue(&H0S.BitwiseToUInt16() = &H0US)
        Assert.IsTrue(&H1S.BitwiseToUInt16() = &H1US)
        Assert.IsTrue(&HFFFFS.BitwiseToUInt16() = &HFFFFUS)
        Assert.IsTrue(&H1234S.BitwiseToUInt16() = &H1234US)
        Assert.IsTrue(&HABCDS.BitwiseToUInt16() = &HABCDUS)
    End Sub
    <TestMethod()>
    Public Sub BitwiseToUInt32Test()
        Assert.IsTrue(&H0.BitwiseToUInt32() = &H0UI)
        Assert.IsTrue(&H1.BitwiseToUInt32() = &H1UI)
        Assert.IsTrue(&HFFFFFFFF.BitwiseToUInt32() = &HFFFFFFFFUI)
        Assert.IsTrue(&H12345678.BitwiseToUInt32() = &H12345678UI)
        Assert.IsTrue(&HABCDEF01.BitwiseToUInt32() = &HABCDEF01UI)
    End Sub
    <TestMethod()>
    Public Sub BitwiseToUInt64Test()
        Assert.IsTrue(&H0L.BitwiseToUInt64() = &H0UL)
        Assert.IsTrue(&H1L.BitwiseToUInt64() = &H1UL)
        Assert.IsTrue(&HFFFFFFFFFFFFFFFFL.BitwiseToUInt64() = &HFFFFFFFFFFFFFFFFUL)
        Assert.IsTrue(&H1234567890ABCDEFL.BitwiseToUInt64() = &H1234567890ABCDEFUL)
        Assert.IsTrue(&HABCDEF0123456789L.BitwiseToUInt64() = &HABCDEF0123456789UL)
    End Sub

    <TestMethod()>
    Public Sub ShiftRotateLeftTest_Byte()
        Assert.IsTrue(CByte(&HFF).ShiftRotateLeft(5) = &HFF)
        Assert.IsTrue(CByte(0).ShiftRotateLeft(3) = 0)
        Assert.IsTrue(CByte(&HF0).ShiftRotateLeft(4) = &HF)
        Assert.IsTrue(CByte(&HF).ShiftRotateLeft(4) = &HF0)
        Assert.IsTrue(CByte(&H1).ShiftRotateLeft(7) = &H80)
        Assert.IsTrue(CByte(&H1).ShiftRotateLeft(8) = &H1)
        Assert.IsTrue(CByte(&H1).ShiftRotateLeft(9) = &H2)
        Assert.IsTrue(CByte(&H6).ShiftRotateLeft(0) = &H6)
    End Sub
    <TestMethod()>
    Public Sub ShiftRotateLeftTest_UInt16()
        Assert.IsTrue(&HFFFFUS.ShiftRotateLeft(5) = &HFFFFUS)
        Assert.IsTrue(0US.ShiftRotateLeft(3) = 0US)
        Assert.IsTrue(&HFF00US.ShiftRotateLeft(8) = &HFFUS)
        Assert.IsTrue(&HFFUS.ShiftRotateLeft(8) = &HFF00US)
        Assert.IsTrue(&H1US.ShiftRotateLeft(15) = &H8000US)
        Assert.IsTrue(&H1US.ShiftRotateLeft(16) = &H1US)
        Assert.IsTrue(&H1US.ShiftRotateLeft(17) = &H2US)
        Assert.IsTrue(&H6US.ShiftRotateLeft(0) = &H6US)
    End Sub
    <TestMethod()>
    Public Sub ShiftRotateLeftTest_UInt32()
        Assert.IsTrue(&HFFFFFFFFUI.ShiftRotateLeft(5) = &HFFFFFFFFUI)
        Assert.IsTrue(0UI.ShiftRotateLeft(3) = 0UI)
        Assert.IsTrue(&HFFFF0000UI.ShiftRotateLeft(16) = &HFFFFUI)
        Assert.IsTrue(&HFFFFUI.ShiftRotateLeft(16) = &HFFFF0000UI)
        Assert.IsTrue(&H1UI.ShiftRotateLeft(31) = &H80000000UI)
        Assert.IsTrue(&H1UI.ShiftRotateLeft(32) = &H1UI)
        Assert.IsTrue(&H1UI.ShiftRotateLeft(33) = &H2UI)
        Assert.IsTrue(&H6UI.ShiftRotateLeft(0) = &H6UI)
    End Sub
    <TestMethod()>
    Public Sub ShiftRotateLeftTest_UInt64()
        Assert.IsTrue(&HFFFFFFFFFFFFFFFFUL.ShiftRotateLeft(5) = &HFFFFFFFFFFFFFFFFUL)
        Assert.IsTrue(0UL.ShiftRotateLeft(3) = 0UL)
        Assert.IsTrue(&HFFFFFFFF00000000UL.ShiftRotateLeft(32) = &HFFFFFFFFUL)
        Assert.IsTrue(&HFFFFFFFFUL.ShiftRotateLeft(32) = &HFFFFFFFF00000000UL)
        Assert.IsTrue(&H1UL.ShiftRotateLeft(63) = &H8000000000000000UL)
        Assert.IsTrue(&H1UL.ShiftRotateLeft(64) = &H1UL)
        Assert.IsTrue(&H1UL.ShiftRotateLeft(65) = &H2UL)
        Assert.IsTrue(&H6UL.ShiftRotateLeft(0) = &H6UL)
    End Sub

    <TestMethod()>
    Public Sub ShiftRotateRightTest_Byte()
        Assert.IsTrue(CByte(&HFF).ShiftRotateRight(5) = &HFF)
        Assert.IsTrue(CByte(0).ShiftRotateRight(3) = 0)
        Assert.IsTrue(CByte(&HF0).ShiftRotateRight(4) = &HF)
        Assert.IsTrue(CByte(&HF).ShiftRotateRight(4) = &HF0)
        Assert.IsTrue(CByte(&H1).ShiftRotateRight(7) = &H2)
        Assert.IsTrue(CByte(&H1).ShiftRotateRight(8) = &H1)
        Assert.IsTrue(CByte(&H1).ShiftRotateRight(9) = &H80)
        Assert.IsTrue(CByte(&H6).ShiftRotateRight(0) = &H6)
    End Sub
    <TestMethod()>
    Public Sub ShiftRotateRightTest_UInt16()
        Assert.IsTrue(&HFFFFUS.ShiftRotateRight(5) = &HFFFFUS)
        Assert.IsTrue(0US.ShiftRotateRight(3) = 0US)
        Assert.IsTrue(&HFF00US.ShiftRotateRight(8) = &HFFUS)
        Assert.IsTrue(&HFFUS.ShiftRotateRight(8) = &HFF00US)
        Assert.IsTrue(&H1US.ShiftRotateRight(15) = &H2US)
        Assert.IsTrue(&H1US.ShiftRotateRight(16) = &H1US)
        Assert.IsTrue(&H1US.ShiftRotateRight(17) = &H8000US)
        Assert.IsTrue(&H6US.ShiftRotateRight(0) = &H6US)
    End Sub
    <TestMethod()>
    Public Sub ShiftRotateRightTest_UInt32()
        Assert.IsTrue(&HFFFFFFFFUI.ShiftRotateRight(5) = &HFFFFFFFFUI)
        Assert.IsTrue(0UI.ShiftRotateRight(3) = 0UI)
        Assert.IsTrue(&HFFFF0000UI.ShiftRotateRight(16) = &HFFFFUI)
        Assert.IsTrue(&HFFFFUI.ShiftRotateRight(16) = &HFFFF0000UI)
        Assert.IsTrue(&H1UI.ShiftRotateRight(31) = &H2UI)
        Assert.IsTrue(&H1UI.ShiftRotateRight(32) = &H1UI)
        Assert.IsTrue(&H1UI.ShiftRotateRight(33) = &H80000000UI)
        Assert.IsTrue(&H6UI.ShiftRotateRight(0) = &H6UI)
    End Sub
    <TestMethod()>
    Public Sub ShiftRotateRightTest_UInt64()
        Assert.IsTrue(&HFFFFFFFFFFFFFFFFUL.ShiftRotateRight(5) = &HFFFFFFFFFFFFFFFFUL)
        Assert.IsTrue(0UL.ShiftRotateRight(3) = 0UL)
        Assert.IsTrue(&HFFFFFFFF00000000UL.ShiftRotateRight(32) = &HFFFFFFFFUL)
        Assert.IsTrue(&HFFFFFFFFUL.ShiftRotateRight(32) = &HFFFFFFFF00000000UL)
        Assert.IsTrue(&H1UL.ShiftRotateRight(63) = &H2UL)
        Assert.IsTrue(&H1UL.ShiftRotateRight(64) = &H1UL)
        Assert.IsTrue(&H1UL.ShiftRotateRight(65) = &H8000000000000000UL)
        Assert.IsTrue(&H6UL.ShiftRotateRight(0) = &H6UL)
    End Sub
End Class
