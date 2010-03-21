Imports Strilbrary.Collections

Namespace Values
    Public Module EnumExtensions
        '''<summary>Parses a string as an Enum type. Throws an ArgumentException if the string does match a defined value.</summary>
        <Pure()> <Extension()>
        Public Function EnumParse(Of TEnum As Structure)(ByVal value As String, ByVal ignoreCase As Boolean) As TEnum
            Dim result = value.EnumTryParse(Of TEnum)(ignoreCase)
            If Not result.HasValue Then Throw New ArgumentException("""{0}"" can't be parsed to an enum of type {1}.".Frmt(value, GetType(TEnum).ToString))
            Return result.Value
        End Function
        '''<summary>Parses a string as an Enum type. Returns nothing if the string does match a defined value.</summary>
        <Pure()> <Extension()>
        Public Function EnumTryParse(Of TEnum As Structure)(ByVal value As String, ByVal ignoreCase As Boolean) As TEnum?
            Dim result As TEnum
            If Not [Enum].TryParse(Of TEnum)(value, ignoreCase, result) Then Return Nothing
            Return result
        End Function

        '''<summary>Enumerates all the defined values for an Enum type.</summary>
        <Pure()>
        Public Function EnumValues(Of TEnum)() As IEnumerable(Of TEnum)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TEnum))() IsNot Nothing)
            Return DirectCast([Enum].GetValues(GetType(TEnum)), IEnumerable(Of TEnum))
        End Function

        '''<summary>Determines the binary flags included in the enum value (including flags which may not be defined), including the power of each flag.</summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId:="Flags")>
        <Extension()> <Pure()>
        Public Function EnumFlagsIndexed(Of TEnum)(ByVal value As TEnum) As IEnumerable(Of Tuple(Of TEnum, Int32))
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of TEnum, Int32)))() IsNot Nothing)
            Select Case [Enum].GetUnderlyingType(GetType(TEnum))
                Case GetType(SByte) : Return value.EnumFlagsHelper((From i In 8.Range Select CSByte(1) << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(Int16) : Return value.EnumFlagsHelper((From i In 16.Range Select 1S << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(Int32) : Return value.EnumFlagsHelper((From i In 32.Range Select 1 << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(Int64) : Return value.EnumFlagsHelper((From i In 64.Range Select 1L << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(Byte) : Return value.EnumFlagsHelper((From i In 8.Range Select CByte(1) << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(UInt16) : Return value.EnumFlagsHelper((From i In 16.Range Select 1US << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(UInt32) : Return value.EnumFlagsHelper((From i In 32.Range Select 1UI << i), Function(e1, e2) (e1 And e2) <> 0)
                Case GetType(UInt64) : Return value.EnumFlagsHelper((From i In 64.Range Select 1UL << i), Function(e1, e2) (e1 And e2) <> 0)
                Case Else
                    Throw New InvalidOperationException("{0} does not have a recognized underlying enum type.".Frmt(GetType(TEnum)))
            End Select
        End Function
        <Extension()> <Pure()>
        Private Function EnumFlagsHelper(Of TEnum, TUnderlying)(ByVal value As TEnum,
                                                                ByVal allFlags As IEnumerable(Of TUnderlying),
                                                                ByVal hasFlag As Func(Of TUnderlying, TUnderlying, Boolean)) As IEnumerable(Of Tuple(Of TEnum, Int32))
            Contract.Requires(allFlags IsNot Nothing)
            Contract.Requires(hasFlag IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Tuple(Of TEnum, Int32)))() IsNot Nothing)
            Dim val = value.DynamicDirectCastTo(Of TUnderlying)()
            Return From pair In allFlags.ZipWithIndexes
                   Where hasFlag(val, pair.Item1)
                   Select Tuple.Create(pair.Item1.DynamicDirectCastTo(Of TEnum)(), pair.Item2)
        End Function
        '''<summary>Determines the binary flags included in the enum value (including flags which may not be defined).</summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId:="Flags")>
        <Extension()> <Pure()>
        Public Function EnumFlags(Of TEnum)(ByVal value As TEnum) As IEnumerable(Of TEnum)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of TEnum))() IsNot Nothing)
            Return From pair In value.EnumFlagsIndexed() Select pair.Item1
        End Function

        '''<summary>Determines if an Enum value is defined or not.</summary>
        <Pure()> <Extension()>
        Public Function EnumValueIsDefined(Of TEnum)(ByVal value As TEnum) As Boolean
            Return [Enum].IsDefined(GetType(TEnum), value)
        End Function
        '''<summary>Determines if all the binary flags included in the Enum value are defined.</summary>
        <Pure()> <Extension()>
        <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId:="Flags")>
        Public Function EnumFlagsAreDefined(Of TEnum)(ByVal value As TEnum) As Boolean
            Return value.EnumFlags().All(Function(flag) flag.EnumValueIsDefined())
        End Function

        '''<summary>Returns a string representation of the binary flags included in the enum value.</summary>
        <Pure()> <Extension()>
        <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId:="Flags")>
        Public Function EnumFlagsToString(Of TEnum)(ByVal value As TEnum) As String
            Dim indexedFlags = value.EnumFlagsIndexed()
            If indexedFlags.None Then Return value.ToString
            Return (From pair In indexedFlags
                    Let flag = pair.Item1
                    Let power = pair.Item2
                    Select If(flag.EnumValueIsDefined, flag.ToString, "1<<{0}".Frmt(power))
                    ).StringJoin(", ")
        End Function
    End Module
End Namespace
