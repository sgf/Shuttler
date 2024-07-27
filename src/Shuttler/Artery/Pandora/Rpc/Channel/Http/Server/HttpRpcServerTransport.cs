using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ProtoBuf;
using ProtoBuf.ServiceModel;
using System.IO;
using System.Reflection;

namespace Shuttler.Artery
{
   public class HttpRpcServerTransport<TEntity, TService> :IRpcServerTransport<TEntity, TService>
   {
      #region Fields

      private Dictionary<string, FastMethodInvoker<TService, RpcContext<TEntity>>> _methods;
      private Action<HttpListenerContext> _gotContext;
      private HttpListener _listener;

      #endregion

      #region Methods

      public HttpRpcServerTransport(HttpListener listener)
      {
         _methods = new Dictionary<string, FastMethodInvoker<TService, RpcContext<TEntity>>>(256);
         _listener = listener;
         _gotContext = GotContext;
      }

      public void Start()
      {
         ListenForContext();
      }

      public void RegisterService(TService service)
      {
         TServiceInstance = service;
         Type serviceType = typeof(TService);
         foreach (MethodInfo method in serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
         {
            if (method.IsGenericMethod
                || method.IsGenericMethodDefinition
                || method.DeclaringType != serviceType) continue;

            string methodName = method.Name;
            if (!_methods.ContainsKey(methodName))
            {
               FastMethodInvoker<TService, RpcContext<TEntity>> fastInvoker = new FastMethodInvoker<TService, RpcContext<TEntity>>(methodName);
               _methods.Add(methodName, fastInvoker);
            }
            else
               throw new InvalidOperationException("Method Registed!");

         }
      }

      public RpcContext<TEntity> Excute(RpcContext<TEntity> context)
      {
         FastMethodInvoker<TService, RpcContext<TEntity>> fastInvoker;
         if (!_methods.TryGetValue(context.MethodName, out fastInvoker))
            throw new NullReferenceException("No MethodName in !");
         return fastInvoker.Invoke(TServiceInstance, new object[] { context.Value });
      }

      public void Dispose()
      { }

      private void ListenForContext()
      {
         AsyncUtility.RunAsync(
                 _listener.BeginGetContext, _listener.EndGetContext,
                 _gotContext, null);
      }

      private void GotContext(HttpListenerContext context)
      {
         try
         {
            ProcessContext(context);
         }
         catch (Exception ex)
         {
            try
            {
               context.Response.ContentType = "text/plain";
               byte[] buffer = Encoding.UTF8.GetBytes(ex.Message);
               context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch { }
         }
         finally
         {
            try
            {
               context.Response.Close();
               ListenForContext();
            }
            catch { }
         }
      }

      private void ProcessContext(HttpListenerContext context)
      {
         try
         {
            string rpcVer = context.Request.Headers[RpcUtils.HTTP_RPC_VERSION_HEADER];
            if (!string.IsNullOrEmpty(rpcVer) && rpcVer != "0.1")
            {
               throw new InvalidOperationException("Incorrect RPC version");
            }
            string[] segments = context.Request.Url.Segments;
            string serviceName = segments[segments.Length - 2].TrimEnd('/'),
                actionName = segments[segments.Length - 1].TrimEnd('/');


            int contextLength = int.Parse(context.Request.Headers["Content-Length"]);
            byte[] body = new byte[contextLength];
            context.Request.InputStream.Read(body, 0, contextLength);

            RpcContext<TEntity> serverContext = SerializerExt.DeserializeBytesToT<RpcContext<TEntity>>(body);
            serverContext.MethodName = actionName;

            RpcContext<TEntity> returnObj = Excute(serverContext);
            byte[] buffer = SerializerExt.SerializerStreamToBytes<RpcContext<TEntity>>(returnObj);
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
         }
         finally
         {
            ArteryPerfCounter.Instance.RateOfRpcServerInvoked.Increment();
         }
      }

      #endregion

      #region Attribute

      public TService TServiceInstance { get; set; }

      #endregion
   }
}
