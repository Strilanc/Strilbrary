Namespace Mathematics
    '''<summary>An arbitrary-precision rational number.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public Structure Rational
        Implements IEquatable(Of Rational)
        Implements IComparable(Of Rational)

        Private ReadOnly _numerator As BigInteger
        Private ReadOnly _denominator As BigInteger

        Public Shared ReadOnly Zero As Rational = 0
        Public Shared ReadOnly One As Rational = 1

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_denominator > 0)
            'Contract.Invariant(BigInteger.GreatestCommonDivisor(_numerator, _denominator) = 1) 'GCD needs to be marked pure
        End Sub

        ''' <summary>
        ''' Creates a Rational equal to numerator/denominator.
        ''' The assigned numerator/denominator pair may not be the same, because common divisors are cancelled and negatives are moved to the numerator.
        ''' </summary>
        Public Sub New(ByVal numerator As BigInteger, ByVal denominator As BigInteger)
            Contract.Requires(denominator <> 0)
            Dim gcd = BigInteger.GreatestCommonDivisor(numerator, denominator)
            If gcd.Sign <> denominator.Sign Then gcd = -gcd
            Me._numerator = numerator / gcd
            Me._denominator = denominator / gcd
            Contract.Assume(_denominator > 0)
            Contract.Assume(BigInteger.GreatestCommonDivisor(_numerator, _denominator) = 1)
        End Sub

        Public ReadOnly Property Numerator As BigInteger
            Get
                Return _numerator
            End Get
        End Property
        Public ReadOnly Property Denominator As BigInteger
            Get
                Contract.Ensures(Contract.Result(Of BigInteger)() > 0)
                Dim r = If(_denominator = 0, 1, _denominator)
                Contract.Assume(r > 0)
                Return r
            End Get
        End Property

        Public Function CompareTo(ByVal other As Rational) As Integer Implements IComparable(Of Rational).CompareTo
            Return (Me.Numerator * other.Denominator - other.Numerator * Me.Denominator).Sign
        End Function
        Public Overloads Function Equals(ByVal other As Rational) As Boolean Implements IEquatable(Of Rational).Equals
            Return Me.Numerator = other.Numerator AndAlso Me.Denominator = other.Denominator
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return TypeOf obj Is Rational AndAlso Me.Equals(DirectCast(obj, Rational))
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Me.Numerator.GetHashCode() Xor Me.Denominator.GetHashCode()
        End Function
        Public Overrides Function ToString() As String
            If Denominator = 1 Then Return Numerator.ToString()
            Return Numerator.ToString() + "/" + Denominator.ToString()
        End Function

        Public Shared Operator +(ByVal value1 As Rational, ByVal value2 As Rational) As Rational
            Contract.Assume(value1.Denominator * value2.Denominator <> 0)
            Return New Rational(value1.Numerator * value2.Denominator + value2.Numerator * value1.Denominator,
                                value1.Denominator * value2.Denominator)
        End Operator
        Public Shared Operator *(ByVal value1 As Rational, ByVal value2 As Rational) As Rational
            Contract.Assume(value1.Denominator * value2.Denominator <> 0)
            Return New Rational(value1.Numerator * value2.Numerator,
                                value1.Denominator * value2.Denominator)
        End Operator
        Public Shared Operator /(ByVal value1 As Rational, ByVal value2 As Rational) As Rational
            Contract.Requires(value2 <> 0)
            Contract.Assume(value1.Denominator * value2.Numerator <> 0)
            Return New Rational(value1.Numerator * value2.Denominator,
                                value1.Denominator * value2.Numerator)
        End Operator
        Public Shared Operator -(ByVal value As Rational) As Rational
            Contract.Assume(value.Denominator <> 0)
            Return New Rational(-value.Numerator, value.Denominator)
        End Operator
        Public Shared Operator Mod(ByVal value1 As Rational, ByVal value2 As Rational) As Rational
            Contract.Assume(value1.Denominator * value2.Denominator <> 0)
            Return New Rational((value1.Numerator * value2.Denominator) Mod (value2.Numerator * value1.Denominator),
                                value1.Denominator * value2.Denominator)
        End Operator
        Public Shared Operator -(ByVal value1 As Rational, ByVal value2 As Rational) As Rational
            Return value1 + -value2
        End Operator

        Public Shared Operator =(ByVal value1 As Rational, ByVal value2 As Rational) As Boolean
            Return value1.Equals(value2)
        End Operator
        Public Shared Operator <>(ByVal value1 As Rational, ByVal value2 As Rational) As Boolean
            Return Not value1.Equals(value2)
        End Operator
        Public Shared Operator >(ByVal value1 As Rational, ByVal value2 As Rational) As Boolean
            Return value1.CompareTo(value2) > 0
        End Operator
        Public Shared Operator >=(ByVal value1 As Rational, ByVal value2 As Rational) As Boolean
            Return value1.CompareTo(value2) >= 0
        End Operator
        Public Shared Operator <=(ByVal value1 As Rational, ByVal value2 As Rational) As Boolean
            Return value1.CompareTo(value2) <= 0
        End Operator
        Public Shared Operator <(ByVal value1 As Rational, ByVal value2 As Rational) As Boolean
            Return value1.CompareTo(value2) < 0
        End Operator

        Public Shared Widening Operator CType(ByVal value As Byte) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As SByte) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int16) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt16) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int32) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt32) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int64) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As UInt64) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
        Public Shared Widening Operator CType(ByVal value As BigInteger) As Rational
            Contract.Assume(CType(1, BigInteger) <> 0)
            Return New Rational(value, 1)
        End Operator
    End Structure
    Public Module RationalExtensions
        '''<summary>Determines the integral result of rounding the rational towards 0.</summary>
        <Pure()> <Extension()>
        Public Function Truncate(ByVal value As Rational) As BigInteger
            Return value.Numerator / value.Denominator 'BigInteger division truncates
        End Function
        '''<summary>Determines the integral result of rounding the rational towards negative infinity.</summary>
        <Pure()> <Extension()>
        Public Function Floor(ByVal value As Rational) As BigInteger
            Dim r = value.Numerator / value.Denominator
            If r > value Then r -= 1
            Return r
        End Function
        '''<summary>Determines the integral result of rounding the rational towards positive infinity.</summary>
        <Pure()> <Extension()>
        Public Function Ceiling(ByVal value As Rational) As BigInteger
            Dim r = value.Numerator / value.Denominator
            If r < value Then r += 1
            Return r
        End Function

        '''<summary>Gets a number that indicates the sign (negative, positive, or zero) of the rational.</summary>
        <Pure()> <Extension()>
        Public Function Sign(ByVal value As Rational) As Int32
            Return value.Numerator.Sign()
        End Function
        '''<summary>Returns the absolute value of the rational.</summary>
        <Pure()> <Extension()>
        Public Function Abs(ByVal value As Rational) As Rational
            Contract.Ensures(Contract.Result(Of Rational)() >= 0)
            Dim r = If(value.Sign() < 0, -value, value)
            Contract.Assume(r >= 0)
            Return r
        End Function
    End Module
End Namespace
