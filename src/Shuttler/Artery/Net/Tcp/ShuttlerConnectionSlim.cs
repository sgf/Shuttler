using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Shuttler.Artery
{
   public class ShuttlerConnectionSlim : IConnection
   {
      #region Fields

      private static IDebugger _debug = new Debugger<ShuttlerConnectionSlim>(ArterySettings.Instance.Level);
      private byte[] _receiveBuf = new byte[1024];
      private PacketParser _packetParser;
      private Socket _socket;

      #endregion

      #region Methods 

      internal ShuttlerConnectionSlim(Socket socket)
      {
         _packetParser = new PacketParser();
         _socket = socket;
         _socket.NoDelay = true;

         Key = string.Format("tcp:{0}", _socket.RemoteEndPoint.ToString());
         _debug.InfoFormat("ShuttlerConnection come,key:{0}", Key);

         ListenerAsync();
      }

      private void ListenerAsync()
      {
         try
         {
           _socket.BeginReceive(_receiveBuf, 0, _receiveBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveAsync), _socket);
         }
         catch (Exception ex)
         {
            _debug.Error(ex, "ShuttlerConnectionSlim::ListenerAsync");
         }
      }

      private void ReceiveAsync(IAsyncResult ar)
      {
         try
         {
            Socket socket = (Socket)ar.AsyncState;
            int cnt = socket.EndReceive(ar);
            if (cnt > 0)
            {
               byte[] data = _receiveBuf;
               _packetParser.CheckPacket(data, (packetContext) =>
               {
                  OnInnerRequestReceived.OnEvent<StreamEventArgs>(this, new StreamEventArgs(this, packetContext));
               });
            }
            else
               Thread.Sleep(0);
            ListenerAsync();
         }
         catch (Exception ex)
         {
            _debug.Error(ex, "ShuttlerConnectionSlim::ReceiveAsync");
         }
      }

      #region Impls

      public void Send(PacketBase packet)
      {
         try
         {
            if (_socket.Connected)
            {
               byte[] msg = packet.AllBuffers;
               _socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(EndSendData), _socket);
            }
            else
               throw new InvalidOperationException("Send Error,Socket closed!");
         }
         catch (Exception ex)
         {
            _debug.Error(ex, "ShuttlerConnectionSlim::Send");
         }
      }

      private void EndSendData(IAsyncResult ar)
      {

      }

      public void Close()
      {
         try
         {
            if (_socket.Connected)
               _socket.Close();
         }
         catch (Exception ex)
         {
            _debug.Error(ex, "ShuttlerConnectionSlim::Close");
         }
      }

      public string Key
      { get; private set; }

      #region EventHandlers

      public event EventHandler<StreamEventArgs> OnInnerRequestReceived;
      public event EventHandler<ConnectionEventArgs> OnInnerDisconnected;

      #endregion

      #endregion

      #endregion
   }
}
