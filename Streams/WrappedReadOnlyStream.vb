Namespace Streams
    '''<summary>Wraps a substream, but forces writing and seeking calls to fail.</summary>
    Public MustInherit Class WrappedReadOnlyStream
        Inherits WrappedStream
        Protected Sub New(ByVal substream As IO.Stream)
            MyBase.New(substream)
            Contract.Requires(substream IsNot Nothing)
        End Sub

        Public NotOverridable Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return True
            End Get
        End Property
        Public NotOverridable Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return False
            End Get
        End Property
        Public NotOverridable Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return False
            End Get
        End Property

        Public NotOverridable Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            Throw New NotSupportedException()
        End Sub
        Public NotOverridable Overrides Sub Flush()
            Throw New NotSupportedException()
        End Sub
        Public NotOverridable Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function
        Public NotOverridable Overrides Sub SetLength(ByVal value As Long)
            Throw New NotSupportedException()
        End Sub
        Public NotOverridable Overrides ReadOnly Property Length() As Long
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public NotOverridable Overrides Property Position() As Long
            Get
                Throw New NotSupportedException()
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException()
            End Set
        End Property
    End Class
End Namespace
