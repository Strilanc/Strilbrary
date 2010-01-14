Namespace Streams
    '''<summary>A read-only stream which reads through the contents of multiple readable streams.</summary>
    Public NotInheritable Class ConcatStream
        Inherits ReadOnlyStream
        Private ReadOnly streams As IEnumerator(Of IO.Stream)
        Private emptied As Boolean

        <ContractInvariantMethod()> Private Shadows Sub ObjectInvariant()
            Contract.Invariant(streams IsNot Nothing)
        End Sub

        Public Sub New(ByVal streams As IEnumerable(Of IO.Stream))
            Contract.Requires(streams IsNot Nothing)
            If (From stream In streams
                Where stream Is Nothing OrElse Not stream.CanRead).Any Then Throw New ArgumentException("streams contains a null or non-readable member.")

            Me.streams = streams.GetEnumerator()
            emptied = Not Me.streams.MoveNext()
        End Sub

        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Dim t = 0
            While count > 0 And Not emptied
                Contract.Assume(streams.Current IsNot Nothing)
                Dim n = streams.Current().Read(buffer, offset, count)
                t += n
                count -= n
                offset += n
                If n = 0 Then
                    streams.Current.Close()
                    emptied = Not Me.streams.MoveNext()
                End If
            End While
            Return t
        End Function

        Public Overrides Sub Close()
            Do Until emptied
                Contract.Assume(streams.Current IsNot Nothing)
                streams.Current.Close()
                emptied = Not streams.MoveNext
            Loop
        End Sub
    End Class
End Namespace
