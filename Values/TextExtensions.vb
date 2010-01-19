Imports Strilbrary.Exceptions

Namespace Values
    Public Module TextExtensions
        ''' <summary>
        ''' Determines a string created by extending the given string up to the given minimum length using the given padding character.
        ''' </summary>
        <Pure()> <Extension()>
        Public Function Padded(ByVal text As String,
                               ByVal minimumLength As Integer,
                               Optional ByVal paddingCharacter As Char = " "c) As String
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
        Public Function Indent(ByVal paragraph As String,
                               Optional ByVal prefix As String = vbTab) As String
            Contract.Requires(paragraph IsNot Nothing)
            Contract.Requires(prefix IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return prefix + paragraph.Replace(Environment.NewLine, Environment.NewLine + prefix)
        End Function

        ''' <summary>
        ''' Determines a string created by replacing format items in the string with a representation of the corresponding arguments.
        ''' </summary>
        <Pure()> <Extension()>
        Public Function Frmt(ByVal format As String, ByVal ParamArray args() As Object) As String
            Contract.Requires(format IsNot Nothing)
            Contract.Requires(args IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Return String.Format(Globalization.CultureInfo.InvariantCulture, format, args)
        End Function

        ''' <summary>
        ''' Concatenates a separator string between the representations of elements in a specified sequence, yielding a single concatenated string.
        ''' </summary>
        <Extension()> <Pure()>
        Public Function StringJoin(Of T)(ByVal this As IEnumerable(Of T), ByVal separator As String) As String
            Contract.Requires(this IsNot Nothing)
            Contract.Requires(separator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Dim words = From arg In this Select String.Concat(arg)
            Return String.Join(separator, words.ToArray)
        End Function

#Region "String <-> Number"
        <Extension()> <Pure()>
        Public Function ToAscBytes(ByVal data As String,
                                   Optional ByVal nullTerminate As Boolean = False) As Byte()
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())().Length = data.Length + If(nullTerminate, 1, 0))
            Dim result(0 To data.Length + If(nullTerminate, 1, 0) - 1) As Byte
            For i = 0 To data.Length - 1
                result(i) = CByte(Asc(data(i)))
            Next i
            Return result
        End Function
        <Extension()> <Pure()>
        Public Function ParseChrString(ByVal data As IEnumerable(Of Byte),
                                       ByVal nullTerminated As Boolean) As String
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)().Length <= data.Count)
            Contract.Ensures(nullTerminated OrElse Contract.Result(Of String)().Length = data.Count)

            Dim textData = If(nullTerminated, data.TakeWhile(Function(b) b <> 0), data)
            Dim result = (From b In textData Select Chr(b)).ToArray
            Contract.Assume(result.Length <= data.Count)
            Contract.Assume(nullTerminated OrElse result.Length = data.Count)
            Return result
        End Function

        <Extension()> <Pure()>
        Public Function ToHexString(ByVal data As IEnumerable(Of Byte),
                                    Optional ByVal minWordLength As Byte = 2,
                                    Optional ByVal separator As String = " ") As String
            Contract.Requires(data IsNot Nothing)
            Contract.Requires(separator IsNot Nothing)
            Contract.Ensures(Contract.Result(Of String)() IsNot Nothing)

            Dim result = New System.Text.StringBuilder()
            For Each b In data
                If result.Length > 0 Then result.Append(separator)
                Dim h = Hex(b)
                Contract.Assume(h IsNot Nothing)
                For i = 1 To minWordLength - h.Length
                    result.Append("0"c)
                Next i
                result.Append(h)
            Next b
            Return result.ToString()
        End Function
        <Extension()> <Pure()>
        Public Function FromHexStringToBytes(ByVal data As String) As Byte()
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Byte())() IsNot Nothing)

            If data Like "*[!0-9A-Fa-f ]*" Then Throw New ArgumentException("Invalid characters.")
            If data Like "*[! ][! ][! ]*" Then Throw New ArgumentException("Contains a hex value which won't fit in a byte.")
            Dim words = data.Split(" "c)
            Dim bb(0 To words.Length - 1) As Byte
            For i = 0 To words.Length - 1
                Contract.Assume(words(i) IsNot Nothing)
                bb(i) = CByte(words(i).FromHexToUInt64(ByteOrder.BigEndian))
            Next i
            Return bb
        End Function

        <Extension()> <Pure()>
        Public Function ToBinary(ByVal value As ULong,
                                 Optional ByVal minLength As Integer = 8) As String
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
        Public Function FromHexToUInt64(ByVal chars As IEnumerable(Of Char),
                                        ByVal byteOrder As ByteOrder) As ULong
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
                    Throw New ArgumentException("Invalid hex character: {0}.".frmt(c), "chars")
                End If
                val <<= 4
                val += HexDictionary(c)
            Next c
            Return val
        End Function
#End Region
    End Module
End Namespace
