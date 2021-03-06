﻿'''<summary>Counts the elements from an enumerator and exposes operations which do not enumerate more items than necessary for the result.</summary>
<DebuggerDisplay("{ToString()}")>
Public NotInheritable Class LazyCounter
    Implements IComparable(Of LazyCounter)
    Implements IEquatable(Of LazyCounter)

    Private counter As IEnumerator
    Private _currentCount As Integer
    Public Property CurrentCount As Integer
        Get
            Contract.Ensures(Contract.Result(Of Integer)() >= 0)
            Return _currentCount
        End Get
        Private Set(value As Integer)
            Contract.Requires(value >= _currentCount)
            _currentCount = value
        End Set
    End Property
    Public ReadOnly Property Finished As Boolean
        Get
            Return counter Is Nothing
        End Get
    End Property

    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(_currentCount >= 0)
    End Sub

    '''<summary>Trivial constructor.</summary>
    '''<param name="counter">The enumerator from which elements are counted. Null is understood as 0 more elements.</param>
    '''<param name="initialCount">The initial current count.</param>
    Public Sub New(Optional counter As IEnumerator = Nothing, Optional initialCount As Int32 = 0)
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
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-CurrentCount >= Contract.OldValue(CurrentCount)")>
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
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-CurrentCount = Contract.Result(Of Integer)()")>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-CurrentCount >= Contract.OldValue(CurrentCount)")>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Finished OrElse CurrentCount >= limit")>
    Public Function CountTo(limit As Integer) As Integer
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
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-CurrentCount = Contract.Result(Of Integer)()")>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-CurrentCount >= Contract.OldValue(CurrentCount)")>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Finished OrElse CurrentCount > limit")>
    Public Function CountPast(limit As Integer) As Integer
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
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Me.CurrentCount >= Contract.OldValue(Me.CurrentCount)")>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-other.CurrentCount >= Contract.OldValue(other.CurrentCount)")>
    Public Function CompareTo(other As LazyCounter) As Integer Implements IComparable(Of LazyCounter).CompareTo
        Contract.Ensures(Me.CurrentCount >= Contract.OldValue(Me.CurrentCount))
        Contract.Ensures(other.CurrentCount >= Contract.OldValue(other.CurrentCount))
        If Me Is other Then Return 0

        Do
            other.CountPast(Me.CurrentCount)
            If Me.Finished Then Exit Do
            Me.CountPast(other.CurrentCount)
            If other.Finished Then Exit Do
        Loop

        Return Me.CurrentCount.CompareTo(other.CurrentCount)
    End Function
    '''<summary>Strictly compares eventual final counts without advancing more than necessary.</summary>
    '''<remarks>Slightly more efficient than CompareTo in some cases.</remarks>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Me.CurrentCount >= Contract.OldValue(Me.CurrentCount)")>
    <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-other.CurrentCount >= Contract.OldValue(other.CurrentCount)")>
    Private Function IsLessThan(other As LazyCounter) As Boolean
        Contract.Ensures(Me.CurrentCount >= Contract.OldValue(Me.CurrentCount))
        Contract.Ensures(other.CurrentCount >= Contract.OldValue(other.CurrentCount))
        If Me Is other Then Return False

        Do
            other.CountPast(Me.CurrentCount)
            If Me.Finished Then Exit Do
            Me.CountTo(other.CurrentCount)
            If other.Finished Then Exit Do
        Loop

        Return Me.CurrentCount < other.CurrentCount
    End Function

    Public Shared Widening Operator CType(count As Int32) As LazyCounter
        Contract.Requires(count >= 0)
        Contract.Ensures(Contract.Result(Of LazyCounter)() IsNot Nothing)
        Return New LazyCounter(initialCount:=count)
    End Operator
    Public Shared Widening Operator CType(value As LazyCounter) As Int32
        Contract.Requires(value IsNot Nothing)
        Return value.FinalCount()
    End Operator
    Public Shared Operator =(value1 As LazyCounter, value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) = 0
    End Operator
    Public Shared Operator <>(value1 As LazyCounter, value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) <> 0
    End Operator
    Public Shared Operator <=(value1 As LazyCounter, value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) <= 0
    End Operator
    Public Shared Operator >=(value1 As LazyCounter, value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.CompareTo(value2) >= 0
    End Operator
    Public Shared Operator <(value1 As LazyCounter, value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value1.IsLessThan(value2)
    End Operator
    Public Shared Operator >(value1 As LazyCounter, value2 As LazyCounter) As Boolean
        Contract.Requires(value1 IsNot Nothing)
        Contract.Requires(value2 IsNot Nothing)
        Return value2.IsLessThan(value1)
    End Operator

    Public Overrides Function GetHashCode() As Integer
        Return FinalCount.GetHashCode()
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        Return Me.Equals(TryCast(obj, LazyCounter))
    End Function
    Public Overloads Function Equals(other As LazyCounter) As Boolean Implements IEquatable(Of LazyCounter).Equals
        Return other IsNot Nothing AndAlso Me = other
    End Function
    Public Overrides Function ToString() As String
        Return If(Finished, "", ">=") + CurrentCount.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function
End Class
