'''<summary>Wraps a value which is computed and stored only once needed.</summary>
<DebuggerDisplay("ToString")>
Public NotInheritable Class LazyValue(Of T)
    Private func As Func(Of T)
    Private _value As T

    Public Sub New(ByVal func As Func(Of T))
        Contract.Requires(func IsNot Nothing)
        Me.func = func
    End Sub
    Public Sub New(ByVal value As T)
        Me._value = value
    End Sub

    Public ReadOnly Property Value As T
        Get
            If func IsNot Nothing Then
                _value = func()
                func = Nothing
            End If
            Return _value
        End Get
    End Property

    Public Shared Widening Operator CType(ByVal func As Func(Of T)) As LazyValue(Of T)
        Contract.Requires(func IsNot Nothing)
        Contract.Ensures(Contract.Result(Of LazyValue(Of T))() IsNot Nothing)
        Return New LazyValue(Of T)(func)
    End Operator
    Public Shared Widening Operator CType(ByVal value As T) As LazyValue(Of T)
        Contract.Ensures(Contract.Result(Of LazyValue(Of T))() IsNot Nothing)
        Return New LazyValue(Of T)(value)
    End Operator

    Public Overrides Function ToString() As String
        Return String.Concat(Me.Value)
    End Function
End Class
