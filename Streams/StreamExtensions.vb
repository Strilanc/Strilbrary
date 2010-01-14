Imports Strilbrary.Values

Namespace Streams
    Public Module StreamExtensions
        ''' <summary>
        ''' Reads until the buffer range has been filled or the stream has ended.
        ''' </summary>
        ''' <remarks>
        ''' Differs from Read in that it definitely won't partially fill the range if the stream has not ended.
        ''' </remarks>
        <Extension()>
        Public Function ReadUntilDone(ByVal stream As IO.Stream,
                                      ByVal buffer As Byte(),
                                      ByVal offset As Integer,
                                      ByVal length As Integer) As Integer
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Requires(buffer IsNot Nothing)
            Contract.Requires(offset >= 0)
            Contract.Requires(length >= 0)
            Contract.Requires(offset + length <= buffer.Length)
            Contract.Ensures(Contract.Result(Of Integer)() >= 0)
            Contract.Ensures(Contract.Result(Of Integer)() <= length)
            Dim numRead = 0
            Do Until numRead >= length
                Dim n = stream.Read(buffer, offset + numRead, length - numRead)
                numRead += n
                If n = 0 Then Exit Do
            Loop
            Contract.Assume(numRead <= length)
            Return numRead
        End Function

        ''' <summary>
        ''' Reads exactly byteCount bytes from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadBytesExact(ByVal stream As IO.Stream,
                                       ByVal length As Integer) As Byte()
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Requires(length >= 0)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = length)
            Dim buffer(0 To length - 1) As Byte
            Dim numRead = stream.ReadUntilDone(buffer, 0, length)
            If numRead < length Then
                Throw New IO.IOException("Stream ended before enough data could be read.")
            End If
            Return buffer
        End Function

        '''<summary>Reads all remaining data from the stream into a byte array.</summary>
        <Extension()>
        Public Function ReadRemaining(ByVal stream As IO.Stream) As Byte()
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Dim result = New List(Of Byte)(capacity:=1024)
            Dim buffer(0 To 4096 - 1) As Byte
            Do
                Dim n = stream.Read(buffer, 0, buffer.Length)
                If n = 0 Then Exit Do
                For i = 0 To n - 1
                    result.Add(buffer(i))
                Next i
            Loop
            Return result.ToArray
        End Function

        '''<summary>Writes the full contents of a buffer to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IO.Stream,
                         ByVal buffer As Byte())
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            Contract.Requires(buffer IsNot Nothing)
            stream.Write(buffer, 0, buffer.Length)
        End Sub

        '''<summary>Writes all remaining data in a stream to the file system.</summary>
        <Extension()>
        Public Sub WriteToFileSystem(ByVal stream As IO.Stream,
                                     ByVal fileName As String,
                                     Optional ByVal fileMode As IO.FileMode = IO.FileMode.CreateNew)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Requires(fileName IsNot Nothing)
            Using bufferedIn = New IO.BufferedStream(stream)
                Using bufferedOut = New IO.BufferedStream(New IO.FileStream(fileName, fileMode, IO.FileAccess.Write))
                    Do
                        Dim i = stream.ReadByte()
                        If i = -1 Then Exit Do
                        bufferedOut.WriteByte(CByte(i))
                    Loop
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Reads a UInt16 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt16(ByVal stream As IO.Stream,
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Return stream.ReadBytesExact(length:=2).ToUInt16(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt32 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt32(ByVal stream As IO.Stream,
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Return stream.ReadBytesExact(length:=4).ToUInt32(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt64 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt64(ByVal stream As IO.Stream,
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Return stream.ReadBytesExact(length:=8).ToUInt64(byteOrder)
        End Function

        '''<summary>Writes a UInt16 to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IO.Stream,
                         ByVal value As UInt16,
                         Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            stream.Write(value.Bytes(byteOrder))
        End Sub
        '''<summary>Writes a UInt32 to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IO.Stream,
                         ByVal value As UInt32,
                         Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            stream.Write(value.Bytes(byteOrder))
        End Sub
        '''<summary>Writes a UInt64 to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IO.Stream,
                         ByVal value As UInt64,
                         Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            stream.Write(value.Bytes(byteOrder))
        End Sub
    End Module
End Namespace
