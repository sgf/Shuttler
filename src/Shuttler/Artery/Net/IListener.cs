using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public interface IListener
   {
	 void Listen();
	 void Stop();

	 event EventHandler<ConnectionEventArgs> OnShuttlerInComing;
   }
}
