using System;
using System.Threading;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public class TransactionManager
   {
      #region Fields

      private Dictionary<long, Vesseler<DateTime, Transaction>> _txIndex = new Dictionary<long, Vesseler<DateTime, Transaction>>(1000);
      private static IDebugger _debug = new Debugger<TransactionManager>(ArterySettings.Instance.Level);
      private object _syncRoot = new object();
      private const int MAX_TIMEOUT = 120;//120s
      private Timer _clearTimer;

      #endregion

      #region Methods
      public TransactionManager()
      {
         _clearTimer = new Timer(new TimerCallback(CheckTransaction),null,1000*10,1000*10);
      }

      public void Register(Transaction transaction)
      {
         long key = transaction.InitialRequest.CallID;
         lock(_syncRoot)_txIndex.Add(key,new Vesseler<DateTime, Transaction>(DateTime.UtcNow, transaction));
      }

      public bool Find(long key)
	 {
	   return _txIndex.ContainsKey(key);
	 }

	 public void UnRegister(long key)
	 {
          lock(_syncRoot)_txIndex.Remove(key);
	 }

      internal void CheckTransaction(object obj)
      {
         if (_txIndex.Count == 0) return;

         DateTime now = DateTime.UtcNow;
         Dictionary<long, Vesseler<DateTime, Transaction>> clone = null;
         lock (_syncRoot) clone = new Dictionary<long, Vesseler<DateTime, Transaction>>(_txIndex);

         foreach (KeyValuePair<long, Vesseler<DateTime, Transaction>> kv in clone)
         {
            if (kv.Value.Key.AddSeconds(MAX_TIMEOUT) > now)
               continue;
            try
            {
               kv.Value.Value.OnTransactionTimeOut();
               UnRegister(kv.Key);
            }
            catch (Exception ex)
            {
               _debug.ErrorFormat(ex, "check transaction");
            }
         }
         clone.Clear();
         clone = null;
         ArteryPerfCounter.Instance.NumberOfTranClear.Increment();
      }

	 public bool OnResponseReceived(ResponseEventArgs e)
	 {
         long key = e.PacketContext.CallID;

         Vesseler<DateTime, Transaction> tx;
	    if (!_txIndex.TryGetValue(key, out tx))return false;

         lock(_syncRoot) tx.Key = DateTime.UtcNow.AddSeconds(MAX_TIMEOUT);
	    tx.Value.OnResponseReceived(e);

	    return true;
      }

      #endregion
   }
}
