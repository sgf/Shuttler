/*Consistent Hashing负载算法
*http://www.cnblogs.com/overred/archive/2009/12/29/Consistent_Hashing.html 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public class ConsistentHashingAlgorithm<TKey> : HashingBase<TKey>
   {
      #region Fields

      private object _sync = new object();
      private IEqualityComparer<TKey> _comparer;
      private SortedDictionary<int, string> _dict;

      #endregion

      #region Methods

      public ConsistentHashingAlgorithm(int dummyNodeNum, IList<string> servers)
      {
         _comparer = EqualityComparer<TKey>.Default;
         _dict = new SortedDictionary<int, string>();

         int c = 1;
         foreach (string server in servers)
         {
            int node = (c++) * dummyNodeNum & 0x7fffffff;
            _dict.Add(node, server);
         }
      }

      public override void Remove(string server)
      {
         lock (_sync)
         {
            foreach (KeyValuePair<int, string> kv in _dict)
            {
               if (kv.Value.Equals(server))
               {
                  _dict.Remove(kv.Key);
                  return;
               }
            }
         }
      }

      public override void Add(int dummyNodeNum, string server)
      {
         dummyNodeNum = dummyNodeNum & 0x7fffffff;
         lock (_sync) _dict.Add(dummyNodeNum, server);
      }

      public override string Get(TKey key)
      {
         int keyHash = _comparer.GetHashCode(key) & 0x7fffffff;
         lock (_sync)
         {
            if (keyHash > _dict.Last().Key)
               return _dict.First().Value;

            return _dict.First(index => index.Key >= keyHash).Value;
         }
      }

      public override string ToString()
      {
         StringBuilder sb = new StringBuilder();
         foreach (KeyValuePair<int, string> kv in _dict)
            sb.AppendFormat("start:{0},server:{1}\r\n", kv.Key, kv.Value);

         return sb.ToString();
      }

      #endregion
   }
}
