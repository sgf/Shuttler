using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
using System.Net.Sockets;

namespace Shuttler.Artery
{
   public class TcpRpcClient : IClientChannel
   {
      public IClientTransport ITrans { get; set; }
      private ShuttlerArtery _artery;

      public TcpRpcClient(string uri)
      {
         _artery = new ShuttlerArtery(string.Format("tcp:127.0.0.1:{0}", ArterySettings.Instance.LocalPort));
         ITrans = new TcpRpcClientTransport(_artery, uri);
      }

   }
}
