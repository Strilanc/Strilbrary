Namespace Enumeration
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
        Public Function MaxPair(Of T, C As IComparable)(ByVal sequence As IEnumerable(Of T),
                                                        ByVal transformation As Func(Of T, C),
                                                        ByRef outElement As T,
                                                        ByRef outImage As C) As Boolean
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
            Dim sequence_ = sequence
            Dim blockSize_ = blockSize
            Return New Enumerator(Of IList(Of T))(
                Function(controller)
                    Contract.Requires(controller IsNot Nothing)
                    Contract.Assume(sequence_ IsNot Nothing)
                    Contract.Assume(blockSize_ > 0)
                    If Not sequence_.MoveNext Then  Return controller.Break()

                    Dim block = New List(Of T)(blockSize_)
                    block.Add(sequence_.Current())
                    While block.Count < blockSize_ AndAlso sequence_.MoveNext
                        block.Add(sequence_.Current)
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
            Return sequence.Transform(Function(enumerator)
                                          Contract.Requires(enumerator IsNot Nothing)
                                          Contract.Assume(enumerator IsNot Nothing)
                                          Contract.Assume(blockSize_ > 0)
                                          Return EnumBlocks(enumerator, blockSize_)
                                      End Function)
        End Function

        '''<summary>Transforms an IEnumerable using a transformation function meant for an IEnumerator.</summary>
        <Extension()> <Pure()>
        Public Function Transform(Of T, D)(ByVal sequence As IEnumerable(Of T),
                                           ByVal transformation As Func(Of IEnumerator(Of T), IEnumerator(Of D))) As IEnumerable(Of D)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(transformation IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of D))() IsNot Nothing)
            Dim sequence_ = sequence
            Dim transformation_ = transformation
            Return New Enumerable(Of D)(Function()
                                            Contract.Assume(transformation_ IsNot Nothing)
                                            Contract.Assume(sequence_ IsNot Nothing)
                                            Return transformation_(sequence_.GetEnumerator())
                                        End Function)
        End Function
#End Region

#Region "IList"
        '''<summary>Creates a list containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function ToList(Of T)(ByVal list As IList(Of T)) As List(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of List(Of T))() IsNot Nothing)
            Dim ret As New List(Of T)(list.Count)
            For i = 0 To list.Count - 1
                ret.Add(list(i))
            Next i
            Return ret
        End Function

        '''<summary>Creates an array containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function ToArray(Of T)(ByVal list As IList(Of T)) As T()
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Dim ret(0 To list.Count - 1) As T
            For i = 0 To list.Count - 1
                ret(i) = list(i)
            Next i
            Return ret
        End Function

        '''<summary>Creates an array containing all the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function SubToArray(Of T)(ByVal list As IList(Of T), ByVal offset As Integer) As T()
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(offset <= list.Count)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Return list.SubToArray(offset, list.Count - offset)
        End Function

        '''<summary>Creates an array containing a contiguous subset of the elements of an IList.</summary>
        <Extension()> <Pure()>
        Public Function SubToArray(Of T)(ByVal list As IList(Of T), ByVal offset As Integer, ByVal count As Integer) As T()
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(count >= 0)
            Contract.Requires(offset <= list.Count - count)
            Contract.Ensures(Contract.Result(Of T())() IsNot Nothing)
            Dim ret(0 To count - 1) As T
            For i = 0 To count - 1
                ret(i) = list(i + offset)
            Next i
            Return ret
        End Function

        '''<summary>Creates an IList with the same elements as the given IList, but in reversed order.</summary>
        <Extension()> <Pure()>
        Public Function Reverse(Of T)(ByVal list As IList(Of T)) As IList(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IList(Of T))() IsNot Nothing)
            Dim n = list.Count
            Dim ret(0 To n - 1) As T
            For i = 0 To n - 1
                Contract.Assume(i >= 0)
                Contract.Assume(i < n)
                ret(i) = list(n - i - 1)
            Next i
            Return ret
        End Function
#End Region
    End Module
End Namespace