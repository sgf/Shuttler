using System;
using Shuttler.Artery;
using System.Collections.Generic;

using ServiceCommon;


namespace TcpServer
{
   class Program
   {
       static void Main(string[] args)
       {
           ISample sample = new Sample();
           TcpRpcServer<ShuttlerEntity, ISample> server = new TcpRpcServer<ShuttlerEntity, ISample>("tcp:127.0.0.1:8888");
           server.RegisterService(sample);
           server.Start();
           Console.WriteLine("RpcServer start...");
           Console.Read();
       }
   }

}
