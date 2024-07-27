using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Shuttler.Artery;
using ServiceCommon;

namespace HttpClient
{
   class Program
   {
      static RpcProxy proxy = null;
      static void Main(string[] args)
      {
         string url = "http://127.0.0.1:8082/request";
         proxy = new RpcProxy(url);

         ShuttlerEntity t = new ShuttlerEntity();
         t.Email = "overred@gmail.com";
         t.Name = "overred";

         proxy.BeginInvoke("GetName", t, (clientContext) =>
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
          });

         proxy.BeginInvoke("GetEmail", t, (clientContext) =>
         {
            try
            {
               ShuttlerEntity se = clientContext.Value as ShuttlerEntity;
               if (!string.IsNullOrEmpty(clientContext.RpcException))
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

        

         //for (int i = 0; i < _loop; i++)
         //{
         //   ThreadPool.QueueUserWorkItem(new WaitCallback(CallBack),context);
         
         //   //Thread.Sleep(1/10);
         //}

         

        

         Console.Read();
      }

      static void CallBack(object obj)
      {
          ShuttlerEntity args = obj as ShuttlerEntity;
         proxy.BeginInvoke("GetEmail", args, (clientContext) =>
           {
              try
              {
                  ShuttlerEntity se = args as ShuttlerEntity;
                 //if (!clientContext.HasException)
                 //   Console.WriteLine("Email:::" + se.Email);
                 //else
                 //   Console.WriteLine(clientContext.RpcException);
              }
              catch (Exception ex)
              {
                 Console.WriteLine(ex.Message);
              }
           });
      }
   }
}
