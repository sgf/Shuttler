using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   [Flags]
   public enum ChanneMode : byte
   {
      TCP = 0x00,
      HTTP = 0x01,
      INPROC = 0x02,
      PIPELINE = 0x03,
      REMOTING = 0x04
   }
}
