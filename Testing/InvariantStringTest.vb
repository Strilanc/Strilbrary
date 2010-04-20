Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class InvariantStringTest
    <TestMethod()>
    Public Sub ConstructorTest()
        Assert.IsTrue(New InvariantString("test").Value = "test")
        Assert.IsTrue(New InvariantString("TEST").Value = "TEST")
        Assert.IsTrue("test".ToInvariant.Value = "test")
        Assert.IsTrue("TEST".ToInvariant.Value = "TEST")
    End Sub
    <TestMethod()>
    Public Sub ToStringTest()
        Assert.IsTrue(New InvariantString("test").ToString = "test")
        Assert.IsTrue(New InvariantString("TEST").ToString = "TEST")
    End Sub

    <TestMethod()>
    Public Sub EqualityTest()
        Assert.IsTrue([Default](Of InvariantString)() = [Default](Of InvariantString)())
        Assert.IsTrue([Default](Of InvariantString)() = "")
        Assert.IsTrue([Default](Of InvariantString)() = "".ToInvariant)
        Assert.IsTrue("".ToInvariant = "")

        Assert.IsTrue("test".ToInvariant = "test")
        Assert.IsTrue("test".ToInvariant = "tesT")
        Assert.IsTrue(Not "test".ToInvariant = "tesT2")
        Assert.IsTrue(Not "test".ToInvariant <> "test")
        Assert.IsTrue(Not "test".ToInvariant <> "tesT")
        Assert.IsTrue("test".ToInvariant <> "tesT2")

        Assert.IsTrue("test".ToInvariant = "test".ToInvariant)
        Assert.IsTrue("test".ToInvariant = "tesT".ToInvariant)
        Assert.IsTrue(Not "test".ToInvariant = "tesT2".ToInvariant)
        Assert.IsTrue(Not "test".ToInvariant <> "test".ToInvariant)
        Assert.IsTrue(Not "test".ToInvariant <> "tesT".ToInvariant)
        Assert.IsTrue("test".ToInvariant <> "tesT2".ToInvariant)

        Assert.IsTrue("test" = "test".ToInvariant)
        Assert.IsTrue("test" = "tesT".ToInvariant)
        Assert.IsTrue(Not "test" = "tesT2".ToInvariant)
        Assert.IsTrue(Not "test" <> "test".ToInvariant)
        Assert.IsTrue(Not "test" <> "tesT".ToInvariant)
        Assert.IsTrue("test" <> "tesT2".ToInvariant)

        Assert.IsTrue("test".ToInvariant.Equals("test"))
        Assert.IsTrue("test".ToInvariant.Equals("tesT"))
        Assert.IsTrue(Not "test".ToInvariant.Equals("tesT2"))

        Assert.IsTrue("test".ToInvariant.Equals("test".ToInvariant))
        Assert.IsTrue("test".ToInvariant.Equals("tesT".ToInvariant))
        Assert.IsTrue(Not "test".ToInvariant.Equals("tesT2".ToInvariant))
    End Sub

    <TestMethod()>
    Public Sub CompareTest()
        Assert.IsTrue("a".ToInvariant < "b".ToInvariant)
        Assert.IsTrue("A".ToInvariant < "b".ToInvariant)
        Assert.IsTrue("A".ToInvariant < "B".ToInvariant)
        Assert.IsTrue("a".ToInvariant < "B".ToInvariant)
        Assert.IsTrue(Not "b".ToInvariant < "b".ToInvariant)
        Assert.IsTrue(Not "b".ToInvariant < "B".ToInvariant)
        Assert.IsTrue(Not "c".ToInvariant < "b".ToInvariant)

        Assert.IsTrue("b".ToInvariant > "a".ToInvariant)
        Assert.IsTrue("b".ToInvariant > "A".ToInvariant)
        Assert.IsTrue("B".ToInvariant > "A".ToInvariant)
        Assert.IsTrue("B".ToInvariant > "a".ToInvariant)
        Assert.IsTrue(Not "b".ToInvariant > "b".ToInvariant)
        Assert.IsTrue(Not "b".ToInvariant > "B".ToInvariant)
        Assert.IsTrue(Not "b".ToInvariant > "c".ToInvariant)

        Assert.IsTrue("a".ToInvariant <= "b".ToInvariant)
        Assert.IsTrue("A".ToInvariant <= "b".ToInvariant)
        Assert.IsTrue("A".ToInvariant <= "B".ToInvariant)
        Assert.IsTrue("a".ToInvariant <= "B".ToInvariant)
        Assert.IsTrue("b".ToInvariant <= "b".ToInvariant)
        Assert.IsTrue("b".ToInvariant <= "B".ToInvariant)
        Assert.IsTrue(Not "c".ToInvariant <= "b".ToInvariant)

        Assert.IsTrue("b".ToInvariant >= "a".ToInvariant)
        Assert.IsTrue("b".ToInvariant >= "A".ToInvariant)
        Assert.IsTrue("B".ToInvariant >= "A".ToInvariant)
        Assert.IsTrue("B".ToInvariant >= "a".ToInvariant)
        Assert.IsTrue("b".ToInvariant >= "b".ToInvariant)
        Assert.IsTrue("b".ToInvariant >= "B".ToInvariant)
        Assert.IsTrue(Not "b".ToInvariant >= "c".ToInvariant)

        Assert.IsTrue("b".ToInvariant.CompareTo("a".ToInvariant) > 0)
        Assert.IsTrue("b".ToInvariant.CompareTo("A".ToInvariant) > 0)
        Assert.IsTrue("B".ToInvariant.CompareTo("a".ToInvariant) > 0)
        Assert.IsTrue("B".ToInvariant.CompareTo("A".ToInvariant) > 0)
        Assert.IsTrue("B".ToInvariant.CompareTo("b".ToInvariant) = 0)
        Assert.IsTrue("b".ToInvariant.CompareTo("b".ToInvariant) = 0)
        Assert.IsTrue("b".ToInvariant.CompareTo("c".ToInvariant) < 0)

        Assert.IsTrue("b".ToInvariant.CompareTo("a") > 0)
        Assert.IsTrue("b".ToInvariant.CompareTo("A") > 0)
        Assert.IsTrue("B".ToInvariant.CompareTo("a") > 0)
        Assert.IsTrue("B".ToInvariant.CompareTo("A") > 0)
        Assert.IsTrue("B".ToInvariant.CompareTo("b") = 0)
        Assert.IsTrue("b".ToInvariant.CompareTo("b") = 0)
        Assert.IsTrue("b".ToInvariant.CompareTo("c") < 0)
    End Sub

    <TestMethod()>
    Public Sub LikeTest()
        Assert.IsTrue("".ToInvariant Like "")
        Assert.IsTrue("a".ToInvariant Like "*")
        Assert.IsTrue("A".ToInvariant Like "a")
        Assert.IsTrue("a".ToInvariant Like "A")
        Assert.IsTrue("A".ToInvariant Like "[aBC]")
        Assert.IsTrue(Not "d".ToInvariant Like "[aBC]")
        Assert.IsTrue(Not "x".ToInvariant Like "??")
        Assert.IsTrue("xa".ToInvariant Like "??")

        Assert.IsTrue("" Like "".ToInvariant)
        Assert.IsTrue("a" Like "*".ToInvariant)
        Assert.IsTrue("A" Like "a".ToInvariant)
        Assert.IsTrue("a" Like "A".ToInvariant)
        Assert.IsTrue("A" Like "[aBC]".ToInvariant)
        Assert.IsTrue(Not "d" Like "[aBC]".ToInvariant)
        Assert.IsTrue(Not "x" Like "??".ToInvariant)
        Assert.IsTrue("xa" Like "??".ToInvariant)

        Assert.IsTrue("".ToInvariant Like "".ToInvariant)
        Assert.IsTrue("a".ToInvariant Like "*".ToInvariant)
        Assert.IsTrue("A".ToInvariant Like "a".ToInvariant)
        Assert.IsTrue("a".ToInvariant Like "A".ToInvariant)
        Assert.IsTrue("A".ToInvariant Like "[aBC]".ToInvariant)
        Assert.IsTrue(Not "d".ToInvariant Like "[aBC]".ToInvariant)
        Assert.IsTrue(Not "x".ToInvariant Like "??".ToInvariant)
        Assert.IsTrue("xa".ToInvariant Like "??".ToInvariant)
    End Sub

    <TestMethod()>
    Public Sub ConversionTest()
        Assert.IsTrue(CType(CType("abcDEF123", InvariantString), String) = "abcDEF123")
        Assert.IsTrue(CType("123aA", InvariantString).Value = "123aA")
    End Sub

    <TestMethod()>
    Public Sub AdditionTest()
        Assert.IsTrue("a".ToInvariant + "b" = "ab")
        Assert.IsTrue("a".ToInvariant + New InvariantString() = "A")
        Assert.IsTrue("" + New InvariantString("bC") = "bc")
    End Sub

    <TestMethod()>
    Public Sub LengthTest()
        Assert.IsTrue(New InvariantString("abc").Length = 3)
        Assert.IsTrue(New InvariantString().Length = 0)
    End Sub

    <TestMethod()>
    Public Sub StartsWithTest()
        Assert.IsTrue(New InvariantString("123").StartsWith("123"))
        Assert.IsTrue(New InvariantString("abc").StartsWith("Ab"))
        Assert.IsTrue(Not New InvariantString("abc").StartsWith("b"))
    End Sub
    <TestMethod()>
    Public Sub EndsWithTest()
        Assert.IsTrue(New InvariantString("123").EndsWith("123"))
        Assert.IsTrue(New InvariantString("abc").EndsWith("bC"))
        Assert.IsTrue(Not New InvariantString("abc").EndsWith("b"))
    End Sub
    <TestMethod()>
    Public Sub SubstringTest()
        Assert.IsTrue(New InvariantString("123").Substring(2) = "3")
        Assert.IsTrue(New InvariantString("123").Substring(2, 1) = "3")
        Assert.IsTrue(New InvariantString("123").Substring(1, 1) = "2")
        Assert.IsTrue(New InvariantString("123").Substring(1, 2) = "23")
        Assert.IsTrue(New InvariantString("123").Substring(1) = "23")
        Assert.IsTrue(New InvariantString("123").Substring(0) = "123")
    End Sub
End Class
