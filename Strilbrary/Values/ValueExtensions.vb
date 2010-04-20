Imports Strilbrary.Collections

Namespace Values
    Public Module ValueExtensions
        '''<summary>Casts a value's type to a type, using information only available at run-time.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function DynamicDirectCastTo(Of TInput, TResult)(ByVal value As TInput) As TResult
            Contract.Ensures((Contract.Result(Of TResult)() IsNot Nothing) = (value IsNot Nothing))
            Return DirectCast(DirectCast(value, Object), TResult)
        End Function
        '''<summary>Converts a value to a type, using information only available at run-time.</summary>
        <Extension()> <Pure()>
        <ContractVerification(False)>
        Public Function DynamicCastTo(Of TInput, TResult)(ByVal value As TInput) As TResult
            Contract.Ensures((Contract.Result(Of TResult)() IsNot Nothing) = (value IsNot Nothing))
            Return CType(DirectCast(value, Object), TResult)
        End Function

        '''<summary>Wraps a value in a NonNull struct.</summary>
        <Pure()> <Extension()>
        Public Function AsNonNull(Of T)(ByVal value As T) As NonNull(Of T)
            Contract.Requires(value IsNot Nothing)
            Return New NonNull(Of T)(value)
        End Function

        '''<summary>Combines a key and value into a KeyValuePair.</summary>
        <Pure()> <Extension()>
        Public Function KeyValue(Of TKey, TValue)(ByVal key As TKey, ByVal value As TValue) As KeyValuePair(Of TKey, TValue)
            Contract.Requires(key IsNot Nothing)
            Return New KeyValuePair(Of TKey, TValue)(key, value)
        End Function

        '''<summary>Returns a type's default value.</summary>
        <Pure()>
        Public Function [Default](Of T)() As T
            Return Nothing
        End Function

        <Pure()> <Extension()>
        Public Function AsString(ByVal chars As IEnumerable(Of Char)) As String
            Contract.Requires(chars IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return New String(chars.ToArray)
        End Function
    End Module
End Namespace
