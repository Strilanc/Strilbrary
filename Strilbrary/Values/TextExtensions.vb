Imports Strilbrary.Exceptions

Namespace Values
    Public Module TextExtensions
        ''' <summary>
        ''' Determines a string created by extending the given string up to the given minimum length using the given padding character.
        ''' </summary>
        <Pure()> <Extension()>
        Public Function Padded(text As String,
                               minimumLength As Integer,
                               Optional paddingCharacter As Char = " "c) As String
            Contract.Requires(text IsNot Nothing)
            Contract.Requires(minimumLength >= 0)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            'Contract.Ensures(Contract.Result(Of String)().Length = Math.Max(minimumLength, text.Length))
            'Contract.Ensures(Contract.Result(Of String)().Substring(0, text.Length) = text)
            'Contract.Ensures(Contract.Result(Of String)().Substring(text.Length) = New String(paddingCharacter, Math.Max(0, minimumLength - text.Length)))
            Return text + New String(paddingCharacter, Math.Max(0, minimumLength - text.Length))
        End Function

        ''' <summary>
        ''' Determines a string created by prefixing every line of the given string with the given prefix.
        ''' </summary>
        <Pure()> <Extension()>
        Public Function Indent(paragraph As String,
                               prefix As String) As String
            Contract.Requires(paragraph IsNot Nothing)
            Contract.Requires(prefix IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return prefix + paragraph.Replace(Environment.NewLine, Environment.NewLine + prefix)
        End Function

        ''' <summary>
        ''' Determines a string created by replacing format items in the string with a representation of the corresponding arguments.
        ''' </summary>
        <Pure()> <Extension()>
        Public Function Frmt(format As String, ParamArray args() As Object) As String
            Contract.Requires(format IsNot Nothing)
            Contract.Requires(args IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return String.Format(Globalization.CultureInfo.InvariantCulture, format, args)
        End Function

        ''' <summary>
        ''' Concatenates a separator string between the representations of elements in a specified sequence, yielding a single concatenated string.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function StringJoin(Of T)(this As IEnumerable(Of T), separator As String) As String
            Contract.Requires(this IsNot Nothing)
            Contract.Requires(separator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Dim words = From arg In this Select String.Concat(arg)
            Return String.Join(separator, words)
        End Function

        <Extension()> <Pure()>
        Public Function ToInvariant(value As String) As InvariantString
            Contract.Requires(value IsNot Nothing)
            Return value
        End Function

#Region "String <-> Number"
        <Extension()> <Pure()>
        Public Function ToAsciiBytes(chars As IEnumerable(Of Char)) As IEnumerable(Of Byte)
            Contract.Requires(chars IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Byte))() IsNot Nothing)
            Return From c In chars
                   Select CByte(Microsoft.VisualBasic.Asc(c))
        End Function
        <Extension()> <Pure()>
        Public Function ToAsciiChars(data As IEnumerable(Of Byte)) As IEnumerable(Of Char)
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IEnumerable(Of Char))() IsNot Nothing)
            Return From b In data
                   Select Microsoft.VisualBasic.Chr(b)
        End Function

        <Extension()> <Pure()>
        Public Function ToHexString(data As IEnumerable(Of Byte),
                                    Optional minWordLength As Byte = 2,
                                    Optional separator As String = " ") As String
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(separator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return (From b In data
                    Select b.ToString("X{0}".Frmt(minWordLength), Globalization.CultureInfo.InvariantCulture)
                    ).StringJoin(separator)
        End Function
        <Extension()> <Pure()>
        Public Function FromHexStringToBytes(data As String) As Byte()
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)

            If data Like "*[!0-9A-Fa-f ]*" Then Throw New ArgumentException("Invalid characters.")
            If data Like "*[! ][! ][! ]*" Then Throw New ArgumentException("Contains a hex value with more than 2 digits.")
            Dim words = data.Split(" "c)
            Dim bb(0 To words.Length - 1) As Byte
            For i = 0 To words.Length - 1
                Contract.Assume(words(i) IsNot Nothing)
                bb(i) = CByte(words(i).FromHexToUInt64(ByteOrder.BigEndian))
            Next i
            Return bb
        End Function

        <Extension()> <Pure()>
        Public Function ToBinary(value As ULong,
                                 Optional minLength As Integer = 8) As String
            Contract.Requires(minLength >= 0)
            Contract.Requires(minLength <= 64)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Dim ret = ""
            While value > 0 Or minLength > 0
                ret = (value And &H1UL).ToString(Globalization.CultureInfo.InvariantCulture) + ret
                value >>= 1
                minLength -= 1
            End While
            Return ret
        End Function

        Private ReadOnly HexDictionary As New Dictionary(Of Char, Byte) From {
                    {"0"c, 0}, {"1"c, 1}, {"2"c, 2}, {"3"c, 3}, {"4"c, 4},
                    {"5"c, 5}, {"6"c, 6}, {"7"c, 7}, {"8"c, 8}, {"9"c, 9},
                    {"a"c, 10}, {"A"c, 10},
                    {"b"c, 11}, {"B"c, 11},
                    {"c"c, 12}, {"C"c, 12},
                    {"d"c, 13}, {"D"c, 13},
                    {"e"c, 14}, {"E"c, 14},
                    {"f"c, 15}, {"F"c, 15}}
        <Extension()> <Pure()>
        Public Function FromHexToUInt64(chars As IEnumerable(Of Char),
                                        byteOrder As ByteOrder) As ULong
            Contract.Requires(chars IsNot Nothing)

            Select Case byteOrder
                Case byteOrder.LittleEndian
                    chars = chars.Reverse()
                Case byteOrder.BigEndian
                    'no change needed
                Case Else
                    Throw byteOrder.MakeArgumentValueException("byteOrder")
            End Select

            Dim val = 0UL
            For Each c In chars
                If Not HexDictionary.ContainsKey(c) Then
                    Throw New ArgumentException("Invalid hex character: {0}.".Frmt(c), "chars")
                End If
                val <<= 4
                val += HexDictionary(c)
            Next c
            Return val
        End Function
#End Region
    End Module
End Namespace
