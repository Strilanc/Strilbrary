Imports System.Threading

Namespace Threading.Queueing
    ''' <summary>
    ''' A multiple-producer, single-consumer lock-free queue.
    ''' Does NOT guarantee an item has been queued when BeginEnqueue finishes.
    ''' Does guarantee an item will eventually be queued after BeginEnqueue finishes.
    ''' Does guarantee that, for calls simultaneously in BeginEnqueue, at least one will finish with its item having been enqueued.
    ''' Does guarantee that, if BeginEnqueue(Y) is called after BeginEnqueue(X) finishes, Y will follow X in the queue.
    ''' </summary>
    ''' <remarks>
    ''' Performance characteristics:
    ''' - All operations are guaranteed constant time.
    ''' - Latency between BeginEnqueue finishing and the item being enqueued can be delayed arbitrarily by slowing down only one of the producers.
    ''' - (How does this compare to CAS-based implementations in terms of average throughput? It should be higher?)
    ''' </remarks>
    Public NotInheritable Class SingleConsumerLockFreeQueue(Of T)
        ''' <summary>
        ''' Owned by the consumer.
        ''' This node is the end marker of the consumed nodes.
        ''' This node's next is the next node to be consumed.
        ''' </summary>
        Private head As Node = New Node(Nothing)
        ''' <summary>
        ''' This node is the tail of the last partially or fully inserted chain.
        ''' The next inserted chain will exchange its tail for the insertionPoint, then set the old insertionPoint's next to the chain's head.
        ''' </summary>
        Private insertionPoint As Node = head
        ''' <summary>
        ''' Singly linked list node containing queue items.
        ''' </summary>
        Private Class Node
            Public ReadOnly value As T
            Public [next] As Node
            Public Sub New(ByVal value As T)
                Me.value = value
            End Sub
        End Class

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(head IsNot Nothing)
            Contract.Invariant(insertionPoint IsNot Nothing)
        End Sub

        ''' <summary>
        ''' Begins adding new items to the queue.
        ''' The items may not be dequeueable when this method finishes, but eventually they will be.
        ''' The items are guaranteed to end up adjacent in the queue and in the correct order.
        ''' </summary>
        ''' <remarks>
        ''' An example of what can occur when two items are queued simultaneously:
        ''' Inital state:
        '''   insert=head -> null
        '''   [queue is empty]
        ''' Step 1: First item is created and exchanged with insertion point.
        '''   head=prev1 -> null
        '''   insert=node1 -> null
        '''   [queue is empty]
        ''' Step 2: Second thread preempts and second item is created and exchanged with insertion point.
        '''   head=prev1 -> null
        '''   node1=prev2 -> null
        '''   insert=node2 -> null
        '''   [queue is empty]
        ''' Step 3: Second thread finishes setting prev.next.
        '''   head=prev1 -> null
        '''   node1=prev2 -> insert=node2 -> null
        '''   [queue is empty]
        ''' Step 4: First thread comes back and finishes setting prev.next.
        '''   head=prev1 -> node1=prev2 -> insert=node2 -> null
        '''   [queue contains 2 elements]
        ''' </remarks>
        ''' <implementation>
        ''' Each producer creates a new chain, and exchanges the shared tail for the tail of the new chain.
        ''' The producer then links the tail of the previous chain to the head of the new chain.
        ''' A new chain might not be in the main chain when the function exits, but it will be in a chain that will eventually be in the main chain.
        ''' </implementation>
        Public Sub BeginEnqueue(ByVal items As IEnumerable(Of T))
            If items Is Nothing Then Throw New ArgumentNullException("items")
            If Not items.Any Then Return

            'Create new chain
            Dim chainHead As Node = Nothing
            Dim chainTail As Node = Nothing
            For Each item In items
                If chainHead Is Nothing Then
                    chainHead = New Node(item)
                    chainTail = chainHead
                Else
                    chainTail.next = New Node(item)
                    chainTail = chainTail.next
                End If
            Next item

            'Append chain to previous chain
            Dim prevChainTail = Interlocked.Exchange(Me.insertionPoint, chainTail)
            Contract.Assume(Me.insertionPoint IsNot Nothing)
            Contract.Assume(prevChainTail IsNot Nothing)
            prevChainTail.next = chainHead
        End Sub
        ''' <summary>
        ''' Begins adding a new item to the queue.
        ''' The item may not be dequeueable when this method finishes, but eventually it will be.
        ''' </summary>
        ''' <implementation>
        ''' Just an inlined and simplified version of BeginEnqueue(IEnumerable(Of T))
        ''' </implementation>
        Public Sub BeginEnqueue(ByVal item As T)
            Dim chainOfOne = New Node(item)
            Dim prevChainTail = Interlocked.Exchange(Me.insertionPoint, chainOfOne)
            Contract.Assume(Me.insertionPoint IsNot Nothing)
            Contract.Assume(prevChainTail IsNot Nothing)
            prevChainTail.next = chainOfOne
        End Sub

        ''' <summary>
        ''' Returns true if there were any items in the queue.
        ''' The return value is only stable if the queue is non-empty and you are calling from the consumer thread.
        ''' </summary>
        Public ReadOnly Property WasEmpty As Boolean
            Get
                Return head.next Is Nothing
            End Get
        End Property

        ''' <summary>
        ''' Returns the next item in the queue.
        ''' This function must only be called from the consumer thread.
        ''' </summary>
        Public Function Peek() As T
            If head.next Is Nothing Then Throw New InvalidOperationException("Empty Queue")
            Return head.next.value
        End Function
        ''' <summary>
        ''' Removes and returns an item from the queue.
        ''' This function must only be called from the consumer thread.
        ''' </summary>
        Public Function Dequeue() As T
            If head.next Is Nothing Then Throw New InvalidOperationException("Empty Queue")
            head = head.next
            Return head.value
        End Function
    End Class
End Namespace