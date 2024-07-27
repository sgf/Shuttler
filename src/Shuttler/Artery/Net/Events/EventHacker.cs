using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public static class EventHacker
   {
      public static void OnEvent<T>(this EventHandler<T> handler, object obj, T e) where T : EventArgs
      {
         var handle = handler;
         if (handle != null)
            handle.Invoke(obj, e);
      }
   }
}
