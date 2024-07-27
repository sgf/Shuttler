using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shuttler.Artery;

namespace Shuttler.IM.Server
{
   public class IMServer
   {
      #region Fields

      private ShuttlerArtery _artery;

      #endregion

      #region Methods

      public IMServer()
      {
         _artery = new ShuttlerArtery("tcp:127.0.0.1:8888");
         _artery.ShuttlerCreated += OnAgentCreated;
      }

      public void Start()
      {
         if(_artery!=null)
            _artery.Start();
      }

      public void Stop()
      {
         if (_artery != null)
            _artery.Stop();
      }


      private void OnAgentCreated(object sender, ShuttlerEventArgs e)
      {
         UserAgent c = new UserAgent(e.Shuttler);
      }

      #endregion
   }
}
