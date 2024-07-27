/*
 * Merge the piece's buffer for Tcp Stream.
 * Author:overred
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Shuttler.Artery
{
   public class PacketParser
   {
      #region Fields

      private static IDebugger _debug = new Debugger<PacketParser>(Level.Error);
      private object _syncRoot = new object();
      private int _bufferSize = 1024 * 1024; //1M
      private double _threshold = 0.9;
      private int _currentIndex;
      private byte[] _buffers;

      #endregion

      #region Methods

      public PacketParser()
      {
         _buffers = new byte[_bufferSize];
      }

      public void CheckPacket(byte[] buffers, Action<PacketBase> action)
      {
         lock (_syncRoot)
         {
            var buffersTemp = buffers.BufferTrimFront();
            if (buffersTemp.Length == 0) return;

            using (StreamBuffer reader = new StreamBuffer(buffersTemp))
            {
               Packet packet = new Packet();
               if (packet.Read(reader))
               {
                  Request request = new Request(packet);
                  action.BeginInvoke(request, null, null);

                  byte[] remain = reader.ToByteArray();
                  remain = remain.BufferTrimFront();

                  if (remain.Length == 0 || _currentIndex > _bufferSize * _threshold)
                  {
                     if (_currentIndex >  0)
                        PacketsArray(action);
                     return;
                  }

                  if (remain[0] == Global.TOAD_HEADER[0])
                     AppendPacket(remain);
                  return;
               }
               AppendPacket(buffersTemp);
            }
         }
      }

      private void AppendPacket(byte[] buffers)
      {

         int len = buffers.Length;
         if (_currentIndex+len > _bufferSize)
            throw new InvalidOperationException("More deformity Exit!! check your Net!!!");

         Buffer.BlockCopy(buffers, 0, _buffers, _currentIndex, len);
         Interlocked.Add(ref _currentIndex, len);
      }

      private void PacketsArray(Action<PacketBase> action)
      {
         var dataTrimFirst = _buffers.BufferTrimFront().BufferTrimEnd();
         if (dataTrimFirst.Length == 0) return;

         using (StreamBuffer stream = new StreamBuffer(dataTrimFirst))
         {
            int len = dataTrimFirst.Length;
            while (stream.Position < len - 1)
            {
               try
               {
                  if ((dataTrimFirst[stream.Position] == 0x00))
                  {
                     stream.Get();
                     continue;
                  }

                  Packet packet = new Packet();
                  if (!packet.Read(stream))
                  {
                     ArteryPerfCounter.Instance.NumberOfParserError.Increment();
                     stream.Get();
                     continue;
                  }

                  Request request = new Request(packet);
                  if (action != null)
                     action.BeginInvoke(request, null, null);
               }
               catch (Exception ex)
               {
                  _debug.Error(ex, "PacketParser::PacketsArray Error!!!");
               }
            }
         }
         _currentIndex = 0;
         //_buffers = new byte[_bufferSize];
         Array.Clear(_buffers, 0, _bufferSize);
      }

      #endregion
   }
}
