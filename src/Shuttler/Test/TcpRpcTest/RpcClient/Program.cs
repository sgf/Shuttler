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
    class Program
    {
       static void Main()
       {
           RpcProxy proxy = new RpcProxy("tcp:127.0.0.1:8888");

           ShuttlerEntity t = new ShuttlerEntity();
           t.Name = "overred";
           t.Email = "overred@gmail.com";

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
           }
           );

           proxy.BeginInvoke("GetEmail", t, (clientContext) =>
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

          Console.Read();
       }
    }

}
