Namespace Numerics
    ''' <summary>
    ''' Represents a double which is not positive infinity, negative infinity, or NaN.
    ''' </summary>
    <DebuggerDisplay("{ToString}")>
    Public Structure FiniteDouble
        Implements IComparable(Of FiniteDouble)
        Implements IEquatable(Of FiniteDouble)

        Private ReadOnly Value As Double
        Public Sub New(ByVal value As Double)
            Contract.Requires(value.IsFinite())
            Contract.Ensures(Me.Value = value)
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
            Contract.Requires(rightOperand <> 0)
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
            Contract.Ensures(Contract.Result(Of Boolean)() = (leftOperand.Value = rightOperand.Value))
            Return leftOperand.Value = rightOperand.Value
        End Operator
        Public Shared Operator <>(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (leftOperand.Value <> rightOperand.Value))
            Return leftOperand.Value <> rightOperand.Value
        End Operator
        Public Shared Operator >(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (leftOperand.Value > rightOperand.Value))
            Return leftOperand.Value > rightOperand.Value
        End Operator
        Public Shared Operator <(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (leftOperand.Value < rightOperand.Value))
            Return leftOperand.Value < rightOperand.Value
        End Operator
        Public Shared Operator <=(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (leftOperand.Value <= rightOperand.Value))
            Return leftOperand.Value <= rightOperand.Value
        End Operator
        Public Shared Operator >=(ByVal leftOperand As FiniteDouble, ByVal rightOperand As FiniteDouble) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = (leftOperand.Value >= rightOperand.Value))
            Return leftOperand.Value >= rightOperand.Value
        End Operator
#End Region

#Region "Conversions"
        Public Shared Widening Operator CType(ByVal value As FiniteDouble) As Double
            Contract.Ensures(Contract.Result(Of Double)() = value.Value)
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
            Return Value.ToString
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
            Return Max(values.First, values.Skip(1).ToArray())
        End Function
        Public Shared Function Min(ByVal values As IEnumerable(Of FiniteDouble)) As FiniteDouble
            Contract.Requires(values IsNot Nothing)
            Contract.Requires(values.Any)
            Return Min(values.First, values.Skip(1).ToArray())
        End Function
        Public Shared Function Min(ByVal val As FiniteDouble, ByVal ParamArray values() As FiniteDouble) As FiniteDouble
            For Each e In values
                If e < val Then val = e
            Next e
            Return val
        End Function
        Public Shared Function Max(ByVal val As FiniteDouble, ByVal ParamArray values() As FiniteDouble) As FiniteDouble
            For Each e In values
                If e > val Then val = e
            Next e
            Return val
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
