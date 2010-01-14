Namespace Values
    ''' <summary>
    ''' A string with case-insensitive equality.
    ''' </summary>
    <DebuggerDisplay("{ToString}")>
    Public Structure InvariantString
        'verification off because code contracts 1.2.21023.14 silently crashes if this class is verified
        Implements IEquatable(Of String)
        Implements IComparable(Of String)
        Implements IEquatable(Of InvariantString)
        Implements IComparable(Of InvariantString)

        Private ReadOnly _value As String

        'verification disabled due to stupid verifier (1.2.30113.1)
        <ContractVerification(False)>
        Public Sub New(ByVal value As String)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Me.Value = value)
            Me._value = value
        End Sub

        Public ReadOnly Property Value As String
            'verification disabled due to stupid verifier (1.2.30113.1)
            <ContractVerification(False)>
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

        Public Shared Operator =(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return String.Equals(value1.Value, value2.Value, StringComparison.OrdinalIgnoreCase)
        End Operator
        Public Shared Operator <>(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return Not value1 = value2
        End Operator
        Public Shared Operator =(ByVal value1 As InvariantString, ByVal value2 As String) As Boolean
            Return String.Equals(value1.Value, value2, StringComparison.OrdinalIgnoreCase)
        End Operator
        Public Shared Operator <>(ByVal value1 As InvariantString, ByVal value2 As String) As Boolean
            Return Not value1 = value2
        End Operator
        Public Shared Operator =(ByVal value1 As String, ByVal value2 As InvariantString) As Boolean
            Return String.Equals(value1, value2.Value, StringComparison.OrdinalIgnoreCase)
        End Operator
        Public Shared Operator <>(ByVal value1 As String, ByVal value2 As InvariantString) As Boolean
            Return Not value1 = value2
        End Operator

        Public Shared Operator <(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) < 0
        End Operator
        Public Shared Operator >(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) > 0
        End Operator
        Public Shared Operator <=(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) <= 0
        End Operator
        Public Shared Operator >=(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return value1.CompareTo(value2) >= 0
        End Operator

        Public Shared Operator Like(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As Boolean
            Return value1.Value.ToUpperInvariant Like value2.Value.ToUpperInvariant
        End Operator
        Public Shared Operator Like(ByVal value1 As InvariantString, ByVal value2 As String) As Boolean
            Return value1.Value.ToUpperInvariant Like If(value2 Is Nothing, Nothing, value2.ToUpperInvariant)
        End Operator
        Public Shared Operator Like(ByVal value1 As String, ByVal value2 As InvariantString) As Boolean
            Return If(value1 Is Nothing, Nothing, value1.ToUpperInvariant) Like value2.Value.ToUpperInvariant
        End Operator

        Public Shared Operator +(ByVal value1 As InvariantString, ByVal value2 As InvariantString) As InvariantString
            Return value1.Value + value2.Value
        End Operator

        Public Shared Widening Operator CType(ByVal value As String) As InvariantString
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Contract.Result(Of InvariantString)().Value = value)
            Return New InvariantString(value)
        End Operator
        'verification disabled due to stupid verifier (1.2.30113.1)
        <ContractVerification(False)>
        Public Shared Widening Operator CType(ByVal value As InvariantString) As String
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() = value.Value)
            Return value.Value
        End Operator

        'verification disabled due to stupid verifier (1.2.30113.1)
        <ContractVerification(False)>
        <Pure()>
        Public Function EndsWith(ByVal value As InvariantString) As Boolean
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.Length >= value.Length)
            Return Me.Value.EndsWith(value.Value, StringComparison.OrdinalIgnoreCase)
        End Function
        'verification disabled due to stupid verifier (1.2.30113.1)
        <ContractVerification(False)>
        <Pure()>
        Function StartsWith(ByVal value As InvariantString) As Boolean
            Contract.Ensures(Not Contract.Result(Of Boolean)() OrElse Me.Length >= value.Length)
            Return Me.Value.StartsWith(value.Value, StringComparison.OrdinalIgnoreCase)
        End Function
        'verification disabled due to stupid verifier (1.2.30113.1)
        <ContractVerification(False)>
        <Pure()>
        Public Function Substring(ByVal startIndex As Integer) As InvariantString
            Contract.Requires(startIndex >= 0)
            Contract.Requires(startIndex <= Me.Length)
            Contract.Ensures(Contract.Result(Of InvariantString)().Length = Me.Length - startIndex)
            Return Me.Value.Substring(startIndex)
        End Function
        <Pure()>
        Public Function Substring(ByVal startIndex As Integer, ByVal length As Integer) As InvariantString
            Contract.Requires(startIndex >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(startIndex + length <= Me.Length)
            Contract.Ensures(Contract.Result(Of InvariantString)().Length = length)
            Return Me.Value.Substring(startIndex, length)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is InvariantString Then
                Return Me = CType(obj, InvariantString)
            ElseIf TypeOf obj Is String Then
                Return Me = CStr(obj)
            Else
                Return False
            End If
        End Function
        Public Overloads Function Equals(ByVal other As String) As Boolean Implements IEquatable(Of String).Equals
            Return Me = other
        End Function
        Public Overloads Function Equals(ByVal other As InvariantString) As Boolean Implements IEquatable(Of InvariantString).Equals
            Return Me = other
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Me.Value.ToUpperInvariant.GetHashCode
        End Function
        Public Overrides Function ToString() As String
            Return Value
        End Function
        Public Function CompareTo(ByVal other As InvariantString) As Integer Implements IComparable(Of InvariantString).CompareTo
            Return String.CompareOrdinal(Me.Value.ToUpperInvariant, other.Value.ToUpperInvariant)
        End Function
        Public Function CompareTo(ByVal other As String) As Integer Implements IComparable(Of String).CompareTo
            Return String.CompareOrdinal(Me.Value.ToUpperInvariant, If(other Is Nothing, Nothing, other.ToUpperInvariant))
        End Function
    End Structure
End Namespace
