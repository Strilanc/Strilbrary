﻿Namespace Values
    '''<summary>Stores up to 64 bits.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public Structure BitWord64
        Implements IEquatable(Of BitWord64)

        Public Const MaxSize As Integer = 64
        Private ReadOnly _bits As UInt64
        Private ReadOnly _bitCount As Integer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_bitCount >= 0)
            Contract.Invariant(_bitCount <= MaxSize)
            Contract.Invariant(_bits.HasNoBitsSetAbovePosition(_bitCount))
        End Sub

        Public Sub New(bits As UInt64, bitCount As Integer)
            Contract.Requires(bitCount >= 0)
            Contract.Requires(bitCount <= MaxSize)
            Contract.Requires(bits.HasNoBitsSetAbovePosition(bitCount))
            Contract.Ensures(Me.Bits = bits)
            Contract.Ensures(Me.BitCount = bitCount)
            Me._bits = bits
            Me._bitCount = bitCount
        End Sub

        Public ReadOnly Property Bits As UInt64
            Get
                Contract.Ensures(Contract.Result(Of UInt64)().HasNoBitsSetAbovePosition(BitCount))
                Contract.Ensures(Contract.Result(Of UInt64)() = _bits)
                Contract.Assume(_bits.HasNoBitsSetAbovePosition(BitCount))
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

        Public ReadOnly Property LowPart(splitIndex As Integer) As BitWord64
            Get
                Contract.Requires(splitIndex >= 0)
                Contract.Requires(splitIndex <= MaxSize)
                Contract.Requires(splitIndex <= Me.BitCount)
                Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = splitIndex)
                If splitIndex = Me.BitCount Then Return Me
                Dim bits = _bits And (1UL << splitIndex) - 1UL
                Contract.Assume(bits.HasNoBitsSetAbovePosition(splitIndex))
                Return New BitWord64(Bits, splitIndex)
            End Get
        End Property
        Public ReadOnly Property HighPart(splitIndex As Integer) As BitWord64
            Get
                Contract.Requires(splitIndex >= 0)
                Contract.Requires(splitIndex <= MaxSize)
                Contract.Requires(splitIndex <= Me.BitCount)
                Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = Me.BitCount - splitIndex)
                If splitIndex = Me.BitCount Then Return New BitWord64()
                Dim bits = _bits >> splitIndex
                Contract.Assume(bits.HasNoBitsSetAbovePosition(Me.BitCount - splitIndex))
                Return New BitWord64(bits, Me.BitCount - splitIndex)
            End Get
        End Property

        Public Shared Operator +(word1 As BitWord64, word2 As BitWord64) As BitWord64
            Contract.Requires(word1.BitCount + word2.BitCount <= MaxSize)
            Contract.Ensures(Contract.Result(Of BitWord64)().BitCount = word1.BitCount + word2.BitCount)
            Dim bits = word1.Bits Or (word2.Bits << word1.BitCount)
            Dim bitCount = word1.BitCount + word2.BitCount
            Contract.Assume(bits.HasNoBitsSetAbovePosition(bitCount))
            Return New BitWord64(bits, bitCount)
        End Operator

        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Boolean)() = (word1.BitCount = word2.BitCount AndAlso word1.Bits = word2.Bits)")>
        Public Shared Operator =(word1 As BitWord64, word2 As BitWord64) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (word1.BitCount = word2.BitCount AndAlso word1.Bits = word2.Bits))
            Return word1.BitCount = word2.BitCount AndAlso word1.Bits = word2.Bits
        End Operator
        Public Shared Operator <>(word1 As BitWord64, word2 As BitWord64) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = Not word1 = word2)
            Return Not word1 = word2
        End Operator
        Public Overloads Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is BitWord64 AndAlso Me = DirectCast(obj, BitWord64)
        End Function
        Public Overloads Function Equals(other As BitWord64) As Boolean Implements IEquatable(Of BitWord64).Equals
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
