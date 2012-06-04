Imports Strilbrary.Values

Namespace Collections
    Public Module LinqExtensions
        '''<summary>Returns a sequence's count if there is a known fast way to get it, or else returns nothing.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Int32?)() Is Nothing OrElse Contract.Result(Of Int32?)().Value >= 0")>
        Friend Function TryFastCount(Of T)(sequence As IEnumerable(Of T)) As Int32?
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Int32?)() Is Nothing OrElse Contract.Result(Of Int32?)().Value >= 0)
            Dim asCollection = TryCast(sequence, ICollection)
            If asCollection IsNot Nothing Then Return asCollection.Count
            Dim asSized = TryCast(sequence, IRist(Of T))
            If asSized IsNot Nothing Then Return asSized.Count
            Return Nothing
        End Function

        '''<summary>Returns the index of the first item in the sequence equal to the target, or null if there is no such item.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Not Contract.Result(Of Int32?).HasValue OrElse Contract.Result(Of Int32?).Value >= 0")>
        Public Function IndexOf(Of T)(sequence As IEnumerable(Of T), target As T) As Int32?
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Not Contract.Result(Of Int32?).HasValue OrElse Contract.Result(Of Int32?).Value >= 0)
            Dim eq = EqualityComparer(Of T).Default
            Dim i = 0
            For Each item In sequence
                If eq.Equals(item, target) Then Return i
                i += 1
            Next item
            Return Nothing
        End Function

        '''<summary>Determines if a sequence has no elements.</summary>
        <Extension()> <Pure()>
        Public Function None(Of T)(sequence As IEnumerable(Of T)) As Boolean
            Contract.Requires(sequence IsNot Nothing)
            Return Not sequence.Any()
        End Function

        '''<summary>Returns a <see cref="LazyCounter" /> for the number of elements in the given sequence.</summary>
        <Extension()> <Pure()>
        Public Function LazyCount(Of T)(sequence As IEnumerable(Of T)) As LazyCounter
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of LazyCounter)() IsNot Nothing)

            Dim fastCount = sequence.TryFastCount()
            If fastCount.HasValue Then
                Contract.Assume(fastCount.Value >= 0)
                Return fastCount.Value
            End If

            Return New LazyCounter(sequence.GetEnumerator())
        End Function

#Region "Min/Max"
        '''<summary>Determines the maximum element in a sequence based on the given comparison function.</summary>
        <Extension()> <Pure()>
        Public Function Max(Of T)(sequence As IEnumerable(Of T),
                                  comparator As Func(Of T, T, Integer)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(comparator IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Aggregate(Function(e1, e2) If(comparator(e1, e2) >= 0, e1, e2))
        End Function
        '''<summary>Determines the minimum element in a sequence based on the given comparison function.</summary>
        <Extension()> <Pure()>
        Public Function Min(Of T)(sequence As IEnumerable(Of T),
                                  comparator As Func(Of T, T, Integer)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(comparator IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Aggregate(Function(e1, e2) If(comparator(e1, e2) <= 0, e1, e2))
        End Function

        '''<summary>Determines the maximum element in a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Max(Of T As IComparable(Of T))(sequence As IEnumerable(Of T)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Max(Function(e) e)
        End Function
        '''<summary>Determines the minimum element in a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Min(Of T As IComparable(Of T))(sequence As IEnumerable(Of T)) As T
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence.Any)
            Return sequence.Min(Function(e) e)
        End Function

        '''<summary>Determines the maximum element in a sequence based on the comparable result of a projection.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "Nonnull-89-0")>
        <SuppressMessage("Microsoft.Contracts", "Requires-48-84")>
        Public Function MaxBy(Of TInput, TComparable As IComparable(Of TComparable))(
                        sequence As IEnumerable(Of TInput),
                        projection As Func(Of TInput, TComparable)) As TInput
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Requires(sequence.Any())
            Return (From item In sequence
                    Let image = projection(item)
                    ).Max(Function(e1, e2) e1.image.CompareTo(e2.image)
                    ).item
        End Function
        '''<summary>Determines the minimum element in a sequence based on the comparable result of a projection.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "Nonnull-89-0")>
        <SuppressMessage("Microsoft.Contracts", "Requires-48-84")>
        Public Function MinBy(Of TInput, TComparable As IComparable(Of TComparable))(
                        sequence As IEnumerable(Of TInput),
                        projection As Func(Of TInput, TComparable)) As TInput
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Requires(sequence.Any())
            Return (From item In sequence
                    Let image = projection(item)
                    ).Min(Function(e1, e2) e1.image.CompareTo(e2.image)
                    ).item
        End Function
#End Region

        '''<summary>Concatenates a sequence of sequences into a single sequence.</summary>
        <Pure()> <Extension()>
        Public Function Concat(Of T)(sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return From sequence In sequences
                   From item In sequence
                   Select item
        End Function

        '''<summary>Concatenates an array of sequences into a single sequence.</summary>
        <Pure()>
        Public Function Concat(Of T)(sequence1 As IEnumerable(Of T),
                                     sequence2 As IEnumerable(Of T),
                                     ParamArray sequences() As IEnumerable(Of T)) As IEnumerable(Of T)
            Contract.Requires(sequence1 IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return {sequence1, sequence2}.Concat(sequences).Concat
        End Function
        <Extension()> <Pure()>
        Public Function Concat(Of T)(list1 As IRist(Of T), list2 As IRist(Of T)) As IRist(Of T)
            Contract.Requires(list1 IsNot Nothing)
            Contract.Requires(list2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = list1.Count + list2.Count)
            Dim r = New Rist(Of T)(counter:=Function() list1.Count + list2.Count,
                                   getter:=Function(i) If(i < list1.Count, list1(i), list2(i - list1.Count)))
            Contract.Assume(r.Count = list1.Count + list2.Count)
            Return r
        End Function

        '''<summary>Appends values to a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Append(Of T)(sequence As IEnumerable(Of T), ParamArray values() As T) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return sequence.Concat(values)
        End Function
        <Pure()> <Extension()>
        Public Function Append(Of T)(list As IRist(Of T), ParamArray values() As T) As IRist(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = list.Count + values.Length)
            Contract.Assume(values.AsRist().Count = values.Length)
            Return list.Concat(values.AsRist())
        End Function

        '''<summary>Prepends values to a sequence.</summary>
        <Pure()> <Extension()>
        Public Function Prepend(Of T)(sequence As IEnumerable(Of T), ParamArray values() As T) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return values.Concat(sequence)
        End Function
        <Pure()> <Extension()>
        Public Function Prepend(Of T)(list As IRist(Of T), ParamArray values() As T) As IRist(Of T)
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = values.Length + list.Count)
            Contract.Assume(values.AsRist().Count = values.Length)
            Return values.AsRist().Concat(list)
        End Function

        '''<summary>Returns the same sequence but with items omitted when they have the same projection as a previous item.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TItem))() IsNot Nothing")>
        Public Function DistinctBy(Of TItem, TProjectionOut)(sequence As IEnumerable(Of TItem), projection As Func(Of TItem, TProjectionOut)) As IEnumerable(Of TItem)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TItem))() IsNot Nothing)

            Return Iterator Function()
                       Dim seen = New HashSet(Of TProjectionOut)()
                       For Each item In sequence
                           If seen.Add(projection(item)) Then
                               Yield item
                           End If
                       Next
                   End Function()
        End Function
        '''<summary>Returns the same sequence but with items omitted unless they have the same projection as a previous item.</summary>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TItem))() IsNot Nothing")>
        <Extension()> <Pure()>
        Public Function DuplicatesBy(Of TItem, TProjectionOut)(sequence As IEnumerable(Of TItem), projection As Func(Of TItem, TProjectionOut)) As IEnumerable(Of TItem)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TItem))() IsNot Nothing)

            Return Iterator Function()
                       Dim seen = New HashSet(Of TProjectionOut)()
                       For Each item In sequence
                           If Not seen.Add(projection(item)) Then
                               Yield item
                           End If
                       Next
                   End Function()
        End Function
        '''<summary>Returns the same sequence but with items omitted unless they were previously encountered in the underlying sequence.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TItem))() IsNot Nothing")>
        Public Function Duplicates(Of TItem)(sequence As IEnumerable(Of TItem)) As IEnumerable(Of TItem)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TItem))() IsNot Nothing)
            Return Iterator Function()
                       Dim seen = New HashSet(Of TItem)()
                       For Each item In sequence
                           If Not seen.Add(item) Then
                               Yield item
                           End If
                       Next
                   End Function()
        End Function

        '''<summary>Caches all the items in a sequence, preventing changes to the sequence from affecting the resulting sequence.</summary>
        <Extension()>
        Public Function Cache(Of T)(sequence As IEnumerable(Of T)) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return sequence.ToArray
        End Function

        '''<summary>Determines the sequence of values less than the given limit, starting at 0 and incrementing.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of UInt32))() IsNot Nothing")>
        Public Function Range(limit As UInt32) As IEnumerable(Of UInt32)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of UInt32))() IsNot Nothing)
            Return Iterator Function()
                       Dim e = 0UI
                       While e < limit
                           Yield e
                           e += 1UI
                       End While
                   End Function()
        End Function
        '''<summary>Determines the sequence of values less than the given limit, starting at 0 and incrementing.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of UInt16))() IsNot Nothing")>
        Public Function Range(limit As UInt16) As IRist(Of UInt16)
            Contract.Ensures(Contract.Result(Of IRist(Of UInt16))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of UInt16))().Count = limit)
            Dim r = New Rist(Of UInt16)(getter:=Function(i) CUShort(i), counter:=Function() CInt(limit))
            Contract.Assume(r.Count = limit)
            Return r
        End Function

        '''<summary>Enumerates items in the sequence, offset by the given amount.</summary>
        <Pure()> <Extension()>
        Public Function OffsetBy(sequence As IEnumerable(Of Int32), offset As Int32) As IEnumerable(Of Int32)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Int32))() IsNot Nothing)
            Return From i In sequence Select i + offset
        End Function

        '''<summary>Zips the elements of two sequences into a sequence of tuples.</summary>
        <Pure()> <Extension()>
        Public Function Zip(Of T1, T2)(sequence As IEnumerable(Of T1),
                                       sequence2 As IEnumerable(Of T2)) As IEnumerable(Of Tuple(Of T1, T2))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequence2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of T1, T2)))() IsNot Nothing)
            Dim result = Enumerable.Zip(sequence, sequence2, Function(e1, e2) Tuple.Create(e1, e2))
            Contract.Assume(result IsNot Nothing)
            Return result
        End Function

        '''<summary>Zips a sequence's elements with their positions in the sequence.</summary>
        <Pure()> <Extension()>
        Public Function ZipWithIndexes(Of T)(sequence As IEnumerable(Of T)) As IEnumerable(Of Tuple(Of T, Integer))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of T, Integer)))() IsNot Nothing)
            Return sequence.Select(Function(item, index) Tuple.Create(item, index))
        End Function

        '''<summary>Returns a never-ending sequence consisting of a repeated value.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of TValue))() IsNot Nothing")>
        Public Function RepeatForever(Of TValue)(value As TValue) As IEnumerable(Of TValue)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TValue))() IsNot Nothing)
            Return Iterator Function()
                       Do
                           Yield value
                       Loop
                   End Function()
        End Function

        '''<summary>Enumerates all contiguous subsequences of the given size from the given sequence.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of IRist(Of T)))() IsNot Nothing")>
        Public Function Roll(Of T)(sequence As IEnumerable(Of T), windowSize As Integer) As IEnumerable(Of IRist(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(windowSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of IRist(Of T)))() IsNot Nothing)

            Return Iterator Function()
                       Dim rollingWindow = New Queue(Of T)(capacity:=windowSize)
                       For Each item In sequence
                           rollingWindow.Enqueue(item)
                           If rollingWindow.Count >= windowSize Then
                               Yield rollingWindow.ToRist()
                               rollingWindow.Dequeue()
                           End If
                       Next item
                   End Function()
        End Function

        '''<summary>Pads a sequence to a given minimum length.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of T))() IsNot Nothing")>
        Public Function PaddedTo(Of T)(sequence As IEnumerable(Of T),
                                       minimumCount As Integer,
                                       Optional paddingValue As T = Nothing) As IEnumerable(Of T)
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
        Public Function IndexesOf(Of T)(sequence As IEnumerable(Of T), value As T) As IEnumerable(Of Integer)
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
        Public Function Interleaved(Of T)(sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequences IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Dim r = Iterator Function()
                        Dim enumerators = (From sequence In sequences
                                           Select sequence.GetEnumerator
                                           ).ToArray
                        Try
                            Do
                                Dim used = False
                                For Each enumerator In enumerators
                                    If Not enumerator.MoveNext() Then Continue For
                                    used = True
                                    Yield enumerator.Current
                                Next enumerator
                                If Not used Then Exit Do
                            Loop
                        Finally
                            For Each enumerator In enumerators
                                enumerator.Dispose()
                            Next enumerator
                        End Try
                    End Function()
            Contract.Assume(r IsNot Nothing)
            Return r
        End Function

        '''<summary>Groups a sequence based on the position of items (modulo the given sequence count).</summary>
        <Pure()> <Extension()>
        Public Function Deinterleaved(Of T)(sequence As IEnumerable(Of T), sequenceCount As Integer) As IRist(Of IEnumerable(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequenceCount > 0)
            Contract.Ensures(Contract.Result(Of IRist(Of IEnumerable(Of T)))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of IEnumerable(Of T)))().Count = sequenceCount)
            Dim r = New Rist(Of IEnumerable(Of T))(counter:=Function() sequenceCount,
                                                   getter:=Function(i) sequence.Skip(i).Step(sequenceCount))
            Contract.Assume(r.Count = sequenceCount)
            Return r
        End Function
        '''<summary>Groups a sequence based on the position of items (modulo the given sequence count).</summary>
        <Pure()> <Extension()>
        Public Function Deinterleaved(Of T)(sequence As IRist(Of T), sequenceCount As Integer) As IRist(Of IRist(Of T))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(sequenceCount > 0)
            Contract.Ensures(Contract.Result(Of IRist(Of IEnumerable(Of T)))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of IEnumerable(Of T)))().Count = sequenceCount)
            Dim r = New Rist(Of IRist(Of T))(counter:=Function() sequenceCount,
                                             getter:=Function(i) sequence.Skip(i).Step(sequenceCount))
            Contract.Assume(r.Count = sequenceCount)
            Return r
        End Function

        '''<summary>Selects every nth item in a sequence, starting with the first item.</summary>
        <Pure()> <Extension()>
        Public Function [Step](Of T)(sequence As IEnumerable(Of T), stepSize As Integer) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(stepSize > 0)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)

            Dim r = Iterator Function()
                        Dim left = 1
                        For Each item In sequence
                            left -= 1
                            If left > 0 Then Continue For
                            left = stepSize
                            Yield item
                        Next item
                    End Function()
            Contract.Assume(r IsNot Nothing)
            Return r
        End Function
        '''<summary>Selects every nth item in a readable list, starting with the first item.</summary>
        <Pure()> <Extension()>
        Public Function [Step](Of T)(sequence As IRist(Of T), stepSize As Integer) As IRist(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(stepSize > 0)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = sequence.Count.CeilingMultiple(stepSize) \ stepSize)
            Dim count = sequence.Count.CeilingMultiple(stepSize) \ stepSize
            Dim r = New Rist(Of T)(counter:=Function() count, getter:=Function(i) sequence(i * stepSize))
            Contract.Assume(r.Count = count)
            Return r
        End Function

        '''<summary>Splits a sequence into continuous segments of a given size (with the last partition possibly being smaller).</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of IEnumerable(Of IEnumerable(Of T)))() IsNot Nothing")>
        Public Function Partitioned(Of T)(sequence As IEnumerable(Of T),
                                          partitionSize As Integer) As IEnumerable(Of IEnumerable(Of T))
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
        '''<summary>Partitions a list into continuous segments of a given size (with the last partition possibly being smaller).</summary>
        <Pure()> <Extension()>
        Public Function Partitioned(Of T)(list As IRist(Of T),
                                          partitionSize As Integer) As IRist(Of IRist(Of T))
            Contract.Requires(list IsNot Nothing)
            Contract.Requires(partitionSize > 0)
            Contract.Ensures(Contract.Result(Of IRist(Of IRist(Of T)))() IsNot Nothing)

            Dim fullCount = list.Count \ partitionSize
            Dim tailSize = list.Count Mod partitionSize
            Dim count = fullCount + If(tailSize > 0, 1, 0)
            Return New Rist(Of IRist(Of T))(
                counter:=Function() count,
                getter:=Function(i) New Rist(Of T)(
                    counter:=Function() If(i < fullCount, partitionSize, tailSize),
                    getter:=Function(j) list(i * partitionSize + j)))
        End Function

        '''<summary>Returns the last specified number of items in a sequence, or the entire sequence if there are fewer items than the specified number.</summary>
        <Extension()>
        Public Function TakeLast(Of T)(sequence As IEnumerable(Of T),
                                       count As Integer) As IEnumerable(Of T)
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
        Public Function SkipLast(Of T)(sequence As IEnumerable(Of T),
                                       count As Integer) As IEnumerable(Of T)
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
        Public Function AggregateBack(Of TValue, TAccumulate)(sequence As IEnumerable(Of TValue),
                                                              seed As TAccumulate,
                                                              func As Func(Of TAccumulate, TValue, TAccumulate)) As IEnumerable(Of TAccumulate)
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
        Public Function ZipAggregateBack(Of TValue, TAggregate)(sequence As IEnumerable(Of TValue),
                                                                seed As TAggregate,
                                                                func As Func(Of TAggregate, TValue, TAggregate)) As IEnumerable(Of Tuple(Of TValue, TAggregate))
            Contract.Requires(sequence IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of TValue, TAggregate)))() IsNot Nothing)
            Return sequence.AggregateBack(seed:=Tuple.Create([Default](Of TValue), seed),
                                          func:=Function(acc, e) Tuple.Create(e, func(acc.Item2, e)))
        End Function

        '''<summary>Returns the first element of a sequence, or a specified default value if the sequence contains no elements.</summary>
        <Pure()> <Extension()>
        Public Function FirstOrDefault(Of T)(sequence As IEnumerable(Of T),
                                             [default] As T) As T
            Contract.Requires(sequence IsNot Nothing)
            Dim e = sequence.GetEnumerator
            Return If(e.MoveNext, e.Current, [default])
        End Function
        '''<summary>Returns a nullable containing the first element of a sequence, or an empty nullable if the sequence contains no elements.</summary>
        <Pure()> <Extension()>
        Public Function FirstOrNullableDefault(Of T As Structure)(sequence As IEnumerable(Of T)) As T?
            Contract.Requires(sequence IsNot Nothing)
            Dim e = sequence.GetEnumerator()
            Return If(e.MoveNext, e.Current, [Default](Of T?))
        End Function

        <Pure()>
        Public Function MakeRist(Of T)(ParamArray values As T()) As IRist(Of T)
            Contract.Requires(values IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().Count = values.Length)
            Contract.Ensures(Contract.Result(Of IRist(Of T))().SequenceEqual(values))
            Dim r = New Rist(Of T)(counter:=Function() values.Length, getter:=Function(i) values(i))
            Contract.Assume(r.Count = values.Length)
            Contract.Assume(r.SequenceEqual(values))
            Return r
        End Function
    End Module
End Namespace
