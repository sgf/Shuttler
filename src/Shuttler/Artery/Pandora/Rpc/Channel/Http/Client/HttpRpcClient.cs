using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public class HttpRpcClient: IClientChannel
   {
      public HttpRpcClient(string uri)
      {
         HttpRpcClientTransport trans = new HttpRpcClientTransport(uri);
         ITrans = trans;
      }

      public IClientTransport ITrans { get; set; }
   }
}
