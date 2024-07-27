using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Shuttler.Artery
{
   internal class DebuggerStack<T>: IEnumerable<T>, IDisposable where T : class
   {
      #region Fields

      private readonly LockFreeStack<T> _stack = new LockFreeStack<T>();
	 private Semaphore _semaphore = new Semaphore(0, int.MaxValue);
      private long _count = 0;

      #endregion

      #region Methods

      internal void Push(T log)
	 {
         try {
            if (log == null) throw new ArgumentNullException("Enqueue log");
            _stack.Push(log);
            _semaphore.Release();
         }
         finally {
            Interlocked.Increment(ref _count);
         }
	 }

	 internal T Pop()
	 {
         try
         {
            _semaphore.WaitOne();
            return _stack.Pop();
         }
         finally
         {
            Interlocked.Decrement(ref _count);
         }
	 }

	 public bool TryPop(int timeout, out T result)
	 {
	    if (!_semaphore.WaitOne(timeout))
	    {
		  result = null;
		  return false;
	    }
         result = _stack.Pop();
	    return true;
	 }

	 public long Count
	 {
         get { return _stack.Count; }
	 }

	 void IDisposable.Dispose()
	 {
	    if (_semaphore != null)
	    {
		  _semaphore.Close();
		  _semaphore = null;
	    }
	 }


	 public IEnumerator<T> GetEnumerator()
	 {
         while (Count>0) yield return Pop();
	 }

	 IEnumerator IEnumerable.GetEnumerator()
	 {
	    return ((IEnumerable<T>)this).GetEnumerator();
      }
      #endregion
   }
}
