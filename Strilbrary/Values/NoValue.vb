Namespace Values
    '''<summary>A type used to represent lack of a value in cases where a value would normally be required.</summary>
    <DebuggerDisplay("{ToString()}")>
    Public Structure NoValue
        Implements IEquatable(Of NoValue)
        Public Overloads Function Equals(other As NoValue) As Boolean Implements IEquatable(Of NoValue).Equals
            Return True
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is NoValue
        End Function
        Public Shared Operator =(value1 As NoValue, value2 As NoValue) As Boolean
            Return value1.Equals(value2)
        End Operator
        Public Shared Operator <>(value1 As NoValue, value2 As NoValue) As Boolean
            Return Not value1.Equals(value2)
        End Operator
        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function
        Public Overrides Function ToString() As String
            Return "Void"
        End Function
    End Structure
End Namespace
