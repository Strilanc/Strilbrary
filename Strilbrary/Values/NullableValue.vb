Namespace Values
    '''<summary>
    ''' A value type that stores an optional value.
    ''' Note that a null reference type is equivalent to a null Nullable, not a Nullable storing a null.
    '''</summary>
    '''<remarks>Useful for ensuring a generic value is nullable when it may be a struct or a class.</remarks>
    <DebuggerDisplay("{ToString()}")>
    Public Structure NullableValue(Of T)
        Implements IEquatable(Of NullableValue(Of T))

        Private ReadOnly _hasValue As Boolean
        Private ReadOnly _value As T

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(Not _hasValue OrElse _value IsNot Nothing)
        End Sub

        Public Sub New(value As T)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Me.HasValue)
            Contract.Ensures(Me.Value.Equals(value))
            Me._value = value
            Me._hasValue = True
            Contract.Assume(Me.Value.Equals(value))
        End Sub

        Public ReadOnly Property HasValue As Boolean
            Get
                Return _hasValue
            End Get
        End Property
        Public ReadOnly Property Value As T
            Get
                Contract.Requires(HasValue)
                Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
                Contract.Assume(_value IsNot Nothing)
                Return _value
            End Get
        End Property

        Public Shared Widening Operator CType(value As T) As NullableValue(Of T)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures((value IsNot Nothing) = (Contract.Result(Of NullableValue(Of T))().HasValue))
            Contract.Ensures(value Is Nothing OrElse Contract.Result(Of NullableValue(Of T))().Value.Equals(value))
            If value Is Nothing Then Return Nothing
            Dim r = New NullableValue(Of T)(value)
            Contract.Assume(value Is Nothing OrElse r.Value.Equals(value))
            Return r
        End Operator
        Public Shared Narrowing Operator CType(value As NullableValue(Of T)) As T
            If value.HasValue Then Return value.Value

            Dim isReferenceType = DirectCast(Nothing, T) Is Nothing
            If Not isReferenceType Then Throw New InvalidCastException(
                "Can't cast null value of type {0} to non-nullable type {1}.".Frmt(
                    GetType(RefNullable(Of T)).FullName,
                    GetType(T).FullName))
            Return Nothing
        End Operator

        Public Shared Narrowing Operator CType(value As NullableValue(Of T)) As RefNullable(Of T)
            If value.HasValue Then Return value.Value
            Return Nothing
        End Operator
        Public Shared Narrowing Operator CType(value As RefNullable(Of T)) As NullableValue(Of T)
            If value Is Nothing Then Return Nothing
            Return value.Value
        End Operator

        Public Overloads Function Equals(other As NullableValue(Of T)) As Boolean Implements IEquatable(Of NullableValue(Of T)).Equals
            If other.HasValue AndAlso Me.HasValue Then Return Me.Value.Equals(other.Value)
            Return other.HasValue = Me.HasValue
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is NullableValue(Of T) AndAlso Me.Equals(DirectCast(obj, NullableValue(Of T)))
        End Function
        Public Overrides Function GetHashCode() As Integer
            If Not HasValue Then Return 0
            Return Value.GetHashCode()
        End Function
        Public Overrides Function ToString() As String
            If Not HasValue Then Return "[No Value]"
            Return Value.ToString()
        End Function
        Public Shared Operator =(value1 As NullableValue(Of T), value2 As NullableValue(Of T)) As Boolean
            Return value1.Equals(value2)
        End Operator
        Public Shared Operator <>(value1 As NullableValue(Of T), value2 As NullableValue(Of T)) As Boolean
            Return Not value1.Equals(value2)
        End Operator
    End Structure
End Namespace
