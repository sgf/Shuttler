using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
namespace Shuttler.Artery
{
   public class Debugger<T> : IDebugger
   {
      #region Fields

      private DebuggerStack<string> _queue = new DebuggerStack<string>();
      private const string DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
      private int MaxQueuedCount = 1024 * 16;
      private int MaxWriteCount = 16;
      private object _type;

      #endregion

      #region Constructor

      public Debugger(Level level)
      {
         Level = level;
         _type = typeof(T);
      }

      #endregion

      #region Methods

      internal void Debug(string message)
      {
         Console.WriteLine(message);
         _queue.Push(message);

         var count = _queue.Count;

         if (count > MaxQueuedCount) return;
         if (count > MaxWriteCount)
            Proc(null);

      }
      private void Proc(object obj)
      {
         StringBuilder sb = new StringBuilder(1024);
         var i = 0;
            while (i++ <= MaxWriteCount)
               sb.AppendLine(_queue.Pop());

         string path = DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt";
         try
         {
            using (FileStream fs = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
               using (StreamWriter sw = new StreamWriter(fs))
                  sw.Write(sb.ToString());
            }
         }
         catch (IOException) { }
      }


      public void Info(string message)
      {
         if (Level < Level.Info) return;
         Write(Level.Info, message);
      }

      public void Info(PacketBase packet)
      {
         if (Level < Level.Info) return;

         Write(Level.Info, AppendMessage(packet));
      }

      public void Info(PacketBase packet,string desc)
      {
         if (Level < Level.Info) return;
         Write(Level.Info, desc);
         Write(Level.Info, AppendMessage(packet));
      }

      public void Error(string message)
      {
         if (Level < Level.Error) return;
         Write(Level.Error, message);
      }

      public void Error(Exception ex, string message)
      {
         if (Level < Level.Error) return;
         Write(Level.Error, message+"\r\n"+ex.Message +"\r\n"+ ex.StackTrace);
      }


      public void ErrorFormat(Exception ex, string format, params object[] args)
      {
         if (Level < Level.Error) return;
         Write(Level.Error, string.Format(format, args) + ex.StackTrace);
      }

      public void InfoFormat(string format, params object[] args)
      {
         if (Level < Level.Info) return;
         Write(Level.Info, string.Format(format, args));

      }

      private void Write(Level lv, string msg)
      {
         var line = string.Format("\r\n{0}:\r\n-----------------------------------------------\r\n{1}  [{2}]:\r\n {3}\r\n-----------------------------------------------\r\n",
             _type.ToString(),
             DateTime.Now.ToString(DateTimeFormat),
             lv,
             msg);
         Debug(line);
      }

      private string AppendMessage(PacketBase stream)
      {
         if (stream == null) return string.Empty;

         StringBuilder message = new StringBuilder(1024 * 10);
         if (stream.Header != null)
            message.AppendFormat("Header<{0}bytes>: {1}\r\n", stream.Header.Length,ByteUtil.BytesToHex(stream.Header));

            message.AppendFormat("Command<16bytes>: {0}\r\n",stream.Command);

         if (stream.From != null)
            message.AppendFormat("From<{0}bytes>: {1}\r\n", stream.From.Length, ByteUtil.BytesToHex(stream.From));

         if (stream.To != null)
            message.AppendFormat("To<{0}bytes>: {1}\r\n", stream.To.Length, ByteUtil.BytesToHex(stream.To));

            message.AppendFormat("CallID<64bytes>: {0}\r\n", stream.CallID);
            message.AppendFormat("Length<32bytes>: {0}\r\n", stream.Length);

         if (stream.Body != null)
            message.AppendFormat("Body<{0}bytes>: {1}\r\n", stream.Body.Length, ByteUtil.BytesToHex(stream.Body));

         message.AppendFormat("Tail<1bytes>: {0}\r\n", ByteUtil.BytesToHex(stream.Tail));

         return message.ToString();
      }

      #endregion

      public Level Level { get; set; }
   }

  [Flags]
  public enum Level : int
  {
     NO=3,
     Info = 2,
     Warn = 1,
     Error = 0
  }
}
