Namespace Collections
    '''<summary>An immutable linked list node.</summary>
    Public Class Link(Of T)
        Public ReadOnly Value As T
        Public ReadOnly [Next] As Link(Of T)

        Public Sub New(value As T, Optional [next] As Link(Of T) = Nothing)
            Contract.Ensures(Strilbrary.Values.NullableEquals(Me.Value, value))
            Contract.Ensures(Me.Next Is [next])
            Me.Value = value
            Me.Next = [next]
            Contract.Assume(Strilbrary.Values.NullableEquals(Me.Value, value))
        End Sub
    End Class
    Public Module LinkExtensions
        '''<summary>Iterates nodes in a linked list, starting with the given node as the head.</summary>
        <Pure()> <Extension()>
        Public Function Nodes(Of T)(this As Link(Of T)) As IEnumerable(Of Link(Of T))
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Link(Of T)))() IsNot Nothing)
            Dim r = Iterator Function()
                        Dim n = this
                        While n IsNot Nothing
                            Yield (n)
                            n = n.Next
                        End While
                    End Function()
            Contract.Assume(r IsNot Nothing)
            Return r
        End Function

        '''<summary>Iterates values in a linked list, starting with the given node as the head.</summary>
        <Pure()> <Extension()>
        Public Function Values(Of T)(this As Link(Of T)) As IEnumerable(Of T)
            Contract.Requires(this IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Return this.Nodes.Select(Function(n) n.Value)
        End Function
    End Module
End Namespace
