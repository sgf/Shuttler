using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public interface IClientTransport:IDisposable
   {
       void BeginInvoke<TArgs>(string method, TArgs args, Action<RpcContext<TArgs>> action);
   }
}
