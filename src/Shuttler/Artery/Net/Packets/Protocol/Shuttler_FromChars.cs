﻿using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   [Serializable]
   public sealed class Shuttler_FromChars : ShuttlerValue<byte[]>
   {
      public override void Parse(StreamBuffer buf)
      {
         Value = buf.GetByteArray(Global.TOAD_TOADIDCHARS_LENGTH);
      }
   }
}