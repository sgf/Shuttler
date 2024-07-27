using System;
using System.Collections.Generic;
using System.Net;

namespace Shuttler.Artery
{
   [Serializable]
   public class StreamEventArgs : EventArgs
   {
      public StreamEventArgs(IConnection conn, PacketBase packetContext)
      {
         ShuttlerConnection = conn;
         PacketContext = packetContext;
      }

      public IConnection ShuttlerConnection { get; set; }
      public PacketBase PacketContext { get; set; }
   }

}
