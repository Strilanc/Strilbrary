Imports System.Threading

Namespace Threading
    Public Module ThreadingCommon
        Public Function ThreadedAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim f As New Future
            Call New Thread(Sub() RunWithDebugTrap(Sub()
                                                       Contract.Assume(action IsNot Nothing)
                                                       Contract.Assume(f IsNot Nothing)
                                                       Call action()
                                                       Call f.SetReady()
                                                   End Sub, "Exception rose past ThreadedAction.")).Start()
            Return f
        End Function
        Public Function ThreadedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim f As New Future(Of TReturn)
            ThreadedAction(Sub()
                               Contract.Assume(func IsNot Nothing)
                               Contract.Assume(f IsNot Nothing)
                               f.SetValue(func())
                           End Sub)
            Return f
        End Function

        Public Function ThreadPooledAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim f As New Future
            ThreadPool.QueueUserWorkItem(Sub() RunWithDebugTrap(Sub()
                                                                    Contract.Assume(action IsNot Nothing)
                                                                    Contract.Assume(f IsNot Nothing)
                                                                    Call action()
                                                                    Call f.SetReady()
                                                                End Sub, "Exception rose past ThreadPooledAction."))
            Return f
        End Function
        Public Function ThreadPooledFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim f As New Future(Of TReturn)
            ThreadPooledAction(Sub()
                                   Contract.Assume(func IsNot Nothing)
                                   Contract.Assume(f IsNot Nothing)
                                   f.SetValue(func())
                               End Sub)
            Return f
        End Function
    End Module
End Namespace
