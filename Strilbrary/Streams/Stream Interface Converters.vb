Imports Strilbrary.Collections

Namespace Streams
    Public Module StreamConverters
        Private NotInheritable Class StreamClassToInterfaceWrapper
            Implements IRandomAccessStream
            Private ReadOnly _stream As IO.Stream

            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_stream IsNot Nothing)
            End Sub

            Public Sub New(ByVal stream As IO.Stream)
                Contract.Requires(stream IsNot Nothing)
                Me._stream = stream
            End Sub

            Public ReadOnly Property BaseStream As IO.Stream
                Get
                    Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
                    Return _stream
                End Get
            End Property

            <ContractVerification(False)>
            Public Function Read(ByVal maxLength As Integer) As IReadableList(Of Byte) Implements IReadableStream.Read
                Dim buffer(0 To maxLength - 1) As Byte
                Dim n = _stream.Read(buffer, 0, maxLength)
                Return New ListView(Of Byte)(buffer.AsReadableList, 0, n)
            End Function

            Public Sub Write(ByVal data As Collections.IReadableList(Of Byte)) Implements IWritableStream.Write
                Contract.Assume(_stream.CanWrite)
                _stream.Write(data.ToArray)
            End Sub
            Public Sub Flush() Implements IWritableStream.Flush
                _stream.Flush()
            End Sub

            Public Property Position As Long Implements ISeekableStream.Position
                Get
                    Contract.Assume(_stream.Position <= Length)
                    Return _stream.Position
                End Get
                Set(ByVal value As Long)
                    _stream.Position = value
                End Set
            End Property
            Public ReadOnly Property Length As Long Implements ISeekableStream.Length
                Get
                    Return _stream.Length
                End Get
            End Property

            Public Sub Dispose() Implements IDisposable.Dispose
                _stream.Dispose()
            End Sub
        End Class

        <Extension()> <Pure()>
        Public Function AsReadableStream(ByVal stream As IO.Stream) As IReadableStream
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Ensures(Contract.Result(Of IReadableStream)() IsNot Nothing)
            Return New StreamClassToInterfaceWrapper(stream)
        End Function
        <Extension()> <Pure()>
        Public Function AsWritableStream(ByVal stream As IO.Stream) As IWritableStream
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            Contract.Ensures(Contract.Result(Of IWritableStream)() IsNot Nothing)
            Return New StreamClassToInterfaceWrapper(stream)
        End Function
        <Extension()> <Pure()>
        Public Function AsRandomWritableStream(ByVal stream As IO.Stream) As IRandomWritableStream
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            Contract.Requires(stream.CanSeek)
            Contract.Ensures(Contract.Result(Of IRandomWritableStream)() IsNot Nothing)
            Return New StreamClassToInterfaceWrapper(stream)
        End Function
        <Extension()> <Pure()>
        Public Function AsRandomReadableStream(ByVal stream As IO.Stream) As IRandomReadableStream
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Requires(stream.CanSeek)
            Contract.Ensures(Contract.Result(Of IRandomReadableStream)() IsNot Nothing)
            Return New StreamClassToInterfaceWrapper(stream)
        End Function
        <Extension()> <Pure()>
        Public Function AsRandomAccessStream(ByVal stream As IO.Stream) As IRandomAccessStream
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Requires(stream.CanWrite)
            Contract.Requires(stream.CanSeek)
            Contract.Ensures(Contract.Result(Of IRandomAccessStream)() IsNot Nothing)
            Return New StreamClassToInterfaceWrapper(stream)
        End Function

        Private NotInheritable Class StreamInterfaceToClassWrapper
            Inherits IO.Stream

            Private ReadOnly _readStream As IReadableStream
            Private ReadOnly _seekStream As ISeekableStream
            Private ReadOnly _writeStream As IWritableStream

            <ContractVerification(False)>
            Public Sub New(ByVal readStream As IReadableStream, ByVal seekStream As ISeekableStream, ByVal writeStream As IWritableStream)
                Contract.Ensures(Me.CanRead = (readStream IsNot Nothing))
                Contract.Ensures(Me.CanSeek = (seekStream IsNot Nothing))
                Contract.Ensures(Me.CanWrite = (writeStream IsNot Nothing))
                Me._readStream = readStream
                Me._seekStream = seekStream
                Me._writeStream = writeStream
            End Sub

            Public Overrides ReadOnly Property CanRead As Boolean
                Get
                    Return _readStream IsNot Nothing
                End Get
            End Property
            Public Overrides ReadOnly Property CanSeek As Boolean
                Get
                    Return _seekStream IsNot Nothing
                End Get
            End Property
            Public Overrides ReadOnly Property CanWrite As Boolean
                Get
                    Return _writeStream IsNot Nothing
                End Get
            End Property

            <ContractVerification(False)>
            Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
                If _readStream Is Nothing Then Throw New NotSupportedException()
                If count = 0 Then Return 0
                Dim data = _readStream.Read(count)
                For i = 0 To data.Count - 1
                    buffer(i + offset) = data(i)
                Next i
                Return data.Count
            End Function

            <ContractVerification(False)>
            Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
                If _writeStream Is Nothing Then Throw New NotSupportedException()
                _writeStream.Write(buffer.AsReadableList.SubView(offset, count))
            End Sub
            Public Overrides Sub Flush()
                If _writeStream IsNot Nothing Then _writeStream.Flush()
            End Sub

            Public Overrides Property Position As Long
                Get
                    If _seekStream Is Nothing Then Throw New NotSupportedException()
                    Return _seekStream.Position
                End Get
                <ContractVerification(False)>
                Set(ByVal value As Long)
                    If _seekStream Is Nothing Then Throw New NotSupportedException()
                    If value > _seekStream.Length Then value = _seekStream.Length
                    _seekStream.Position = value
                End Set
            End Property
            Public Overrides ReadOnly Property Length As Long
                Get
                    If _seekStream Is Nothing Then Throw New NotSupportedException()
                    Return _seekStream.Length
                End Get
            End Property
            Public Overrides Function Seek(ByVal offset As Long, ByVal origin As IO.SeekOrigin) As Long
                If _seekStream Is Nothing Then Throw New NotSupportedException()
                If origin = IO.SeekOrigin.Current Then offset += Position
                If origin = IO.SeekOrigin.End Then offset += Length
                If offset < 0 Then offset = 0
                Position = offset
                Return Position
            End Function
            Public Overrides Sub SetLength(ByVal value As Long)
                Throw New NotSupportedException()
            End Sub

            Public Overrides Sub Close()
                If _readStream IsNot Nothing Then _readStream.Dispose()
                If _seekStream IsNot Nothing Then _seekStream.Dispose()
                If _writeStream IsNot Nothing Then _writeStream.Dispose()
                MyBase.Close()
            End Sub
        End Class

        <Extension()> <Pure()>
        Public Function AsStream(ByVal stream As IReadableStream) As IO.Stream
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanRead)
            Dim wrapper = TryCast(stream, StreamClassToInterfaceWrapper)
            If wrapper IsNot Nothing Then
                Contract.Assume(wrapper.BaseStream.CanRead)
                Return wrapper.BaseStream
            End If
            Return New StreamInterfaceToClassWrapper(readStream:=stream, seekStream:=Nothing, writeStream:=Nothing)
        End Function
        <Extension()> <Pure()>
        Public Function AsStream(ByVal stream As IWritableStream) As IO.Stream
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanWrite)
            Dim wrapper = TryCast(stream, StreamClassToInterfaceWrapper)
            If wrapper IsNot Nothing Then
                Contract.Assume(wrapper.BaseStream.CanWrite)
                Return wrapper.BaseStream
            End If
            Return New StreamInterfaceToClassWrapper(readStream:=Nothing, seekStream:=Nothing, writeStream:=stream)
        End Function
        <Extension()> <Pure()>
        Public Function AsStream(ByVal stream As IRandomReadableStream) As IO.Stream
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanRead)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanSeek)
            Dim wrapper = TryCast(stream, StreamClassToInterfaceWrapper)
            If wrapper IsNot Nothing Then
                Contract.Assume(wrapper.BaseStream.CanRead)
                Contract.Assume(wrapper.BaseStream.CanSeek)
                Return wrapper.BaseStream
            End If
            Return New StreamInterfaceToClassWrapper(readStream:=stream, seekStream:=stream, writeStream:=Nothing)
        End Function
        <Extension()> <Pure()>
        Public Function AsStream(ByVal stream As IRandomWritableStream) As IO.Stream
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanWrite)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanSeek)
            Dim wrapper = TryCast(stream, StreamClassToInterfaceWrapper)
            If wrapper IsNot Nothing Then
                Contract.Assume(wrapper.BaseStream.CanWrite)
                Contract.Assume(wrapper.BaseStream.CanSeek)
                Return wrapper.BaseStream
            End If
            Return New StreamInterfaceToClassWrapper(readStream:=Nothing, seekStream:=stream, writeStream:=stream)
        End Function
        <Extension()> <Pure()>
        Public Function AsStream(ByVal stream As IRandomAccessStream) As IO.Stream
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanRead)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanWrite)
            Contract.Ensures(Contract.Result(Of IO.Stream)().CanSeek)
            Dim wrapper = TryCast(stream, StreamClassToInterfaceWrapper)
            If wrapper IsNot Nothing Then
                Contract.Assume(wrapper.BaseStream.CanRead)
                Contract.Assume(wrapper.BaseStream.CanWrite)
                Contract.Assume(wrapper.BaseStream.CanSeek)
                Return wrapper.BaseStream
            End If
            Return New StreamInterfaceToClassWrapper(readStream:=stream, seekStream:=stream, writeStream:=stream)
        End Function
    End Module
End Namespace