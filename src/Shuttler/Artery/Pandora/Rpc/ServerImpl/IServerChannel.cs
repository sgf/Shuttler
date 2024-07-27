using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public interface IServerChannel<TEntity, TService>
   {
      void RegisterService(TService t);
      void Start();
      void Stop();
   }
}
