Imports Strilbrary.Values
Imports Strilbrary.Collections

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
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
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

    Public Module StreamInterfaceExtensions
        ''' <summary>
        ''' Reads an exact amount of bytes from the stream (as opposed to the default Read which may return fewer bytes).
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        <ContractVerification(False)>
        Public Function ReadExact(ByVal stream As IReadableStream,
                                  ByVal exactCount As Integer) As IReadableList(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(exactCount > 0)
            Contract.Ensures(Contract.Result(Of IReadableList(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IReadableList(Of Byte))().Count = exactCount)
            Dim result = New List(Of Byte)(capacity:=exactCount)
            While result.Count < exactCount
                Dim read = stream.Read(exactCount - result.Count)
                If read.Count = 0 Then Throw New IO.IOException("Stream ended before enough data could be read.")
                result.AddRange(read)
            End While
            Contract.Assume(result.Count = exactCount)
            Return result.AsReadableList
        End Function
        ''' <summary>
        ''' Writes bytes to the stream, starting at the given position.
        ''' </summary>
        <Extension()>
        <ContractVerification(False)>
        Public Sub WriteAt(ByVal stream As IRandomWritableStream,
                           ByVal position As Long,
                           ByVal data As IReadableList(Of Byte))
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(position >= 0)
            Contract.Requires(position <= stream.Length)
            Contract.Ensures(stream.Position = position + data.Count)
            stream.Position = position
            stream.Write(data)
        End Sub
        ''' <summary>
        ''' Reads an exact amount of bytes from the stream, starting at the given position.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        <ContractVerification(False)>
        Public Function ReadExactAt(ByVal stream As IRandomReadableStream,
                                    ByVal position As Long,
                                    ByVal exactCount As Integer) As IReadableList(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Requires(position >= 0)
            Contract.Requires(exactCount > 0)
            Contract.Requires(position + exactCount <= stream.Length)
            Contract.Ensures(Contract.Result(Of IReadableList(Of Byte))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IReadableList(Of Byte))().Count = exactCount)
            Contract.Ensures(stream.Position = position + exactCount)
            stream.Position = position
            Return stream.ReadExact(exactCount)
        End Function

        ''' <summary>
        ''' Reads a Byte from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        <ContractVerification(False)>
        Public Function ReadByte(ByVal stream As IReadableStream) As Byte
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=1)(0)
        End Function
        ''' <summary>
        ''' Reads a UInt16 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt16(ByVal stream As IReadableStream,
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt16
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=2).ToUInt16(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt32 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt32(ByVal stream As IReadableStream,
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt32
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=4).ToUInt32(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a UInt64 from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadUInt64(ByVal stream As IReadableStream,
                                   Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian) As UInt64
            Contract.Requires(stream IsNot Nothing)
            Return stream.ReadExact(exactCount:=8).ToUInt64(byteOrder)
        End Function
        ''' <summary>
        ''' Reads a Single from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadSingle(ByVal stream As IReadableStream) As Single
            Contract.Requires(stream IsNot Nothing)
            Return BitConverter.ToSingle(stream.ReadExact(4).ToArray, 0)
        End Function
        ''' <summary>
        ''' Reads a Double from the stream.
        ''' Throws an IOException if the stream ends prematurely.
        ''' </summary>
        <Extension()>
        Public Function ReadDouble(ByVal stream As IReadableStream) As Double
            Contract.Requires(stream IsNot Nothing)
            Return BitConverter.ToDouble(stream.ReadExact(8).ToArray, 0)
        End Function

        '''<summary>Writes a Byte to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IWritableStream,
                         ByVal value As Byte)
            Contract.Requires(stream IsNot Nothing)
            stream.Write({value}.AsReadableList)
        End Sub
        '''<summary>Writes a UInt16 to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IWritableStream,
                         ByVal value As UInt16,
                         Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(value.Bytes(byteOrder).AsReadableList)
        End Sub
        '''<summary>Writes a UInt32 to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IWritableStream,
                         ByVal value As UInt32,
                         Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(value.Bytes(byteOrder).AsReadableList)
        End Sub
        '''<summary>Writes a UInt64 to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IWritableStream,
                         ByVal value As UInt64,
                         Optional ByVal byteOrder As ByteOrder = ByteOrder.LittleEndian)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(value.Bytes(byteOrder).AsReadableList)
        End Sub
        '''<summary>Writes a Single to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IWritableStream,
                         ByVal value As Single)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(BitConverter.GetBytes(value).AsReadableList)
        End Sub
        '''<summary>Writes a Double to the stream.</summary>
        <Extension()>
        Public Sub Write(ByVal stream As IWritableStream,
                         ByVal value As Double)
            Contract.Requires(stream IsNot Nothing)
            stream.Write(BitConverter.GetBytes(value).AsReadableList)
        End Sub
    End Module
End Namespace
