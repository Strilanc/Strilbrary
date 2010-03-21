Namespace Values
    '''<summary>Stores up to 64 bits.</summary>
    <DebuggerDisplay("{ToString}")>
    Public Structure BitWord64
        Implements IEquatable(Of BitWord64)

        Public Const MaxSize As Integer = 64
        Private ReadOnly _bits As UInt64
        Private ReadOnly _bitCount As Integer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_bitCount >= 0)
            Contract.Invariant(_bitCount <= MaxSize)
            Contract.Invariant(_bitCount = MaxSize OrElse _bits >> _bitCount = 0)
        End Sub

        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Sub New(ByVal bits As UInt64, ByVal bitCount As Integer)
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= MaxSize)
            Contract.Requires(bitCount = MaxSize OrElse bits >> bitCount = 0)
            Contract.Ensures(Me.Bits = bits)
            Contract.Ensures(Me.BitCount = bitCount)
            Me._bits = bits
            Me._bitCount = bitCount
        End Sub

        Public ReadOnly Property Bits As UInt64
            'verification disabled due to stupid verifier (1.2.30312.0)
            <ContractVerification(False)>
            Get
                Contract.Ensures(BitCount = MaxSize OrElse Contract.Result(Of UInt64)() >> BitCount = 0)
                Contract.Ensures(Contract.Result(Of UInt64)() = _bits)
                Return _bits
            End Get
        End Property
        Public ReadOnly Property BitCount As Integer
            Get
                Contract.Ensures(Contract.Result(Of Integer)() >= 0)
                Contract.Ensures(Contract.Result(Of Integer)() <= MaxSize)
                Contract.Ensures(Contract.Result(Of Integer)() = _bitCount)
                Return _bitCount
            End Get
        End Property

        Public ReadOnly Property LowPart(ByVal splitIndex As Integer) As BitWord64
            'verification disabled due to stupid verifier (1.2.30312.0)
            <ContractVerification(False)>
            Get
                Contract.Requires(splitIndex >= 0)
                Contract.Requires(splitIndex <= MaxSize)
                Contract.Requires(splitIndex <= Me.BitCount)
                Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = splitIndex)
                If splitIndex = Me.BitCount Then Return Me
                Return New BitWord64(_bits And (1UL << splitIndex) - 1UL, splitIndex)
            End Get
        End Property
        Public ReadOnly Property HighPart(ByVal splitIndex As Integer) As BitWord64
            'verification disabled due to stupid verifier (1.2.30312.0)
            <ContractVerification(False)>
            Get
                Contract.Requires(splitIndex >= 0)
                Contract.Requires(splitIndex <= MaxSize)
                Contract.Requires(splitIndex <= Me.BitCount)
                Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = Me.BitCount - splitIndex)
                If splitIndex = Me.BitCount Then Return New BitWord64(0, 0)
                Return New BitWord64(_bits >> splitIndex, Me.BitCount - splitIndex)
            End Get
        End Property

        'verification disabled due to stupid verifier (1.2.30312.0)
        <ContractVerification(False)>
        Public Shared Operator +(ByVal word1 As BitWord64, ByVal word2 As BitWord64) As BitWord64
            Contract.Requires(word1.BitCount + word2.BitCount <= MaxSize)
            Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = word1.BitCount + word2.BitCount)
            Return New BitWord64(word1.Bits Or (word2.Bits << word1.BitCount), word1.BitCount + word2.BitCount)
        End Operator

        Public Shared Operator =(ByVal word1 As BitWord64, ByVal word2 As BitWord64) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (word1.BitCount = word2.BitCount AndAlso word1.Bits = word2.Bits))
            Return word1.BitCount = word2.BitCount AndAlso word1.Bits = word2.Bits
        End Operator
        Public Shared Operator <>(ByVal word1 As BitWord64, ByVal word2 As BitWord64) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = Not word1 = word2)
            Return Not word1 = word2
        End Operator
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is BitWord64 Then Return False
            Return Me = CType(obj, BitWord64)
        End Function
        Public Overloads Function Equals(ByVal other As BitWord64) As Boolean Implements System.IEquatable(Of BitWord64).Equals
            Return Me = other
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Me.Bits.GetHashCode
        End Function

        Public Overrides Function ToString() As String
            Return Bits.ToBinary(minLength:=BitCount)
        End Function
    End Structure
End Namespace