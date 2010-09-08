Imports Strilbrary.Exceptions
Imports Strilbrary.Collections

Namespace Values
    Public Module BitExtensions
        '''<summary>Determines if a value's specified bit is set.</summary>
        <Extension()> <Pure()>
        Public Function HasBitSet(ByVal value As Byte, ByVal bitPosition As Integer) As Boolean
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 8)
            Return ((value >> bitPosition) And CByte(1)) <> 0
        End Function
        '''<summary>Determines if a value's specified bit is set.</summary>
        <Extension()> <Pure()>
        Public Function HasBitSet(ByVal value As UInt16, ByVal bitPosition As Integer) As Boolean
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 16)
            Return ((value >> bitPosition) And 1US) <> 0
        End Function
        '''<summary>Determines if a value's specified bit is set.</summary>
        <Extension()> <Pure()>
        Public Function HasBitSet(ByVal value As UInt32, ByVal bitPosition As Integer) As Boolean
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 32)
            Return ((value >> bitPosition) And 1UI) <> 0
        End Function
        '''<summary>Determines if a value's specified bit is set.</summary>
        <Extension()> <Pure()>
        Public Function HasBitSet(ByVal value As UInt64, ByVal bitPosition As Integer) As Boolean
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 64)
            Return ((value >> bitPosition) And 1UL) <> 0
        End Function

        '''<summary>Determines the result of modifying one of a value's bits.</summary>
        <Extension()> <Pure()>
        Public Function WithBitSetTo(ByVal value As Byte, ByVal bitPosition As Integer, ByVal bitValue As Boolean) As Byte
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 8)
            Dim bit = CByte(1) << bitPosition
            Return If(bitValue, value Or bit, value And Not bit)
        End Function
        '''<summary>Determines the result of modifying one of a value's bits.</summary>
        <Extension()> <Pure()>
        Public Function WithBitSetTo(ByVal value As UInt16, ByVal bitPosition As Integer, ByVal bitValue As Boolean) As UInt16
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 16)
            Dim bit = 1US << bitPosition
            Return If(bitValue, value Or bit, value And Not bit)
        End Function
        '''<summary>Determines the result of modifying one of a value's bits.</summary>
        <Extension()> <Pure()>
        Public Function WithBitSetTo(ByVal value As UInt32, ByVal bitPosition As Integer, ByVal bitValue As Boolean) As UInt32
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 32)
            Dim bit = 1UI << bitPosition
            Return If(bitValue, value Or bit, value And Not bit)
        End Function
        '''<summary>Determines the result of modifying one of a value's bits.</summary>
        <Extension()> <Pure()>
        Public Function WithBitSetTo(ByVal value As UInt64, ByVal bitPosition As Integer, ByVal bitValue As Boolean) As UInt64
            Contract.Requires(bitPosition >= 0)
            Contract.Requires(bitPosition < 64)
            Dim bit = 1UL << bitPosition
            Return If(bitValue, value Or bit, value And Not bit)
        End Function

        '''<summary>Enumerates the bits in a value, in little-endian order.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function Bits(ByVal value As UInt64) As IEnumerable(Of Boolean)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))().Count = 64)
            Return From i In 64.Range Select value.HasBitSet(i)
        End Function
        '''<summary>Enumerates the bits in a value, in little-endian order.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function Bits(ByVal value As UInt32) As IEnumerable(Of Boolean)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))().Count = 32)
            Return From i In 32.Range Select value.HasBitSet(i)
        End Function
        '''<summary>Enumerates the bits in a value, in little-endian order.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function Bits(ByVal value As UInt16) As IEnumerable(Of Boolean)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))().Count = 16)
            Return From i In 16.Range Select value.HasBitSet(i)
        End Function
        '''<summary>Enumerates the bits in a value, in little-endian order.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function Bits(ByVal value As Byte) As IEnumerable(Of Boolean)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Boolean))().Count = 8)
            Return From i In 8.Range Select value.HasBitSet(i)
        End Function

        '''<summary>Determines the result of clearing all bits except the specified number of lowest bits.</summary>
        <Extension()> <Pure()>
        Public Function LowMasked(ByVal value As Byte, ByVal bitsKept As Int32) As Byte
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 8)
            If bitsKept = 0 Then Return 0
            If bitsKept = 8 Then Return value
            Return value And ((CByte(1) << bitsKept) - CByte(1))
        End Function
        '''<summary>Determines the result of clearing all bits except the specified number of lowest bits.</summary>
        <Extension()> <Pure()>
        Public Function LowMasked(ByVal value As UInt16, ByVal bitsKept As Int32) As UInt16
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 16)
            If bitsKept = 0 Then Return 0
            If bitsKept = 16 Then Return value
            Return value And ((1US << bitsKept) - 1US)
        End Function
        '''<summary>Determines the result of clearing all bits except the specified number of lowest bits.</summary>
        <Extension()> <Pure()>
        Public Function LowMasked(ByVal value As UInt32, ByVal bitsKept As Int32) As UInt32
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 32)
            If bitsKept = 0 Then Return 0
            If bitsKept = 32 Then Return value
            Return value And ((1UI << bitsKept) - 1UI)
        End Function
        '''<summary>Determines the result of clearing all bits except the specified number of lowest bits.</summary>
        <Extension()> <Pure()>
        Public Function LowMasked(ByVal value As UInt64, ByVal bitsKept As Int32) As UInt64
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 64)
            If bitsKept = 0 Then Return 0
            If bitsKept = 64 Then Return value
            Return value And ((1UL << bitsKept) - 1UL)
        End Function

        '''<summary>Determines the result of clearing all bits except the specified number of highest bits.</summary>
        <Extension()> <Pure()>
        Public Function HighMasked(ByVal value As Byte, ByVal bitsKept As Int32) As Byte
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 8)
            Return Not LowMasked(value, 8 - bitsKept)
        End Function
        '''<summary>Determines the result of clearing all bits except the specified number of highest bits.</summary>
        <Extension()> <Pure()>
        Public Function HighMasked(ByVal value As UInt16, ByVal bitsKept As Int32) As UInt16
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 16)
            Return Not LowMasked(value, 16 - bitsKept)
        End Function
        '''<summary>Determines the result of clearing all bits except the specified number of highest bits.</summary>
        <Extension()> <Pure()>
        Public Function HighMasked(ByVal value As UInt32, ByVal bitsKept As Int32) As UInt32
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 32)
            Return Not LowMasked(value, 32 - bitsKept)
        End Function
        '''<summary>Determines the result of clearing all bits except the specified number of highest bits.</summary>
        <Extension()> <Pure()>
        Public Function HighMasked(ByVal value As UInt64, ByVal bitsKept As Int32) As UInt64
            Contract.Requires(bitsKept >= 0)
            Contract.Requires(bitsKept <= 64)
            Return Not LowMasked(value, 64 - bitsKept)
        End Function
    End Module
End Namespace
