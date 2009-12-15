Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values

<TestClass()>
Public Class InvariantStringTest
    <TestMethod()>
    Public Sub EqualityTest()
        Assert.IsTrue(New InvariantString("") = "")
        Assert.IsTrue(New InvariantString() = "")
        Assert.IsTrue(New InvariantString() = New InvariantString())

        Assert.IsTrue(New InvariantString("test") = "test")
        Assert.IsTrue(New InvariantString("test") = "tesT")
        Assert.IsTrue(Not New InvariantString("test") = "tesT2")
        Assert.IsTrue(Not New InvariantString("test") <> "test")
        Assert.IsTrue(Not New InvariantString("test") <> "tesT")
        Assert.IsTrue(New InvariantString("test") <> "tesT2")

        Assert.IsTrue(New InvariantString("test") = New InvariantString("test"))
        Assert.IsTrue(New InvariantString("test") = New InvariantString("tesT"))
        Assert.IsTrue(Not New InvariantString("test") = New InvariantString("tesT2"))
        Assert.IsTrue(Not New InvariantString("test") <> New InvariantString("test"))
        Assert.IsTrue(Not New InvariantString("test") <> New InvariantString("tesT"))
        Assert.IsTrue(New InvariantString("test") <> New InvariantString("tesT2"))

        Assert.IsTrue("test" = New InvariantString("test"))
        Assert.IsTrue("test" = New InvariantString("tesT"))
        Assert.IsTrue(Not "test" = New InvariantString("tesT2"))
        Assert.IsTrue(Not "test" <> New InvariantString("test"))
        Assert.IsTrue(Not "test" <> New InvariantString("tesT"))
        Assert.IsTrue("test" <> New InvariantString("tesT2"))

        Assert.IsTrue(New InvariantString("test").Equals("test"))
        Assert.IsTrue(New InvariantString("test").Equals("tesT"))
        Assert.IsTrue(Not New InvariantString("test").Equals("tesT2"))

        Assert.IsTrue(New InvariantString("test").Equals(New InvariantString("test")))
        Assert.IsTrue(New InvariantString("test").Equals(New InvariantString("tesT")))
        Assert.IsTrue(Not New InvariantString("test").Equals(New InvariantString("tesT2")))
    End Sub

    <TestMethod()>
    Public Sub CompareTest()
        Assert.IsTrue(New InvariantString("a") < New InvariantString("b"))
        Assert.IsTrue(New InvariantString("A") < New InvariantString("b"))
        Assert.IsTrue(New InvariantString("A") < New InvariantString("B"))
        Assert.IsTrue(New InvariantString("a") < New InvariantString("B"))
        Assert.IsTrue(Not New InvariantString("b") < New InvariantString("b"))
        Assert.IsTrue(Not New InvariantString("b") < New InvariantString("B"))
        Assert.IsTrue(Not New InvariantString("c") < New InvariantString("b"))

        Assert.IsTrue(New InvariantString("b") > New InvariantString("a"))
        Assert.IsTrue(New InvariantString("b") > New InvariantString("A"))
        Assert.IsTrue(New InvariantString("B") > New InvariantString("A"))
        Assert.IsTrue(New InvariantString("B") > New InvariantString("a"))
        Assert.IsTrue(Not New InvariantString("b") > New InvariantString("b"))
        Assert.IsTrue(Not New InvariantString("b") > New InvariantString("B"))
        Assert.IsTrue(Not New InvariantString("b") > New InvariantString("c"))

        Assert.IsTrue(New InvariantString("a") <= New InvariantString("b"))
        Assert.IsTrue(New InvariantString("A") <= New InvariantString("b"))
        Assert.IsTrue(New InvariantString("A") <= New InvariantString("B"))
        Assert.IsTrue(New InvariantString("a") <= New InvariantString("B"))
        Assert.IsTrue(New InvariantString("b") <= New InvariantString("b"))
        Assert.IsTrue(New InvariantString("b") <= New InvariantString("B"))
        Assert.IsTrue(Not New InvariantString("c") <= New InvariantString("b"))

        Assert.IsTrue(New InvariantString("b") >= New InvariantString("a"))
        Assert.IsTrue(New InvariantString("b") >= New InvariantString("A"))
        Assert.IsTrue(New InvariantString("B") >= New InvariantString("A"))
        Assert.IsTrue(New InvariantString("B") >= New InvariantString("a"))
        Assert.IsTrue(New InvariantString("b") >= New InvariantString("b"))
        Assert.IsTrue(New InvariantString("b") >= New InvariantString("B"))
        Assert.IsTrue(Not New InvariantString("b") >= New InvariantString("c"))

        Assert.IsTrue(New InvariantString("b").CompareTo(New InvariantString("a")) > 0)
        Assert.IsTrue(New InvariantString("b").CompareTo(New InvariantString("A")) > 0)
        Assert.IsTrue(New InvariantString("B").CompareTo(New InvariantString("a")) > 0)
        Assert.IsTrue(New InvariantString("B").CompareTo(New InvariantString("A")) > 0)
        Assert.IsTrue(New InvariantString("B").CompareTo(New InvariantString("b")) = 0)
        Assert.IsTrue(New InvariantString("b").CompareTo(New InvariantString("b")) = 0)
        Assert.IsTrue(New InvariantString("b").CompareTo(New InvariantString("c")) < 0)

        Assert.IsTrue(New InvariantString("b").CompareTo("a") > 0)
        Assert.IsTrue(New InvariantString("b").CompareTo("A") > 0)
        Assert.IsTrue(New InvariantString("B").CompareTo("a") > 0)
        Assert.IsTrue(New InvariantString("B").CompareTo("A") > 0)
        Assert.IsTrue(New InvariantString("B").CompareTo("b") = 0)
        Assert.IsTrue(New InvariantString("b").CompareTo("b") = 0)
        Assert.IsTrue(New InvariantString("b").CompareTo("c") < 0)
    End Sub

    <TestMethod()>
    Public Sub LikeTest()
        Assert.IsTrue(New InvariantString("") Like "")
        Assert.IsTrue(New InvariantString("a") Like "*")
        Assert.IsTrue(New InvariantString("A") Like "a")
        Assert.IsTrue(New InvariantString("a") Like "A")
        Assert.IsTrue(New InvariantString("A") Like "[aBC]")
        Assert.IsTrue(Not New InvariantString("d") Like "[aBC]")
        Assert.IsTrue(Not New InvariantString("x") Like "??")
        Assert.IsTrue(New InvariantString("xa") Like "??")
    End Sub

    <TestMethod()>
    Public Sub ConversionTest()
        Assert.IsTrue(CType(CType("abcDEF123", InvariantString), String) = "abcDEF123")
        Assert.IsTrue(CType("123aA", InvariantString).Value = "123aA")
    End Sub

    <TestMethod()>
    Public Sub AdditionTest()
        Assert.IsTrue(New InvariantString("a") + "b" = "ab")
        Assert.IsTrue(New InvariantString("a") + New InvariantString() = "A")
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
