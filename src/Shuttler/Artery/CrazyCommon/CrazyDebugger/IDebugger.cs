using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public interface IDebugger
   {
      void Info(PacketBase packet);
      void Info(PacketBase packet, string desc);
      void Info(string message);
      void Error(Exception ex, string message);
      void Error(string message);
      void ErrorFormat(Exception ex, string format, params object[] args);
      void InfoFormat(string format, params object[] args);
      Level Level { get; set; }
   }
}
