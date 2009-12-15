Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class FiniteDoubleTest
    <TestMethod()>
    Public Sub ConstructTest()
        Assert.IsTrue(New FiniteDouble().Value = 0)
        Assert.IsTrue(New FiniteDouble(2).Value = 2)
        Assert.IsTrue(New FiniteDouble(2.5).Value = 2.5)
        Assert.IsTrue(CType(2, FiniteDouble).Value = 2.0)
        Assert.IsTrue(CType(2.5, FiniteDouble).Value = 2.5)
    End Sub

    <TestMethod()>
    Public Sub RoundTest()
        Assert.IsTrue(New FiniteDouble(2.2).Round = 2)
        Assert.IsTrue(New FiniteDouble(2.6).Round = 3)
        Assert.IsTrue(New FiniteDouble(-2.2).Round = -2)
        Assert.IsTrue(New FiniteDouble(-2.6).Round = -3)
    End Sub

    <TestMethod()>
    Public Sub FloorTest()
        Assert.IsTrue(New FiniteDouble(2.2).Floor = 2)
        Assert.IsTrue(New FiniteDouble(2.6).Floor = 2)
        Assert.IsTrue(New FiniteDouble(-2.2).Floor = -3)
        Assert.IsTrue(New FiniteDouble(-2.6).Floor = -3)
    End Sub

    <TestMethod()>
    Public Sub CeilingTest()
        Assert.IsTrue(New FiniteDouble(2.2).Ceiling = 3)
        Assert.IsTrue(New FiniteDouble(2.6).Ceiling = 3)
        Assert.IsTrue(New FiniteDouble(-2.2).Ceiling = -2)
        Assert.IsTrue(New FiniteDouble(-2.6).Ceiling = -2)
    End Sub

    <TestMethod()>
    Public Sub AbsTest()
        Assert.IsTrue(New FiniteDouble(3).Abs = 3)
        Assert.IsTrue(New FiniteDouble(-1.5).Abs = 1.5)
    End Sub

    <TestMethod()>
    Public Sub op_UnaryNegationTest()
        Assert.IsTrue(-New FiniteDouble(3) = -3)
        Assert.IsTrue(--New FiniteDouble(3) = 3)
    End Sub

    <TestMethod()>
    Public Sub op_SubtractionTest()
        Assert.IsTrue(New FiniteDouble(3) - New FiniteDouble(4) = -1)
        Assert.IsTrue(New FiniteDouble(2) - New FiniteDouble(0.5) = 1.5)
        Assert.IsTrue(New FiniteDouble(1) - New FiniteDouble(-0.5) = 1.5)
    End Sub

    <TestMethod()>
    Public Sub op_MultiplyTest()
        Assert.IsTrue(New FiniteDouble(3) * New FiniteDouble(4) = 12)
        Assert.IsTrue(New FiniteDouble(2) * New FiniteDouble(0.5) = 1)
        Assert.IsTrue(New FiniteDouble(1) * New FiniteDouble(-0.5) = -0.5)
    End Sub

    <TestMethod()>
    Public Sub op_LessThanOrEqualTest()
        Assert.IsTrue(New FiniteDouble(5) <= New FiniteDouble(5))
        Assert.IsTrue(New FiniteDouble(3) <= New FiniteDouble(4))
        Assert.IsTrue(Not New FiniteDouble(2) <= New FiniteDouble(0.5))
        Assert.IsTrue(Not New FiniteDouble(1) <= New FiniteDouble(-0.5))
    End Sub

    <TestMethod()>
    Public Sub op_LessThanTest()
        Assert.IsTrue(Not New FiniteDouble(5) < New FiniteDouble(5))
        Assert.IsTrue(New FiniteDouble(3) < New FiniteDouble(4))
        Assert.IsTrue(Not New FiniteDouble(2) < New FiniteDouble(0.5))
        Assert.IsTrue(Not New FiniteDouble(1) < New FiniteDouble(-0.5))
    End Sub

    <TestMethod()>
    Public Sub op_InequalityTest()
        Assert.IsTrue(Not New FiniteDouble(5) <> New FiniteDouble(5))
        Assert.IsTrue(New FiniteDouble(3) <> New FiniteDouble(4))
    End Sub

    <TestMethod()>
    Public Sub op_GreaterThanOrEqualTest()
        Assert.IsTrue(New FiniteDouble(5) >= New FiniteDouble(5))
        Assert.IsTrue(Not New FiniteDouble(3) >= New FiniteDouble(4))
        Assert.IsTrue(New FiniteDouble(2) >= New FiniteDouble(0.5))
        Assert.IsTrue(New FiniteDouble(1) >= New FiniteDouble(-0.5))
    End Sub

    <TestMethod()>
    Public Sub op_GreaterThanTest()
        Assert.IsTrue(Not New FiniteDouble(5) > New FiniteDouble(5))
        Assert.IsTrue(Not New FiniteDouble(3) > New FiniteDouble(4))
        Assert.IsTrue(New FiniteDouble(2) > New FiniteDouble(0.5))
        Assert.IsTrue(New FiniteDouble(1) > New FiniteDouble(-0.5))
    End Sub

    <TestMethod()>
    Public Sub op_ExponentTest()
        Assert.IsTrue(New FiniteDouble(5) ^ 5 = (5 ^ 5))
        Assert.IsTrue(New FiniteDouble(5) ^ New FiniteDouble(5) = (5 ^ 5))
        Assert.IsTrue(New FiniteDouble(4) ^ New FiniteDouble(1.5) = 8)
        Assert.IsTrue(New FiniteDouble(3) ^ 0 = 1)
        Assert.IsTrue(New FiniteDouble(0) ^ 2 = 0)
    End Sub

    <TestMethod()>
    Public Sub op_EqualityTest()
        Assert.IsTrue(New FiniteDouble(5) = New FiniteDouble(5))
        Assert.IsTrue(Not New FiniteDouble(3) = New FiniteDouble(4))
        Assert.IsTrue(New FiniteDouble(5).Equals(New FiniteDouble(5)))
        Assert.IsTrue(Not New FiniteDouble(3).Equals(New FiniteDouble(4)))
        Assert.IsTrue(New FiniteDouble(5).Equals(CType(New FiniteDouble(5), Object)))
        Assert.IsTrue(Not New FiniteDouble(3).Equals(CType(New FiniteDouble(4), Object)))
    End Sub

    <TestMethod()>
    Public Sub op_DivisionTest()
        Assert.IsTrue(New FiniteDouble(5) / New FiniteDouble(2) = 2.5)
        Assert.IsTrue(New FiniteDouble(0) / New FiniteDouble(2) = 0)
        Assert.IsTrue(New FiniteDouble(-5) / New FiniteDouble(-2) = 2.5)
        Assert.IsTrue(New FiniteDouble(6) / New FiniteDouble(-2) = -3)
    End Sub

    <TestMethod()>
    Public Sub op_AdditionTest()
        Assert.IsTrue(New FiniteDouble(3) + New FiniteDouble(4) = 7)
        Assert.IsTrue(New FiniteDouble(2) + New FiniteDouble(0.5) = 2.5)
        Assert.IsTrue(New FiniteDouble(1) + New FiniteDouble(-0.5) = 0.5)
    End Sub

    <TestMethod()>
    Public Sub MinTest()
        Assert.IsTrue(FiniteDouble.Min({5, 4, 8, 5}) = 4)
        Assert.IsTrue(FiniteDouble.Min({5, 4, 2, 5}) = 2)
        Assert.IsTrue(FiniteDouble.Min(5, 4, 8, 5) = 4)
        Assert.IsTrue(FiniteDouble.Min(5, 4, 2, 5) = 2)
    End Sub

    <TestMethod()>
    Public Sub MaxTest()
        Assert.IsTrue(FiniteDouble.Max({5, 4, 8, 5}) = 8)
        Assert.IsTrue(FiniteDouble.Max({5, 4, 2, 5}) = 5)
        Assert.IsTrue(FiniteDouble.Max(5, 4, 8, 5) = 8)
        Assert.IsTrue(FiniteDouble.Max(5, 4, 2, 5) = 5)
    End Sub

    <TestMethod()>
    Public Sub CompareToTest()
        Assert.IsTrue(New FiniteDouble(5).CompareTo(New FiniteDouble(3)) > 0)
        Assert.IsTrue(New FiniteDouble(-5).CompareTo(New FiniteDouble(3)) < 0)
        Assert.IsTrue(New FiniteDouble(4).CompareTo(New FiniteDouble(4)) = 0)
    End Sub
End Class
