using System;
using System.Collections.Generic;
using System.Text;

namespace Shuttler.Artery
{
   public sealed class Response : PacketBase
   {
      #region Methods

      public Response(Request request)
      {
         Header = request.Header;
         From = request.From;
         To = request.To;
         Command = request.Command;
         CallID = request.CallID;
         Tail = request.Tail;
      }

      public Response(IPacket packet)
      {
         Header = packet.Header.Value;
         Command = packet.Command.Value;
         From = packet.From.Value;
         To = packet.To.Value;
         CallID = packet.CallID.Value;
         Body = packet.Body.Value;
         Tail = packet.Tail.Value;
      }

      #endregion
   }
}
