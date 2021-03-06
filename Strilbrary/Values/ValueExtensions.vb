Namespace Values
    Public Module ValueExtensions
        '''<summary>Casts a value's type to a type, using information only available at run-time.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "Nonnull-41-0")>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-(Contract.Result(Of TResult)() IsNot Nothing) = (value IsNot Nothing)")>
        Public Function DynamicDirectCastTo(Of TInput, TResult)(value As TInput) As TResult
            Contract.Ensures((Contract.Result(Of TResult)() IsNot Nothing) = (value IsNot Nothing))
            Return DirectCast(DirectCast(value, Object), TResult)
        End Function
        '''<summary>Converts a value to a type, using information only available at run-time.</summary>
        <Extension()> <Pure()>
        Public Function DynamicCastTo(Of TInput, TResult)(value As TInput) As TResult
            Return CType(DirectCast(value, Object), TResult)
        End Function

        '''<summary>Wraps a value in a NonNull struct.</summary>
        <Pure()> <Extension()>
        Public Function AsNonNull(Of T)(value As T) As NonNull(Of T)
            Contract.Requires(value IsNot Nothing)
            Return New NonNull(Of T)(value)
        End Function
        '''<summary>Exposes a sequence's underlying non-null values as a sequence.</summary>
        <Pure()> <Extension()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.ForAll(Contract.Result(Of IEnumerable(Of T))(), Function(e) e IsNot Nothing)")>
        Public Function Values(Of T)(sequence As IEnumerable(Of NonNull(Of T))) As IEnumerable(Of T)
            Contract.Requires(sequence IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of T))() IsNot Nothing)
            Contract.Ensures(Contract.ForAll(Contract.Result(Of IEnumerable(Of T))(), Function(e) e IsNot Nothing))
            Return From e In sequence Select e.Value
        End Function

        '''<summary>Returns a type's default value.</summary>
        <Pure()>
        Public Function [Default](Of T)() As T
            Return Nothing
        End Function

        <Pure()> <Extension()>
        Public Function AsString(chars As IEnumerable(Of Char)) As String
            Contract.Requires(chars IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return New String(chars.ToArray)
        End Function

        '''<summary>Compares two generic values, handling the null case.</summary>
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "EnsuresInMethod-Contract.Result(Of Boolean)() = If(value1 Is Nothing, value2 Is Nothing, value1.Equals(value2))")>
        Public Function NullableEquals(Of T)(value1 As T, value2 As T) As Boolean
            Contract.Ensures(Contract.Result(Of Boolean)() = If(value1 Is Nothing, value2 Is Nothing, value1.Equals(value2)))
            Return If(value1 Is Nothing, value2 Is Nothing, value1.Equals(value2))
        End Function
    End Module
End Namespace
