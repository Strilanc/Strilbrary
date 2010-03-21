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
    End Module
End Namespace
