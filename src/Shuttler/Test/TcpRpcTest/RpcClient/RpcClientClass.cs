//#define RPC_CRAZY
using System;
using System.IO;
using System.Text;
using ProtoBuf;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Shuttler.Artery;
using ServiceCommon;

namespace TcpClient
{

   public class RpcClientClass
   {
      private long _loop = 10000 * 1000;

      public void Start()
      {
         RpcProxy<ShuttlerEntity> proxy = new RpcProxy<ShuttlerEntity>("tcp:127.0.0.1:8888");

         ShuttlerEntity t = new ShuttlerEntity();
         t.Name = "overred";
         t.Email = "overred@gmail.com";

         RpcContext<ShuttlerEntity> context = new RpcContext<ShuttlerEntity>();
         context.Value = t;

         #region  BeginInvoke
         /*
          *proxy.BeginInvoke(MethodName,RpcContext<T> context, Action<RpcContext<T>> action) 
          */
         proxy.BeginInvoke("GetName", context, (clientContext) =>
         {
            try
            {
               ShuttlerEntity se = clientContext.Value as ShuttlerEntity;
               if (string.IsNullOrEmpty(clientContext.RpcException))
                  Console.WriteLine("Name:::" + se.Name);
               else
                  Console.WriteLine(clientContext.RpcException);

            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
         }
         );

         proxy.BeginInvoke("GetEmail", context, (clientContext) =>
         {
            try
            {
               ShuttlerEntity se = clientContext.Value as ShuttlerEntity;
               if (string.IsNullOrEmpty(clientContext.RpcException))
                  Console.WriteLine("Email:::" + se.Email);
               else
                  Console.WriteLine(clientContext.RpcException);

            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
         }
            );

         #endregion

         #region LoadRunner
#if(RPC_CRAZY)
         ThreadPool.QueueUserWorkItem((cb) =>
             {
                 for (int i = 0; i < _loop; i++)
                 {
                     proxy.BeginInvoke("GetEmail", context, (clientContext) =>
                     {
                         //todo
                     }
                   );
                     Thread.Sleep(1);
                 }
             });

         ThreadPool.QueueUserWorkItem((cb) =>
         {
             for (int i = 0; i < _loop; i++)
             {
                 proxy.BeginInvoke("GetEmail", context, (clientContext) =>
                 {
                     //todo
                 }
               );
                 Thread.Sleep(1/2);
             }
         });

         ThreadPool.QueueUserWorkItem((cb) =>
         {
             for (int i = 0; i < _loop; i++)
             {
                 proxy.BeginInvoke("GetName", context, (clientContext) =>
                 {
                     //todo
                 }
               );
                 Thread.Sleep(1);
             }
         });

         ThreadPool.QueueUserWorkItem((cb) =>
         {
             for (int i = 0; i < _loop; i++)
             {
                 proxy.BeginInvoke("GetName", context, (clientContext) =>
                 {
                     //todo
                 }
               );
                 Thread.Sleep(1/2);
             }
         });
#endif
         #endregion

      }
   }
}
