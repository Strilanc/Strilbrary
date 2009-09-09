Namespace Numerics
    ''' <summary>
    ''' Represents a double which is not positive infinity, negative infinity, or NaN.
    ''' </summary>
    <DebuggerDisplay("{ToString}")>
    Public Structure FiniteDouble
        Implements IComparable(Of FiniteDouble)
        Implements IEquatable(Of FiniteDouble)

        Public ReadOnly Value As Double
        Public Sub New(ByVal value As Double)
            Contract.Ensures(Me.Value = value)
            Contract.Assume(value.IsFinite()) '[not a Requires because the static verifier is horrible at proving things about doubles]
            Me.Value = value
        End Sub

#Region "Arithmetic"
        Public Shared Operator +(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As FiniteDouble
            Return New FiniteDouble(leftOperand.Value + rightOperand.Value)
        End Operator
        Public Shared Operator -(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As FiniteDouble
            Return New FiniteDouble(leftOperand.Value - rightOperand.Value)
        End Operator
        Public Shared Operator -(ByVal rightOperand As FiniteDouble) As FiniteDouble
            Return New FiniteDouble(-rightOperand.Value)
        End Operator
        Public Shared Operator *(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As FiniteDouble
            Return New FiniteDouble(leftOperand.Value * rightOperand.Value)
        End Operator
        Public Shared Operator /(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As FiniteDouble
            Contract.Requires(rightOperand.Value <> 0)
            Return New FiniteDouble(leftOperand.Value / rightOperand.Value)
        End Operator
        Public Shared Operator ^(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As FiniteDouble
            Contract.Requires(leftOperand <> 0 OrElse rightOperand <> 0)
            Contract.Requires(leftOperand >= 0)
            Return New FiniteDouble(leftOperand.Value ^ rightOperand.Value)
        End Operator
        Public Shared Operator ^(ByVal leftOperand As FiniteDouble, ByVal rightOperand As Integer) As FiniteDouble
            Contract.Requires(leftOperand <> 0 OrElse rightOperand <> 0)
            Return New FiniteDouble(leftOperand.Value ^ rightOperand)
        End Operator
#End Region

#Region "Comparisons"
        Public Shared Operator =(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Return leftOperand.Value = rightOperand.Value
        End Operator
        Public Shared Operator <>(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Return leftOperand.Value <> rightOperand.Value
        End Operator
        Public Shared Operator >(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Return leftOperand.Value > rightOperand.Value
        End Operator
        Public Shared Operator <(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Return leftOperand.Value < rightOperand.Value
        End Operator
        Public Shared Operator <=(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Return leftOperand.Value <= rightOperand.Value
        End Operator
        Public Shared Operator >=(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Return leftOperand.Value >= rightOperand.Value
        End Operator
#End Region

#Region "Conversions"
        Public Shared Widening Operator CType(ByVal value As FiniteDouble) As Double
            Return value.Value
        End Operator
        Public Shared Narrowing Operator CType(ByVal value As Double) As FiniteDouble
            Contract.Requires(value.IsFinite())
            Return New FiniteDouble(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Integer) As FiniteDouble
            Return New FiniteDouble(value)
        End Operator
#End Region

#Region "Interfaces and Inheritance"
        Public Overloads Function CompareTo(ByVal other As FiniteDouble) As Integer Implements IComparable(Of FiniteDouble).CompareTo
            Return Me.Value.CompareTo(other.Value)
        End Function
        Public Overloads Function Equals(ByVal other As FiniteDouble) As Boolean Implements IEquatable(Of FiniteDouble).Equals
            Return Me.Value.Equals(other.Value)
        End Function
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf obj Is FiniteDouble Then Return False
            Dim x = CType(obj, FiniteDouble)
            Return Me.Value.Equals(x.Value)
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Me.Value.GetHashCode()
        End Function
        Public Overrides Function ToString() As String
            Return Value.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function
#End Region

#Region "Convenience"
        Public ReadOnly Property Abs() As FiniteDouble
            Get
                Return New FiniteDouble(Math.Abs(Me.Value))
            End Get
        End Property
        Public Shared Function Max(ByVal values As IEnumerable(Of FiniteDouble)) As FiniteDouble
            Contract.Requires(values IsNot Nothing)
            Contract.Requires(values.Any)
            Return Max(values.First, values.SkipNonNull(1).ToArrayNonNull())
        End Function
        Public Shared Function Min(ByVal values As IEnumerable(Of FiniteDouble)) As FiniteDouble
            Contract.Requires(values IsNot Nothing)
            Contract.Requires(values.Any)
            Return Min(values.First, values.SkipNonNull(1).ToArrayNonNull())
        End Function
        Public Shared Function Min(ByVal initialValue As FiniteDouble, ByVal ParamArray values() As FiniteDouble) As FiniteDouble
            Contract.Requires(values IsNot Nothing)
            Dim value = initialValue
            For Each e In values
                If e < value Then value = e
            Next e
            Return value
        End Function
        Public Shared Function Max(ByVal initialValue As FiniteDouble, ByVal ParamArray values() As FiniteDouble) As FiniteDouble
            Contract.Requires(values IsNot Nothing)
            Dim value = initialValue
            For Each e In values
                If e > value Then value = e
            Next e
            Return value
        End Function
        Public ReadOnly Property Ceiling() As FiniteDouble
            Get
                Return New FiniteDouble(Math.Ceiling(Me.Value))
            End Get
        End Property
        Public ReadOnly Property Floor() As FiniteDouble
            Get
                Return New FiniteDouble(Math.Floor(Me.Value))
            End Get
        End Property
        Public ReadOnly Property Round() As FiniteDouble
            Get
                Return New FiniteDouble(Math.Round(Me.Value))
            End Get
        End Property
#End Region
    End Structure
End Namespace
