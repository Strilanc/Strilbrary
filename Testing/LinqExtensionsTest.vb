Imports System
Imports System.Numerics
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values
Imports Strilbrary.Collections

<TestClass()>
Public Class LinqExtensionsTest
    Private Shared ReadOnly L0() As Int32 = {}
    Private Shared ReadOnly L1() As Int32 = {1}
    Private Shared ReadOnly L2() As Int32 = {1, 2}
    Private Function SequenceSequenceEqual(Of T)(ByVal s1 As IEnumerable(Of IEnumerable(Of T)), ByVal s2 As IEnumerable(Of IEnumerable(Of T))) As Boolean
        Return Enumerable.Zip(s1, s2, Function(e1, e2) e1.SequenceEqual(e2)).All(Function(x) x)
    End Function

    <TestMethod()>
    Public Sub NoneTest()
        Assert.IsTrue(L0.None)
        Assert.IsTrue(Not L1.None)
        Assert.IsTrue(Not L2.None)
    End Sub

    <TestMethod()>
    Public Sub CountIsAtLeastTest()
        Assert.IsTrue(L0.CountIsAtLeast(0))
        Assert.IsTrue(Not L0.CountIsAtLeast(1))
        Assert.IsTrue(Not L0.CountIsAtLeast(2))
        Assert.IsTrue(L1.CountIsAtLeast(0))
        Assert.IsTrue(L1.CountIsAtLeast(1))
        Assert.IsTrue(Not L1.CountIsAtLeast(2))
        Assert.IsTrue(L2.CountIsAtLeast(0))
        Assert.IsTrue(L2.CountIsAtLeast(1))
        Assert.IsTrue(L2.CountIsAtLeast(2))
    End Sub
    <TestMethod()>
    Public Sub CountIsAtLessThanTest()
        Assert.IsTrue(Not L0.CountIsLessThan(0))
        Assert.IsTrue(L0.CountIsLessThan(1))
        Assert.IsTrue(L0.CountIsLessThan(2))
        Assert.IsTrue(Not L1.CountIsLessThan(0))
        Assert.IsTrue(Not L1.CountIsLessThan(1))
        Assert.IsTrue(L1.CountIsLessThan(2))
        Assert.IsTrue(Not L2.CountIsLessThan(0))
        Assert.IsTrue(Not L2.CountIsLessThan(1))
        Assert.IsTrue(Not L2.CountIsLessThan(2))
    End Sub
    <TestMethod()>
    Public Sub CountIsAtMostTest()
        Assert.IsTrue(L0.CountIsAtMost(0))
        Assert.IsTrue(L0.CountIsAtMost(1))
        Assert.IsTrue(L0.CountIsAtMost(2))
        Assert.IsTrue(Not L1.CountIsAtMost(0))
        Assert.IsTrue(L1.CountIsAtMost(1))
        Assert.IsTrue(L1.CountIsAtMost(2))
        Assert.IsTrue(Not L2.CountIsAtMost(0))
        Assert.IsTrue(Not L2.CountIsAtMost(1))
        Assert.IsTrue(L2.CountIsAtMost(2))
    End Sub
    <TestMethod()>
    Public Sub CountIsGreaterThanTest()
        Assert.IsTrue(Not L0.CountIsGreaterThan(0))
        Assert.IsTrue(Not L0.CountIsGreaterThan(1))
        Assert.IsTrue(Not L0.CountIsGreaterThan(2))
        Assert.IsTrue(L1.CountIsGreaterThan(0))
        Assert.IsTrue(Not L1.CountIsGreaterThan(1))
        Assert.IsTrue(Not L1.CountIsGreaterThan(2))
        Assert.IsTrue(L2.CountIsGreaterThan(0))
        Assert.IsTrue(L2.CountIsGreaterThan(1))
        Assert.IsTrue(Not L2.CountIsGreaterThan(2))
    End Sub
    <TestMethod()>
    Public Sub CountUpToTest()
        Assert.IsTrue(L0.CountUpTo(0) = 0)
        Assert.IsTrue(L0.CountUpTo(1) = 0)
        Assert.IsTrue(L0.CountUpTo(2) = 0)
        Assert.IsTrue(L1.CountUpTo(0) = 0)
        Assert.IsTrue(L1.CountUpTo(1) = 1)
        Assert.IsTrue(L1.CountUpTo(2) = 1)
        Assert.IsTrue(L2.CountUpTo(0) = 0)
        Assert.IsTrue(L2.CountUpTo(1) = 1)
        Assert.IsTrue(L2.CountUpTo(2) = 2)
    End Sub

    <TestMethod()>
    Public Sub MaxComparatorTest()
        Assert.IsTrue({0, 1}.Max(Function(e1, e2) e1.CompareTo(e2)) = 1)
        Assert.IsTrue({2, 1, 3}.Max(Function(e1, e2) e1.CompareTo(e2)) = 3)
        Assert.IsTrue({4, 2, -1, 3}.Max(Function(e1, e2) e1.CompareTo(e2)) = 4)
        Assert.IsTrue({-1}.Max(Function(e1, e2) e1.CompareTo(e2)) = -1)
        Assert.IsTrue({100, 10, 0}.Max(Function(e1, e2) If(e1 = 10, 1, If(e2 = 10, -1, 0))) = 10)
    End Sub
    <TestMethod()>
    Public Sub MinComparatorTest()
        Assert.IsTrue({0, 1}.Min(Function(e1, e2) e1.CompareTo(e2)) = 0)
        Assert.IsTrue({2, 1, 3}.Min(Function(e1, e2) e1.CompareTo(e2)) = 1)
        Assert.IsTrue({4, 2, -1, 3}.Min(Function(e1, e2) e1.CompareTo(e2)) = -1)
        Assert.IsTrue({-1}.Min(Function(e1, e2) e1.CompareTo(e2)) = -1)
        Assert.IsTrue({100, 10, 0}.Min(Function(e1, e2) If(e1 = 10, -1, If(e2 = 10, 1, 0))) = 10)
    End Sub

    <TestMethod()>
    Public Sub MaxDefaultTest()
        Assert.IsTrue({0, 1}.Max = 1)
        Assert.IsTrue({2, 1, 3}.Max = 3)
        Assert.IsTrue({4, 2, -1, 3}.Max = 4)
        Assert.IsTrue({-1}.Max = -1)
    End Sub
    <TestMethod()>
    Public Sub MinDefaultTest()
        Assert.IsTrue({0, 1}.Min = 0)
        Assert.IsTrue({2, 1, 3}.Min = 1)
        Assert.IsTrue({4, 2, -1, 3}.Min = -1)
        Assert.IsTrue({-1}.Min = -1)
    End Sub

    <TestMethod()>
    Public Sub MaxRelativeToTest()
        Assert.IsTrue({0, 1}.MaxRelativeTo(Function(e) e * e) = 1)
        Assert.IsTrue({4, 2, -10, 3}.MaxRelativeTo(Function(e) e * e) = -10)
        Assert.IsTrue({-1}.MaxRelativeTo(Function(e) e * e) = -1)
    End Sub
    <TestMethod()>
    Public Sub MinRelativeToTest()
        Assert.IsTrue({0, 1}.MinRelativeTo(Function(e) e * e) = 0)
        Assert.IsTrue({4, 2, -10, 3}.MinRelativeTo(Function(e) e * e) = 2)
        Assert.IsTrue({-1}.MinRelativeTo(Function(e) e * e) = -1)
    End Sub

    <TestMethod()>
    Public Sub ConcatTest()
        Assert.IsTrue({New Int32() {0, 1}, New Int32() {2, 3}}.Concat.SequenceEqual({0, 1, 2, 3}))
        Assert.IsTrue({New Int32() {0, 1}, New Int32() {2, 3}, New Int32() {4, 5, 6}}.Concat.SequenceEqual({0, 1, 2, 3, 4, 5, 6}))
        Assert.IsTrue({New Int32() {0, 1}, New Int32() {}, New Int32() {4, 5, 6}}.Concat.SequenceEqual({0, 1, 4, 5, 6}))
        Assert.IsTrue(New IEnumerable(Of Int32)() {}.Concat.SequenceEqual({}))
        Assert.IsTrue(Concat({0, 1}, {2}, New Int32() {}, {3}).SequenceEqual({0, 1, 2, 3}))
    End Sub
    <TestMethod()>
    Public Sub AppendTest()
        Assert.IsTrue({0, 1}.Append(2, 3).SequenceEqual({0, 1, 2, 3}))
        Assert.IsTrue({1}.Append().Append(2).SequenceEqual({1, 2}))
    End Sub
    <TestMethod()>
    Public Sub PrependTest()
        Assert.IsTrue({0, 1}.Prepend(2, 3).SequenceEqual({2, 3, 0, 1}))
        Assert.IsTrue({1}.Prepend().Prepend(2).SequenceEqual({2, 1}))
    End Sub

    <TestMethod()>
    Public Sub CacheTest()
        Dim L = New List(Of Int32)
        Dim e1 = L.Cache
        L.Add(1)
        Dim e2 = L.Cache
        L.Add(2)
        Dim e3 = L2.Cache
        L(0) = 5
        Assert.IsTrue(e1.SequenceEqual({}))
        Assert.IsTrue(e2.SequenceEqual({1}))
        Assert.IsTrue(e3.SequenceEqual({1, 2}))
        Assert.IsTrue(L.SequenceEqual({5, 2}))
    End Sub

    <TestMethod()>
    Public Sub RangeTest()
        Assert.IsTrue(5.Range.SequenceEqual({0, 1, 2, 3, 4}))
        Assert.IsTrue(2US.Range.SequenceEqual({0, 1}))
        Assert.IsTrue(CByte(4).Range.SequenceEqual({0, 1, 2, 3}))
        Assert.IsTrue(3UI.Range.SequenceEqual({0, 1, 2}))
    End Sub

    <TestMethod()>
    Public Sub OffsetByTest()
        Assert.IsTrue({2, 3, 5, 7, 2}.OffsetBy(2).SequenceEqual({4, 5, 7, 9, 4}))
    End Sub
    <TestMethod()>
    Public Sub ZipTest()
        Assert.IsTrue({0, 1}.Zip({2, 3}).SequenceEqual({Tuple.Create(0, 2), Tuple.Create(1, 3)}))
        Assert.IsTrue({0, 1, 2}.Zip({2, 3}).SequenceEqual({Tuple.Create(0, 2), Tuple.Create(1, 3)}))
        Assert.IsTrue({0}.Zip({2, 3}).SequenceEqual({Tuple.Create(0, 2)}))
    End Sub
    <TestMethod()>
    Public Sub ZipWithIndexesTest()
        Assert.IsTrue({2, 3, 5, 7, 2}.ZipWithIndexes.SequenceEqual({Tuple.Create(2, 0),
                                                                    Tuple.Create(3, 1),
                                                                    Tuple.Create(5, 2),
                                                                    Tuple.Create(7, 3),
                                                                    Tuple.Create(2, 4)}))
    End Sub
    <TestMethod()>
    Public Sub RepeatedTest()
        Assert.IsTrue(2.Repeated(5).SequenceEqual({2, 2, 2, 2, 2}))
    End Sub
    <TestMethod()>
    Public Sub IndexesOfTest()
        Assert.IsTrue({1, 2, 3, 2, 3, 2, 1}.IndexesOf(2).SequenceEqual({1, 3, 5}))
    End Sub
    <TestMethod()>
    Public Sub InterleavedTest()
        Assert.IsTrue({New Int32() {1, 2, 3, 2}}.Interleaved.SequenceEqual({1, 2, 3, 2}))
        Assert.IsTrue({New Int32() {1, 2, 3, 2}, New Int32() {4, 5}}.Interleaved.SequenceEqual({1, 4, 2, 5, 3, 2}))
        Assert.IsTrue({New Int32() {1, 2, 3, 2}, New Int32() {4}, New Int32() {6, 7}}.Interleaved.SequenceEqual({1, 4, 6, 2, 7, 3, 2}))
    End Sub
    <TestMethod()>
    Public Sub DeinterleavedTest()
        Assert.IsTrue(SequenceSequenceEqual({1, 2, 3, 4, 5, 6, 7}.Deinterleaved(2),
                                            {New Int32() {1, 3, 5, 7}, New Int32() {2, 4, 6}}))
        Assert.IsTrue(SequenceSequenceEqual({1, 2, 3, 4, 5, 6, 7}.Deinterleaved(3),
                                            {New Int32() {1, 4, 7}, New Int32() {2, 5}, New Int32() {3, 6}}))
    End Sub
    <TestMethod()>
    Public Sub PartitionedTest()
        Assert.IsTrue(SequenceSequenceEqual({1, 2, 3, 4, 5, 6, 7}.Partitioned(3),
                                            {New Int32() {1, 2, 3}, New Int32() {4, 5, 6}, New Int32() {7}}))
    End Sub

    <TestMethod()>
    Public Sub ToListTest()
        Assert.IsTrue({1, 2, 3}.ToList.SequenceEqual({1, 2, 3}))
    End Sub
    <TestMethod()>
    Public Sub ToArrayTest()
        Assert.IsTrue({1, 2, 3}.ToArray.SequenceEqual({1, 2, 3}))
    End Sub
    <TestMethod()>
    Public Sub ReverseTest()
        Assert.IsTrue(TypeOf {1, 2, 3}.Reverse Is IList(Of Int32))
        Assert.IsTrue({1, 2, 3}.Reverse.SequenceEqual({3, 2, 1}))
    End Sub
    <TestMethod()>
    Public Sub SubViewTest()
        Assert.IsTrue({1, 2, 3}.AsReadableList.SubView(0).SequenceEqual({1, 2, 3}))
        Assert.IsTrue({1, 2, 3}.AsReadableList.SubView(1).SequenceEqual({2, 3}))
        Assert.IsTrue({1, 2, 3}.AsReadableList.SubView(1, 1).SequenceEqual({2}))
        Assert.IsTrue({1, 2, 3}.AsReadableList.SubView(1, 0).SequenceEqual({}))
    End Sub
End Class
