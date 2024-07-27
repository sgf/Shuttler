using System;
using System.IO;
using System.Text;
using ProtoBuf;
using Shuttler.Artery;
using System.Collections.Generic;
using System.Threading;

namespace ServiceCommon
{
   public class Sample : ISample
   {
      public RpcContext<ShuttlerEntity> GetName(ShuttlerEntity entity)
      {
         RpcContext<ShuttlerEntity> context=null;
         try
         {
            ShuttlerEntity contextEntity = new ShuttlerEntity();
            contextEntity.Name = "overred:::" + entity.Name + "---" + DateTime.Now.ToString();
            context = new RpcContext<ShuttlerEntity>();
            context.Value = contextEntity;
         }
         catch (Exception ex)
         {
            context.RpcException = ex.Message;
         }
         return context;
      }

      public RpcContext<ShuttlerEntity> GetEmail(ShuttlerEntity entity)
      {
         RpcContext<ShuttlerEntity> context = null;
         try
         {
            ShuttlerEntity contextEntity = new ShuttlerEntity();
            contextEntity.Email = entity.Name + "@gmail.com---" + DateTime.Now.ToString();
            context = new RpcContext<ShuttlerEntity>();
            context.Value = contextEntity;
         }
         catch (Exception ex)
         {
            context.RpcException = ex.Message;
         }
         return context;
      }
   }

   public interface ISample
   {
      RpcContext<ShuttlerEntity> GetName(ShuttlerEntity s);
      RpcContext<ShuttlerEntity> GetEmail(ShuttlerEntity s);
   }

   [ProtoContract]
   public class ShuttlerEntity
   {
      [ProtoMember(1)]
      public string Name;

      [ProtoMember(2)]
      public string Email;
   }
}
