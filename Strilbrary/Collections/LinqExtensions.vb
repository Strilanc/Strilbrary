﻿Namespace Collections
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

        '''<summary>Concatenates a sequence of sequences into a single sequence.</summary>
        <Pure()> <Extension()>
        Public Function Concat(Of T)(ByVal sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return From sequence In sequences
                   From item In sequence
                   Select item
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

        '''<summary>Prepends values to a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Prepend(Of T)(ByVal sequence As IEnumerable(Of T), ByVal ParamArray values() As T) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return values.Concat(sequence)
        End Function

        '''<summary>Caches all the items in a sequence, preventing changes to the sequence from affecting the resulting sequence.</summary>
        <Extension()>
        Public Function Cache(Of T)(ByVal sequence As IEnumerable(Of T)) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return sequence.ToArray
        End Function

        '''<summary>Determines the sequence of values less than the given limit, starting at 0 and incrementing.</summary>
        <Pure()> <Extension()>
        Public Function Range(ByVal limit As Int32) As IEnumerable(Of Int32)
            Contract.Requires(limit >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Int32))() IsNot Nothing)
            Return Enumerable.Range(0, limit)
        End Function
        '''<summary>Determines the sequence of values less than the given limit, starting at 0 and incrementing.</summary>
        <Pure()> <Extension()>
        Public Function Range(ByVal limit As UInt32) As IEnumerable(Of UInt32)
            Contract.Requires(limit >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of UInt32))() IsNot Nothing)
            Return From i In CInt(limit).Range Select CUInt(i)
        End Function
        '''<summary>Determines the sequence of values less than the given limit, starting at 0 and incrementing.</summary>
        <Pure()> <Extension()>
        Public Function Range(ByVal limit As UInt16) As IEnumerable(Of UInt16)
            Contract.Requires(limit >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of UInt16))() IsNot Nothing)
            Return From i In CInt(limit).Range Select CUShort(i)
        End Function
        '''<summary>Determines the sequence of values less than the given limit, starting at 0 and incrementing.</summary>
        <Pure()> <Extension()>
        Public Function Range(ByVal limit As Byte) As IEnumerable(Of Byte)
            Contract.Requires(limit >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Byte))() IsNot Nothing)
            Return From i In CInt(limit).Range Select CByte(i)
        End Function

        '''<summary>Enumerates items in the sequence, offset by the given amount.</summary>
        <Pure()> <Extension()>
        Public Function OffsetBy(ByVal sequence As IEnumerable(Of Int32), ByVal offset As Int32) As IEnumerable(Of Int32)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Int32))() IsNot Nothing)
            Return From i In sequence Select i + offset
        End Function

        '''<summary>Zips the elements of two sequences into a sequence of tuples.</summary>
        <Pure()> <Extension()>
        <ContractVerification(False)>
        Public Function Zip(Of T1, T2)(ByVal sequence As IEnumerable(Of T1),
                                       ByVal sequence2 As IEnumerable(Of T2)) As IEnumerable(Of Tuple(Of T1, T2))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of T1, T2)))() IsNot Nothing)
            Return Enumerable.Zip(sequence, sequence2, Function(e1, e2) Tuple.Create(e1, e2))
        End Function

        '''<summary>Zips a sequence's elements with their positions in the sequence.</summary>
        <Pure()> <Extension()>
        Public Function ZipWithIndexes(Of T)(ByVal sequence As IEnumerable(Of T)) As IEnumerable(Of Tuple(Of T, Integer))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of T, Integer)))() IsNot Nothing)
            Return sequence.Zip(Int32.MaxValue.Range)
        End Function

        '''<summary>Returns a sequence consisting of a repeated value.</summary>
        <Pure()> <Extension()>
        <ContractVerification(False)>
        Public Function Repeated(Of T)(ByVal value As T, ByVal count As Integer) As IEnumerable(Of T)
            Contract.Requires(count >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))().Count = count)
            Return Enumerable.Repeat(value, count)
        End Function

        '''<summary>Pads a sequence to a given minimum length.</summary>
        <Pure()> <Extension()>
        Public Function PaddedTo(Of T)(ByVal sequence As IEnumerable(Of T),
                                       ByVal minimumCount As Integer,
                                       Optional ByVal paddingValue As T = Nothing) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(minimumCount >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return sequence.Concat(paddingValue.Repeated(Math.Max(0, minimumCount - sequence.Count)))
        End Function

        '''<summary>Determines the positions of items in the sequence equivalent to the given value.</summary>
        <Pure()> <Extension()>
        Public Function IndexesOf(Of T)(ByVal sequence As IEnumerable(Of T), ByVal value As T) As IEnumerable(Of Integer)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Integer))() IsNot Nothing)
            Return From pair In sequence.ZipWithIndexes
                   Let item = pair.Item1
                   Let position = pair.Item2
                   Where value.Equals(item)
                   Select position
        End Function

        '''<summary>Interleaves the items of multiple sequences into a single sequence.</summary>
        <Pure()> <Extension()>
        <ContractVerification(False)>
        Public Function Interleaved(Of T)(ByVal sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Dim result = New List(Of T)
            Dim enumerators = (From sequence In sequences Select sequence.GetEnumerator).ToList
            Dim index = 0
            Do
                While Not enumerators(index).MoveNext
                    enumerators.RemoveAt(index)
                    If enumerators.Count = 0 Then Exit Do
                    index = index Mod enumerators.Count
                End While

                result.Add(enumerators(index).Current)
                index = (index + 1) Mod enumerators.Count
            Loop
            Return result
        End Function

        '''<summary>Groups a sequence based on the position of items (modulo the given sequence count).</summary>
        <Pure()> <Extension()>
        Public Function Deinterleaved(Of T)(ByVal sequence As IEnumerable(Of T), ByVal sequenceCount As Integer) As IEnumerable(Of IEnumerable(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequenceCount > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing)
            Return sequence.ZipWithIndexes.GroupBy(keySelector:=Function(pair) pair.Item2 Mod sequenceCount,
                                                   elementSelector:=Function(pair) pair.Item1).PaddedTo(sequenceCount)
        End Function

        '''<summary>Splits a sequence into continuous segments of a given size (with the last partition possibly being smaller).</summary>
        <Pure()> <Extension()>
        Public Function Partitioned(Of T)(ByVal sequence As IEnumerable(Of T),
                                          ByVal partitionSize As Integer) As IEnumerable(Of IEnumerable(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(partitionSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing)
            Return sequence.ZipWithIndexes.GroupBy(keySelector:=Function(e) e.Item2 \ partitionSize,
                                                   elementSelector:=Function(e) e.Item1)
        End Function
    End Module
End Namespace