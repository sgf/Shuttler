using System;
using System.Collections.Generic;
using System.Net;

namespace Shuttler.Artery
{
   [Serializable]
   public class ResponseEventArgs : EventArgs
   {
      public PacketBase PacketContext { get; set; }
   }
}
