using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public class HashingManager<TKey>
   {
      private static HashingBase<TKey> _consistentHashing;

      public static void Initialize(IList<string> proxyUrl)
      {
         _consistentHashing = new ConsistentHashingAlgorithm<TKey>(Int32.MaxValue / proxyUrl.Count, proxyUrl);
      }

      public void Add(int dummyNodeNum, string server)
      {
         _consistentHashing.Add(dummyNodeNum,server);
      }

      public static string Get(TKey key)
      {
         return _consistentHashing.Get(key);
      }

      public static void Remove(string server)
      {
         _consistentHashing.Remove(server);
      }
   }

   public abstract class HashingBase<TKey>
   {
      public abstract void Add(int dummyNodeNum, string server);
      public abstract string Get(TKey key);
      public abstract void Remove(string server);
   }
}
