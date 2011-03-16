Namespace Values
    ''' <summary>
    ''' A string with case-insensitive equality.
    ''' </summary>
    <DebuggerDisplay("{ToString()}")>
    Public Structure InvariantString
        Implements IEquatable(Of String)
        Implements IComparable(Of String)
        Implements IEquatable(Of InvariantString)
        Implements IComparable(Of InvariantString)

        Private ReadOnly _value As String

        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Me.Value = value")>
        Public Sub New(value As String)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Me.Value = value)
            Me._value = value
        End Sub

        Public ReadOnly Property Value As String
            <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of String)() = If(_value, """")")>
            Get
                Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
                Contract.Ensures(Contract.Result(Of String)() = If(_value, ""))
                Return If(_value, "")
            End Get
        End Property
        Public ReadOnly Property Length As Integer
            Get
                Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                Contract.Ensures(Contract.Result(Of Integer)() = Me.Value.Length)
                Return Value.Length
            End Get
        End Property

        Public Shared Operator =(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return String.Equals(value1.Value, value2.Value, StringComparison.OrdinalIgnoreCase)
        End Operator
        Public Shared Operator <>(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return Not value1 = value2
        End Operator
        Public Shared Operator =(value1 As InvariantString, value2 As String) As Boolean
            Return String.Equals(value1.Value, value2, StringComparison.OrdinalIgnoreCase)
        End Operator
        Public Shared Operator <>(value1 As InvariantString, value2 As String) As Boolean
            Return Not value1 = value2
        End Operator
        Public Shared Operator =(value1 As String, value2 As InvariantString) As Boolean
            Return String.Equals(value1, value2.Value, StringComparison.OrdinalIgnoreCase)
        End Operator
        Public Shared Operator <>(value1 As String, value2 As InvariantString) As Boolean
            Return Not value1 = value2
        End Operator

        Public Shared Operator <(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) < 0
        End Operator
        Public Shared Operator >(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) > 0
        End Operator
        Public Shared Operator <=(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) <= 0
        End Operator
        Public Shared Operator >=(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) >= 0
        End Operator

        Public Shared Operator Like(value1 As InvariantString, value2 As InvariantString) As Boolean
            Return value1.Value.ToUpperInvariant Like value2.Value.ToUpperInvariant
        End Operator
        Public Shared Operator Like(value1 As InvariantString, value2 As String) As Boolean
            Return value1.Value.ToUpperInvariant Like If(value2 Is Nothing, Nothing, value2.ToUpperInvariant)
        End Operator
        Public Shared Operator Like(value1 As String, value2 As InvariantString) As Boolean
            Return If(value1 Is Nothing, Nothing, value1.ToUpperInvariant) Like value2.Value.ToUpperInvariant
        End Operator

        Public Shared Operator +(value1 As InvariantString, value2 As InvariantString) As InvariantString
            Return value1.Value + value2.Value
        End Operator

        Public Shared Widening Operator CType(value As String) As InvariantString
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Contract.Result(Of InvariantString)().Value = value)
            Return New InvariantString(value)
        End Operator
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of String)() = value.Value")>
        Public Shared Widening Operator CType(value As InvariantString) As String
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() = value.Value)
            Return value.Value
        End Operator

        <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Not Contract.Result(Of Boolean)() OrElse Me.Length >= value.Length")>
        Public Function EndsWith(value As InvariantString) As Boolean
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.Length >= value.Length)
            Return Me.Value.EndsWith(value.Value, StringComparison.OrdinalIgnoreCase)
        End Function
        <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Not Contract.Result(Of Boolean)() OrElse Me.Length >= value.Length")>
        Function StartsWith(value As InvariantString) As Boolean
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.Length >= value.Length)
            Return Me.Value.StartsWith(value.Value, StringComparison.OrdinalIgnoreCase)
        End Function
        <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of InvariantString)().Length = Me.Length - startIndex")>
        Public Function Substring(startIndex As Integer) As InvariantString
            Contract.Requires(startIndex >= 0)
            Contract.Requires(startIndex <= Me.Length)
            Contract.Ensures(Contract.Result(Of InvariantString)().Length = Me.Length - startIndex)
            Return Me.Value.Substring(startIndex)
        End Function
        <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of InvariantString)().Length = length")>
        Public Function Substring(startIndex As Integer, length As Integer) As InvariantString
            Contract.Requires(startIndex >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(startIndex + length <= Me.Length)
            Contract.Ensures(Contract.Result(Of InvariantString)().Length = length)
            Return Me.Value.Substring(startIndex, length)
        End Function

        <SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is InvariantString Then
                Return Me = DirectCast(obj, InvariantString)
            End If

            Dim asString = TryCast(obj, String)
            If asString IsNot Nothing Then Return Me = asString

            Return False
        End Function
        Public Overloads Function Equals(other As String) As Boolean Implements IEquatable(Of String).Equals
            Return Me = other
        End Function
        Public Overloads Function Equals(other As InvariantString) As Boolean Implements IEquatable(Of InvariantString).Equals
            Return Me = other
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Me.Value.ToUpperInvariant.GetHashCode
        End Function
        Public Overrides Function ToString() As String
            Return Value
        End Function
        Public Function CompareTo(other As InvariantString) As Integer Implements IComparable(Of InvariantString).CompareTo
            Return String.CompareOrdinal(Me.Value.ToUpperInvariant, other.Value.ToUpperInvariant)
        End Function
        Public Function CompareTo(other As String) As Integer Implements IComparable(Of String).CompareTo
            Return String.CompareOrdinal(Me.Value.ToUpperInvariant, If(other Is Nothing, Nothing, other.ToUpperInvariant))
        End Function
    End Structure
End Namespace
