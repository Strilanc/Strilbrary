Imports System.Runtime.CompilerServices

Namespace Enumeration
    <ContractClass(GetType(ContractClassForIConverter(Of ,)))>
    Public Interface IConverter(Of In TInput, Out TOutput)
        Function Convert(ByVal sequence As IEnumerator(Of TInput)) As IEnumerator(Of TOutput)
    End Interface

    <ContractClassFor(GetType(IConverter(Of ,)))>
    Public NotInheritable Class ContractClassForIConverter(Of TInput, TOutput)
        Implements IConverter(Of TInput, TOutput)
        Public Function Convert(ByVal sequence As IEnumerator(Of TInput)) As IEnumerator(Of TOutput) Implements IConverter(Of TInput, TOutput).Convert
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerator(Of TOutput))() IsNot Nothing)
            Throw New NotSupportedException()
        End Function
    End Class

    Public Module ExtensionsIConverter
        <Extension()> Public Function Convert(Of TIn, TOut)(ByVal converter As IConverter(Of TIn, TOut),
                                                            ByVal sequence As IEnumerable(Of TIn)) As IEnumerable(Of TOut)
            Contract.Requires(converter IsNot Nothing)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TOut))() IsNot Nothing)
            Return sequence.Transform(AddressOf converter.Convert)
        End Function

        <Extension()> Public Function ToWritableStream(ByVal enumerator As PushEnumerator(Of Byte)) As IO.Stream
            Contract.Requires(enumerator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Return New PushEnumeratorStream(enumerator)
        End Function
        <Extension()> Public Function ToReadableStream(ByVal enumerator As IEnumerator(Of Byte)) As IO.Stream
            Contract.Requires(enumerator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Return New EnumeratorStream(enumerator)
        End Function
        <Extension()> Public Function ToReadEnumerator(ByVal stream As IO.Stream) As IEnumerator(Of Byte)
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerator(Of Byte))() IsNot Nothing)
            Return New Enumerator(Of Byte)(Function(controller)
                                               Dim r = stream.ReadByte()
                                               If r = -1 Then Return controller.Break()
                                               Return CByte(r)
                                           End Function,
                                           AddressOf stream.Dispose)
        End Function
        <Extension()> Public Function ToWritePushEnumerator(Of T)(ByVal stream As IO.Stream,
                                                                  ByVal converter As IConverter(Of T, Byte)) As PushEnumerator(Of T)
            Contract.Requires(converter IsNot Nothing)
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of PushEnumerator(Of T))() IsNot Nothing)
            Return New PushEnumerator(Of T)(Sub(sequenceT)
                                                Dim sequence = converter.Convert(sequenceT)
                                                While sequence.MoveNext
                                                    stream.WriteByte(sequence.Current)
                                                End While
                                                stream.Close()
                                            End Sub,
                                            AddressOf stream.Dispose)
        End Function

        <Extension()> Public Function ConvertReadOnlyStream(ByVal converter As IConverter(Of Byte, Byte), ByVal stream As IO.Stream) As IO.Stream
            Contract.Requires(converter IsNot Nothing)
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Return converter.Convert(stream.ToReadEnumerator()).ToReadableStream()
        End Function
        <Extension()> Public Function ConvertWriteOnlyStream(ByVal converter As IConverter(Of Byte, Byte), ByVal stream As IO.Stream) As IO.Stream
            Contract.Requires(converter IsNot Nothing)
            Contract.Requires(stream IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IO.Stream)() IsNot Nothing)
            Return stream.ToWritePushEnumerator(converter).ToWritableStream()
        End Function

        <Extension()> Public Function MoveNextAndReturn(Of T)(ByVal enumerator As IEnumerator(Of T)) As T
            Contract.Requires(enumerator IsNot Nothing)
            If Not enumerator.MoveNext Then Throw New InvalidOperationException("Ran past end of enumerator")
            Return enumerator.Current()
        End Function
    End Module

    Friend NotInheritable Class EnumeratorStream
        Inherits IO.Stream
        Private ReadOnly sequence As IEnumerator(Of Byte)
        Private closed As Boolean

        <ContractInvariantMethod()> Private Shadows Sub ObjectInvariant()
            Contract.Invariant(sequence IsNot Nothing)
        End Sub

        Public Sub New(ByVal sequence As IEnumerator(Of Byte))
            Contract.Requires(sequence IsNot Nothing)
            Me.sequence = sequence
        End Sub

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return True
            End Get
        End Property

        Public Shadows Function ReadByte() As Integer
            If Not sequence.MoveNext Then Return -1
            Return sequence.Current
        End Function
        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Contract.Assume(buffer IsNot Nothing)
            Contract.Assume(offset >= 0)
            Contract.Assume(count >= 0)
            Contract.Assume(offset + count <= buffer.Length)
            For n = 0 To count - 1
                Dim r = ReadByte()
                If r = -1 Then Return n
                buffer(n + offset) = CByte(r)
            Next n
            Return count
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso Not closed Then
                closed = True
                sequence.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Not Supported"
        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property Length As Long
            Get
                Throw New NotSupportedException()
            End Get
        End Property

        Public Overrides Property Position As Long
            Get
                Throw New NotSupportedException()
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException()
            End Set
        End Property
        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function
        Public Overrides Sub SetLength(ByVal value As Long)
            Throw New NotSupportedException()
        End Sub
        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            Throw New NotSupportedException()
        End Sub
        Public Overrides Sub Flush()
        End Sub
#End Region
    End Class

    Friend NotInheritable Class PushEnumeratorStream
        Inherits IO.Stream
        Private ReadOnly pusher As PushEnumerator(Of Byte)
        Private closed As Boolean

        <ContractInvariantMethod()> Private Shadows Sub ObjectInvariant()
            Contract.Invariant(pusher IsNot Nothing)
        End Sub

        Public Sub New(ByVal pusher As PushEnumerator(Of Byte))
            Contract.Requires(pusher IsNot Nothing)
            Me.pusher = pusher
        End Sub

        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return True
            End Get
        End Property

        <CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")>
        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            If closed Then Throw New InvalidOperationException("Closed streams are not writable.")
            Dim index = 0
            pusher.Push(New Enumerator(Of Byte)(Function(controller)
                                                    If index >= count Then  Return controller.Break()
                                                    index += 1
                                                    Return buffer(index + offset - 1)
                                                End Function))
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso Not closed Then
                closed = True
                pusher.PushDone()
                pusher.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Not Supported"
        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property Length As Long
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides Property Position As Long
            Get
                Throw New NotSupportedException()
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException()
            End Set
        End Property
        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function
        Public Overrides Sub SetLength(ByVal value As Long)
            Throw New NotSupportedException()
        End Sub
        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Throw New NotSupportedException()
        End Function
        Public Overrides Sub Flush()
        End Sub
#End Region
    End Class
End Namespace
