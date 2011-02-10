Imports Strilbrary.Values

Namespace Collections
    '''<summary>Augments an IEnumerable with a count.</summary>
    Friend Class SizedEnumerable(Of T)
        Implements ISizedEnumerable(Of T)

        Private ReadOnly _count As Int32
        Private ReadOnly _iterator As IEnumerable(Of T)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_count >= 0)
            Contract.Invariant(_iterator IsNot Nothing)
        End Sub

        Public Sub New(ByVal iterator As IEnumerable(Of T),
                       ByVal count As Int32)
            Contract.Requires(iterator IsNot Nothing)
            Contract.Requires(count >= 0)
            Me._iterator = iterator
            Me._count = count
        End Sub

        Public ReadOnly Property Count As Integer Implements ICounted.Count
            Get
                Return _count
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Dim iterCount = 0
            For Each item In _iterator
                iterCount += 1
                If iterCount > _count Then Exit For
                Yield item
            Next item
            If iterCount <> _count Then
                Throw New Exceptions.InvalidStateException("Number of items in sequence doesn't match count.")
            End If
        End Function
        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class

    '''<summary>Implements a readable list based on given delegates.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public Class Rist(Of T)
        Implements IRist(Of T)

        Private ReadOnly _counter As Func(Of Int32)
        Private ReadOnly _getter As Func(Of Int32, T)
        Private ReadOnly _iterator As IEnumerable(Of T)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_counter IsNot Nothing)
            Contract.Invariant(_getter IsNot Nothing)
            Contract.Invariant(_iterator IsNot Nothing)
        End Sub

        Public Sub New(ByVal getter As Func(Of Int32, T),
                       ByVal counter As Func(Of Int32),
                       Optional ByVal efficientIterator As IEnumerable(Of T) = Nothing)
            Contract.Requires(getter IsNot Nothing)
            Contract.Requires(counter IsNot Nothing)
            Me._getter = getter
            Me._counter = counter
            Me._iterator = If(efficientIterator, Iterator Function()
                                                     For i = 0 To _counter() - 1
                                                         Yield _getter(i)
                                                     Next i
                                                 End Function())
            Contract.Assume(_iterator IsNot Nothing)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICounted.Count
            Get
                Dim result = _counter()
                If result < 0 Then Throw New Exceptions.InvalidStateException("Invalid count.")
                Return result
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
            Dim r = New Rist(Of T)(getter:=Function(i) list(i),
                                   counter:=Function() list.Count,
                                   efficientIterator:=list)
            Contract.Assume(r.Count = list.Count)
            Return r
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
        ''' <summary>
        ''' Exposes a sequence as a readable list.
        ''' The exposed list is lazily cached as items are requested, or delegates directly if the underlying sequence is a list.
        ''' Not safe to access from multiple threads.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function AsRistLazy(Of T)(ByVal sequence As IEnumerable(Of T)) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)

            Dim asRist = TryCast(sequence, IRist(Of T))
            If asRist IsNot Nothing Then Return asRist

            Dim asList = TryCast(sequence, IList(Of T))
            If asList IsNot Nothing Then Return asList.AsRist()

            Dim knownCount = sequence.TryFastCount()
            Dim buffer = New List(Of T)()
            Dim finished = False
            Dim iter = sequence.GetEnumerator()
            Return New Rist(Of T)(
                getter:=Function(i)
                            If Not finished Then
                                Do
                                    If buffer.Count > i Then Exit Do
                                    If Not iter.MoveNext() Then
                                        finished = True
                                        iter.Dispose()
                                        Exit Do
                                    End If
                                    buffer.Add(iter.Current())
                                Loop
                            End If
                            Return buffer(i)
                        End Function,
                counter:=Function()
                             If knownCount.HasValue Then Return knownCount.Value
                             If Not finished Then
                                 Try
                                     While iter.MoveNext()
                                         buffer.Add(iter.Current())
                                     End While
                                     finished = True
                                 Finally
                                     iter.Dispose()
                                 End Try
                             End If
                             Return buffer.Count
                         End Function,
                efficientIterator:=sequence)
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
                counter:=Function() sequence.Count)
        End Function

        '''<summary>Exposes the projected items of a readable list as a readable list.</summary>
        <Extension()> <Pure()>
        Public Function [Select](Of TIn, TOut)(ByVal sequence As IRist(Of TIn), ByVal projection As Func(Of TIn, TOut)) As IRist(Of TOut)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of TOut))() IsNot Nothing)
            Return New Rist(Of TOut)(
                getter:=Function(i) projection(sequence(i)),
                counter:=Function() sequence.Count,
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
                counter:=Function() Math.Min(sequence1.Count, sequence2.Count),
                efficientIterator:=sequence1.AsEnumerable().Zip(sequence2, projection))
        End Function

        '''<summary>Exposes the projected items of a sized sequence as a sized sequence.</summary>
        <Extension()> <Pure()>
        Public Function [Select](Of TIn, TOut)(ByVal sequence As ISizedEnumerable(Of TIn),
                                               ByVal projection As Func(Of TIn, TOut)) As ISizedEnumerable(Of TOut)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ISizedEnumerable(Of TOut))() IsNot Nothing)
            Return New SizedEnumerable(Of TOut)(
                count:=sequence.Count,
                iterator:=sequence.AsEnumerable().Select(projection))
        End Function

        '''<summary>Exposes the projected items of a pair of sized sequences as a sized sequence.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of ISizedEnumerable(Of TOut))().Count = Math.Min(sequence1.Count, sequence2.Count)")>
        Public Function Zip(Of TIn1, TIn2, TOut)(ByVal sequence1 As ISizedEnumerable(Of TIn1),
                                                 ByVal sequence2 As ISizedEnumerable(Of TIn2),
                                                 ByVal projection As Func(Of TIn1, TIn2, TOut)) As ISizedEnumerable(Of TOut)
            Contract.Requires(sequence1 IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ISizedEnumerable(Of TOut))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ISizedEnumerable(Of TOut))().Count = Math.Min(sequence1.Count, sequence2.Count))
            Return New SizedEnumerable(Of TOut)(
                count:=Math.Min(sequence1.Count, sequence2.Count),
                iterator:=sequence1.AsEnumerable().Zip(sequence2, projection))
        End Function

        '''<summary>Exposes the intermediate results of aggregating a sized sequence as a sized sequence.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of ISizedEnumerable(Of TAccumulate))().Count = sequence.Count")>
        Public Function PartialAggregates(Of TValue, TAccumulate)(ByVal sequence As ISizedEnumerable(Of TValue),
                                                                  ByVal seed As TAccumulate,
                                                                  ByVal func As Func(Of TAccumulate, TValue, TAccumulate)) As ISizedEnumerable(Of TAccumulate)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ISizedEnumerable(Of TAccumulate))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ISizedEnumerable(Of TAccumulate))().Count = sequence.Count)

            Return New SizedEnumerable(Of TAccumulate)(
                count:=sequence.Count,
                iterator:=sequence.AsEnumerable().PartialAggregates(seed, func))
        End Function
    End Module
End Namespace
