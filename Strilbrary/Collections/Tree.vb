Imports Strilbrary.Values

Namespace Collections
    <ContractClass(GetType(ITreeContractClass(Of )))>
    Public Interface ITree(Of Out T)
        ReadOnly Property Value As T
        ReadOnly Property Children As IRist(Of ITree(Of T))
    End Interface
    <ContractClassFor(GetType(ITree(Of )))>
    Public MustInherit Class ITreeContractClass(Of T)
        Implements ITree(Of T)
        Public ReadOnly Property Children As IRist(Of ITree(Of T)) Implements ITree(Of T).Children
            Get
                Contract.Ensures(Contract.Result(Of IRist(Of ITree(Of T)))() IsNot Nothing)
                Throw New NotSupportedException
            End Get
        End Property
        Public ReadOnly Property Value As T Implements ITree(Of T).Value
            Get
                Throw New NotSupportedException
            End Get
        End Property
    End Class

    <DebuggerDisplay("{ToString()}")>
    Public Class Tree(Of T)
        Implements ITree(Of T)
        Private ReadOnly _value As T
        Private ReadOnly _children As IRist(Of ITree(Of T))

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_children IsNot Nothing)
        End Sub

        Public Sub New(ByVal value As T, ByVal children As IRist(Of ITree(Of T)))
            Contract.Requires(children IsNot Nothing)
            Contract.Ensures(Me._children Is children)
            Me._value = value
            Me._children = children
        End Sub

        Public ReadOnly Property Value As T Implements ITree(Of T).Value
            Get
                Return _value
            End Get
        End Property
        Public ReadOnly Property Children As IRist(Of ITree(Of T)) Implements ITree(Of T).Children
            Get
                Contract.Ensures(Contract.Result(Of IRist(Of ITree(Of T)))() IsNot Nothing)
                Return _children
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return "Children: {0}, Value: {1}".Frmt(_children.Count, _value)
        End Function
    End Class

    Public Module TreeExtensions
        <Extension()> <Pure()>
        Public Function [Select](Of TArg, TResult)(ByVal tree As ITree(Of TArg),
                                                   ByVal projection As Func(Of TArg, TResult)) As ITree(Of TResult)
            Contract.Requires(tree IsNot Nothing)
            Contract.Requires(projection IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ITree(Of TResult))() IsNot Nothing)
            Return New Tree(Of TResult)(value:=projection(tree.Value),
                                        children:=(From child In tree.Children
                                                   Select child.Select(projection)
                                                   ).ToReadableList)
        End Function
        <Extension()> <Pure()>
        <SuppressMessage("Microsoft.Contracts", "Nonnull-111-0")>
        Public Function SelectMany(Of TArg, TMid, TResult)(ByVal tree As ITree(Of TArg),
                                                           ByVal projection1 As Func(Of TArg, ITree(Of TMid)),
                                                           ByVal projection2 As Func(Of TArg, TMid, TResult)) As ITree(Of TResult)
            Contract.Requires(tree IsNot Nothing)
            Contract.Requires(projection1 IsNot Nothing)
            Contract.Requires(projection2 IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ITree(Of TResult))() IsNot Nothing)
            Return New Tree(Of TResult)(value:=projection2(tree.Value, projection1(tree.Value).Value),
                                        children:=(From child In tree.Children
                                                   Select child.SelectMany(projection1, projection2)
                                                   ).ToReadableList)
        End Function
        <Extension()> <Pure()>
        Public Function Where(Of TValue)(ByVal tree As ITree(Of TValue),
                                         ByVal predicate As Func(Of TValue, Boolean)) As ITree(Of TValue)
            Contract.Requires(tree IsNot Nothing)
            Contract.Requires(predicate IsNot Nothing)
            If Not predicate(tree.Value) Then Return Nothing
            Return tree.WhereHelper(predicate)
        End Function
        <Extension()> <Pure()>
        Private Function WhereHelper(Of TValue)(ByVal tree As ITree(Of TValue),
                                                ByVal predicate As Func(Of TValue, Boolean)) As ITree(Of TValue)
            Contract.Requires(tree IsNot Nothing)
            Contract.Requires(predicate IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ITree(Of TValue))() IsNot Nothing)
            Return New Tree(Of TValue)(tree.Value, (From child In tree.Children
                                                    Where predicate(child.Value)
                                                    ).ToReadableList)
        End Function
    End Module
End Namespace
