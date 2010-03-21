Imports Strilbrary.Exceptions

Namespace Values
    <DebuggerDisplay("{ToString}")>
    Public Structure NonNull(Of T As Class)
        Private ReadOnly _value As T

        Public Sub New(ByVal value As T)
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

        Public Shared Widening Operator CType(ByVal value As T) As NonNull(Of T)
            Contract.Requires(value IsNot Nothing)
            Return New NonNull(Of T)(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As NonNull(Of T)) As T
            Contract.Ensures(Contract.Result(Of T)() IsNot Nothing)
            Return value.Value
        End Operator

        Public Overrides Function ToString() As String
            Return _value.ToString
        End Function
    End Structure

    Public Module NonNullExtensions
        <Pure()> <Extension()>
        Public Function AsNonNull(Of T As Class)(ByVal value As T) As NonNull(Of T)
            Contract.Requires(value IsNot Nothing)
            Return New NonNull(Of T)(value)
        End Function
    End Module
End Namespace
