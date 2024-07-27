using System;
using System.Linq;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public sealed class Packet : IPacket
   {
      private static IDebugger _debug = new Debugger<ShuttlerAgent>(ArterySettings.Instance.Level);

      #region Attributes

      public Shuttler_Header Header
      { get; private set; }

      public Shuttler_FromChars From
      { get; private set; }

      public Shuttler_ToChars To
      { get; private set; }

      public Shuttler_Command Command
      { get; private set; }

      public Shuttler_CallID CallID
      { get; private set; }

      public Shuttler_Length Length
      { get; private set; }

      public Shuttler_Body Body
      { get; set; }

      public Shuttler_Tail Tail
      { get; private set; }

      #endregion

      #region Constructor

      public Packet()
      {
         Header = new Shuttler_Header();
         Command = new Shuttler_Command();
         From = new Shuttler_FromChars();
         To = new Shuttler_ToChars();
         CallID = new Shuttler_CallID();
         Length = new Shuttler_Length();
         Tail = new Shuttler_Tail();
      }

      #endregion

      #region Read Method

      public bool Read(StreamBuffer stream)
      {
         try
         {
            Header.Parse(stream);
            if (!Header.Value.SequenceEqual(Global.TOAD_HEADER)) return false;

            Command.Parse(stream);
            From.Parse(stream);
            To.Parse(stream);
            CallID.Parse(stream);
            Length.Parse(stream);
            Body = new Shuttler_Body(Length.Value);
            Body.Parse(stream);
            Tail.Parse(stream);
            return true;
         }
         catch (Exception ex)
         {
            _debug.Error(ex, "Packet read");
            return false;
         }
      }

      #endregion
   }
}
