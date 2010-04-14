Imports System
Imports System.Numerics
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values
Imports Strilbrary.Collections

<TestClass()>
Public Class ValueExtensionTest
    <TestMethod()>
    Public Sub ReversedByteOrderUInt16Test()
        Assert.IsTrue(0US.ReversedByteOrder() = 0US)
        Assert.IsTrue(UInt16.MaxValue.ReversedByteOrder() = UInt16.MaxValue)
        Assert.IsTrue(&H5US.ReversedByteOrder() = &H500US)
        Assert.IsTrue(&H1234US.ReversedByteOrder() = &H3412US)
    End Sub
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
        Dim vals = {{27, 27, 0},
                    {16, 15, 1},
                    {15, 16, 15},
                    {6, 7, 6},
                    {18, 9, 0},
                    {-5, 10, 5},
                    {-5, 9, 4},
                    {23, 11, 1},
                    {-9, 10, 1},
                    {-10, 10, 0},
                    {-11, 10, 9},
                    {9, 10, 9},
                    {10, 10, 0},
                    {11, 10, 1}}

        For i = 0 To vals.GetLength(0) - 1
            Dim num = vals(i, 0)
            Dim div = vals(i, 1)
            Dim res = vals(i, 2)
            Assert.IsTrue(CShort(num).ProperMod(CShort(div)) = CShort(res))
            Assert.IsTrue(CInt(num).ProperMod(CInt(div)) = CInt(res))
            Assert.IsTrue(CLng(num).ProperMod(CLng(div)) = CLng(res))
            Assert.IsTrue(New BigInteger(num).ProperMod(New BigInteger(div)) = New BigInteger(res))
        Next i
    End Sub
    <TestMethod()>
    Public Sub PositiveModTest()
        Dim vals = {{27, 27, 27},
                    {16, 15, 1},
                    {15, 16, 15},
                    {6, 7, 6},
                    {18, 9, 9},
                    {-5, 10, 5},
                    {-5, 9, 4},
                    {23, 11, 1},
                    {-9, 10, 1},
                    {-10, 10, 10},
                    {-11, 10, 9},
                    {9, 10, 9},
                    {10, 10, 10},
                    {11, 10, 1}}

        For i = 0 To vals.GetLength(0) - 1
            Dim num = vals(i, 0)
            Dim div = vals(i, 1)
            Dim res = vals(i, 2)
            Assert.IsTrue(CShort(num).PositiveMod(CShort(div)) = CShort(res))
            Assert.IsTrue(CInt(num).PositiveMod(CInt(div)) = CInt(res))
            Assert.IsTrue(CLng(num).PositiveMod(CLng(div)) = CLng(res))
            Assert.IsTrue(New BigInteger(num).PositiveMod(New BigInteger(div)) = New BigInteger(res))
            If num >= 0 AndAlso div >= 0 Then
                Assert.IsTrue(CUShort(num).PositiveMod(CUShort(div)) = CUShort(res))
                Assert.IsTrue(CUInt(num).PositiveMod(CUInt(div)) = CUInt(res))
                Assert.IsTrue(CULng(num).PositiveMod(CULng(div)) = CULng(res))
            End If
        Next i
    End Sub

    <TestMethod()>
    Public Sub CeilingMultipleTest()
        Dim vals = {{27, 27, 27},
                    {16, 15, 30},
                    {15, 16, 16},
                    {6, 7, 7},
                    {18, 9, 18},
                    {-5, 10, 0},
                    {-5, 9, 0},
                    {23, 11, 33},
                    {-9, 10, 0},
                    {-10, 10, -10},
                    {-11, 10, -10},
                    {9, 10, 10},
                    {10, 10, 10},
                    {11, 10, 20}}

        For i = 0 To vals.GetLength(0) - 1
            Dim num = vals(i, 0)
            Dim div = vals(i, 1)
            Dim res = vals(i, 2)
            Assert.IsTrue(CShort(num).CeilingMultiple(CShort(div)) = CShort(res))
            Assert.IsTrue(CInt(num).CeilingMultiple(CInt(div)) = CInt(res))
            Assert.IsTrue(CLng(num).CeilingMultiple(CLng(div)) = CLng(res))
            Assert.IsTrue(New BigInteger(num).CeilingMultiple(New BigInteger(div)) = New BigInteger(res))
            If num >= 0 AndAlso div >= 0 Then
                Assert.IsTrue(CUShort(num).CeilingMultiple(CUShort(div)) = CUShort(res))
                Assert.IsTrue(CUInt(num).CeilingMultiple(CUInt(div)) = CUInt(res))
                Assert.IsTrue(CULng(num).CeilingMultiple(CULng(div)) = CULng(res))
            End If
        Next i
    End Sub
    <TestMethod()>
    Public Sub FloorMultipleTest()
        Dim vals = {{27, 27, 27},
                    {16, 15, 15},
                    {15, 16, 0},
                    {6, 7, 0},
                    {18, 9, 18},
                    {-5, 10, -10},
                    {-5, 9, -9},
                    {23, 11, 22},
                    {-9, 10, -10},
                    {-10, 10, -10},
                    {-11, 10, -20},
                    {9, 10, 0},
                    {10, 10, 10},
                    {11, 10, 10}}

        For i = 0 To vals.GetLength(0) - 1
            Dim num = vals(i, 0)
            Dim div = vals(i, 1)
            Dim res = vals(i, 2)
            Assert.IsTrue(CShort(num).FloorMultiple(CShort(div)) = CShort(res))
            Assert.IsTrue(CInt(num).FloorMultiple(CInt(div)) = CInt(res))
            Assert.IsTrue(CLng(num).FloorMultiple(CLng(div)) = CLng(res))
            Assert.IsTrue(New BigInteger(num).FloorMultiple(New BigInteger(div)) = New BigInteger(res))
            If num >= 0 AndAlso div >= 0 Then
                Assert.IsTrue(CUShort(num).FloorMultiple(CUShort(div)) = CUShort(res))
                Assert.IsTrue(CUInt(num).FloorMultiple(CUInt(div)) = CUInt(res))
                Assert.IsTrue(CULng(num).FloorMultiple(CULng(div)) = CULng(res))
            End If
        Next i
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
        Assert.IsTrue(New Byte() {0, 0}.ToUInt16(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1, 0}.ToUInt16(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2}.ToUInt16(ByteOrder.LittleEndian) = &H201US)
        Assert.IsTrue(New Byte() {0, 0}.ToUInt16(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {0, 1}.ToUInt16(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2}.ToUInt16(ByteOrder.BigEndian) = &H102US)
        Assert.IsTrue(New Byte() {3, 4}.ToUInt16(ByteOrder.BigEndian) = &H304US)
        Assert.IsTrue(New Byte() {0, 0}.AsEnumerable.ToUInt16(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1, 0}.AsEnumerable.ToUInt16(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2}.AsEnumerable.ToUInt16(ByteOrder.LittleEndian) = &H201US)
        Assert.IsTrue(New Byte() {0, 0}.AsEnumerable.ToUInt16(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {0, 1}.AsEnumerable.ToUInt16(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2}.AsEnumerable.ToUInt16(ByteOrder.BigEndian) = &H102US)
        Assert.IsTrue(New Byte() {3, 4}.AsEnumerable.ToUInt16(ByteOrder.BigEndian) = &H304US)
    End Sub
    <TestMethod()>
    Public Sub ToUInt32Test()
        Assert.IsTrue(New Byte() {0, 0, 0, 0}.AsEnumerable.ToUInt32(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1, 0, 0, 0}.AsEnumerable.ToUInt32(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4}.AsEnumerable.ToUInt32(ByteOrder.LittleEndian) = &H4030201UI)
        Assert.IsTrue(New Byte() {1, 2, 3, 4}.AsEnumerable.ToUInt32(ByteOrder.BigEndian) = &H1020304UI)
        Assert.IsTrue(New Byte() {5, 6, 7, 8}.AsEnumerable.ToUInt32(ByteOrder.BigEndian) = &H5060708UI)
        Assert.IsTrue(New Byte() {0, 0, 0, 0}.ToUInt32(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1, 0, 0, 0}.ToUInt32(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4}.ToUInt32(ByteOrder.LittleEndian) = &H4030201UI)
        Assert.IsTrue(New Byte() {1, 2, 3, 4}.ToUInt32(ByteOrder.BigEndian) = &H1020304UI)
        Assert.IsTrue(New Byte() {5, 6, 7, 8}.ToUInt32(ByteOrder.BigEndian) = &H5060708UI)
    End Sub
    <TestMethod()>
    Public Sub ToUInt64Test()
        Assert.IsTrue(New Byte() {0, 0, 0, 0, 0, 0, 0, 0}.ToUInt64(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1, 0, 0, 0, 0, 0, 0, 0}.ToUInt64(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}.ToUInt64(ByteOrder.LittleEndian) = &H807060504030201UL)
        Assert.IsTrue(New Byte() {0, 0, 0, 0, 0, 0, 0, 0}.ToUInt64(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {0, 0, 0, 0, 0, 0, 0, 1}.ToUInt64(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}.ToUInt64(ByteOrder.BigEndian) = &H102030405060708UL)
        Assert.IsTrue(New Byte() {9, 10, 11, 12, 13, 14, 15, 16}.ToUInt64(ByteOrder.BigEndian) = &H90A0B0C0D0E0F10UL)
        Assert.IsTrue(New Byte() {0, 0, 0, 0, 0, 0, 0, 0}.AsEnumerable.ToUInt64(ByteOrder.LittleEndian) = 0)
        Assert.IsTrue(New Byte() {1, 0, 0, 0, 0, 0, 0, 0}.AsEnumerable.ToUInt64(ByteOrder.LittleEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}.AsEnumerable.ToUInt64(ByteOrder.LittleEndian) = &H807060504030201UL)
        Assert.IsTrue(New Byte() {0, 0, 0, 0, 0, 0, 0, 0}.AsEnumerable.ToUInt64(ByteOrder.BigEndian) = 0)
        Assert.IsTrue(New Byte() {0, 0, 0, 0, 0, 0, 0, 1}.AsEnumerable.ToUInt64(ByteOrder.BigEndian) = 1)
        Assert.IsTrue(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}.AsEnumerable.ToUInt64(ByteOrder.BigEndian) = &H102030405060708UL)
        Assert.IsTrue(New Byte() {9, 10, 11, 12, 13, 14, 15, 16}.AsEnumerable.ToUInt64(ByteOrder.BigEndian) = &H90A0B0C0D0E0F10UL)
    End Sub

    <TestMethod()>
    Public Sub BytesUInt16Test()
        Assert.IsTrue(&H201US.Bytes(ByteOrder.LittleEndian).SequenceEqual({1, 2}))
        Assert.IsTrue(&H201US.Bytes(ByteOrder.BigEndian).SequenceEqual({2, 1}))
        Assert.IsTrue(&H304US.Bytes(ByteOrder.BigEndian).SequenceEqual({3, 4}))
    End Sub
    <TestMethod()>
    Public Sub BytesUInt32Test()
        Assert.IsTrue(&H4030201UI.Bytes(ByteOrder.LittleEndian).SequenceEqual({1, 2, 3, 4}))
        Assert.IsTrue(&H1020304UI.Bytes(ByteOrder.BigEndian).SequenceEqual({1, 2, 3, 4}))
        Assert.IsTrue(&H5060708UI.Bytes(ByteOrder.BigEndian).SequenceEqual({5, 6, 7, 8}))
    End Sub
    <TestMethod()>
    Public Sub BytesUInt64Test()
        Assert.IsTrue(&H807060504030201UL.Bytes(ByteOrder.LittleEndian).SequenceEqual({1, 2, 3, 4, 5, 6, 7, 8}))
        Assert.IsTrue(&H102030405060708UL.Bytes(ByteOrder.BigEndian).SequenceEqual({1, 2, 3, 4, 5, 6, 7, 8}))
        Assert.IsTrue(&H90A0B0C0D0E0F10UL.Bytes(ByteOrder.BigEndian).SequenceEqual({9, 10, 11, 12, 13, 14, 15, 16}))
    End Sub

    <TestMethod()>
    Public Sub ToAscBytesTest()
        Assert.IsTrue(" 0a".ToAscBytes.SequenceEqual({&H20, &H30, &H61}))
        Assert.IsTrue("".ToAscBytes.SequenceEqual({}))
        Assert.IsTrue("A".ToAscBytes.SequenceEqual({&H41}))
        Assert.IsTrue(("B" + Chr(0) + "C").ToAscBytes(nullTerminate:=True).SequenceEqual({&H42, 0, &H43, 0}))
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
        Assert.IsTrue("FF 10 AB 24 05".FromHexStringToBytes.SequenceEqual({&HFF, &H10, &HAB, &H24, &H5}))
        Assert.IsTrue("ff 10 ab 24 5".FromHexStringToBytes.SequenceEqual({&HFF, &H10, &HAB, &H24, &H5}))
        Assert.IsTrue("1 2 3".FromHexStringToBytes.SequenceEqual({1, 2, 3}))
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

    <TestMethod()>
    Public Sub BetweenTest()
        Assert.IsTrue(1.Between(2, 3) = 2)
        Assert.IsTrue(1.Between(3, 2) = 2)
        Assert.IsTrue(2.Between(1, 3) = 2)
        Assert.IsTrue(2.Between(3, 1) = 2)
        Assert.IsTrue(3.Between(1, 2) = 2)
        Assert.IsTrue(3.Between(2, 1) = 2)
        Assert.IsTrue(1.Between(1, 2) = 1)
        Assert.IsTrue(1.Between(1, 0) = 1)
        Assert.IsTrue(0.Between(1, 1) = 1)
        Assert.IsTrue(2.Between(1, 1) = 1)
        Assert.IsTrue(1.Between(1, 1) = 1)
    End Sub

    <TestMethod()>
    Public Sub ClampAtOrAboveTest()
        Assert.IsTrue(1.ClampAtOrAbove(0) = 1)
        Assert.IsTrue(0.ClampAtOrAbove(2) = 2)
        Assert.IsTrue(3.ClampAtOrAbove(3) = 3)
    End Sub

    <TestMethod()>
    Public Sub ClampAtOrBelowTest()
        Assert.IsTrue(1.ClampAtOrBelow(-1) = -1)
        Assert.IsTrue(0.ClampAtOrBelow(2) = 0)
        Assert.IsTrue(3.ClampAtOrAbove(3) = 3)
    End Sub

    Private Enum EV As UInt64
        v1 = 1
        v3 = 3
        v10 = 10
    End Enum
    Private Enum FV As Int32
        None = 0
        f0 = 1 << 0
        f2 = 1 << 2
        f5 = 1 << 5
        f8 = 1 << 8
    End Enum
    <TestMethod()>
    Public Sub EnumParseTest()
        Assert.IsTrue("v1".EnumParse(Of EV)(ignoreCase:=False) = EV.v1)
        Assert.IsTrue("v3".EnumParse(Of EV)(ignoreCase:=False) = EV.v3)
        Assert.IsTrue("v10".EnumParse(Of EV)(ignoreCase:=False) = EV.v10)
        Assert.IsTrue("V1".EnumParse(Of EV)(ignoreCase:=True) = EV.v1)
        ExpectException(Of ArgumentException)(Sub() Call "V1".EnumParse(Of EV)(ignoreCase:=False))
        ExpectException(Of ArgumentException)(Sub() Call "v2".EnumParse(Of EV)(ignoreCase:=True))

        Assert.IsTrue("v1".EnumTryParse(Of EV)(ignoreCase:=False).Value = EV.v1)
        Assert.IsTrue("v3".EnumTryParse(Of EV)(ignoreCase:=False).Value = EV.v3)
        Assert.IsTrue("v10".EnumTryParse(Of EV)(ignoreCase:=False).Value = EV.v10)
        Assert.IsTrue("V1".EnumTryParse(Of EV)(ignoreCase:=True).Value = EV.v1)
        Assert.IsTrue(Not "V1".EnumTryParse(Of EV)(ignoreCase:=False).HasValue)
        Assert.IsTrue(Not "v2".EnumTryParse(Of EV)(ignoreCase:=True).HasValue)
    End Sub
    <TestMethod()>
    Public Sub EnumValuesTest()
        Assert.IsTrue(EnumValues(Of EV)().SequenceEqual({EV.v1, EV.v3, EV.v10}))
        Assert.IsTrue(EnumValues(Of FV)().SequenceEqual({FV.None, FV.f0, FV.f2, FV.f5, FV.f8}))
    End Sub
    <TestMethod()>
    Public Sub EnumFlagsTest()
        Assert.IsTrue(FV.f2.EnumFlags().SequenceEqual({FV.f2}))
        Assert.IsTrue((FV.f2 Or FV.f5).EnumFlags().SequenceEqual({FV.f2, FV.f5}))
        Assert.IsTrue(CType((1 << 2) Or (1 << 3), FV).EnumFlags().SequenceEqual({FV.f2, CType(1 << 3, FV)}))
        Assert.IsTrue((FV.f2 Or FV.f5).EnumFlagsIndexed().SequenceEqual({Tuple.Create(FV.f2, 2), Tuple.Create(FV.f5, 5)}))
    End Sub
    <TestMethod()>
    Public Sub EnumValueIsDefinedTest()
        Assert.IsTrue(EV.v1.EnumValueIsDefined)
        Assert.IsTrue(Not CType(255, EV).EnumValueIsDefined)
        Assert.IsTrue(FV.f5.EnumValueIsDefined)
        Assert.IsTrue(Not (FV.f5 Or FV.f2).EnumValueIsDefined)
    End Sub
    <TestMethod()>
    Public Sub EnumFlagsAreDefinedTest()
        Assert.IsTrue(FV.f5.EnumFlagsAreDefined())
        Assert.IsTrue((FV.f2 Or FV.f5).EnumFlagsAreDefined())
        Assert.IsTrue(Not CType(1 << 3, FV).EnumFlagsAreDefined())
    End Sub
    <TestMethod()>
    Public Sub EnumFlagsToStringTest()
        Assert.IsTrue((FV.f2 Or FV.f5).EnumFlagsToString = "f2, f5")
        Assert.IsTrue((CType(1 << 1, FV) Or FV.f5).EnumFlagsToString = "1<<1, f5")
    End Sub

    <TestMethod()>
    Public Sub NonNullTests()
        Assert.IsTrue(New NonNull(Of Byte)(1).Value = 1)
        Assert.IsTrue(CType(2, NonNull(Of Int32)).Value = 2)
        Assert.IsTrue(CType(New NonNull(Of UInt32)(4), UInt32) = 4UI)
        Assert.IsTrue(New NonNull(Of Byte)(1).ToString = CByte(1).ToString)
        ExpectException(Of Exception)(Sub()
                                          Dim r = New NonNull(Of Object)().Value
                                      End Sub)
    End Sub

    <TestMethod()>
    Public Sub DefaultTest()
        Assert.IsTrue([Default](Of Byte)() = 0)
        Assert.IsTrue([Default](Of Action(Of Byte))() Is Nothing)
    End Sub
End Class
