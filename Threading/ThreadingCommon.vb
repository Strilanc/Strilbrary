Imports System.Threading

Namespace Threading
    Public Module Common
        Public Function ThreadedAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim action_ = action 'avoid contract verification issue on hoisted arguments
            Dim f As New Future
            Call New Thread(Sub() RunWithDebugTrap(Sub()
                                                       Call action_()
                                                       Call f.SetReady()
                                                   End Sub, "Exception rose past ThreadedAction.")).Start()
            Return f
        End Function
        Public Function ThreadedFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim func_ = func 'avoid contract verification issue on hoisted arguments
            Dim f As New Future(Of TReturn)
            ThreadedAction(Sub() f.SetValue(func_()))
            Return f
        End Function

        Public Function ThreadPooledAction(ByVal action As Action) As IFuture
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Dim action_ = action 'avoid contract verification issue on hoisted arguments
            Dim f As New Future
            ThreadPool.QueueUserWorkItem(Sub() RunWithDebugTrap(Sub()
                                                                    Call action_()
                                                                    Call f.SetReady()
                                                                End Sub, "Exception rose past ThreadPooledAction."))
            Return f
        End Function
        Public Function ThreadPooledFunc(Of TReturn)(ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim func_ = func 'avoid contract verification issue on hoisted arguments
            Dim f As New Future(Of TReturn)
            ThreadPooledAction(Sub() f.SetValue(func_()))
            Return f
        End Function
    End Module
End Namespace
