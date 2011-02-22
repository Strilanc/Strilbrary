Imports System
Imports System.Numerics
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class BitExtensionTest
    <TestMethod()>
    Public Sub HasBitSetTest()
        Assert.IsTrue(Not CByte(0).HasBitSet(0))
        Assert.IsTrue(Not CByte(0).HasBitSet(5))
        Assert.IsTrue(Not CByte(0).HasBitSet(7))
        Assert.IsTrue(Byte.MaxValue.HasBitSet(0))
        Assert.IsTrue(Byte.MaxValue.HasBitSet(4))
        Assert.IsTrue(Byte.MaxValue.HasBitSet(7))
        Assert.IsTrue(CByte(1).HasBitSet(0))
        Assert.IsTrue(Not CByte(1).HasBitSet(5))
        Assert.IsTrue(Not CByte(4).HasBitSet(0))
        Assert.IsTrue(CByte(4).HasBitSet(2))

        Assert.IsTrue(Not 0US.HasBitSet(0))
        Assert.IsTrue(Not 0US.HasBitSet(5))
        Assert.IsTrue(Not 0US.HasBitSet(7))
        Assert.IsTrue(UInt16.MaxValue.HasBitSet(0))
        Assert.IsTrue(UInt16.MaxValue.HasBitSet(4))
        Assert.IsTrue(UInt16.MaxValue.HasBitSet(15))
        Assert.IsTrue(1US.HasBitSet(0))
        Assert.IsTrue(Not 1US.HasBitSet(5))
        Assert.IsTrue(Not 4US.HasBitSet(0))
        Assert.IsTrue(4US.HasBitSet(2))

        Assert.IsTrue(Not 0UI.HasBitSet(0))
        Assert.IsTrue(Not 0UI.HasBitSet(5))
        Assert.IsTrue(Not 0UI.HasBitSet(7))
        Assert.IsTrue(UInt32.MaxValue.HasBitSet(0))
        Assert.IsTrue(UInt32.MaxValue.HasBitSet(4))
        Assert.IsTrue(UInt32.MaxValue.HasBitSet(31))
        Assert.IsTrue(1UI.HasBitSet(0))
        Assert.IsTrue(Not 1UI.HasBitSet(5))
        Assert.IsTrue(Not 4UI.HasBitSet(0))
        Assert.IsTrue(4UI.HasBitSet(2))

        Assert.IsTrue(Not 0UL.HasBitSet(0))
        Assert.IsTrue(Not 0UL.HasBitSet(5))
        Assert.IsTrue(Not 0UL.HasBitSet(7))
        Assert.IsTrue(UInt64.MaxValue.HasBitSet(0))
        Assert.IsTrue(UInt64.MaxValue.HasBitSet(4))
        Assert.IsTrue(UInt64.MaxValue.HasBitSet(63))
        Assert.IsTrue(1UL.HasBitSet(0))
        Assert.IsTrue(Not 1UL.HasBitSet(5))
        Assert.IsTrue(Not 4UL.HasBitSet(0))
        Assert.IsTrue(4UL.HasBitSet(2))
    End Sub

    <TestMethod()>
    Public Sub WithBitSetToTest()
        Assert.IsTrue(CByte(0).WithBitSetTo(0, True) = 1)
        Assert.IsTrue(CByte(0).WithBitSetTo(1, True) = 2)
        Assert.IsTrue(CByte(1).WithBitSetTo(1, True) = 3)
        Assert.IsTrue(CByte(1).WithBitSetTo(0, False) = 0)
        Assert.IsTrue(CByte(1).WithBitSetTo(7, True) = 129)
        Assert.IsTrue(CByte(&HFF).WithBitSetTo(7, False) = &H7F)

        Assert.IsTrue(0US.WithBitSetTo(0, True) = 1)
        Assert.IsTrue(0US.WithBitSetTo(1, True) = 2)
        Assert.IsTrue(1US.WithBitSetTo(1, True) = 3)
        Assert.IsTrue(1US.WithBitSetTo(0, False) = 0)
        Assert.IsTrue(1US.WithBitSetTo(15, True) = (1UL << 15) + 1UL)
        Assert.IsTrue(&HFFFFUS.WithBitSetTo(15, False) = &H7FFF)

        Assert.IsTrue(0UI.WithBitSetTo(0, True) = 1)
        Assert.IsTrue(0UI.WithBitSetTo(1, True) = 2)
        Assert.IsTrue(1UI.WithBitSetTo(1, True) = 3)
        Assert.IsTrue(1UI.WithBitSetTo(0, False) = 0)
        Assert.IsTrue(1UI.WithBitSetTo(31, True) = (1UL << 31) + 1UL)
        Assert.IsTrue(&HFFFFFFFFUI.WithBitSetTo(31, False) = &H7FFFFFFFUI)

        Assert.IsTrue(0UL.WithBitSetTo(0, True) = 1)
        Assert.IsTrue(0UL.WithBitSetTo(1, True) = 2)
        Assert.IsTrue(1UL.WithBitSetTo(1, True) = 3)
        Assert.IsTrue(1UL.WithBitSetTo(0, False) = 0)
        Assert.IsTrue(1UL.WithBitSetTo(63, True) = (1UL << 63) + 1UL)
        Assert.IsTrue(ULong.MaxValue.WithBitSetTo(63, False) = &H7FFFFFFFFFFFFFFFUL)
    End Sub
End Class
