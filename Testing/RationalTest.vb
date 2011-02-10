Imports System
Imports System.Numerics
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Mathematics

<TestClass()>
Public Class RationalTest
    <TestMethod()>
    Public Sub RationalConstructorTest()
        Dim d = New Rational()
        Dim n0d3 = New Rational(0, 3)
        Dim n1d1 = New Rational(1, 1)
        Dim n2d3 = New Rational(2, 3)
        Dim n3d2 = New Rational(3, 2)
        Dim n15d3 = New Rational(15, 3)
        Dim n4d32 = New Rational(4, 32)
        Dim n1d_1 = New Rational(1, -1)
        Dim n_1d1 = New Rational(-1, 1)
        Dim n_1d_1 = New Rational(-1, -1)
        Dim n6d_6 = New Rational(6, -6)
        Dim n_10d5 = New Rational(-10, 5)

        Assert.IsTrue(d.Numerator = 0 And d.Denominator = 1)
        Assert.IsTrue(n0d3.Numerator = 0 And n0d3.Denominator = 1)
        Assert.IsTrue(n1d1.Numerator = 1 And n1d1.Denominator = 1)
        Assert.IsTrue(n2d3.Numerator = 2 And n2d3.Denominator = 3)
        Assert.IsTrue(n3d2.Numerator = 3 And n3d2.Denominator = 2)
        Assert.IsTrue(n15d3.Numerator = 5 And n15d3.Denominator = 1)
        Assert.IsTrue(n4d32.Numerator = 1 And n4d32.Denominator = 8)
        Assert.IsTrue(n1d_1.Numerator = -1 And n1d_1.Denominator = 1)
        Assert.IsTrue(n_1d1.Numerator = -1 And n_1d1.Denominator = 1)
        Assert.IsTrue(n_1d_1.Numerator = 1 And n_1d_1.Denominator = 1)
        Assert.IsTrue(n6d_6.Numerator = -1 And n6d_6.Denominator = 1)
        Assert.IsTrue(n_10d5.Numerator = -2 And n_10d5.Denominator = 1)
    End Sub

    <TestMethod()>
    Public Sub CompareToTest()
        Assert.IsTrue(New Rational(1, 1).CompareTo(New Rational(0, 1)) > 0)
        Assert.IsTrue(New Rational(0, 1).CompareTo(New Rational(0, 1)) = 0)
        Assert.IsTrue(New Rational(-1, 1).CompareTo(New Rational(0, 1)) < 0)
        Assert.IsTrue(New Rational(3, 5).CompareTo(New Rational(5, 3)) < 0)
        Assert.IsTrue(New Rational(11, 17).CompareTo(New Rational(1, 2)) > 0)
        Assert.IsTrue(New Rational(-501, 17).CompareTo(New Rational(1, 2)) < 0)
        Assert.IsTrue(New Rational(5, -10).CompareTo(New Rational(-1, 2)) = 0)

        Assert.IsTrue(New Rational(1, 1) > New Rational(0, 1))
        Assert.IsTrue(Not New Rational(0, 1) > New Rational(0, 1))
        Assert.IsTrue(Not New Rational(-1, 1) > New Rational(0, 1))
        Assert.IsTrue(Not New Rational(3, 5) > New Rational(5, 3))
        Assert.IsTrue(New Rational(11, 17) > New Rational(1, 2))
        Assert.IsTrue(Not New Rational(-501, 17) > New Rational(1, 2))
        Assert.IsTrue(Not New Rational(5, -10) > New Rational(-1, 2))

        Assert.IsTrue(New Rational(1, 1) >= New Rational(0, 1))
        Assert.IsTrue(New Rational(0, 1) >= New Rational(0, 1))
        Assert.IsTrue(Not New Rational(-1, 1) >= New Rational(0, 1))
        Assert.IsTrue(Not New Rational(3, 5) >= New Rational(5, 3))
        Assert.IsTrue(New Rational(11, 17) >= New Rational(1, 2))
        Assert.IsTrue(Not New Rational(-501, 17) >= New Rational(1, 2))
        Assert.IsTrue(New Rational(5, -10) >= New Rational(-1, 2))

        Assert.IsTrue(Not New Rational(1, 1) < New Rational(0, 1))
        Assert.IsTrue(Not New Rational(0, 1) < New Rational(0, 1))
        Assert.IsTrue(New Rational(-1, 1) < New Rational(0, 1))
        Assert.IsTrue(New Rational(3, 5) < New Rational(5, 3))
        Assert.IsTrue(Not New Rational(11, 17) < New Rational(1, 2))
        Assert.IsTrue(New Rational(-501, 17) < New Rational(1, 2))
        Assert.IsTrue(Not New Rational(5, -10) < New Rational(-1, 2))

        Assert.IsTrue(Not New Rational(1, 1) <= New Rational(0, 1))
        Assert.IsTrue(New Rational(0, 1) <= New Rational(0, 1))
        Assert.IsTrue(New Rational(-1, 1) <= New Rational(0, 1))
        Assert.IsTrue(New Rational(3, 5) <= New Rational(5, 3))
        Assert.IsTrue(Not New Rational(11, 17) <= New Rational(1, 2))
        Assert.IsTrue(New Rational(-501, 17) <= New Rational(1, 2))
        Assert.IsTrue(New Rational(5, -10) <= New Rational(-1, 2))
    End Sub

    <TestMethod()>
    Public Sub EqualsTest()
        Assert.IsTrue(Rational.Zero.Equals(New Rational(0, 1)))
        Assert.IsTrue(Not Rational.Zero.Equals(New Rational(-1, 1)))
        Assert.IsTrue(Rational.One.Equals(New Rational(1, 1)))
        Assert.IsTrue(Rational.One.Equals(New Rational(2, 2)))
        Assert.IsTrue(Not Rational.One.Equals(New Rational(1, 2)))
        Assert.IsTrue(New Rational(1, 2).Equals(New Rational(1, 2)))
        Assert.IsTrue(Not New Rational(1, 2).Equals(New Rational(3, 2)))
        Assert.IsTrue(Not New Rational(-1, 2).Equals(New Rational(1, 2)))
        Assert.IsTrue(New Rational(0, 2).Equals(New Rational(0, -3)))
        Assert.IsTrue(New Rational(0, 2).Equals(New Rational(0, 3)))

        Assert.IsTrue(Rational.Zero = New Rational(0, 1))
        Assert.IsTrue(Not Rational.Zero = New Rational(-1, 1))
        Assert.IsTrue(Rational.One = New Rational(1, 1))
        Assert.IsTrue(Rational.One = New Rational(2, 2))
        Assert.IsTrue(Not Rational.One = New Rational(1, 2))
        Assert.IsTrue(New Rational(1, 2) = New Rational(1, 2))
        Assert.IsTrue(Not New Rational(1, 2) = New Rational(3, 2))
        Assert.IsTrue(Not New Rational(-1, 2) = New Rational(1, 2))
        Assert.IsTrue(New Rational(0, 2) = New Rational(0, -3))
        Assert.IsTrue(New Rational(0, 2) = New Rational(0, 3))

        Assert.IsTrue(Not Rational.Zero <> New Rational(0, 1))
        Assert.IsTrue(Rational.Zero <> New Rational(-1, 1))
        Assert.IsTrue(Not Rational.One <> New Rational(1, 1))
        Assert.IsTrue(Not Rational.One <> New Rational(2, 2))
        Assert.IsTrue(Rational.One <> New Rational(1, 2))
        Assert.IsTrue(Not New Rational(1, 2) <> New Rational(1, 2))
        Assert.IsTrue(New Rational(1, 2) <> New Rational(3, 2))
        Assert.IsTrue(New Rational(-1, 2) <> New Rational(1, 2))
        Assert.IsTrue(Not New Rational(0, 2) <> New Rational(0, -3))
        Assert.IsTrue(Not New Rational(0, 2) <> New Rational(0, 3))

        Assert.IsTrue(Not Rational.One.Equals(New Object()))
        Assert.IsTrue(Rational.One.Equals(DirectCast(Rational.One, Object)))
        Assert.IsTrue(Not Rational.One.Equals(DirectCast(Rational.Zero, Object)))
    End Sub

    <TestMethod()>
    Public Sub GetHashCodeTest()
        Assert.IsTrue(Rational.One.GetHashCode() = New Rational(5, 5).GetHashCode())
    End Sub

    <TestMethod()>
    Public Sub ToStringTest()
        Assert.IsTrue(Rational.Zero.ToString() = "0")
        Assert.IsTrue(Rational.One.ToString() = "1")
        Assert.IsTrue(New Rational(1, 2).ToString() = "1/2")
        Assert.IsTrue(New Rational(-1, 2).ToString() = "-1/2")
        Assert.IsTrue(New Rational(-1, 1).ToString() = "-1")
    End Sub

    <TestMethod()>
    Public Sub op_AdditionTest()
        Assert.IsTrue(Rational.One + Rational.One = New Rational(2, 1))
        Assert.IsTrue(Rational.One + Rational.Zero = Rational.One)
        Assert.IsTrue(Rational.One + New Rational(-1, 1) = Rational.Zero)
        Assert.IsTrue(New Rational(2, 5) + New Rational(-3, 7) = New Rational(-1, 35))
    End Sub
    <TestMethod()>
    Public Sub op_SubtractionTest()
        Assert.IsTrue(Rational.One - Rational.One = Rational.Zero)
        Assert.IsTrue(Rational.One - Rational.Zero = Rational.One)
        Assert.IsTrue(Rational.One - New Rational(-1, 1) = New Rational(2, 1))
        Assert.IsTrue(New Rational(2, 5) - New Rational(-3, 7) = New Rational(29, 35))
        Assert.IsTrue(New Rational(-3, 7) - New Rational(2, 5) = New Rational(-29, 35))
    End Sub

    <TestMethod()>
    Public Sub op_DivisionTest()
        Assert.IsTrue(Rational.One / Rational.One = Rational.One)
        Assert.IsTrue(Rational.Zero / Rational.One = Rational.Zero)
        Assert.IsTrue(New Rational(-7, 2) / New Rational(3, 5) = New Rational(-35, 6))
    End Sub
    <TestMethod()>
    Public Sub op_ModulusTest()
        Assert.IsTrue(5 Mod New Rational(2, 3) = New Rational(1, 3))
        Assert.IsTrue(1 Mod Rational.One = 0)
    End Sub
    <TestMethod()>
    Public Sub op_MultiplyTest()
        Assert.IsTrue(5 * Rational.One = 5)
        Assert.IsTrue(New Rational(2, 3) * New Rational(5, 7) = New Rational(10, 21))
        Assert.IsTrue(New Rational(-2, 3) * New Rational(5, 7) = New Rational(-10, 21))
    End Sub

    <TestMethod()>
    Public Sub op_ImplicitTest()
        Assert.IsTrue(CSByte(1) = Rational.One)
        Assert.IsTrue(CByte(1) = Rational.One)
        Assert.IsTrue(1S = Rational.One)
        Assert.IsTrue(1US = Rational.One)
        Assert.IsTrue(1 = Rational.One)
        Assert.IsTrue(1UI = Rational.One)
        Assert.IsTrue(1L = Rational.One)
        Assert.IsTrue(1UL = Rational.One)
        Assert.IsTrue(BigInteger.One = Rational.One)

        Assert.IsTrue(CSByte(0) = Rational.Zero)
        Assert.IsTrue(CByte(0) = Rational.Zero)
        Assert.IsTrue(0S = Rational.Zero)
        Assert.IsTrue(0US = Rational.Zero)
        Assert.IsTrue(0 = Rational.Zero)
        Assert.IsTrue(0UI = Rational.Zero)
        Assert.IsTrue(0L = Rational.Zero)
        Assert.IsTrue(0UL = Rational.Zero)
        Assert.IsTrue(BigInteger.Zero = Rational.Zero)
    End Sub

    <TestMethod()>
    Public Sub op_UnaryNegationTest()
        Assert.IsTrue(-Rational.One = New Rational(-1, 1))
        Assert.IsTrue(-Rational.Zero = Rational.Zero)
        Assert.IsTrue(-New Rational(-3, 5) = New Rational(3, 5))
    End Sub

    <TestMethod()>
    Public Sub AbsTest()
        Assert.IsTrue(Rational.Zero.Abs() = 0)
        Assert.IsTrue(Rational.One.Abs() = 1)
        Assert.IsTrue((-Rational.One).Abs() = 1)
        Assert.IsTrue(New Rational(3, 5).Abs() = New Rational(3, 5))
        Assert.IsTrue(New Rational(-2, 5).Abs() = New Rational(2, 5))
    End Sub
    <TestMethod()>
    Public Sub SignTest()
        Assert.IsTrue(Rational.Zero.Sign() = 0)
        Assert.IsTrue(Rational.One.Sign() > 0)
        Assert.IsTrue((-Rational.One).Sign() < 0)
        Assert.IsTrue(New Rational(3, 5).Sign() > 0)
        Assert.IsTrue(New Rational(-2, 5).Sign() < 0)
    End Sub

    <TestMethod()>
    Public Sub CeilingTest()
        Assert.IsTrue(Rational.One.Ceiling() = 1)
        Assert.IsTrue(Rational.Zero.Ceiling() = 0)
        Assert.IsTrue(New Rational(-1, 1).Ceiling() = -1)
        Assert.IsTrue(New Rational(7, 5).Ceiling() = 2)
        Assert.IsTrue(New Rational(2, 5).Ceiling() = 1)
        Assert.IsTrue(New Rational(-7, 5).Ceiling() = -1)
        Assert.IsTrue(New Rational(-2, 5).Ceiling() = 0)
    End Sub
    <TestMethod()>
    Public Sub FloorTest()
        Assert.IsTrue(Rational.One.Floor() = 1)
        Assert.IsTrue(Rational.Zero.Floor() = 0)
        Assert.IsTrue(New Rational(-1, 1).Floor() = -1)
        Assert.IsTrue(New Rational(7, 5).Floor() = 1)
        Assert.IsTrue(New Rational(2, 5).Floor() = 0)
        Assert.IsTrue(New Rational(-7, 5).Floor() = -2)
        Assert.IsTrue(New Rational(-2, 5).Floor() = -1)
    End Sub
    <TestMethod()>
    Public Sub TruncateTest()
        Assert.IsTrue(Rational.One.Truncate() = 1)
        Assert.IsTrue(Rational.Zero.Truncate() = 0)
        Assert.IsTrue(New Rational(-1, 1).Truncate() = -1)
        Assert.IsTrue(New Rational(7, 5).Truncate() = 1)
        Assert.IsTrue(New Rational(2, 5).Truncate() = 0)
        Assert.IsTrue(New Rational(-7, 5).Truncate() = -1)
        Assert.IsTrue(New Rational(-2, 5).Truncate() = 0)
    End Sub
End Class
