Imports Strilbrary.Collections

Namespace Streams
    '''<summary>A readable view of a sequence of bytes.</summary>
    <ContractClass(GetType(IReadableStream.ContractClass))>
    Public Interface IReadableStream
        Inherits IDisposable
        Function Read(ByVal maxCount As Integer) As IReadableList(Of Byte)

        <ContractClassFor(GetType(IReadableStream))>
        NotInheritable Class ContractClass
            Implements IReadableStream
            Public Function Read(ByVal maxCount As Integer) As Collections.IReadableList(Of Byte) Implements IReadableStream.Read
                Contract.Requires(maxCount > 0)
                Contract.Ensures(Contract.Result(Of IReadableList(Of Byte))() IsNot Nothing)
                Contract.Ensures(Contract.Result(Of IReadableList(Of Byte))().Count <= maxCount)
                Throw New NotSupportedException
            End Function
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A writable view of a sequence of bytes.</summary>
    <ContractClass(GetType(IWritableStream.ContractClass))>
    Public Interface IWritableStream
        Inherits IDisposable
        Sub Write(ByVal data As IReadableList(Of Byte))
        Sub Flush()

        <ContractClassFor(GetType(IWritableStream))>
        NotInheritable Class ContractClass
            Implements IWritableStream
            Public Sub Write(ByVal data As IReadableList(Of Byte)) Implements IWritableStream.Write
                Contract.Requires(data IsNot Nothing)
                Throw New NotSupportedException
            End Sub
            Public Sub Flush() Implements IWritableStream.Flush
                Throw New NotSupportedException
            End Sub
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A seekable view of a sequence of bytes.</summary>
    <ContractClass(GetType(ISeekableStream.ContractClass))>
    Public Interface ISeekableStream
        Inherits IDisposable
        ReadOnly Property Length As Long
        Property Position As Long

        <ContractClassFor(GetType(ISeekableStream))>
        NotInheritable Class ContractClass
            Implements ISeekableStream
            Public ReadOnly Property Length As Long Implements ISeekableStream.Length
                Get
                    Contract.Ensures(Contract.Result(Of Long)() >= 0)
                    Throw New NotSupportedException
                End Get
            End Property
            Public Property Position As Long Implements ISeekableStream.Position
                Get
                    Contract.Ensures(Contract.Result(Of Long)() >= 0)
                    Contract.Ensures(Contract.Result(Of Long)() <= Length)
                    Throw New NotSupportedException
                End Get
                Set(ByVal value As Long)
                    Contract.Requires(value >= 0)
                    Contract.Requires(value <= Length)
                    Throw New NotSupportedException
                End Set
            End Property
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A readable and seekable view of a sequence of bytes.</summary>
    <ContractClass(GetType(IRandomReadableStream.ContractClass))>
    Public Interface IRandomReadableStream
        Inherits IReadableStream
        Inherits ISeekableStream

        <ContractClassFor(GetType(IRandomReadableStream))>
        NotInheritable Shadows Class ContractClass
            Implements IRandomReadableStream
            Public Function Read(ByVal maxCount As Integer) As IReadableList(Of Byte) Implements IReadableStream.Read
                Contract.Ensures(Me.Position = Contract.OldValue(Me.Position) + Contract.Result(Of IReadableList(Of Byte))().Count)
                Throw New NotSupportedException
            End Function
            Public ReadOnly Property Length As Long Implements ISeekableStream.Length
                Get
                    Throw New NotSupportedException
                End Get
            End Property
            Public Property Position As Long Implements ISeekableStream.Position
                Get
                    Throw New NotSupportedException
                End Get
                Set(ByVal value As Long)
                    Throw New NotSupportedException
                End Set
            End Property
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A writable and seekable view of a sequence of bytes.</summary>
    <ContractClass(GetType(IRandomWritableStream.ContractClass))>
    Public Interface IRandomWritableStream
        Inherits IWritableStream
        Inherits ISeekableStream

        <ContractClassFor(GetType(IRandomWritableStream))>
        NotInheritable Shadows Class ContractClass
            Implements IRandomWritableStream
            Public Sub Write(ByVal data As IReadableList(Of Byte)) Implements IWritableStream.Write
                Contract.Ensures(Me.Position = Contract.OldValue(Me.Position) + data.Count)
                Throw New NotSupportedException
            End Sub
            Public Sub Flush() Implements IWritableStream.Flush
                Throw New NotSupportedException
            End Sub
            Public ReadOnly Property Length As Long Implements ISeekableStream.Length
                Get
                    Throw New NotSupportedException
                End Get
            End Property
            Public Property Position As Long Implements ISeekableStream.Position
                Get
                    Throw New NotSupportedException
                End Get
                Set(ByVal value As Long)
                    Throw New NotSupportedException
                End Set
            End Property
            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Interface

    '''<summary>A readable, writable and seekable view of a sequence of bytes.</summary>
    Public Interface IRandomAccessStream
        Inherits IRandomReadableStream
        Inherits IRandomWritableStream
    End Interface
End Namespace
