using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.Threading;

namespace Shuttler.Artery
{
   public class CacherHeap<TKeyType> : CacheBase<CacherContext<TKeyType>, TKeyType>
   {
      #region Fields

      private const int ItemMaxSize = 1024 * 1024 * 100;//100m
      private long _maxCacheHeap =0;//bytes
      private long _cacheHeap = 0;

      #endregion

      #region Methods

      public CacherHeap(long maxHeapSize)
         : base(2)
      {
         _maxCacheHeap = maxHeapSize;
      }

      public CacherHeap(long maxHeapSize, int removeRatio)
         : base((int)maxHeapSize,removeRatio)
      {
         _maxCacheHeap = maxHeapSize;
      }

      public void Add(IItem<TKeyType> items)
      {
         CacherContext<TKeyType> context = new CacherContext<TKeyType>();
         byte[] buffer = SerializerExt.SerializerStreamToBytes<IItem<TKeyType>>(items);
         long len = buffer.Length;
         if (len > ItemMaxSize) throw new InvalidOperationException("Items size overflow 100M!!");

         if (_maxCacheHeap < (_cacheHeap + len))
            RemoveForRatio();
         Interlocked.Add(ref _cacheHeap,len);
         context.Value = buffer;
         context.Key = items.Key;
         base.Add(context);
      }

      public new IItem<TKeyType> Get(TKeyType key)
      {
         return SerializerExt.DeserializeBytesToT<IItem<TKeyType>>(base.Get(key).Value);
      }

      protected override void RemoveForRatio()
      {
         var l = (_ratio / 100) * _maxItems; var t = 0;
         do
         {
            if (Nodes.Count == 0) return;
            int len = Nodes.Last.Value.Value.Length;
            Interlocked.Add(ref _cacheHeap,-(len));
            Nodes.RemoveLast();
            Interlocked.Decrement(ref _itemTotal);
         }
         while (t++ < l);
      }

      #endregion
   }
}
