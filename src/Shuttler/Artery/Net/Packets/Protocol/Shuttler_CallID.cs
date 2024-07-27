using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   [Serializable]
   public sealed class Shuttler_CallID : ShuttlerValue<long>
   {
      public override void Parse(StreamBuffer buf)
      {
         Value = buf.GetLong();
      }
   }
}
