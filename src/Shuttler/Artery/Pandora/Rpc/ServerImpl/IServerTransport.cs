using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public interface IRpcServerTransport<TEntity, TService>:IDisposable
   {
      void RegisterService(TService service);
      RpcContext<TEntity> Excute(RpcContext<TEntity> context);
      TService TServiceInstance { get; set; }
   }
}
