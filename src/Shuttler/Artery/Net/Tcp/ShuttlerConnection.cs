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
   public class ShuttlerConnection : IConnection
   {
      #region Fields

      private static IDebugger _debug = new Debugger<ShuttlerConnection>(ArterySettings.Instance.Level);
      private object _receivedRoot = new object();
      private object _sendRoot = new object();
      private PacketParser _packetParser;

      #endregion

      #region Methods

      internal ShuttlerConnection(SocketAsyncEventArgs args)
      {
         _packetParser = new PacketParser();

         AcceptSocket = args.AcceptSocket;
         //AcceptSocket.NoDelay = true;
         Key = string.Format("tcp:{0}", AcceptSocket.RemoteEndPoint.ToString());
         _debug.InfoFormat("ShuttlerConnection come,key:{0}", Key);

         ConnectionArgs = new ConnectionEventArgs(AcceptSocket, this);
         TxManager = new TransactionManager();

         Args = args;
         Args.Completed += ReceivedCompleted;

         ListenForData(Args);
      }


      private void ListenForData(SocketAsyncEventArgs args)
      {
         var socket = AcceptSocket;
         if (socket.Connected) {
            bool asyncReceive = false;
            using (ExecutionContext.SuppressFlow()) asyncReceive=socket.ReceiveAsync(Args);

            if (!asyncReceive)
               ReceivedCompleted(this, Args);
         }
      }

      private void ReceivedCompleted(object sender, SocketAsyncEventArgs args)
      {
         switch (args.LastOperation) {
            case (SocketAsyncOperation.Receive):
               ProcessReceived(args);
               break;
         }
      }

      private void ProcessReceived(SocketAsyncEventArgs args)
      {
         int offset = 0;

         lock (_receivedRoot)
         {

           int transLen = args.BytesTransferred;
            if (transLen == 0 || args.SocketError != SocketError.Success)
            {
               Close(); return;
            }

            try
            {
                offset = args.Offset;
               Byte[] data = new Byte[transLen];
               Buffer.BlockCopy(args.Buffer, offset, data, 0, data.Length);

               if (AcceptSocket.Connected)
               {
                   _packetParser.CheckPacket(data, (packetContext) =>
                   {
                       OnInnerRequestReceived.OnEvent<StreamEventArgs>(this, new StreamEventArgs(this, packetContext));
                   });

                  Array.Clear(args.Buffer, args.Offset, transLen);
                  ListenForData(args);
               }
            }
            catch (Exception ex)
            {
               //Close();
               _debug.ErrorFormat(ex, "Received **ERROR**");
            }
         }
      }

      public void Close()
      {
         var s = AcceptSocket;
         try {
            s.Shutdown(SocketShutdown.Both);
         }
         catch (SocketException ex) {
            _debug.ErrorFormat(ex, "Close **ERROR**");
         }
         s.Close();
         s = null;

         Args.Completed -= ReceivedCompleted;
         ConnectionManager.Instance.UnRegister(this);

         ArgsPoolTuples.ReceiveArgsTuple.CheckIn(Args);
         ArgsPoolTuples.ReceiveBufferTuple.CheckIn(Args);

         OnInnerDisconnected.OnEvent<ConnectionEventArgs>(this, ConnectionArgs);
         ArteryPerfCounter.Instance.RateOfDisConnected.Increment();
      }

      public void Send(PacketBase packet)
      {
         SendStream(packet.AllBuffers);
      }

      private void SendStream(byte[] buffer)
      {
         SocketAsyncEventArgs _sendArgs = null;

         try
         {
            if (AcceptSocket == null) return;
            _sendArgs = ArgsPoolTuples.SendArgsTuple.CheckOut();
            ArgsPoolTuples.SendBufferTuple.CheckOut(_sendArgs);

            lock (_sendRoot)
            {
              Buffer.BlockCopy(buffer, 0, _sendArgs.Buffer, _sendArgs.Offset, buffer.Length);
              if (AcceptSocket.Connected)
              {
                 AcceptSocket.SendAsync(_sendArgs);
                 Array.Clear(_sendArgs.Buffer, _sendArgs.Offset, buffer.Length);
              }
            }
         }
         catch (Exception ex)
         {
            _debug.ErrorFormat(ex, "ShuttlerConnection::SendStream Error!!!");
         }
         finally
         {
            _sendArgs.Dispose();
            _sendArgs = new SocketAsyncEventArgs();

            ArgsPoolTuples.SendArgsTuple.CheckIn(_sendArgs);
            ArgsPoolTuples.SendBufferTuple.CheckIn(_sendArgs);
         }
      }

      #endregion

      #region Attribute

      private ConnectionEventArgs ConnectionArgs
      { get; set; }

      private TransactionManager TxManager
      { get; set; }

      private SocketAsyncEventArgs Args
      { get; set; }

      private Socket AcceptSocket
      { get; set; }

      public string Key
      { get; private set; }

      #endregion

      #region EventHandler

      public event EventHandler<StreamEventArgs> OnInnerRequestReceived;
      public event EventHandler<ConnectionEventArgs> OnInnerDisconnected;

      #endregion
   }
}
