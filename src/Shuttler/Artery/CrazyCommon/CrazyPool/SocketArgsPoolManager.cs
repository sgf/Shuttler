using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime;
using System.Net.Sockets;

namespace Shuttler.Artery
{
   internal  class SocketArgsPoolManager:IDisposable
   {
	 #region Fields

	 private object _syncRoot = new object();
	 private int _capacity = 1000;

	 #endregion

	 #region Constructor Method

	 public   SocketArgsPoolManager()
	 {
         _capacity = ArterySettings.Instance.ArgsCapacity;
	 }

	 public void Initialize()
	 {
	    GCLatencyMode mode = GCSettings.LatencyMode;
	    GCSettings.LatencyMode = GCLatencyMode.Batch;

	    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
	    GC.WaitForPendingFinalizers();
	    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);


	    ArgsPool = new Stack<SocketAsyncEventArgs>();
	    Overlapped[] overlapped=new Overlapped[_capacity]; 

	    for (int i = 0; i < _capacity; i++)
	    {
		  overlapped[i] = new Overlapped();
		  CheckIn(new SocketAsyncEventArgs());
	    }
	    GC.KeepAlive(overlapped);
	    overlapped = null;

	    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
	    GC.WaitForPendingFinalizers();
	    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

	    GCSettings.LatencyMode = mode;
	 }

	 #endregion

	 #region Methods

	 internal void CheckIn(SocketAsyncEventArgs item)
	 {
	    lock (_syncRoot) ArgsPool.Push(item);
	 }

	 internal SocketAsyncEventArgs CheckOut()
	 {
	    lock (_syncRoot)
		  return ArgsPool.Count > 0 ? ArgsPool.Pop() : (new SocketAsyncEventArgs());
	 }

	 #endregion

	 #region IDisposable Member

	 ~SocketArgsPoolManager()
	 {
	    Dispose(false); 
	 }

	 private bool _disposed = false;

	 public void Dispose()
	 {
	    Dispose(true);
	    GC.SuppressFinalize(this);
	 }

	 private void Dispose(bool disposing)
	 {
	    if (!_disposed)
	    {
		  if (disposing)
		  {
			foreach (SocketAsyncEventArgs args in ArgsPool)
			   args.Dispose();
			ArgsPool.Clear();
		  }
		  _disposed = true;
	    }
	 }

	 #endregion

      #region Attribute

      private Stack<SocketAsyncEventArgs> ArgsPool
      { get; set; }


      #endregion
   }

}
