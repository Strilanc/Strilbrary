Imports Strilbrary.Values

Namespace Collections
    Public Module LinqExtensions
        '''<summary>Determines if a sequence has no elements.</summary>
        <Extension()> <Pure()>
        Public Function None(Of T)(ByVal sequence As IEnumerable(Of T)) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Return Not sequence.Any()
        End Function

        '''<summary>Returns a <see cref="LazyCounter" /> for the number of elements in the given sequence.</summary>
        <Extension()> <Pure()>
        Public Function LazyCount(ByVal sequence As IEnumerable) As LazyCounter
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of LazyCounter)() IsNot Nothing)
            Return New LazyCounter(sequence.GetEnumerator())
        End Function

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
        <SuppressMessage("Microsoft.Contracts", "Nonnull-89-0")>
        <SuppressMessage("Microsoft.Contracts", "Requires-48-84")>
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
        <SuppressMessage("Microsoft.Contracts", "Nonnull-89-0")>
        <SuppressMessage("Microsoft.Contracts", "Requires-48-84")>
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
        <ContractVerification(False)>
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
            Return sequence.Select(Function(item, index) Tuple.Create(item, index))
        End Function

        '''<summary>Returns a sequence consisting of a value repeated a specified number of times.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of T))().Count = count")>
        Public Function Repeated(Of T)(ByVal value As T, ByVal count As Integer) As IEnumerable(Of T)
            Contract.Requires(count >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))().Count = count)
            Return Enumerable.Repeat(value, count)
        End Function
        '''<summary>Returns a never-ending sequence consisting of a repeated value.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TValue))() IsNot Nothing")>
        Public Function RepeatForever(Of TValue)(ByVal value As TValue) As IEnumerable(Of TValue)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TValue))() IsNot Nothing)
            Return Iterator Function()
                       Do
                           Yield value
                       Loop
                   End Function()
        End Function

        '''<summary>Enumerates all contiguous subsequences of the given size from the given sequence.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of IReadableList(Of T)))() IsNot Nothing")>
        Public Function Roll(Of T)(ByVal sequence As IEnumerable(Of T), ByVal windowSize As Integer) As IEnumerable(Of IReadableList(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(windowSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IReadableList(Of T)))() IsNot Nothing)

            Return Iterator Function()
                       Dim rollingWindow = New Queue(Of T)(capacity:=windowSize)
                       For Each item In sequence
                           rollingWindow.Enqueue(item)
                           If rollingWindow.Count >= windowSize Then
                               Yield rollingWindow.ToReadableList()
                               rollingWindow.Dequeue()
                           End If
                       Next item
                   End Function()
        End Function

        '''<summary>Pads a sequence to a given minimum length.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of T))() IsNot Nothing")>
        Public Function PaddedTo(Of T)(ByVal sequence As IEnumerable(Of T),
                                       ByVal minimumCount As Integer,
                                       Optional ByVal paddingValue As T = Nothing) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(minimumCount >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Return Iterator Function()
                       Dim count = 0
                       For Each item In sequence
                           count += 1
                           Yield item
                       Next item
                       While count < minimumCount
                           count += 1
                           Yield paddingValue
                       End While
                   End Function()
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
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of T))() IsNot Nothing")>
        Public Function Interleaved(Of T)(ByVal sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Return Iterator Function()
                       Dim enumerators = (From sequence In sequences
                                          Select sequence.GetEnumerator
                                          ).ToArray
                       Do
                           Dim used = False
                           For Each enumerator In enumerators
                               If Not enumerator.MoveNext() Then Continue For
                               used = True
                               Yield enumerator.Current
                           Next enumerator
                           If Not used Then Exit Do
                       Loop
                   End Function()
        End Function

        '''<summary>Groups a sequence based on the position of items (modulo the given sequence count).</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing")>
        Public Function Deinterleaved(Of T)(ByVal sequence As IEnumerable(Of T), ByVal sequenceCount As Integer) As IEnumerable(Of IEnumerable(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequenceCount > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing)

            Return Iterator Function()
                       For i = 0 To sequenceCount - 1
                           Yield sequence.Skip(i).Step(sequenceCount)
                       Next i
                   End Function()
        End Function

        '''<summary>Selects every nth item in a sequence, starting with the first item.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of T))() IsNot Nothing")>
        Public Function [Step](Of T)(ByVal sequence As IEnumerable(Of T), ByVal stepSize As Integer) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(stepSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Return Iterator Function()
                       Dim left = 1
                       For Each item In sequence
                           left -= 1
                           If left > 0 Then Continue For
                           left = stepSize
                           Yield item
                       Next item
                   End Function()
        End Function

        '''<summary>Splits a sequence into continuous segments of a given size (with the last partition possibly being smaller).</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing")>
        Public Function Partitioned(Of T)(ByVal sequence As IEnumerable(Of T),
                                          ByVal partitionSize As Integer) As IEnumerable(Of IEnumerable(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(partitionSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing)

            Return Iterator Function()
                       Dim r = New List(Of T)(capacity:=partitionSize)
                       For Each item In sequence
                           r.Add(item)
                           If r.Count = partitionSize Then
                               Yield r
                               r = New List(Of T)(capacity:=partitionSize)
                           End If
                       Next item
                       If r.Count > 0 Then Yield r
                   End Function()
        End Function

        '''<summary>Returns the last specified number of items in a sequence, or the entire sequence if there are fewer items than the specified number.</summary>
        <Extension()>
        Public Function TakeLast(Of T)(ByVal sequence As IEnumerable(Of T),
                                       ByVal count As Integer) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(count >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Dim tail = New Queue(Of T)
            For Each item In sequence
                tail.Enqueue(item)
                If tail.Count > count Then tail.Dequeue()
            Next item
            Return tail
        End Function
        '''<summary>Returns all but the last specified number of items in a sequence, or no items if there are fewer items than the specified number.</summary>
        <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of T))() IsNot Nothing")>
        Public Function SkipLast(Of T)(ByVal sequence As IEnumerable(Of T),
                                       ByVal count As Integer) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(count >= 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Return Iterator Function()
                       Dim tail = New Queue(Of T)(capacity:=count + 1)
                       For Each item In sequence
                           tail.Enqueue(item)
                           If tail.Count <= count Then Continue For
                           Yield tail.Dequeue()
                       Next item
                   End Function()
        End Function

        ''' <summary>
        ''' Returns the intermediate results of applying an accumulator function over a sequence.
        ''' The specified seed is used as the initial accumulator value, and is not included in the results.
        ''' </summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TAccumulate))() IsNot Nothing")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TAccumulate))().Count = sequence.Count")>
        Public Function PartialAggregates(Of TValue, TAccumulate)(ByVal sequence As IEnumerable(Of TValue),
                                                                  ByVal seed As TAccumulate,
                                                                  ByVal func As Func(Of TAccumulate, TValue, TAccumulate)) As IEnumerable(Of TAccumulate)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TAccumulate))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TAccumulate))().Count = sequence.Count)

            Return Iterator Function()
                       Dim accumulator = seed
                       For Each item In sequence
                           accumulator = func(accumulator, item)
                           Yield accumulator
                       Next item
                   End Function()
        End Function
        ''' <summary>
        ''' Zips the sequence with the intermediate results of applying an accumulator function over the sequence.
        ''' The specified seed is used as the initial accumulator value, and is not included in the results.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function ZipWithPartialAggregates(Of TValue, TAggregate)(ByVal sequence As IEnumerable(Of TValue),
                                                                        ByVal seed As TAggregate,
                                                                        ByVal func As Func(Of TAggregate, TValue, TAggregate)) As IEnumerable(Of Tuple(Of TValue, TAggregate))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of TValue, TAggregate)))() IsNot Nothing)
            Return sequence.PartialAggregates(seed:=Tuple.Create([Default](Of TValue), seed),
                                              func:=Function(acc, e) Tuple.Create(e, func(acc.Item2, e)))
        End Function

        ''' <summary>
        ''' Iteratively generates the values of a sequence.
        ''' The sequence ends once the state function returns null.
        ''' </summary>
        <Pure()> <Extension()>
        Public Function Iterate(Of TState, TResult)(ByVal seed As TState,
                                                    ByVal func As Func(Of TState, Tuple(Of TState, TResult))) As IEnumerable(Of TResult)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TResult))() IsNot Nothing)
            Return True.RepeatForever().
                   PartialAggregates(seed:=Tuple.Create(seed, [Default](Of TResult)),
                                     func:=Function(acc, e)
                                               If acc Is Nothing Then Return acc
                                               Return func(acc.Item1)
                                           End Function).
                   TakeWhile(Function(e) e IsNot Nothing).
                   Select(Function(e) e.Item2)
        End Function

        '''<summary>Returns the first element of a sequence, or a specified default value if the sequence contains no elements.</summary>
        <Pure()> <Extension()>
        Public Function FirstOrDefault(Of T)(ByVal sequence As IEnumerable(Of T),
                                             ByVal [default] As T) As T
            Contract.Requires(sequence IsNot Nothing)
            Dim e = sequence.GetEnumerator
            Return If(e.MoveNext, e.Current, [default])
        End Function
        '''<summary>Returns a nullable containing the first element of a sequence, or an empty nullable if the sequence contains no elements.</summary>
        <Pure()> <Extension()>
        Public Function FirstOrNullableDefault(Of T As Structure)(ByVal sequence As IEnumerable(Of T)) As T?
            Contract.Requires(sequence IsNot Nothing)
            Dim e = sequence.GetEnumerator()
            Return If(e.MoveNext, e.Current, [Default](Of T?))
        End Function
    End Module
End Namespace
