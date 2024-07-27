using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public class CacherContext<TKeyType> : IItem<TKeyType>
   {
      public TKeyType Key { get; set; }
      public byte[] Value { get; set; }
   }
}
