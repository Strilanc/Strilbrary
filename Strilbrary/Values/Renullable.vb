Namespace Values
    '''<summary>
    ''' A nullable reference to a (potentially nullable) value.
    ''' Note that a null reference type is a equivalent to a Renullable storing a null, not a null Renullable.
    '''</summary>
    '''<remarks>Useful for optional arguments that need to distinguish between 'not specified' and 'null value'.</remarks>
    <DebuggerDisplay("{ToString()}")>
    Public Class Renullable(Of T)
        Implements IEquatable(Of Renullable(Of T))

        Private ReadOnly _value As T

        Public Sub New(value As T)
            Contract.Ensures(Me.Value.Equals(value))
            Me._value = value
            Contract.Assume(Me.Value.Equals(value))
        End Sub

        Public ReadOnly Property Value As T
            Get
                Return _value
            End Get
        End Property

        Public Shared Widening Operator CType(value As T) As Renullable(Of T)
            Contract.Requires(value IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Renullable(Of T))().Value.NullableEquals(value))
            Contract.Ensures(Contract.Result(Of Renullable(Of T))() IsNot Nothing)
            Dim r = New Renullable(Of T)(value)
            Contract.Assume(r.Value.NullableEquals(value))
            Return r
        End Operator
        Public Shared Narrowing Operator CType(value As Renullable(Of T)) As T
            If value IsNot Nothing Then Return value.Value
            Throw New InvalidCastException(
                "Can't cast null value of type {0} to type {1}.".Frmt(
                    GetType(RefNullable(Of T)).FullName,
                    GetType(T).FullName))
        End Operator

        Public Overloads Function Equals(other As Renullable(Of T)) As Boolean Implements IEquatable(Of Renullable(Of T)).Equals
            If other Is Nothing Then Return False
            Return Me.Value.NullableEquals(other.Value)
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return Me.Equals(TryCast(obj, Renullable(Of T)))
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Value.GetHashCode()
        End Function
        Public Overrides Function ToString() As String
            Return Value.ToString()
        End Function
        Public Shared Operator =(value1 As Renullable(Of T), value2 As Renullable(Of T)) As Boolean
            Return value1.NullableEquals(value2)
        End Operator
        Public Shared Operator <>(value1 As Renullable(Of T), value2 As Renullable(Of T)) As Boolean
            Return Not value1.NullableEquals(value2)
        End Operator
    End Class
End Namespace
