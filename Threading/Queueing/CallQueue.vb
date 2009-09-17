Namespace Threading
    '''<summary>Describes a thread-safe call queue for non-blocking calls.</summary>
    <ContractClass(GetType(ContractClassForICallQueue))>
    Public Interface ICallQueue
        '''<summary>Queues an action to be run and returns a future for the action's eventual completion.</summary>
        Function QueueAction(ByVal action As Action) As IFuture
    End Interface
    <ContractClassFor(GetType(ICallQueue))>
    Public NotInheritable Class ContractClassForICallQueue
        Implements ICallQueue
        Public Function QueueAction(ByVal action As Action) As IFuture Implements ICallQueue.QueueAction
            Contract.Requires(action IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture)() IsNot Nothing)
            Return Nothing
        End Function
    End Class

    Public Module ExtensionsForICallQueue
        '''<summary>Queues a function to be run and returns a future for the function's eventual output.</summary>
        <Extension()>
        Public Function QueueFunc(Of TReturn)(ByVal queue As ICallQueue,
                                              ByVal func As Func(Of TReturn)) As IFuture(Of TReturn)
            Contract.Requires(queue IsNot Nothing)
            Contract.Requires(func IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IFuture(Of TReturn))() IsNot Nothing)
            Dim f As New Future(Of TReturn)
            queue.QueueAction(Sub()
                                  Contract.Assume(f IsNot Nothing)
                                  Contract.Assume(func IsNot Nothing)
                                  f.SetValue(func())
                              End Sub)
            Return f
        End Function
    End Module

    ''' <summary>
    ''' A multiple-producer, single-consumer queue for running actions in order.
    ''' Logs unexpected exceptions from queued calls.
    ''' </summary>
    ''' <remarks>
    ''' The consumer is generated by the producers when items to consume exist and no consumer exists.
    ''' The queue should never end up non-empty and potentially permanently non-consumed.
    ''' </remarks>
    Public MustInherit Class AbstractLockFreeCallQueue
        Inherits AbstractLockFreeConsumer(Of Node)
        Implements ICallQueue
        Public NotInheritable Class Node
            Public ReadOnly _action As Action
            Public ReadOnly _future As New Future

            Public ReadOnly Property Action As Action
                Get
                    Contract.Ensures(Contract.Result(Of Action)() IsNot Nothing)
                    Return _action
                End Get
            End Property
            Public ReadOnly Property Future As Future
                Get
                    Contract.Ensures(Contract.Result(Of Future)() IsNot Nothing)
                    Return _future
                End Get
            End Property
            <ContractInvariantMethod()> Private Sub ObjectInvariant()
                Contract.Invariant(_action IsNot Nothing)
                Contract.Invariant(_future IsNot Nothing)
            End Sub

            Public Sub New(ByVal action As Action)
                Contract.Requires(action IsNot Nothing)
                Me._action = action
            End Sub
        End Class

        ''' <summary>
        ''' Queues an action to be run and returns a future for the action's eventual completion.
        ''' Starts running calls from the if they were not already being run.
        '''</summary>
        Public Function QueueAction(ByVal action As Action) As IFuture Implements ICallQueue.QueueAction
            Dim item = New Node(action)
            EnqueueConsume(item)
            Return item.Future
        End Function

        '''<summary>Runs queued calls until there are none left.</summary>
        Protected Overrides Sub Consume(ByVal item As Node)
            RunWithUnexpectedExceptionTrap(Sub()
                                               Contract.Assume(item IsNot Nothing)
                                               Call item.Action()()
                                               Call item.Future.SetReady()
                                           End Sub, "Exception rose past {0}.Run()".Frmt(Me.GetType.Name))
        End Sub
    End Class

    '''<summary>Runs queued calls on a control's thread.</summary>
    Public NotInheritable Class InvokedCallQueue
        Inherits AbstractLockFreeCallQueue
        Private ReadOnly control As Control

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(control IsNot Nothing)
        End Sub

        Public Sub New(ByVal control As Control)
            Contract.Requires(control IsNot Nothing)
            Me.control = control
        End Sub
        Protected Overrides Sub StartRunning()
            Try
                control.BeginInvoke(Sub() Run())
            Catch e As InvalidOperationException
                LogUnexpectedException("Invalid Invoke from {0}.StartRunning() ({1}, {2})".Frmt(Me.GetType.Name, control.GetType.Name, control.Name), e)
            End Try
        End Sub
    End Class

    '''<summary>Runs queued calls on an independent thread.</summary>
    Public NotInheritable Class ThreadedCallQueue
        Inherits AbstractLockFreeCallQueue
        Protected Overrides Sub StartRunning()
            ThreadedAction(Sub() Run())
        End Sub
    End Class

    '''<summary>Runs queued calls on the thread pool.</summary>
    Public NotInheritable Class ThreadPooledCallQueue
        Inherits AbstractLockFreeCallQueue
        Protected Overrides Sub StartRunning()
            ThreadPooledAction(Sub() Run())
        End Sub
    End Class
End Namespace
