Namespace Collections
    Public Class Tree(Of T)
        Private ReadOnly _children As IReadableList(Of Tree(Of T))
        Private ReadOnly _value As T

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_children IsNot Nothing)
        End Sub

        Public Sub New(ByVal value As T, ByVal children As IReadableList(Of Tree(Of T)))
            Contract.Requires(children IsNot Nothing)
            Contract.Ensures(Me._children Is children)
            Me._value = value
            Me._children = children
        End Sub

        Public ReadOnly Property Value As T
            Get
                Return _value
            End Get
        End Property
        Public ReadOnly Property Children As IReadableList(Of Tree(Of T))
            Get
                Contract.Ensures(Contract.Result(Of IReadableList(Of Tree(Of T)))() IsNot Nothing)
                Return _children
            End Get
        End Property
    End Class
End Namespace
