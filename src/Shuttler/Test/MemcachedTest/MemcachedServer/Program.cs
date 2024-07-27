using System;
using Shuttler.Artery;
using System.Collections.Generic;

using ServiceCommon;


namespace MemcachedServer
{
   class Program
   {
      static void Main(string[] args)
      {
         IMemcached cached = new MemcachedSvr();
         TcpRpcServer<MemcachedEntity, IMemcached> server1 = new TcpRpcServer<MemcachedEntity, IMemcached>("tcp:127.0.0.1:8887");
         server1.RegisterService(cached);
         server1.Start();
         Console.WriteLine("The MemcachedServer1(127.0.0.1:8887) start....");

         TcpRpcServer<MemcachedEntity, IMemcached> server2 = new TcpRpcServer<MemcachedEntity, IMemcached>("tcp:127.0.0.1:8886");
         server2.RegisterService(cached);
         server2.Start();
         Console.WriteLine("The MemcachedServer2(127.0.0.1:8886) start....");

         TcpRpcServer<MemcachedEntity, IMemcached> server3 = new TcpRpcServer<MemcachedEntity, IMemcached>("tcp:127.0.0.1:8885");
         server3.RegisterService(cached);
         server3.Start();
         Console.WriteLine("The MemcachedServer3(127.0.0.1:8885) start....");
         Console.Read();
      }
   }
}
