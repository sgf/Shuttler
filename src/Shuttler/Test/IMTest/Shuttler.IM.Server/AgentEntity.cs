using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.IM.Server
{
   public class AgentEntity
   {
      public byte[] ID
      { get; private set; }

      public byte Status
      { get; private set; }

      public Dictionary<byte[], AgentEntity> Buddies
      { get; set; }

      public AgentEntity(byte[] id, byte status)
      {
         ID = id;
         Status = status;
      }
   }
}
