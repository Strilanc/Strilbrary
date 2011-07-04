Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Strilbrary.Collections

<TestClass()>
Public Class PriorityQueueTest
    <TestMethod()>
    Public Sub EnumerateTest()
        Dim pq = New PriorityQueue(Of Integer)(comparer:=Function(x, y) x.CompareTo(y))
        pq.Enqueue(1)
        pq.Enqueue(2)
        pq.Enqueue(4)
        pq.Enqueue(3)
        Assert.IsTrue(pq.SequenceEqual({4, 3, 2, 1}))
    End Sub

    <TestMethod()>
    Public Sub EnqueueTest()
        Dim pq = New PriorityQueue(Of Integer)(comparer:=Function(x, y) x.CompareTo(y))
        Assert.IsTrue(pq.Count = 0)
        pq.Enqueue(1)
        Assert.IsTrue(pq.Count = 1)
        pq.Enqueue(2)
        pq.Enqueue(4)
        pq.Enqueue(3)
        Assert.IsTrue(pq.Count = 4)
    End Sub

    <TestMethod()>
    Public Sub PeekTest()
        Dim pq = New PriorityQueue(Of Integer)(comparer:=Function(x, y) x.CompareTo(y))
        pq.Enqueue(1)
        Assert.IsTrue(pq.Peek = 1)
        pq.Enqueue(2)
        Assert.IsTrue(pq.Peek = 2)
        pq.Enqueue(4)
        Assert.IsTrue(pq.Peek = 4)
        pq.Enqueue(3)
        Assert.IsTrue(pq.Peek = 4)
    End Sub

    <TestMethod()>
    Public Sub DequeueTest()
        Dim pq = New PriorityQueue(Of Integer)(comparer:=Function(x, y) x.CompareTo(y))
        pq.Enqueue(1)
        Assert.IsTrue(pq.Dequeue = 1)
        Assert.IsTrue(pq.Count = 0)
        pq.Enqueue(2)
        pq.Enqueue(1)
        Assert.IsTrue(pq.Dequeue = 2)
        Assert.IsTrue(pq.Count = 1)
        Assert.IsTrue(pq.Dequeue = 1)
        Assert.IsTrue(pq.Count = 0)
        pq.Enqueue(4)
        pq.Enqueue(1)
        pq.Enqueue(2)
        pq.Enqueue(3)
        Assert.IsTrue(pq.Dequeue = 4)
        Assert.IsTrue(pq.Count = 3)
        Assert.IsTrue(pq.Dequeue = 3)
        Assert.IsTrue(pq.Count = 2)
        Assert.IsTrue(pq.Dequeue = 2)
        Assert.IsTrue(pq.Count = 1)
        Assert.IsTrue(pq.Dequeue = 1)
        Assert.IsTrue(pq.Count = 0)
    End Sub
End Class
