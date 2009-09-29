Public NotInheritable Class ExpensiveValue(Of T)
    Private ReadOnly func As Func(Of T)
    Private _value As T
    Private computed As Boolean

    <ContractInvariantMethod()> Private Sub ObjectInvariant()
        Contract.Invariant(computed OrElse func IsNot Nothing)
    End Sub

    Public Sub New(ByVal func As Func(Of T))
        Contract.Requires(func IsNot Nothing)
        Me.func = func
    End Sub
    Public Sub New(ByVal value As T)
        Me._value = value
        Me.computed = True
    End Sub
    Public ReadOnly Property Value As T
        Get
            If Not computed Then
                computed = True
                _value = func()
            End If
            Return _value
        End Get
    End Property
    Public Shared Widening Operator CType(ByVal func As Func(Of T)) As ExpensiveValue(Of T)
        Contract.Requires(func IsNot Nothing)
        Contract.Ensures(Contract.Result(Of ExpensiveValue(Of T))() IsNot Nothing)
        Return New ExpensiveValue(Of T)(func)
    End Operator
    Public Shared Widening Operator CType(ByVal value As T) As ExpensiveValue(Of T)
        Contract.Ensures(Contract.Result(Of ExpensiveValue(Of T))() IsNot Nothing)
        Return New ExpensiveValue(Of T)(value)
    End Operator
End Class
