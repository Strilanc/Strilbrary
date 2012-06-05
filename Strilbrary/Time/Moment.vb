Namespace Time
    ''' <summary>
    ''' A moment in time as measured by an <see cref="IClock"/>.
    ''' Moments from the same clock can be compared to get the elapsed time between them.
    ''' </summary>
    <DebuggerDisplay("{ToString()}")>
    Public Structure Moment
        Implements IEquatable(Of Moment)
        Implements IEquatable(Of Moment?)
        Implements IComparable(Of Moment)

        Public ReadOnly Ticks As Long
        Public ReadOnly Basis As IClock
        Public Sub New(ticks As Long, basis As IClock)
            Me.Ticks = ticks
            Me.Basis = basis
        End Sub
        Public Shared Operator +(moment As Moment, delta As TimeSpan) As Moment
            Return New Moment(moment.Ticks + delta.Ticks, moment.Basis)
        End Operator
        Public Shared Operator -(moment As Moment, delta As TimeSpan) As Moment
            Return New Moment(moment.Ticks - delta.Ticks, moment.Basis)
        End Operator
        Public Shared Operator -(moment1 As Moment, moment2 As Moment) As TimeSpan
            Contract.Requires(Object.Equals(moment1.Basis, moment2.Basis))
            Return New TimeSpan(moment1.Ticks - moment2.Ticks)
        End Operator
        Public Shared Operator >(moment1 As Moment, moment2 As Moment) As Boolean
            Contract.Requires(Object.Equals(moment1.Basis, moment2.Basis))
            Return moment1.Ticks > moment2.Ticks
        End Operator
        Public Shared Operator <(moment1 As Moment, moment2 As Moment) As Boolean
            Contract.Requires(Object.Equals(moment1.Basis, moment2.Basis))
            Return moment1.Ticks < moment2.Ticks
        End Operator
        Public Shared Operator >=(moment1 As Moment, moment2 As Moment) As Boolean
            Contract.Requires(Object.Equals(moment1.Basis, moment2.Basis))
            Return moment1.Ticks >= moment2.Ticks
        End Operator
        Public Shared Operator <=(moment1 As Moment, moment2 As Moment) As Boolean
            Contract.Requires(Object.Equals(moment1.Basis, moment2.Basis))
            Return moment1.Ticks <= moment2.Ticks
        End Operator
        Public Shared Operator =(moment1 As Moment, moment2 As Moment) As Boolean
            Return Object.Equals(moment1.Basis, moment2.Basis) AndAlso
                   moment1.Ticks <= moment2.Ticks
        End Operator
        Public Shared Operator <>(moment1 As Moment, moment2 As Moment) As Boolean
            Return Not (moment1 = moment2)
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("Moment(Ticks: {0}, Basis: {1})", Ticks, Basis)
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Ticks.GetHashCode()
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is Moment AndAlso Me = DirectCast(obj, Moment)
        End Function

        Public Overloads Function Equals(other As Moment?) As Boolean Implements IEquatable(Of Moment?).Equals
            Return other.HasValue AndAlso Me = other.Value
        End Function
        Public Overloads Function Equals(other As Moment) As Boolean Implements IEquatable(Of Moment).Equals
            Return Me = other
        End Function
        Public Function CompareTo(other As Moment) As Integer Implements IComparable(Of Moment).CompareTo
            Contract.Requires(Of InvalidOperationException)(Object.Equals(Me.Basis, other.Basis))
            Return Me.Ticks.CompareTo(other.Ticks)
        End Function
    End Structure
End Namespace
