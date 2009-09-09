Namespace Streams
    '''<summary>A DeflateStream with two magic bytes preceding the compressed data.</summary>
    Public Class ZLibStream
        Inherits WrappedStream

        Private Shared Function wrap(ByVal stream As IO.Stream,
                                     ByVal mode As IO.Compression.CompressionMode,
                                     ByVal leaveOpen As Boolean) As IO.Stream
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)

            Select Case mode
                Case IO.Compression.CompressionMode.Decompress
                    stream.ReadByte()
                    stream.ReadByte()
                Case IO.Compression.CompressionMode.Compress
                    stream.WriteByte(120)
                    stream.WriteByte(156)
                Case Else
                    Throw mode.MakeArgumentValueException("mode")
            End Select

            Return New IO.Compression.DeflateStream(stream, mode, leaveOpen)
        End Function
        Public Sub New(ByVal substream As IO.Stream,
                       ByVal mode As IO.Compression.CompressionMode,
                       Optional ByVal leaveOpen As Boolean = False)
            MyBase.New(wrap(substream, mode, leaveOpen))
            Contract.Requires(substream IsNot Nothing)
        End Sub
    End Class
End Namespace
