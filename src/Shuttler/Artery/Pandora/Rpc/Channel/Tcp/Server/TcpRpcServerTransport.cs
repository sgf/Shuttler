using System;
using System.Collections.Generic;
using ProtoBuf;
using System.Reflection;
using System.Threading;

namespace Shuttler.Artery
{
   public class RpcServerTransport<TEntity, TService> : IRpcServerTransport<TEntity, TService>
   {
      #region Fields

      private static IDebugger _debug = new Debugger<RpcServerTransport<TEntity, TService>>(ArterySettings.Instance.Level);
      private Dictionary<string, FastMethodInvoker<TService, RpcContext<TEntity>>> _methods;
      public TService TServiceInstance { get; set; }

      #endregion

      #region Methods

      public RpcServerTransport(ShuttlerArtery artery)
      {
         _methods = new Dictionary<string, FastMethodInvoker<TService, RpcContext<TEntity>>>(256);
         artery.ShuttlerAgentManager.OnTransactionCreated += OnShuttlerAgentManagerTransactionCreated;
      }

      private void OnShuttlerAgentManagerTransactionCreated(object sender, TransationEventArgs e)
      {
          ArteryPerfCounter.Instance.RateOfRpcServerInvoked.Increment();

         ThreadPool.QueueUserWorkItem(state =>
         {
            string exStr = string.Empty;
            TransationEventArgs args = e;

            RpcContext<TEntity> context=null,returnObj=null;
            Response response = new Response(args.Transaction.InitialRequest);
            try
            {
               context = SerializerExt.DeserializeBytesToT<RpcContext<TEntity>>(args.Transaction.InitialRequest.Body.BufferTrimEnd());
               if (context == null) return;
               returnObj = Excute(context);
            }
            catch (Exception ex)
            {
               exStr =string.Concat(new object[]{ ex.Message,ex.StackTrace});
               _debug.Error(ex, "RpcServerTransport Error!!");
            }
            finally
            {
               //ArteryPerfCounter.Instance.RateOfRpcServerInvoked.Increment();
            }

            if (returnObj == null)
               returnObj = new RpcContext<TEntity>();
            returnObj.MethodName = context.MethodName;

            if (!string.IsNullOrEmpty(exStr))
               returnObj.RpcException = exStr;

            response.Body = SerializerExt.SerializerStreamToBytes<RpcContext<TEntity>>(returnObj);
            e.Transaction.SendResponse(response);
         },null);
      }

      #endregion

      #region Impls

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
            throw new NullReferenceException("Not Support MethodName:"+context.MethodName);
         return fastInvoker.Invoke(TServiceInstance, new object[] { context.Value });
      }

      public void Dispose()
      { }

      #endregion
   }
}
