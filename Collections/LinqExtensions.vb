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

        '''<summary>Concatenates a sequence of sequences into a single sequence.</summary>
        <Pure()> <Extension()>
        Public Function Fold(Of T)(ByVal sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
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

#Region "Transformations"
        '''<summary>Determines the maximum element in a sequence based on the given comparison function.</summary>
        <Extension()> <Pure()>
        Public Function Max(Of T)(ByVal sequence As IEnumerable(Of T),
                                  ByVal comparator As Func(Of T, T, Integer)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(comparator IsNot Nothing)
            Dim any = False
            Dim maxElement As T = Nothing

            For Each e In sequence
                If Not any OrElse comparator(maxElement, e) < 0 Then
                    maxElement = e
                End If
                any = True
            Next e

            Return maxElement
        End Function

        '''<summary>Determines the element with the maximum output from a transformation function.</summary>
        <Pure()> <Extension()>
        Public Function MaxPair(Of TSequence, TComparable As IComparable)(ByVal sequence As IEnumerable(Of TSequence),
                                                                          ByVal transformation As Func(Of TSequence, TComparable),
                                                                          ByRef outElement As TSequence,
                                                                          ByRef outImage As TComparable) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(transformation IsNot Nothing)
            Dim any = False
            Dim maxElement = outElement
            Dim maxImage = outImage

            For Each e In sequence
                Dim f = transformation(e)
                If Not any OrElse f.CompareTo(maxImage) > 0 Then
                    maxElement = e
                    maxImage = f
                End If
                any = True
            Next e

            If any Then
                outElement = maxElement
                outImage = maxImage
            End If
            Return any
        End Function

        '''<summary>Folds a sequence using the given reduction function.</summary>
        <Extension()> <Pure()>
        Public Function ReduceUsing(Of TSource, TResult)(ByVal sequence As IEnumerable(Of TSource),
                                                         ByVal reduction As Func(Of TResult, TSource, TResult),
                                                         Optional ByVal initialValue As TResult = Nothing) As TResult
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(reduction IsNot Nothing)
            Dim accumulator = initialValue
            For Each item In sequence
                accumulator = reduction(accumulator, item)
            Next item
            Return accumulator
        End Function

        '''<summary>Folds a sequence using the given reduction function.</summary>
        <Extension()> <Pure()>
        Public Function ReduceUsing(Of T)(ByVal sequence As IEnumerable(Of T),
                                          ByVal reduction As Func(Of T, T, T),
                                          Optional ByVal initialValue As T = Nothing) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(reduction IsNot Nothing)
            Return ReduceUsing(Of T, T)(sequence, reduction, initialValue)
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
#End Region
    End Module
End Namespace
