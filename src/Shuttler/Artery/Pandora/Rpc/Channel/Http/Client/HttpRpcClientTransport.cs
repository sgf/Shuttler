using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using ProtoBuf;

namespace Shuttler.Artery
{
   public class HttpRpcClientTransport : IClientTransport
   {
      private string _uri = string.Empty;

      public HttpRpcClientTransport(string uri)
      {
         _uri = uri;
      }

      public void BeginInvoke<TArgs>(string method, TArgs args, Action<RpcContext<TArgs>> action)
      {
         try
         {
            string url = string.Format("{0}/{1}/", _uri, method);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            RpcContext<TArgs> context = new RpcContext<TArgs>();
            context.Value = args;

            byte[] body = SerializerExt.SerializerStreamToBytes<RpcContext<TArgs>>(context);
            int len = body.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(body, 0, len);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
               byte[] buffers = Encoding.UTF8.GetBytes(sr.ReadToEnd());
               RpcContext<TArgs> con = SerializerExt.DeserializeBytesToT<RpcContext<TArgs>>(buffers);
               con.MethodName = method;
               if (action != null)
                  action(con);
            }
         }
         finally
         {
            ArteryPerfCounter.Instance.RateOfRpcClientInvoke.Increment();
         }
      }

      public void Dispose()
      { }
   }
}
