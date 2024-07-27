using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
using System.Reflection;

namespace Shuttler.Artery
{
   public class TcpRpcServer<TEntity, TService> : IServerChannel<TEntity, TService>
   {
      #region Fields

      private RpcServerTransport<TEntity, TService> _transport;
      private ShuttlerArtery _artery;

      #endregion

      #region Methods

      public TcpRpcServer(string uri)
      {
         _artery = new ShuttlerArtery(uri);
         _transport = new RpcServerTransport<TEntity, TService>(_artery);
      }

      public void RegisterService(TService t)
      {
         _transport.RegisterService(t);
      }

      public void Start()
      {
         _artery.Start();
      }

      public void Stop()
      {
         _artery.Stop();
      }

      #endregion
   }
}
