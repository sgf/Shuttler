/* 分布式缓存，通过hash策略查找。
 * 
 * 本示例演示了多个缓存server端，一个client端如何调用的情况
 * 两个 Memcached server: tcp:127.0.0.1:8887;tcp:127.0.0.1:8886
 * 按哈希策略循环添加100个缓存项，然后对其进行修改(set),删除(remove),获取(get)操作
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Shuttler.Artery;
using ServiceCommon;

namespace MemcachedClient
{
   class Program
   {
      private static IDebugger _debug = new Debugger<Program>(Level.Info);
      static void Main(string[] args)
      {
         Test t = new Test();
         t.InitializeProxy();

         t.Add();

         if (t.Set(5))
            _debug.Info("set key:5 success!");

         Thread.Sleep(1000);
         MemcachedEntity m = t.Get(5);
         _debug.Info("get key:5 name:" + m.Name + "  email:" + m.Email);

         if(t.Remove(88))
            _debug.Info("remove key:88 success!");

         MemcachedEntity m88 = t.Get(88);
         if (m88 == null)
            _debug.Info("88 is NULL!");


         Console.Read();
      }

   }


   class Test
   {
      private static IDebugger _debug = new Debugger<Test>(Level.Info);
      private static RpcProxy _proxy;

      public void InitializeProxy()
      {
         IList<string> proxys = new List<string>();
         proxys.Add("tcp:127.0.0.1:8887");
         proxys.Add("tcp:127.0.0.1:8886");
         proxys.Add("tcp:127.0.0.1:8885");
         RpcProxyManager.InitializeAll<string>(proxys);
      }


      public void Add()
      {
         RpcContext<MemcachedEntity> context = new RpcContext<MemcachedEntity>();
         for (int i = 0; i < 1000; i++)
         {
            MemcachedEntity entity1 = new MemcachedEntity();
            entity1.Key = i;
            entity1.Email = string.Format("overred{0}@gmail.com", i);
            entity1.Name = "overred" + i;
            context.Value = entity1;

            _proxy = RpcProxyManager.GetProxy<string>(i.ToString());
            _debug.Info("add uri:" + _proxy.Uri);
            _proxy.BeginInvoke("AddCache", context, (clientContext) =>
            {
            }
            );
         } 
      }
      public bool Set(int key)
      {
         ManualResetEvent evt = new ManualResetEvent(false);
         bool set = false;
         RpcContext<MemcachedEntity> context = new RpcContext<MemcachedEntity>();
         MemcachedEntity entity = new MemcachedEntity();
         entity.Key = key;
         entity.Email = "overred-set@gmail.com";
         entity.Name = "overred-set";
         context.Value = entity;

         _proxy = RpcProxyManager.GetProxy<string>(key.ToString());
         _debug.Info("set uri:" + _proxy.Uri);
         _proxy.BeginInvoke("SetCache", context, (clientContext) =>
         {
               set = clientContext.RetBoolResult;
               evt.Set();
         }
         );
         if (!evt.WaitOne(1000))
            _debug.Error("set timeout!");
            
         return set;
      }

      public MemcachedEntity Get(int key)
      {
         ManualResetEvent evt = new ManualResetEvent(false);
         MemcachedEntity entity = new MemcachedEntity();
         entity.Key = key;
         _proxy = RpcProxyManager.GetProxy<string>(key.ToString());

         _debug.Info("get uri:" + _proxy.Uri);
         _proxy.BeginInvoke("GetCache", entity, (clientContext) =>
         {
            if (string.IsNullOrEmpty(clientContext.RpcException))
               entity = clientContext.Value;
            else
               entity = null;
            evt.Set();
         });
         if (!evt.WaitOne(1000))
            _debug.Error("get timeout!");

         return entity;
      }

      public bool Remove(int key)
      {
         ManualResetEvent evt = new ManualResetEvent(false);
         bool remove = false;
         MemcachedEntity entity = new MemcachedEntity();
         entity.Key = key;

         _proxy = RpcProxyManager.GetProxy<string>(key.ToString());
         _debug.Info("remove uri:"+_proxy.Uri);
         _proxy.BeginInvoke("RemoveCache", entity, (clientContext) =>
         {
            remove = clientContext.RetBoolResult;
            evt.Set();
         });
         if (!evt.WaitOne(1000))
            _debug.Error("remove timeout!");

         return remove;
      }
   }
}
