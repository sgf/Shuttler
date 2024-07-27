using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public  class HashingAlgorithm<TKey> : HashingBase<TKey>
   {
      private static Dictionary<int, string> _proxys = new Dictionary<int, string>();
      private static int _count = 0;
      public HashingAlgorithm(int dummyNodeNum, IList<string> servers)
      {
         _count = servers.Count;
         int i = 0;
         foreach (string s in servers)
            _proxys.Add(i++, s);
         
      }

      public override void Add(int dummyNodeNum, string server)
      {
         throw new NotImplementedException();
      }

      public override string Get(TKey key)
      {
         int hash = NewHashingAlgorithm(key.ToString());
         int index = hash % _count;
         if(_proxys.ContainsKey(index))
            return _proxys[index];

         return string.Empty;
      }

      public override void Remove(string server)
      {
         throw new NotImplementedException();
      }

      /*Found to be fast and have very good distribution.*/
      public  int NewHashingAlgorithm(string key)
      {
         CRCHelper checksum = new CRCHelper();
         checksum.Init(CRCHelper.CRCCode.CRC32);
         int crc = (int)checksum.crctablefast(UTF8Encoding.UTF8.GetBytes(key));

         return (crc >> 16) & 0x7fff;
      }



   }
}
