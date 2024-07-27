using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   [Serializable]
   public sealed class Shuttler_Body : ShuttlerValue<byte[]>
   {
      private uint Len { get; set; }

      public Shuttler_Body(uint length)
      {
         Len = length;
      }

      public override void Parse(StreamBuffer buf)
      {
         if (Len > Global.TOAD_BODY_LENGTH || Len<0)
         {
            throw new OverflowException("Body length out Max:"+Global.TOAD_BODY_LENGTH);
         }
         Value = buf.GetByteArray(Len);
      }

   }
}
