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

    Public Module SizedExtensions
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
