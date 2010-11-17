'''<summary>Counts the elements from an enumerator and exposes operations which do not enumerate more items than necessary for the result.</summary>
<DebuggerDisplay("{ToString}")>
Public NotInheritable Class LazyCounter
    Implements IComparable(Of LazyCounter)
    Implements IEquatable(Of LazyCounter)

    Private counter As IEnumerator
    Private _currentCount As Integer
    Public Property CurrentCount As Integer
        Get
            Return _currentCount
        End Get
        Private Set(ByVal value As Integer)
            _currentCount = value
        End Set
    End Property
    Public ReadOnly Property Finished As Boolean
        Get
            Return counter Is Nothing
        End Get
    End Property

    <ContractInvariantMethod()>
    Private Sub ObjectInvariant()
        Contract.Invariant(CurrentCount >= 0)
    End Sub

    '''<summary>Trivial constructor.</summary>
    '''<param name="counter">The enumerator from which elements are counted. Null is understood as 0 more elements.</param>
    '''<param name="initialCount">The initial current count.</param>
    Public Sub New(Optional ByVal counter As IEnumerator = Nothing, Optional ByVal initialCount As Int32 = 0)
        Contract.Requires(initialCount >= 0)
        Me.counter = counter
        Me.CurrentCount = initialCount
    End Sub

    '''<summary>Tries to advance the counter, either incrementing the current count or finishing.</summary>
    Private Sub Advance()
        Contract.Requires(Not Finished)
        Contract.Ensures(Finished OrElse CurrentCount = Contract.OldValue(CurrentCount) + 1)
        Contract.Ensures(CurrentCount >= Contract.OldValue(CurrentCount))
        Contract.Ensures(CurrentCount <= Contract.OldValue(CurrentCount) + 1)
        If counter.MoveNext() Then
            CurrentCount += 1
        Else
            counter = Nothing 'Finished
        End If
    End Sub
    '''<summary>The current count after advancing until finished.</summary>
    <ContractVerification(False)>
    Public Function FinalCount() As Integer
        Contract.Ensures(Finished)
        Contract.Ensures(CurrentCount = Contract.Result(Of Integer)())
        Contract.Ensures(CurrentCount >= Contract.OldValue(CurrentCount))
        While Not Finished
            Advance()
        End While
        Return CurrentCount
    End Function
    '''<summary>Advances until finished or the current count is at least the limit.</summary>
    '''<returns>The new current count.</returns>
    <ContractVerification(False)>
    Public Function CountTo(ByVal limit As Integer) As Integer
        Contract.Requires(limit >= 0)
        Contract.Ensures(CurrentCount = Contract.Result(Of Integer)())
        Contract.Ensures(CurrentCount >= Contract.OldValue(CurrentCount))
        Contract.Ensures(Finished OrElse CurrentCount >= limit)
        While Not Finished AndAlso CurrentCount < limit
            Advance()
        End While
        Return CurrentCount
    End Function
    '''<summary>Advances until finished or the current count is over the limit.</summary>
    '''<returns>The new current count.</returns>
    <ContractVerification(False)>
    Public Function CountPast(ByVal limit As Integer) As Integer
        Contract.Requires(limit >= 0)
        Contract.Ensures(CurrentCount = Contract.Result(Of Integer)())
        Contract.Ensures(CurrentCount >= Contract.OldValue(CurrentCount))
        Contract.Ensures(Finished OrElse CurrentCount > limit)
        While Not Finished AndAlso CurrentCount <= limit
            Advance()
        End While
        Return CurrentCount
    End Function

    '''<summary>Compares eventual final counts without advancing more than necessary.</summary>
    <ContractVerification(False)>
    Public Function CompareTo(ByVal other As LazyCounter) As Integer Implements IComparable(Of LazyCounter).CompareTo
        Contract.Ensures(Me.CurrentCount >= Contract.OldValue(Me.CurrentCount))
        Contract.Ensures(other.CurrentCount >= Contract.OldValue(other.CurrentCount))

        While Not other.Finished AndAlso Not Me.Finished
            other.CountPast(Me.CurrentCount)
            Me.CountPast(other.CurrentCount)
        End While
        Me.CountPast(other.CurrentCount)
        other.CountPast(Me.CurrentCount)

        Return Me.CurrentCount.CompareTo(other.CurrentCount)
    End Function
    '''<summary>Strictly compares eventual final counts without advancing more than necessary.</summary>
    '''<remarks>Slightly more efficient than CompareTo in some cases.</remarks>
    <ContractVerification(False)>
    Private Function IsLessThan(ByVal other As LazyCounter) As Boolean
        Contract.Ensures(Me.CurrentCount >= Contract.OldValue(Me.CurrentCount))
        Contract.Ensures(other.CurrentCount >= Contract.OldValue(other.CurrentCount))

        While Not other.Finished AndAlso Not Me.Finished
            other.CountPast(Me.CurrentCount)
            Me.CountTo(other.CurrentCount)
        End While
        other.CountPast(Me.CurrentCount)
        Me.CountTo(other.CurrentCount)

        Return Me.CurrentCount < other.CurrentCount
    End Function

    Public Shared Widening Operator CType(ByVal count As Int32) As LazyCounter
        Contract.Requires(count >= 0)
        Return New LazyCounter(initialCount:=count)
    End Operator
    Public Shared Widening Operator CType(ByVal value As LazyCounter) As Int32
        Contract.Requires(value IsNot Nothing)
        Return value.FinalCount()
    End Operator
    Public Shared Operator =(ByVal value1 As LazyCounter, ByVal value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) = 0
    End Operator
    Public Shared Operator <>(ByVal value1 As LazyCounter, ByVal value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) <> 0
    End Operator
    Public Shared Operator <=(ByVal value1 As LazyCounter, ByVal value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) <= 0
    End Operator
    Public Shared Operator >=(ByVal value1 As LazyCounter, ByVal value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) >= 0
    End Operator
    Public Shared Operator <(ByVal value1 As LazyCounter, ByVal value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.IsLessThan(value2)
    End Operator
    Public Shared Operator >(ByVal value1 As LazyCounter, ByVal value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value2.IsLessThan(value1)
    End Operator

    Public Overrides Function GetHashCode() As Integer
        Return FinalCount.GetHashCode()
    End Function
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return Me.Equals(TryCast(obj, LazyCounter))
    End Function
    Public Overloads Function Equals(ByVal other As LazyCounter) As Boolean Implements IEquatable(Of LazyCounter).Equals
        Return other IsNot Nothing AndAlso Me = other
    End Function
    Public Overrides Function ToString() As String
        Return If(Finished, "", ">=") + CurrentCount.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function
End Class
