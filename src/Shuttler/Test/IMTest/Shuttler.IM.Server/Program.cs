using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.IM.Server
{
   class Program
   {
      static void Main(string[] args)
      {
         IMServer server = new IMServer();
         server.Start();
         Console.WriteLine("Server start...");
         Console.Read();
      }
   }
}
