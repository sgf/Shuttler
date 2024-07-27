using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public interface IClientChannel
   {
      IClientTransport ITrans { get; set; }
   }
}
