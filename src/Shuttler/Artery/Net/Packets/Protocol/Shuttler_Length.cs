using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   [Serializable]
   public sealed class Shuttler_Length : ShuttlerValue<uint>
   {
      public override void Parse(StreamBuffer buf)
      {
         Value = buf.GetUInt();
         if (Value > uint.MaxValue)
            throw new OverflowException("Body len overflow:" + Value);
      }
   }
}
