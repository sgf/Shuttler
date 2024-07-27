using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public class RpcProxy
   {
      private IClientChannel _clientChannel;
      public RpcProxy(string uri)
      {
         Uri = uri;
         if (uri.ToLower().StartsWith("http:"))
            _clientChannel = new HttpRpcClient(uri);
         else if (uri.ToLower().StartsWith("tcp:"))
            _clientChannel = new TcpRpcClient(uri);
         else
            throw new NotSupportedException("Not support Uri");
      }

      public void BeginInvoke<TArgs>(string method, TArgs args, Action<RpcContext<TArgs>> action)
      {
         _clientChannel.ITrans.BeginInvoke(method, args, action);
      }

      public string Uri
      { get; private set; }

   }

}
