using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Shuttler.Artery
{
   public abstract class CacheBase<TValue, KeyType> where TValue : IItem<KeyType>
   {
      #region Fields

      protected static object _syncRoot = new object();
      protected int _itemTotal = 0;
      protected int _maxItems = 0;
      protected int _ratio = 0;

      #endregion

      #region Constructors

      protected CacheBase(int maxItems, int removeRatio)
      {
         _maxItems = maxItems;
         _ratio = removeRatio;

         Dict = new Dictionary<KeyType, WeakReference>(500 << 10);
         Nodes = new LinkedList<TValue>();
      }

      protected CacheBase(int maxItems)
         : this(maxItems, 2)
      {
      }

      #endregion

      #region Methods

      /*add*/
      protected virtual void Add(TValue value)
      {
         if (Dict.ContainsKey(value.Key)) return;

         if (_itemTotal > _maxItems) RemoveForRatio();
         lock (_syncRoot) Nodes.AddFirst(value);

         var weak = new WeakReference(value, false);
         Dict[value.Key] = weak;

         Interlocked.Increment(ref _itemTotal);
      }

      /*get*/
      protected virtual TValue Get(KeyType key)
      {
         var value = default(TValue);
         WeakReference weak;
         if (!Dict.TryGetValue(key, out weak) || weak.Target == null)
            return default(TValue);

         value = (TValue)weak.Target;
         lock (_syncRoot)
         {
            Nodes.Remove(value);
            Nodes.AddFirst(value);
         }
         return value;
      }

      /*set*/
      protected virtual bool Set(TValue value)
      {
         if (!Dict.ContainsKey(value.Key)) return false;

         if (_itemTotal > _maxItems) RemoveForRatio();
         lock (_syncRoot) Nodes.AddFirst(value);
         var weak = new WeakReference(value, false);
         Dict[value.Key] = weak;
         return true;
      }

      protected virtual bool Remove(KeyType key)
      {
         try
         {
            var value = default(TValue);
            WeakReference weak;
            if (!Dict.TryGetValue(key, out weak) || weak.Target == null)
               return false;

            value = (TValue)weak.Target;
            lock (_syncRoot)
            {
               Nodes.Remove(value);
               weak.Target = null;
            }
         }
         finally
         {
            Interlocked.Decrement(ref _itemTotal);
         }
         return true;
      }

      protected virtual void RemoveForRatio()
      {
         lock (_syncRoot)
         {
            var l = (_ratio / 100) * _maxItems; var t = 0;
            do
            {
               if (Nodes.Count == 0) return;
               Nodes.RemoveLast();
               Interlocked.Decrement(ref _itemTotal);
            }
            while (t++ < l);
         }
      }

      #endregion

      #region Attributes

      protected Dictionary<KeyType, WeakReference> Dict
      { get;  set; }

      protected LinkedList<TValue> Nodes
      { get;  set; }

      #endregion

   }

   public interface IItem<T>
   {
      T Key { get; set; }
   }
}
