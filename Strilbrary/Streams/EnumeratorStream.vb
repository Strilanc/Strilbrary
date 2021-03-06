Namespace Streams
    Public NotInheritable Class EnumeratorStream
        Implements IReadableStream
        Private ReadOnly _enumerator As IEnumerator(Of Byte)

        <ContractInvariantMethod()> Private Shadows Sub ObjectInvariant()
            Contract.Invariant(_enumerator IsNot Nothing)
        End Sub

        Public Sub New(enumerator As IEnumerator(Of Byte))
            Contract.Requires(enumerator IsNot Nothing)
            Me._enumerator = enumerator
        End Sub

        Public Function Read(maxCount As Integer) As IRist(Of Byte) Implements IReadableStream.Read
            Dim result = New List(Of Byte)(capacity:=maxCount)
            While result.Count < maxCount AndAlso _enumerator.MoveNext
                result.Add(_enumerator.Current)
            End While
            Return result.AsRist
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            _enumerator.Dispose()
        End Sub
    End Class
End Namespace
