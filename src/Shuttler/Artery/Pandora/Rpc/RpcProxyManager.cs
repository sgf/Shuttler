using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   /*
    RpcProxy Hash策略:
    * TKey为你的哈希key类型
    * 此类适用于多个分布式服务，通过key算出服务地址的情况。
    * 哈希算法的选择取决于HashingManager的设置，默认是ConsistentHashing算法！
    * 若一对一的模式请直接使用:
    * RpcProxy proxy = new RpcProxy("tcp:127.0.0.1:8888");
    */
   public class RpcProxyManager
   {
      private static Dictionary<string, RpcProxy> _proxys = new Dictionary<string, RpcProxy>();
      private static IList<string> _proxyUrl;

      public static void InitializeAll<TKey>(IList<string> proxyUrl)
      {
         _proxyUrl = proxyUrl;
         Initialize();
         HashingManager<TKey>.Initialize(_proxyUrl);
      }

      private static void Initialize()
      {
         if (_proxyUrl.Count > 0)
         {
            foreach (string url in _proxyUrl)
            {
               if (!_proxys.ContainsKey(url))
               {
                  RpcProxy proxy = new RpcProxy(url);
                  _proxys.Add(url, proxy);
               }
            }
         }
      }

      public static RpcProxy GetProxy<TKey>(TKey key)
      {
          string url = HashingManager<TKey>.Get(key);
         RpcProxy proxy = null;
         if (!_proxys.TryGetValue(url, out proxy))
         {
            proxy = new RpcProxy(url);
            _proxys.Add(url, proxy); 
         }
         return proxy;
      }
   }
}
