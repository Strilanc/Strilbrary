Namespace Streams
    '''<summary>Forwards all IO.Stream calls to a substream by default</summary>
    Public MustInherit Class WrappedStream
        Inherits IO.Stream
        Protected ReadOnly substream As IO.Stream
        Public Sub New(ByVal substream As IO.Stream)
            Contract.Requires(substream IsNot Nothing)
            Me.substream = substream
        End Sub
        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return substream.CanRead()
            End Get
        End Property
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return substream.CanSeek()
            End Get
        End Property
        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return substream.CanWrite()
            End Get
        End Property
        Public Overrides Sub Flush()
            substream.Flush()
        End Sub
        Public Overrides ReadOnly Property Length() As Long
            Get
                Return substream.Length
            End Get
        End Property
        Public Overrides Property Position() As Long
            Get
                Return substream.Position
            End Get
            Set(ByVal value As Long)
                substream.Position = value
            End Set
        End Property
        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Return substream.Read(buffer, offset, count)
        End Function
        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Return substream.Seek(offset, origin)
        End Function
        Public Overrides Sub SetLength(ByVal value As Long)
            substream.SetLength(value)
        End Sub
        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            substream.Write(buffer, offset, count)
        End Sub
        Public Overrides Sub Close()
            substream.Close()
            MyBase.Close()
        End Sub
        Public Overrides Function BeginRead(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, ByVal callback As System.AsyncCallback, ByVal state As Object) As System.IAsyncResult
            Return substream.BeginRead(buffer, offset, count, callback, state)
        End Function
        Public Overrides Function BeginWrite(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, ByVal callback As System.AsyncCallback, ByVal state As Object) As System.IAsyncResult
            Return substream.BeginWrite(buffer, offset, count, callback, state)
        End Function
        Public Overrides Function EndRead(ByVal asyncResult As System.IAsyncResult) As Integer
            Return substream.EndRead(asyncResult)
        End Function
        Public Overrides Sub EndWrite(ByVal asyncResult As System.IAsyncResult)
            substream.EndWrite(asyncResult)
        End Sub
    End Class
End Namespace
