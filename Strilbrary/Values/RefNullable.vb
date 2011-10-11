Namespace Values
    '''<summary>
    ''' A reference type that stores a non-null value, effectively augmenting value types with a null value.
    ''' Note that a null reference type is equivalent to a null RefNullable, not a RefNullable storing a null.
    '''</summary>
    '''<remarks>Useful for ensuring a generic value is nullable when it may be a struct or a class.</remarks>
    <DebuggerDisplay("{ToString()}")>
    Public Class RefNullable(Of T)
        Implements IEquatable(Of RefNullable(Of T))

        Private ReadOnly _value As T

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_value IsNot Nothing)
        End Sub

        Public Sub New(value As T)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Me.Value.Equals(value))
            Me._value = value
            Contract.Assume(Me.Value.Equals(value))
        End Sub

        Public ReadOnly Property Value As T
            Get
                Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
                Return _value
            End Get
        End Property

        Public Shared Widening Operator CType(value As T) As RefNullable(Of T)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures((value Is Nothing) = (Contract.Result(Of RefNullable(Of T))() Is Nothing))
            Contract.Ensures(value Is Nothing OrElse Contract.Result(Of RefNullable(Of T))().Value.Equals(value))
            If value Is Nothing Then Return Nothing
            Dim r = New RefNullable(Of T)(value)
            Contract.Assume(value Is Nothing OrElse r.Value.Equals(value))
            Return r
        End Operator
        Public Shared Narrowing Operator CType(value As RefNullable(Of T)) As T
            If value IsNot Nothing Then Return value.Value

            Dim isReferenceType = DirectCast(Nothing, T) Is Nothing
            If Not isReferenceType Then Throw New InvalidCastException(
                "Can't cast null value of type {0} to non-nullable type {1}.".Frmt(
                    GetType(RefNullable(Of T)).FullName,
                    GetType(T).FullName))
            Return Nothing
        End Operator

        Public Overloads Function Equals(other As RefNullable(Of T)) As Boolean Implements IEquatable(Of RefNullable(Of T)).Equals
            If other Is Nothing Then Return False
            Return Me.Value.Equals(other.Value)
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return Me.Equals(TryCast(obj, RefNullable(Of T)))
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Value.GetHashCode()
        End Function
        Public Overrides Function ToString() As String
            Return Value.ToString()
        End Function
        Public Shared Operator =(value1 As RefNullable(Of T), value2 As RefNullable(Of T)) As Boolean
            Return value1.NullableEquals(value2)
        End Operator
        Public Shared Operator <>(value1 As RefNullable(Of T), value2 As RefNullable(Of T)) As Boolean
            Return Not value1.NullableEquals(value2)
        End Operator
    End Class
End Namespace
