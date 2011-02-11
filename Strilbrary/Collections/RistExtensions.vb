Imports Strilbrary.Values

Namespace Collections
    '''<summary>Implements a readable list based on given delegates.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public Class Rist(Of T)
        Implements IRist(Of T)

        Private ReadOnly _count As Int32
        Private ReadOnly _getter As Func(Of Int32, T)
        Private ReadOnly _iterator As IEnumerable(Of T)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_count >= 0)
            Contract.Invariant(_getter IsNot Nothing)
            Contract.Invariant(_iterator IsNot Nothing)
        End Sub

        Public Sub New(ByVal getter As Func(Of Int32, T),
                       ByVal count As Int32,
                       Optional ByVal efficientIterator As IEnumerable(Of T) = Nothing)
            Contract.Requires(getter IsNot Nothing)
            Contract.Requires(count >= 0)
            Contract.Ensures(Me.Count = count)
            Me._getter = getter
            Me._count = count
            Me._iterator = If(efficientIterator, Iterator Function()
                                                     For i = 0 To _count - 1
                                                         Yield _getter(i)
                                                     Next i
                                                 End Function())
            Contract.Assume(Me.Count = count)
            Contract.Assume(_iterator IsNot Nothing)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICounted.Count
            Get
                Return _count
            End Get
        End Property
        Default Public ReadOnly Property Item(ByVal index As Integer) As T Implements IRist(Of T).Item
            Get
                Return _getter(index)
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return _iterator.GetEnumerator()
        End Function

        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Overrides Function ToString() As String
            Const MaxItems As Integer = 10
            Return "Count: {0}, Items: [{1}{2}".Frmt(Me.Count,
                                                     Me.Take(MaxItems).StringJoin(", "),
                                                     If(Me.Count <= MaxItems, "]", ", ..."))
        End Function
    End Class

    Public Module IndexedLinqExtensions
        '''<summary>Exposes a list as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function AsRist(Of T)(ByVal list As IList(Of T)) As IRist(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = list.Count)
            Return New Rist(Of T)(getter:=Function(i) list(i),
                                  count:=list.Count,
                                  efficientIterator:=list)
        End Function
        '''<summary>Creates a copy of the given sequence and exposes the copy as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function ToRist(Of T)(ByVal sequence As IEnumerable(Of T)) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Return sequence.ToArray().AsRist()
        End Function
        '''<summary>Creates a copy of the given sequence, unless it is already an IRist, and exposes it as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function AsRist(Of T)(ByVal sequence As IEnumerable(Of T)) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Return If(TryCast(sequence, IRist(Of T)), sequence.ToRist())
        End Function

        '''<summary>Wraps a readable list in a list view.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of ListView(Of T))().Count = list.Count")>
        Private Function AsView(Of T)(ByVal list As IRist(Of T)) As ListView(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ListView(Of T))().Count = list.Count)
            Return If(TryCast(list, ListView(Of T)), New ListView(Of T)(list, 0, list.Count))
        End Function
        '''<summary>Returns a list containing a contiguous subset of the given list starting at the given offset.</summary>
        <Extension()> <Pure()>
        Public Function SubView(Of T)(ByVal list As IRist(Of T), ByVal offset As Integer) As IRist(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(offset <= list.Count)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = list.Count - offset)
            Return list.AsView().SubView(offset)
        End Function
        '''<summary>Returns a list containing a contiguous subset of the given list starting at the given offset and running for the given length.</summary>
        <Extension()> <Pure()>
        Public Function SubView(Of T)(ByVal list As IRist(Of T), ByVal offset As Integer, ByVal length As Integer) As IRist(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(offset + length <= list.Count)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = length)
            Return list.AsView().SubView(offset, length)
        End Function

        '''<summary>Wraps a caching layer around a readable list.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IRist(Of T))().Count = sequence.Count")>
        Public Function WithCaching(Of T)(ByVal sequence As IRist(Of T)) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = sequence.Count)
            Dim cache(0 To sequence.Count - 1) As Values.Maybe(Of T)
            Return New Rist(Of T)(
                getter:=Function(i)
                            If Not cache(i).HasValue Then cache(i) = sequence(i)
                            Return cache(i).Value
                        End Function,
                count:=sequence.Count)
        End Function

        '''<summary>Exposes the projected items of a readable list as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function [Select](Of TIn, TOut)(ByVal sequence As IRist(Of TIn), ByVal projection As Func(Of TIn, TOut)) As IRist(Of TOut)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of TOut))() IsNot Nothing)
            Return New Rist(Of TOut)(
                getter:=Function(i) projection(sequence(i)),
                count:=sequence.Count,
                efficientIterator:=sequence.AsEnumerable().Select(projection))
        End Function

        '''<summary>Exposes the projected items of a pair of readable lists as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function Zip(Of TIn1, TIn2, TOut)(ByVal sequence1 As IRist(Of TIn1),
                                                 ByVal sequence2 As IRist(Of TIn2),
                                                 ByVal projection As Func(Of TIn1, TIn2, TOut)) As IRist(Of TOut)
            Contract.Requires(sequence1 IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of TOut))() IsNot Nothing)
            Return New Rist(Of TOut)(
                getter:=Function(i) projection(sequence1(i), sequence2(i)),
                count:=Math.Min(sequence1.Count, sequence2.Count),
                efficientIterator:=sequence1.AsEnumerable().Zip(sequence2, projection))
        End Function

        <Pure()> <Extension()>
        Public Function Take(Of T)(ByVal rist As IRist(Of T), ByVal maxTakeCount As Int32) As IRist(Of T)
            Contract.Requires(rist IsNot Nothing)
            Contract.Requires(maxTakeCount >= 0)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = Math.Min(rist.Count, maxTakeCount))
            Return New Rist(Of T)(getter:=Function(i) rist(i),
                                  count:=Math.Min(rist.Count, maxTakeCount))
        End Function
        <Pure()> <Extension()>
        Public Function Skip(Of T)(ByVal rist As IRist(Of T), ByVal maxSkipCount As Int32) As IRist(Of T)
            Contract.Requires(rist IsNot Nothing)
            Contract.Requires(maxSkipCount >= 0)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = rist.Count - Math.Min(rist.Count, maxSkipCount))
            Return New Rist(Of T)(getter:=Function(i) rist(i + maxSkipCount),
                                  count:=rist.Count - Math.Min(rist.Count, maxSkipCount))
        End Function
        <Pure()> <Extension()>
        Public Function TakeLastExact(Of T)(ByVal rist As IRist(Of T), ByVal exactTakeCount As Int32) As IRist(Of T)
            Contract.Requires(rist IsNot Nothing)
            Contract.Requires(exactTakeCount >= 0)
            Contract.Requires(exactTakeCount <= rist.Count)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = exactTakeCount)
            Return New Rist(Of T)(getter:=Function(i) rist(rist.Count - exactTakeCount + i),
                                  count:=exactTakeCount)
        End Function
        <Pure()> <Extension()>
        Public Function SkipLastExact(Of T)(ByVal rist As IRist(Of T), ByVal exactSkipCount As Int32) As IRist(Of T)
            Contract.Requires(rist IsNot Nothing)
            Contract.Requires(exactSkipCount >= 0)
            Contract.Requires(exactSkipCount <= rist.Count)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = rist.Count - exactSkipCount)
            Return New Rist(Of T)(getter:=Function(i) rist(i),
                                  count:=rist.Count - exactSkipCount)
        End Function
        <Pure()> <Extension()>
        Public Function TakeLast(Of T)(ByVal rist As IRist(Of T), ByVal maxTakeCount As Int32) As IRist(Of T)
            Contract.Requires(rist IsNot Nothing)
            Contract.Requires(maxTakeCount >= 0)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = Math.Min(rist.Count, maxTakeCount))
            Return New Rist(Of T)(getter:=Function(i) rist(rist.Count - Math.Min(rist.Count, maxTakeCount) + i),
                                  count:=Math.Min(rist.Count, maxTakeCount))
        End Function
        <Pure()> <Extension()>
        Public Function SkipLast(Of T)(ByVal rist As IRist(Of T), ByVal maxSkipCount As Int32) As IRist(Of T)
            Contract.Requires(rist IsNot Nothing)
            Contract.Requires(maxSkipCount >= 0)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = rist.Count - Math.Min(rist.Count, maxSkipCount))
            Return New Rist(Of T)(getter:=Function(i) rist(i),
                                  count:=rist.Count - Math.Min(rist.Count, maxSkipCount))
        End Function
    End Module
End Namespace
