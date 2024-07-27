using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Shuttler.Artery
{
   public class LockFreeStack<T> : IEnumerable<T>
   {

      #region Fields

      private Node _nodeList;

      #endregion

      #region Methods 

      public T Pop()
      {
         try {
            Node n;
            do {
               n = _nodeList;
               if (n == null) throw new ArgumentNullException("stack empty!");
            }
            while (Interlocked.CompareExchange(ref _nodeList, n.Next, n) != n);

            return n.Value;
         }
         finally {
            Interlocked.Decrement(ref _count);
         }
      }

      public void Push(T value)
      {
         try {
            Node n = new Node();
            n.Value = value;

            Node o;
            do {
               o = _nodeList;
               n.Next = o;
            }
            while (Interlocked.CompareExchange(ref _nodeList, n, o) != o);
         }
         finally {
            Interlocked.Increment(ref _count);
         }
      }

      public IEnumerator<T> GetEnumerator()
      {
         while (_nodeList != null) {
            yield return _nodeList.Value;
            _nodeList = _nodeList.Next;
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         Interlocked.Exchange(ref _count, 0);
         return ((IEnumerable<T>)this).GetEnumerator();
      }

      #endregion

      #region Attribute

      private long _count;
      public long Count
      {
         get { return Interlocked.Read(ref _count); }
         set { _count = value; }
      }

      #endregion

      private class Node
      {
         internal T Value;
         internal Node Next;
      }
   }
}
