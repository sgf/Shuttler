using System;
using System.Threading;

namespace Shuttler.Artery
{

  public delegate T Func<T>();

  public class LazyAsyncResultL<T> : IAsyncResult
  {
     private volatile int _isCompleted; 
     private readonly ManualResetEvent _asyncWaitHandle;
     private readonly AsyncCallback _callback;
     private readonly object _asyncState;

     private Exception _exception;
     private T _result;

     public LazyAsyncResultL(
         Func<T> work, AsyncCallback callback, object state)
     {
        _callback = callback;
        _asyncState = state;
        _asyncWaitHandle = new ManualResetEvent(false);
        RunWorkAsynchronously(work);
     }

     public bool IsCompleted
     {
        get { return (_isCompleted == 1); }
     }

     public bool CompletedSynchronously
     {
        get { return false; }
     }

     public WaitHandle AsyncWaitHandle
     {
        get { return _asyncWaitHandle; }
     }

     public object AsyncState
     {
        get { return _asyncState; }
     }

     // Runs the work on the thread pool, capturing exceptions,
     // results, and signaling completion.
     private void RunWorkAsynchronously(Func<T> work)
     {
        ThreadPool.QueueUserWorkItem(delegate
        {
           try
           {
              _result = work();
           }
           catch (Exception e)
           {
              _exception = e;
           }
           finally
           {
              _isCompleted = 1;
              _asyncWaitHandle.Set();
              if (_callback != null)
                 _callback(this);
           }
        });
     }

     // Helper function to end the result. Only safe to be called
     // once by one thread, ever.
     public T End()
     {
        // Wait for the work to finish, if it hasn't already.
        if (_isCompleted == 0)
        {
           _asyncWaitHandle.WaitOne();
           _asyncWaitHandle.Close();
        }

        // Propagate any exceptions or return the result.
        if (_exception != null)
           throw _exception;

        return _result;
     }
  }
}
