using System;
using System.IO;
using System.Text;
using ProtoBuf;
using Shuttler.Artery;
using System.Collections.Generic;
using System.Threading;

namespace ServiceCommon
{
   public class MemcachedSvr :CacheBase<MemcachedEntity, int>, IMemcached
   {
      private static IDebugger _debug = new Debugger<MemcachedSvr>(Level.Info);

      public MemcachedSvr()
         : base(500 << 10)
      { }

      public RpcContext<MemcachedEntity> AddCache(MemcachedEntity entity)
      {
         _debug.Info("add");
         base.Add(entity);
         return null;
      }

      public RpcContext<MemcachedEntity> GetCache(MemcachedEntity entity)
      {
         _debug.Info("get key:" + entity.Key);
         MemcachedEntity contextEntity = base.Get(entity.Key);

         RpcContext<MemcachedEntity> context = new RpcContext<MemcachedEntity>();
         context.Value = contextEntity;
         context.RetBoolResult = true;
         return context;
      }

      public  RpcContext<MemcachedEntity> SetCache(MemcachedEntity entity)
      {
         _debug.Info("set key:" + entity.Key);
         RpcContext<MemcachedEntity> context = new RpcContext<MemcachedEntity>();
         context.RetBoolResult=base.Set(entity);
         return context;
      }

      public RpcContext<MemcachedEntity> RemoveCache(MemcachedEntity entity)
      {
         _debug.Info("remove key:" + entity.Key);
         RpcContext<MemcachedEntity> context = new RpcContext<MemcachedEntity>();
         context.RetBoolResult = base.Remove(entity.Key);
         return context;
      }

   }

   public interface IMemcached
   {
      RpcContext<MemcachedEntity> AddCache(MemcachedEntity entity);
      RpcContext<MemcachedEntity> GetCache(MemcachedEntity entity);
      RpcContext<MemcachedEntity> SetCache(MemcachedEntity entity);
      RpcContext<MemcachedEntity> RemoveCache(MemcachedEntity entity);
   }

   [ProtoContract]
   public class MemcachedEntity : IItem<int>
   {
      [ProtoMember(1)]
      public int Key
      { get; set; }

      [ProtoMember(2)]
      public string Name;

      [ProtoMember(3)]
      public string Email;
   }
}
