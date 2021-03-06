﻿Imports Strilbrary.Values
Imports Strilbrary.Exceptions

Namespace Streams
    Public Module StreamExtensions
        ''' <summary>
        ''' Reads until the buffer range has been filled or the stream has ended.
        ''' </summary>
        ''' <remarks>
        ''' Differs from Read in that it definitely won't partially fill the range if the stream has not ended.
        ''' </remarks>
        <Extension()>
        Public Function ReadUntilDone(stream As IO.Stream,
                                      buffer As Byte(),
                                      offset As Integer,
                                      length As Integer) As Integer
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
        Public Function ReadBytesExact(stream As IO.Stream,
                                       length As Integer) As Byte()
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
        Public Function ReadRemaining(stream As IO.Stream) As Byte()
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
        Public Sub Write(stream As IO.Stream,
                         buffer As Byte())
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            Contract.Requires(buffer IsNot Nothing)
            stream.Write(buffer, 0, buffer.Length)
        End Sub

        '''<summary>Writes all remaining data in a stream to the file system.</summary>
        <Extension()>
        Public Sub WriteToFileSystem(stream As IO.Stream,
                                     fileName As String,
                                     Optional fileMode As IO.FileMode = IO.FileMode.CreateNew)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Contract.Requires(Not String.IsNullOrEmpty(fileName))
            Using bufferedOut = New IO.BufferedStream(New IO.FileStream(fileName, fileMode, IO.FileAccess.Write))
                Do
                    Dim i = stream.ReadByte()
                    If i = -1 Then Exit Do
                    bufferedOut.WriteByte(CByte(i))
                Loop
            End Using
        End Sub

        ''' <summary>
        ''' Reads a UInt16 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt16(stream As IO.Stream,
                                   Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Return stream.ReadBytesExact(length:=2).ToUInt16(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt32 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt32(stream As IO.Stream,
                                   Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Return stream.ReadBytesExact(length:=4).ToUInt32(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt64 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt64(stream As IO.Stream,
                                   Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanRead)
            Return stream.ReadBytesExact(length:=8).ToUInt64(byteOrder)
        End Function

        '''<summary>Writes a UInt16 to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IO.Stream,
                         value As UInt16,
                         Optional byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            stream.Write(value.Bytes(byteOrder).ToArray())
        End Sub
        '''<summary>Writes a UInt32 to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IO.Stream,
                         value As UInt32,
                         Optional byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            stream.Write(value.Bytes(byteOrder).ToArray())
        End Sub
        '''<summary>Writes a UInt64 to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IO.Stream,
                         value As UInt64,
                         Optional byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(stream.CanWrite)
            stream.Write(value.Bytes(byteOrder).ToArray())
        End Sub
    End Module

    Public Module StreamInterfaceExtensions
        ''' <summary>
        ''' Reads bytes from the stream.
        ''' If less than the given maxCount is read, then the stream has ended.
        ''' </summary>
        <Extension()>
        Public Function ReadBestEffort(stream As IReadableStream,
                                       maxCount As Integer) As IRist(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(maxCount >= 0)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))().Count <= maxCount)
            Dim result = New List(Of Byte)(capacity:=maxCount)
            While result.Count < maxCount
                Dim readData = stream.Read(maxCount - result.Count)
                Select Case readData.Count
                    Case 0 : Exit While
                    Case maxCount : Return readData
                    Case Else : result.AddRange(readData)
                End Select
            End While
            Contract.Assume(result.Count = maxCount)
            Return result.AsRist
        End Function
        ''' <summary>
        ''' Reads an exact amount of bytes from the stream (as opposed to the default Read which may return fewer bytes).
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadExact(stream As IReadableStream,
                                  exactCount As Integer) As IRist(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(exactCount >= 0)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))().Count = exactCount)
            Dim result = ReadBestEffort(stream, maxCount:=exactCount)
            If result.Count < exactCount Then
                Throw New IO.IOException("Stream ended before enough data could be read.")
            End If
            Return result
        End Function
        ''' <summary>
        ''' Writes bytes to the stream, starting at the given position.
        ''' </summary>
        <Extension()>
        Public Sub WriteAt(stream As IRandomWritableStream,
                           position As Long,
                           data As IRist(Of Byte))
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(position >= 0)
            Contract.Requires(position <= stream.Length)
            Contract.Ensures(stream.Position = position + data.Count)
            stream.Position = position
            stream.Write(data)
            Contract.Assume(stream.Position = position + data.Count)
        End Sub
        ''' <summary>
        ''' Reads an exact amount of bytes from the stream, starting at the given position.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadExactAt(stream As IRandomReadableStream,
                                    position As Long,
                                    exactCount As Integer) As IRist(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(position >= 0)
            Contract.Requires(exactCount >= 0)
            Contract.Requires(position + exactCount <= stream.Length)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))().Count = exactCount)
            Contract.Ensures(stream.Position = position + exactCount)
            stream.Position = position
            Dim result = stream.ReadExact(exactCount)
            Contract.Assume(stream.Position = position + exactCount)
            Return result
        End Function

        ''' <summary>
        ''' Reads a Byte from the stream.
        ''' Returns nothing if at the end of the stream.
        ''' </summary>
        <Extension()>
        Public Function TryReadByte(stream As IReadableStream) As Byte?
            Contract.Requires(stream IsNot Nothing)
            Dim read = stream.Read(1)
            If read.Count <> 1 Then Return Nothing
            Contract.Assume(DirectCast(read, IEnumerable(Of Byte)).Count() = 1)
            Return read.Single()
        End Function

        ''' <summary>
        ''' Reads a Byte from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadByte(stream As IReadableStream) As Byte
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=1)(0)
        End Function
        ''' <summary>
        ''' Reads a UInt16 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt16(stream As IReadableStream,
                                   Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=2).ToUInt16(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt32 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt32(stream As IReadableStream,
                                   Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=4).ToUInt32(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt64 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt64(stream As IReadableStream,
                                   Optional byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=8).ToUInt64(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a Single from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        <SuppressMessage("Microsoft.Contracts", "Requires-53-25")>
        Public Function ReadSingle(stream As IReadableStream) As Single
            Contract.Requires(stream IsNot Nothing)
            Return BitConverter.ToSingle(stream.ReadExact(4).ToArray, 0)
        End Function
        ''' <summary>
        ''' Reads a Double from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        <SuppressMessage("Microsoft.Contracts", "Requires-53-25")>
        Public Function ReadDouble(stream As IReadableStream) As Double
            Contract.Requires(stream IsNot Nothing)
            Return BitConverter.ToDouble(stream.ReadExact(8).ToArray, 0)
        End Function

        '''<summary>Writes a Byte to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IWritableStream,
                         value As Byte)
            Contract.Requires(stream IsNot Nothing)
            stream.Write({value}.AsRist)
        End Sub
        '''<summary>Writes a UInt16 to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IWritableStream,
                         value As UInt16,
                         Optional byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(value.Bytes(byteOrder))
        End Sub
        '''<summary>Writes a UInt32 to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IWritableStream,
                         value As UInt32,
                         Optional byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(value.Bytes(byteOrder))
        End Sub
        '''<summary>Writes a UInt64 to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IWritableStream,
                         value As UInt64,
                         Optional byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(value.Bytes(byteOrder))
        End Sub
        '''<summary>Writes a Single to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IWritableStream,
                         value As Single)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(BitConverter.GetBytes(value).AsRist())
        End Sub
        '''<summary>Writes a Double to the stream.</summary>
        <Extension()>
        Public Sub Write(stream As IWritableStream,
                         value As Double)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(BitConverter.GetBytes(value).AsRist())
        End Sub

        '''<summary>Reads all remaining data from the stream.</summary>
        <Extension()>
        Public Function ReadRemaining(stream As IReadableStream) As IRist(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IRist(Of Byte))() IsNot Nothing)
            Dim result = New List(Of Byte)(capacity:=1024)
            Do
                Dim read = stream.Read(1024)
                If read.Count = 0 Then Exit Do
                result.AddRange(read)
            Loop
            Return result.AsRist
        End Function

        '''<summary>Exposes a sequence of bytes as an IReadableStream.</summary>
        <Extension()> <Pure()>
        Public Function AsReadableStream(sequence As IEnumerable(Of Byte)) As IReadableStream
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IReadableStream)() IsNot Nothing)
            Return New EnumeratorStream(sequence.GetEnumerator)
        End Function
        '''<summary>Exposes a sequence of bytes as an IReadableStream.</summary>
        <Extension()> <Pure()>
        Public Function AsReadableStream(sequence As IEnumerator(Of Byte)) As IReadableStream
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IReadableStream)() IsNot Nothing)
            Return New EnumeratorStream(sequence)
        End Function

        '''<summary>A ZLibStream is a DeflateStream with two magic bytes preceding the compressed data.</summary>
        Public Function MakeZLibStream(stream As IO.Stream,
                                       mode As IO.Compression.CompressionMode,
                                       Optional leaveOpen As Boolean = False) As IO.Stream
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
    End Module
End Namespace
