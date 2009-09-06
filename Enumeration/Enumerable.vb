Namespace Enumeration
    ''' <summary>
    ''' Uses a lambda expression to create enumerators.
    ''' </summary>
    Public NotInheritable Class Enumerable(Of T)
        Implements IEnumerable(Of T)
        Private ReadOnly generator As Func(Of IEnumerator(Of T))

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(generator IsNot Nothing)
        End Sub

        Public Sub New(ByVal generator As Func(Of IEnumerator(Of T)))
            Contract.Requires(generator IsNot Nothing)
            Me.generator = generator
        End Sub
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Dim x = generator()
            If x Is Nothing Then Throw New OperationFailedException("The generator function returned a null value.")
            Return x
        End Function
        Private Function GetEnumeratorObj() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace