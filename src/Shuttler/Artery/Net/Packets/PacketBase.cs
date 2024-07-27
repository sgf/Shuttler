using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public abstract class PacketBase
   {
      #region Attributes

      public byte[] Header
      { get; set; }

      public byte[] From
      { get; set; }

      public byte[] To
      { get; set; }

      public ushort Command
      { get; set; }

      public byte[] Body
      { get; set; }

      public long CallID
      { get; set; }

      public int Length
      { get { return Body.Length; } }

      public byte[] Tail
      { get; set; }

      public string Uri
      { get; set; }

      public virtual byte[] AllBuffers
      {
         get
         {
            using (StreamBuffer stream = new StreamBuffer())
            {
               try
               {
                  stream.Put(Header);
                  stream.Put(Command);
                  stream.Put(From);
                  stream.Put(To);
                  stream.Put(CallID);
                  stream.Put(Length);
                  stream.Put(Body);
                  stream.Put(Tail);
               }
               catch (Exception ex) { }
               return stream.ToByteArrays();
            }
         }
      }

      #endregion
   }
}
