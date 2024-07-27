using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using Shuttler.Artery;

namespace Shuttler.IM.Server
{
   public class UserAgent
   {
      #region Fields

      private IDebugger _debug = new Debugger<UserAgent>(Level.NO);

      #endregion

      #region Methods

      public UserAgent(ShuttlerAgent shuttlerAgent)
      {
         _debug.InfoFormat("Toad coming!key:{0}", shuttlerAgent.ToString());

         ShuttlerAgent = shuttlerAgent;
         ShuttlerAgent.OnTransactionCreated += OnTransactionCreated;
         ShuttlerAgent.OnDisconnected += OnDisconnected;
      }

      private void OnDisconnected(object sender, ConnectionEventArgs e)
      {
         _debug.InfoFormat("user-closed!key:{0}", ShuttlerAgent.ToString());
      }

      private void OnTransactionCreated(object sender, TransationEventArgs e)
      {
         Request request = e.Transaction.InitialRequest;
         _debug.Info(request);

         switch (request.Command)
         {
            case (ushort)Command.Login:
               ProcessLogin(request, e);
               break;
            case (ushort)Command.Get_Buddy_List:
               ProcessGetBuddyList(request, e);
               break;
            default: break;
         }

      }

      private void ProcessLogin(Request request, TransationEventArgs e)
      {
         Response response = new Response(request);
         UserAgentID = request.From;

         using (StreamBuffer reader = new StreamBuffer(request.Body))
         {
            var cPwd = reader.GetByteArray(20);
            var status = reader.Get();
            var sPwd = Encoding.UTF8.GetBytes("overred");

            if (SHA1.Create().ComputeHash(sPwd).SequenceEqual(cPwd))
               response.Body = new byte[] { 0x01 };
            else
               response.Body = new byte[] { 0x00 };
         }

         e.Transaction.SendResponse(response);
      }

      void ProcessGetBuddyList(Request request, TransationEventArgs e)
      {
         //nun todo
      }

      #endregion

      #region Attributes

      private ShuttlerAgent ShuttlerAgent
      { get; set; }

      private AgentEntity Items
      { get; set; }

      private byte[] UserAgentID
      { get; set; }

      #endregion
   }
}
