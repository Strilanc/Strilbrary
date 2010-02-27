Imports Strilbrary.Enumeration

Namespace Collections
    Public Module LinqExtensions
#Region "Count"
        '''<summary>Determines if a sequence has no elements.</summary>
        <Extension()> <Pure()>
        Public Function None(Of T)(ByVal sequence As IEnumerable(Of T)) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Return Not sequence.Any()
        End Function

        '''<summary>Determines if there are at least as many elements in the sequence as the specified minimum.</summary>
        <Extension()> <Pure()>
        Public Function CountIsAtLeast(Of T)(ByVal sequence As IEnumerable(Of T), ByVal min As Integer) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(min >= 0)
            Return sequence.CountUpTo(min) >= min
        End Function

        '''<summary>Determines if there are less elements in the sequence than the specified maximum.</summary>
        <Extension()> <Pure()>
        Public Function CountIsLessThan(Of T)(ByVal sequence As IEnumerable(Of T), ByVal max As Integer) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(max >= 0)
            Return sequence.CountUpTo(max) < max
        End Function

        '''<summary>Determines if there are at most as many elements in the sequence as the specified maximum.</summary>
        <Extension()> <Pure()>
        Public Function CountIsAtMost(Of T)(ByVal sequence As IEnumerable(Of T), ByVal max As Integer) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(max >= 0)
            Contract.Requires(max < Int32.MaxValue)
            Return sequence.CountUpTo(max + 1) <= max
        End Function

        '''<summary>Determines if there are more elements in the sequence than the specified minimum.</summary>
        <Extension()> <Pure()>
        Public Function CountIsGreaterThan(Of T)(ByVal sequence As IEnumerable(Of T), ByVal min As Integer) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(min >= 0)
            Contract.Requires(min < Int32.MaxValue)
            Return sequence.CountUpTo(min + 1) > min
        End Function

        '''<summary>Counts the number of elements in a sequence, but stops once the specified maximum is reached.</summary>
        <Extension()> <Pure()>
        Public Function CountUpTo(Of T)(ByVal sequence As IEnumerable(Of T), ByVal maxCount As Integer) As Integer
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(maxCount >= 0)
            Contract.Ensures(Contract.Result(Of Integer)() >= 0)
            Contract.Ensures(Contract.Result(Of Integer)() <= maxCount)
            Dim count = 0
            Dim enumerator = sequence.GetEnumerator()
            While count < maxCount AndAlso enumerator.MoveNext
                count += 1
            End While
            Return count
        End Function
#End Region

#Region "Min/Max"
        '''<summary>Determines the maximum element in a sequence based on the given comparison function.</summary>
        <Extension()> <Pure()>
        Public Function Max(Of T)(ByVal sequence As IEnumerable(Of T),
                                  ByVal comparator As Func(Of T, T, Integer)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(comparator IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Aggregate(Function(e1, e2) If(comparator(e1, e2) >= 0, e1, e2))
        End Function
        '''<summary>Determines the minimum element in a sequence based on the given comparison function.</summary>
        <Extension()> <Pure()>
        Public Function Min(Of T)(ByVal sequence As IEnumerable(Of T),
                                  ByVal comparator As Func(Of T, T, Integer)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(comparator IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Aggregate(Function(e1, e2) If(comparator(e1, e2) <= 0, e1, e2))
        End Function

        '''<summary>Determines the maximum element in a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Max(Of T As IComparable(Of T))(ByVal sequence As IEnumerable(Of T)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Max(Function(e) e)
        End Function
        '''<summary>Determines the minimum element in a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Min(Of T As IComparable(Of T))(ByVal sequence As IEnumerable(Of T)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Min(Function(e) e)
        End Function

        '''<summary>Determines the maximum element in a sequence based on the comparable result of a projection.</summary>
        <Pure()> <Extension()>
        <ContractVerification(False)>
        Public Function MaxRelativeTo(Of TInput, TComparable As IComparable(Of TComparable))(
                        ByVal sequence As IEnumerable(Of TInput),
                        ByVal projection As Func(Of TInput, TComparable)) As TInput
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return (From item In sequence
                    Let image = projection(item)
                    ).Max(Function(e1, e2) e1.image.CompareTo(e2.image)
                    ).item
        End Function
        '''<summary>Determines the minimum element in a sequence based on the comparable result of a projection.</summary>
        <Pure()> <Extension()>
        <ContractVerification(False)>
        Public Function MinRelativeTo(Of TInput, TComparable As IComparable(Of TComparable))(
                        ByVal sequence As IEnumerable(Of TInput),
                        ByVal projection As Func(Of TInput, TComparable)) As TInput
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return (From item In sequence
                    Let image = projection(item)
                    ).Min(Function(e1, e2) e1.image.CompareTo(e2.image)
                    ).item
        End Function
#End Region

        '''<summary>Zips the elements of two sequences into a sequence of pairs of elements.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function Zip(Of T1, T2)(ByVal sequence As IEnumerable(Of T1),
                                       ByVal sequence2 As IEnumerable(Of T2)) As IEnumerable(Of Tuple(Of T1, T2))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of T1, T2)))() IsNot Nothing)
            Return Enumerable.Zip(sequence, sequence2, Function(e1, e2) Tuple.Create(e1, e2))
        End Function

        '''<summary>Concatenates a sequence of sequences into a single sequence.</summary>
        <Pure()> <Extension()>
        Public Function Concat(Of T)(ByVal sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return New Enumerable(Of T)(
                Function()
                    Dim e = sequences.GetEnumerator
                    Return New Enumerator(Of T)(Function(controller) If(e.MoveNext,
                                                                        controller.Sequence(e.Current),
                                                                        controller.Break()))
                End Function)
        End Function

        '''<summary>Concatenates an array of sequences into a single sequence.</summary>
        <Pure()>
        Public Function Concat(Of T)(ByVal sequence1 As IEnumerable(Of T),
                                     ByVal sequence2 As IEnumerable(Of T),
                                     ByVal ParamArray sequences() As IEnumerable(Of T)) As IEnumerable(Of T)
            Contract.Requires(sequence1 IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return {sequence1, sequence2}.Concat(sequences).Concat
        End Function

        '''<summary>Appends values to a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Append(Of T)(ByVal sequence As IEnumerable(Of T), ByVal ParamArray values() As T) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return sequence.Concat(values)
        End Function

        '''<summary>Caches all the items in a sequence, preventing changes to the sequence from affecting the resulting sequence.</summary>
        <Extension()>
        Public Function Cache(Of T)(ByVal sequence As IEnumerable(Of T)) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return sequence.ToArray
        End Function

        '''<summary>Partitions a sequence into fixed sized blocks, with the last block potentially being smaller.</summary>
        <Extension()> <Pure()>
        Public Function EnumBlocks(Of T)(ByVal sequence As IEnumerator(Of T),
                                         ByVal blockSize As Integer) As IEnumerator(Of IList(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(blockSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerator(Of IList(Of T)))() IsNot Nothing)
            Return New Enumerator(Of IList(Of T))(
                Function(controller)
                    If Not sequence.MoveNext Then Return controller.Break()

                    Dim block = New List(Of T)(blockSize)
                    block.Add(sequence.Current())
                    While block.Count < blockSize AndAlso sequence.MoveNext
                        block.Add(sequence.Current)
                    End While
                    Return block
                End Function
            )
        End Function

        '''<summary>Partitions a sequence into fixed sized blocks, with the last block potentially being smaller.</summary>
        <Extension()> <Pure()>
        Public Function EnumBlocks(Of T)(ByVal sequence As IEnumerable(Of T),
                                         ByVal blockSize As Integer) As IEnumerable(Of IList(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(blockSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IList(Of T)))() IsNot Nothing)
            Dim blockSize_ = blockSize
            Return sequence.Transform(Function(enumerator) EnumBlocks(enumerator, blockSize_))
        End Function

        '''<summary>Transforms an IEnumerable using a transformation function meant for an IEnumerator.</summary>
        <Extension()> <Pure()>
        Public Function Transform(Of TIn, TOut)(ByVal sequence As IEnumerable(Of TIn),
                                                ByVal transformation As Func(Of IEnumerator(Of TIn), IEnumerator(Of TOut))) As IEnumerable(Of TOut)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(transformation IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TOut))() IsNot Nothing)
            Return New Enumerable(Of TOut)(Function() transformation(sequence.GetEnumerator()))
        End Function
    End Module
End Namespace
