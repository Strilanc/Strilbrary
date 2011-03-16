Imports Strilbrary.Exceptions

Namespace Values
    <DebuggerDisplay("{ToString()}")>
    Public Structure NonNull(Of T)
        Implements IEquatable(Of NonNull(Of T))

        Private ReadOnly _value As T

        Public Sub New(value As T)
            Contract.Requires(value IsNot Nothing)
            Me._value = value
        End Sub

        Public ReadOnly Property Value As T
            Get
                Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
                If _value Is Nothing Then Throw New InvalidStateException("NonNull value of type {0} is Null.".Frmt(GetType(T).FullName))
                Return _value
            End Get
        End Property

        Public Shared Widening Operator CType(value As T) As NonNull(Of T)
            Contract.Requires(value IsNot Nothing)
            Return New NonNull(Of T)(value)
        End Operator
        Public Shared Widening Operator CType(value As NonNull(Of T)) As T
            Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
            Return value.Value
        End Operator

        Public Overrides Function ToString() As String
            Return Value.ToString()
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Value.GetHashCode()
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is NonNull(Of T) Then Return Value.Equals(DirectCast(obj, NonNull(Of T)).Value)
            If TypeOf obj Is T Then Return Value.Equals(DirectCast(obj, T))
            Return False
        End Function
        Public Overloads Function Equals(other As NonNull(Of T)) As Boolean Implements IEquatable(Of NonNull(Of T)).Equals
            Return Me.Value.Equals(other.Value)
        End Function
        Public Shared Operator =(value1 As NonNull(Of T), value2 As NonNull(Of T)) As Boolean
            Return value1.Value.Equals(value2.Value)
        End Operator
        Public Shared Operator <>(value1 As NonNull(Of T), value2 As NonNull(Of T)) As Boolean
            Return Not value1.Value.Equals(value2.Value)
        End Operator
    End Structure
End Namespace
