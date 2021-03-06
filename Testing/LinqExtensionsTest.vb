﻿Imports System
Imports System.Numerics
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Values
Imports Strilbrary.Collections
Imports LinqToCollections.List

<TestClass()>
Public Class LinqExtensionsTest
    Private Shared ReadOnly L0 As IEnumerable(Of Int32) = From x In New Int32() {}
    Private Shared ReadOnly L1 As IEnumerable(Of Int32) = From x In {1}
    Private Shared ReadOnly L2 As IEnumerable(Of Int32) = From x In {1, 2}
    Private Shared ReadOnly LInf As IEnumerable(Of Int32) = Strilbrary.Collections.RepeatForever(0)

    Private Function SequenceSequenceEqual(Of T)(s1 As IEnumerable(Of IEnumerable(Of T)), s2 As IEnumerable(Of IEnumerable(Of T))) As Boolean
        Return Enumerable.Zip(s1, s2, Function(e1, e2) e1.SequenceEqual(e2)).All(Function(x) x)
    End Function
    Private Function Array(Of T)(ParamArray vals() As T) As T()
        Return vals
    End Function

    <TestMethod()>
    Public Sub NoneTest()
        Assert.IsTrue(L0.None)
        Assert.IsTrue(Not L1.None)
        Assert.IsTrue(Not L2.None)
    End Sub

    <TestMethod()>
    Public Sub LazyCountTest()
        Assert.IsTrue(LInf.LazyCount() >= 5)
        Assert.IsTrue(Not LInf.LazyCount() < 52)

        Assert.IsTrue(L0.LazyCount() >= 0)
        Assert.IsTrue(Not L0.LazyCount() >= 1)
        Assert.IsTrue(Not L0.LazyCount() >= 2)
        Assert.IsTrue(L1.LazyCount() >= 0)
        Assert.IsTrue(L1.LazyCount() >= 1)
        Assert.IsTrue(Not L1.LazyCount() >= 2)
        Assert.IsTrue(L2.LazyCount() >= 0)
        Assert.IsTrue(L2.LazyCount() >= 1)
        Assert.IsTrue(L2.LazyCount() >= 2)

        Assert.IsTrue(Not L0.LazyCount() < 0)
        Assert.IsTrue(L0.LazyCount() < 1)
        Assert.IsTrue(L0.LazyCount() < 2)
        Assert.IsTrue(Not L1.LazyCount() < 0)
        Assert.IsTrue(Not L1.LazyCount() < 1)
        Assert.IsTrue(L1.LazyCount() < 2)
        Assert.IsTrue(Not L2.LazyCount() < 0)
        Assert.IsTrue(Not L2.LazyCount() < 1)
        Assert.IsTrue(Not L2.LazyCount() < 2)

        Assert.IsTrue(L0.LazyCount() <= 0)
        Assert.IsTrue(L0.LazyCount() <= 1)
        Assert.IsTrue(L0.LazyCount() <= 2)
        Assert.IsTrue(Not L1.LazyCount() <= 0)
        Assert.IsTrue(L1.LazyCount() <= 1)
        Assert.IsTrue(L1.LazyCount() <= 2)
        Assert.IsTrue(Not L2.LazyCount() <= 0)
        Assert.IsTrue(Not L2.LazyCount() <= 1)
        Assert.IsTrue(L2.LazyCount() <= 2)

        Assert.IsTrue(Not L0.LazyCount() > 0)
        Assert.IsTrue(Not L0.LazyCount() > 1)
        Assert.IsTrue(Not L0.LazyCount() > 2)
        Assert.IsTrue(L1.LazyCount() > 0)
        Assert.IsTrue(Not L1.LazyCount() > 1)
        Assert.IsTrue(Not L1.LazyCount() > 2)
        Assert.IsTrue(L2.LazyCount() > 0)
        Assert.IsTrue(L2.LazyCount() > 1)
        Assert.IsTrue(Not L2.LazyCount() > 2)

        Assert.IsTrue(L0.LazyCount().CountTo(0) = 0)
        Assert.IsTrue(L0.LazyCount().CountTo(1) = 0)
        Assert.IsTrue(L0.LazyCount().CountTo(2) = 0)
        Assert.IsTrue(L1.LazyCount().CountTo(0) = 0)
        Assert.IsTrue(L1.LazyCount().CountTo(1) = 1)
        Assert.IsTrue(L1.LazyCount().CountTo(2) = 1)
        Assert.IsTrue(L2.LazyCount().CountTo(0) = 0)
        Assert.IsTrue(L2.LazyCount().CountTo(1) = 1)
        Assert.IsTrue(L2.LazyCount().CountTo(2) = 2)

        Assert.IsTrue(L0.LazyCount().CountPast(0) = 0)
        Assert.IsTrue(L0.LazyCount().CountPast(1) = 0)
        Assert.IsTrue(L0.LazyCount().CountPast(2) = 0)
        Assert.IsTrue(L1.LazyCount().CountPast(0) = 1)
        Assert.IsTrue(L1.LazyCount().CountPast(1) = 1)
        Assert.IsTrue(L1.LazyCount().CountPast(2) = 1)
        Assert.IsTrue(L2.LazyCount().CountPast(0) = 1)
        Assert.IsTrue(L2.LazyCount().CountPast(1) = 2)
        Assert.IsTrue(L2.LazyCount().CountPast(2) = 2)
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
    Public Sub MaxTest()
        Assert.IsTrue(Strilbrary.Collections.LinqExtensions.Max({1, 3, 2}) = 3)
        Assert.IsTrue(Strilbrary.Collections.LinqExtensions.Max({1, 3.0, Double.PositiveInfinity}) = Double.PositiveInfinity)
    End Sub
    <TestMethod()>
    Public Sub MinTest()
        Assert.IsTrue(Strilbrary.Collections.LinqExtensions.Min({1, 3, 2}) = 1)
        Assert.IsTrue(Strilbrary.Collections.LinqExtensions.Min({3, 1, 2}) = 1)
        Assert.IsTrue(Strilbrary.Collections.LinqExtensions.Min({1, 3.0, Double.PositiveInfinity}) = 1)
    End Sub
    <TestMethod()>
    Public Sub ConcatRistTest()
        Assert.IsTrue({1, 2, 3}.ToRist().Concat({4, 5}.ToRist()).SequenceEqual({1, 2, 3, 4, 5}))
        Assert.IsTrue({1, 2, 3}.ToRist().Concat({4, 5}.ToRist())(4) = 5)
    End Sub
    <TestMethod()>
    Public Sub AppendRistTest()
        Assert.IsTrue({1, 2, 3}.ToRist().Append(4, 5).SequenceEqual({1, 2, 3, 4, 5}))
        Assert.IsTrue({1, 2, 3}.ToRist().Append(4, 5)(4) = 5)
    End Sub
    <TestMethod()>
    Public Sub PreppendRistTest()
        Assert.IsTrue({1, 2, 3}.ToRist().Prepend(4, 5).SequenceEqual({4, 5, 1, 2, 3}))
        Assert.IsTrue({1, 2, 3}.ToRist().Prepend(4, 5)(4) = 3)
    End Sub
    <TestMethod()>
    Public Sub DistinctByTest()
        Assert.IsTrue({1, 2, 3, 7, 10, 4, 5, 2, 7, 5, 11}.DistinctBy(Function(e) e Mod 5).SequenceEqual({1, 2, 3, 10, 4}))
    End Sub
    <TestMethod()>
    Public Sub DuplicatesByTest()
        Assert.IsTrue({1, 2, 3, 7, 10, 4, 5, 2, 7, 5, 11}.DuplicatesBy(Function(e) e Mod 5).SequenceEqual({7, 5, 2, 7, 5, 11}))
    End Sub
    <TestMethod()>
    Public Sub DuplicatesTest()
        Assert.IsTrue({1, 2, 3, 7, 10, 4, 5, 2, 7, 5, 11}.Duplicates().SequenceEqual({2, 7, 5}))
    End Sub
    <TestMethod()>
    Public Sub PaddedToTest()
        Assert.IsTrue({1, 2, 3}.PaddedTo(7, 2).SequenceEqual({1, 2, 3, 2, 2, 2, 2}))
        Assert.IsTrue({1, 2, 3}.PaddedTo(1, 2).SequenceEqual({1, 2, 3}))
        Assert.IsTrue({1, 2, 3}.PaddedTo(3, 2).SequenceEqual({1, 2, 3}))
    End Sub
    <TestMethod()>
    Public Sub IndexOfTest()
        Assert.IsTrue({1, 2, 3}.AsEnumerable().IndexOf(4) Is Nothing)
        Assert.IsTrue({1, 2, 3}.AsEnumerable().IndexOf(2).Value = 1)
    End Sub
    <TestMethod()>
    Public Sub DeinterleavedRistTest()
        Assert.IsTrue({1, 2, 3}.ToRist().Deinterleaved(2).Count = 2)
        Assert.IsTrue({1, 2, 3}.ToRist().Deinterleaved(2)(0).SequenceEqual({1, 3}))
        Assert.IsTrue({1, 2, 3}.ToRist().Deinterleaved(2)(1).SequenceEqual({2}))
        Assert.IsTrue({1, 2, 3, 4, 5}.ToRist().Deinterleaved(3)(1).SequenceEqual({2, 5}))
    End Sub
    <TestMethod()>
    Public Sub StepRistTest()
        Assert.IsTrue({1, 2, 3, 4, 5}.ToRist().Step(2).SequenceEqual({1, 3, 5}))
        Assert.IsTrue({1, 2, 3, 4, 5}.ToRist().Step(3).SequenceEqual({1, 4}))
        Assert.IsTrue({1, 2, 3, 4, 5}.ToRist().Step(1).SequenceEqual({1, 2, 3, 4, 5}))
    End Sub
    <TestMethod()>
    Public Sub PartitionedRistTest()
        Assert.IsTrue({1, 2, 3}.ToRist().Partitioned(2).Count = 2)
        Assert.IsTrue({1, 2, 3}.ToRist().Partitioned(2)(0).SequenceEqual({1, 2}))
        Assert.IsTrue({1, 2, 3}.ToRist().Partitioned(2)(1).SequenceEqual({3}))
        Assert.IsTrue({1, 2, 3, 4, 5}.ToRist().Partitioned(3)(1).SequenceEqual({4, 5}))
    End Sub
    <TestMethod()>
    Public Sub MakeRistTest()
        Assert.IsTrue(MakeRist(1, 2, 3).SequenceEqual({1, 2, 3}))
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
        Assert.IsTrue({0, 1}.MaxBy(Function(e) e * e) = 1)
        Assert.IsTrue({4, 2, -10, 3}.MaxBy(Function(e) e * e) = -10)
        Assert.IsTrue({-1}.MaxBy(Function(e) e * e) = -1)
    End Sub
    <TestMethod()>
    Public Sub MinRelativeToTest()
        Assert.IsTrue({0, 1}.MinBy(Function(e) e * e) = 0)
        Assert.IsTrue({4, 2, -10, 3}.MinBy(Function(e) e * e) = 2)
        Assert.IsTrue({-1}.MinBy(Function(e) e * e) = -1)
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
        Assert.IsTrue(2US.Range.SequenceEqual({0, 1}))
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
    Public Sub PartialAggregatesTest()
        Assert.IsTrue(New Int32() {}.AggregateBack(0, Function(acc, e) acc + e).SequenceEqual({}))
        Assert.IsTrue({1, 2, 3, 4, 5}.AggregateBack(0, Function(acc, e) acc + e).SequenceEqual({1, 3, 6, 10, 15}))
        Assert.IsTrue({1, 2, 3, 4, 5, 0}.AggregateBack(1, Function(acc, e) acc * e).SequenceEqual({1, 2, 6, 24, 120, 0}))
    End Sub
    <TestMethod()>
    Public Sub SkipLastTest()
        Assert.IsTrue({1, 2, 3, 4, 5}.SkipLast(0).SequenceEqual({1, 2, 3, 4, 5}))
        Assert.IsTrue({1, 2, 3, 4, 5}.SkipLast(1).SequenceEqual({1, 2, 3, 4}))
        Assert.IsTrue({1, 2, 3, 4, 5}.SkipLast(2).SequenceEqual({1, 2, 3}))
        Assert.IsTrue({1, 2, 3, 4, 5}.SkipLast(5).SequenceEqual({}))
        Assert.IsTrue({1, 2, 3, 4, 5}.SkipLast(6).SequenceEqual({}))
    End Sub
    <TestMethod()>
    Public Sub TakeLastTest()
        Assert.IsTrue({1, 2, 3, 4, 5}.TakeLast(0).SequenceEqual({}))
        Assert.IsTrue({1, 2, 3, 4, 5}.TakeLast(1).SequenceEqual({5}))
        Assert.IsTrue({1, 2, 3, 4, 5}.TakeLast(2).SequenceEqual({4, 5}))
        Assert.IsTrue({1, 2, 3, 4, 5}.TakeLast(5).SequenceEqual({1, 2, 3, 4, 5}))
        Assert.IsTrue({1, 2, 3, 4, 5}.TakeLast(6).SequenceEqual({1, 2, 3, 4, 5}))
    End Sub

    <TestMethod()>
    Public Sub RepeatForeverTest()
        Assert.IsTrue(5.RepeatForever.Take(100).SequenceEqual(Enumerable.Repeat(5, 100)))
    End Sub

    <TestMethod()>
    Public Sub ZipWithPartialAggregatesTest()
        Assert.IsTrue({1, 2, 3, 4, 5}.ZipAggregateBack(0, Function(acc, e) acc + e).SequenceEqual({1, 2, 3, 4, 5}.Zip({1, 3, 6, 10, 15})))
        Assert.IsTrue(New Int32() {}.ZipAggregateBack(0, Function(acc, e) acc + e).SequenceEqual({}))
    End Sub

    <TestMethod()>
    Public Sub FirstOrNullableDefaultTest()
        Assert.IsTrue({2}.FirstOrNullableDefault.Value = 2)
        Assert.IsTrue({1, 2, 3}.FirstOrNullableDefault.Value = 1)
        Assert.IsTrue(Not New Int32() {}.FirstOrNullableDefault.HasValue)
    End Sub
    <TestMethod()>
    Public Sub FirstOrDefaultTest()
        Assert.IsTrue({2}.FirstOrDefault([default]:=3) = 2)
        Assert.IsTrue({1, 2, 3}.FirstOrDefault([default]:=4) = 1)
        Assert.IsTrue(New Int32() {}.FirstOrDefault([default]:=5) = 5)
        Assert.IsTrue(New Int32() {}.FirstOrDefault([default]:=6) = 6)
    End Sub

    <TestMethod()>
    Public Sub StepTest()
        Assert.IsTrue(5UI.Range().Step(2).SequenceEqual({0, 2, 4}))
        Assert.IsTrue(6UI.Range().Step(2).SequenceEqual({0, 2, 4}))
        Assert.IsTrue(10UI.Range().Step(1).SequenceEqual(10UI.Range()))
        Assert.IsTrue(New UInt32() {}.Step(2).SequenceEqual({}))
        Assert.IsTrue(New UInt32() {}.Step(1).SequenceEqual({}))
        Assert.IsTrue(5UI.Range().Step(3).SequenceEqual({0, 3}))
        Assert.IsTrue(5UI.Range().Step(4).SequenceEqual({0, 4}))
    End Sub

    <TestMethod()>
    Public Sub RollTest()
        Assert.IsTrue(SequenceSequenceEqual(Of UInt32)(5UI.Range().Roll(2), Array({0UI, 1UI}, {1UI, 2UI}, {2UI, 3UI}, {3UI, 4UI})))
        Assert.IsTrue(SequenceSequenceEqual(Of UInt32)(5UI.Range().Roll(1), Array({0UI}, {1UI}, {2UI}, {3UI}, {4UI})))
        Assert.IsTrue(SequenceSequenceEqual(Of UInt32)(5UI.Range().Roll(6), Array(Of UInt32())()))
        Assert.IsTrue(SequenceSequenceEqual(Of UInt32)(5UI.Range().Roll(5), Array(5UI.Range())))
    End Sub
End Class
