using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Shuttler.Artery
{
   public class TcpListener:IListener
   {
	 #region  Fields

	 private object _syncRoot = new object();
	 private const int _backLog = 10;

	 #endregion

	 #region Methods

	 public TcpListener(string remotEp)
	 {
	    EndPoint = Utility.IPEndPointParser(remotEp);
	    Args = new SocketAsyncEventArgs();
	    Args.Completed += BeginAcceptCallBack;
	 }

	 public void Listen()
	 {
	    lock(_syncRoot)
	    {
		  if (!IsRunning)
		  {
			ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			ListenerSocket.Bind(EndPoint);
			ListenerSocket.Listen(_backLog);
			BeginAccept(Args);
		  }
		  else
		  {
			ListenerSocket.Close();
			throw new InvalidOperationException("Listener socket is already running.");
		  }
	    }

	 }

	 private void BeginAccept(SocketAsyncEventArgs e)
	 {
	    Socket socket = ListenerSocket;
	    e.AcceptSocket = null;

	    bool async = socket.AcceptAsync(e);
	    if (!async)
		  BeginAcceptCallBack(socket, e);
	 }

      private void BeginAcceptCallBack(object sender, SocketAsyncEventArgs e)
      {
         Socket c = e.AcceptSocket;
         //c.NoDelay = true;
         e.AcceptSocket = null;

         EventHandler<ConnectionEventArgs> handler = OnShuttlerInComing;
         if (e.SocketError == SocketError.Success && handler != null)
         {
            SocketAsyncEventArgs args =ArgsPoolTuples.ReceiveArgsTuple.CheckOut();
            ArgsPoolTuples.ReceiveBufferTuple.CheckOut(args);
            args.AcceptSocket = c;
            IConnection conn = new ShuttlerConnection(args);
            //IConnection conn = new ShuttlerConnectionSlim(c);
            ConnectionEventArgs connectionArgs = new ConnectionEventArgs(c, conn);
            handler.OnEvent<ConnectionEventArgs>(this, connectionArgs);
         }

         BeginAccept(e);
      }

	 public void Stop()
	 {
	    if (ListenerSocket != null)
		  ListenerSocket.Close();
      }

      #endregion

      #region Attributes

      private SocketAsyncEventArgs Args
      { get; set; }

      private Socket ListenerSocket
      { get; set; }

      private IPEndPoint EndPoint
      { get; set; }

      public bool IsRunning
      {
         get { return ListenerSocket != null; }
      }

      #endregion

      #region EventHandler

      public event EventHandler<ConnectionEventArgs> OnShuttlerInComing;

      #endregion
   }
}
