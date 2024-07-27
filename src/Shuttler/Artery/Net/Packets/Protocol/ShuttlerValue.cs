using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public abstract class ShuttlerValue<T>
   {
      public abstract void Parse(StreamBuffer buf);
      public virtual T Value { get; set; }
      protected void PutBody(StreamBuffer buf) { }
   }
}
