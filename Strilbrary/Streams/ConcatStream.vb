Namespace Streams
    '''<summary>A read-only stream which reads through the contents of multiple readable streams.</summary>
    '''<remarks>Verification disabled due to weird notices appearing (without any associated location, making it hard to track down).</remarks>
    <ContractVerification(False)>
    Public NotInheritable Class ConcatStream
        Implements IReadableStream
        Private ReadOnly _streams As IEnumerator(Of IReadableStream)
        Private _emptied As Boolean

        <ContractInvariantMethod()> Private Shadows Sub ObjectInvariant()
            Contract.Invariant(_streams IsNot Nothing)
        End Sub

        Public Sub New(streams As IEnumerable(Of IReadableStream))
            Contract.Requires(streams IsNot Nothing)
            If (From stream In streams Where stream Is Nothing).Any Then
                Throw New ArgumentException("Null stream.")
            End If

            Me._streams = streams.GetEnumerator()
            _emptied = Not Me._streams.MoveNext()
        End Sub

        Public Function Read(maxCount As Integer) As IRist(Of Byte) Implements IReadableStream.Read
            Dim result = New List(Of Byte)
            While result.Count < maxCount AndAlso Not _emptied
                Contract.Assume(_streams.Current IsNot Nothing)
                Dim data = _streams.Current().Read(maxCount - result.Count)
                Select Case data.Count
                    Case 0
                        _streams.Current.Dispose()
                        _emptied = Not Me._streams.MoveNext()
                    Case maxCount
                        Return data
                    Case Else
                        result.AddRange(data)
                End Select
            End While
            Contract.Assume(result.Count <= maxCount)
            Return result.AsRist
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Do Until _emptied
                Contract.Assume(_streams.Current IsNot Nothing)
                _streams.Current.Dispose()
                _emptied = Not _streams.MoveNext
            Loop
        End Sub
    End Class
End Namespace
