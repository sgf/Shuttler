using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shuttler.Artery;
using ServiceCommon;

namespace HttpServer
{
   class Program
   {
      static void Main(string[] args)
      {
         ISample sample = new Sample();

         IServerChannel<ShuttlerEntity, ISample> server = new HttpRpcServer<ShuttlerEntity, ISample>("http://127.0.0.1:8082/request/");
         server.RegisterService(sample);
         server.Start();
         Console.WriteLine("Http server start...");
         Console.Read();
      }
   }
}
