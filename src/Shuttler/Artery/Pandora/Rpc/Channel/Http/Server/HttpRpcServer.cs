using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Shuttler.Artery
{
   public class HttpRpcServer<TEntity, TService> : IServerChannel<TEntity, TService>
   {
      #region Fields

      HttpRpcServerTransport<TEntity, TService> _transport;
      HttpListener _listener;

      #endregion

      #region Methods

      public HttpRpcServer(string uri)
      {
         _listener = new HttpListener();
         _listener.Prefixes.Add(uri);

         _transport = new HttpRpcServerTransport<TEntity, TService>(_listener);
      }

      public void Start()
      {
         _listener.Start();
         _transport.Start();
      }

      public void Stop()
      {
         _transport.Dispose();
         _listener.Stop();
      }

      public void RegisterService(TService service)
      {
         _transport.RegisterService(service);
      }
      #endregion
   }
}
