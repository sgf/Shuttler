using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Shuttler.Artery
{
   public class LockFreeQueue<T> : IEnumerable<T>
   {
      class Node
      {
         internal T Value;
         internal volatile Node Next;
      }

      private volatile Node _head;
      private volatile Node _tail;

      public LockFreeQueue()
      {
         _head = _tail = new Node();
      }

      public int Count
      {
         get
         {
            int count = 0;
            for (Node curr = _head.Next;
                curr != null; curr = curr.Next) count++;
            return count;
         }
      }

      public bool IsEmpty
      {
         get { return _head.Next == null; }
      }

      private Node GetTailAndCatchUp()
      {
         Node tail = _tail;
         Node next = tail.Next;

         // Update the tail until it really points to the end.
         while (next != null) {
            Interlocked.CompareExchange(ref _tail, next, tail);
            tail = _tail;
            next = tail.Next;
         }

         return tail;
      }

      public void Enqueue(T obj)
      {
         // Create a new node.
         Node newNode = new Node();
         newNode.Value = obj;

         // Add to the tail end.
         Node tail;
         do {
            tail = GetTailAndCatchUp();
            newNode.Next = tail.Next;
         }
         while (Interlocked.CompareExchange(
             ref tail.Next, newNode, null) != null);

         // Try to swing the tail. If it fails, we'll do it later.
         Interlocked.CompareExchange(ref _tail, newNode, tail);
      }

      public bool TryDequeue(out T val)
      {
         while (true) {
            Node head = _head;
            Node next = head.Next;

            if (next == null) {
               val = default(T);
               return false;
            }
            else {
               if (Interlocked.CompareExchange(
                   ref _head, next, head) == head) {
                  // Note: this read would be unsafe with a C++
                  // implementation. Another thread may have dequeued
                  // and freed 'next' by the time we get here, at
                  // which point we would try to dereference a bad
                  // pointer. Because we're in a GC-based system,
                  // we're OK doing this -- GC keeps it alive.
                  val = next.Value;
                  return true;
               }
            }
         }
      }

      public bool TryPeek(out T val)
      {
         Node curr = _head.Next;

         if (curr == null) {
            val = default(T);
            return false;
         }
         else {
            val = curr.Value;
            return true;
         }
      }

      public IEnumerator<T> GetEnumerator()
      {
         Node curr = _head.Next;
         Node tail = GetTailAndCatchUp();

         while (curr != null) {
            yield return curr.Value;

            if (curr == tail)
               break;

            curr = curr.Next;
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return ((IEnumerable<T>)this).GetEnumerator();
      }
   }
}
