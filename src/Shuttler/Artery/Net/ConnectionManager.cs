using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public class ConnectionManager
   {
      #region Fields

      public readonly static ConnectionManager Instance = new ConnectionManager();
	 private static readonly int INIT_CAPACTIY = 1000;
	 private static object _syncRoot = new object();

      #endregion

      #region Methods

      static ConnectionManager()
	 {
	    Connections = new Dictionary<string, IConnection>(INIT_CAPACTIY);
	 }

      /// <summary>
      /// Create new channel from local-->server
      /// </summary>
      /// <param name="uri"></param>
      /// <returns></returns>
      public IConnection CreateNewConnection(string uri)
      {
         SocketAsyncEventArgs args = ArgsPoolTuples.ReceiveArgsTuple.CheckOut();
         ArgsPoolTuples.ReceiveBufferTuple.CheckOut(args);

         Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
         //socket.NoDelay = true;
         socket.Connect(Utility.IPEndPointParser(uri));
         args.AcceptSocket = socket;

         IConnection conntemp = new ShuttlerConnection(args);
         Register(conntemp);
         return conntemp;
      }

	 public void Register(IConnection connection)
	 {
	    if (!Connections.ContainsKey(connection.Key))
		  lock (_syncRoot) Connections.Add(connection.Key,connection);
	 }

	 public IConnection  Find(string key)
	 {
	    if (Connections.ContainsKey(key))
		  return Connections[key];
	    return null;
	 }

	 public void UnRegister(IConnection connection)
	 {
	    if (Connections.ContainsKey(connection.Key))
		  lock (_syncRoot) Connections.Remove(connection.Key);
      }

      #endregion

      #region Attributes

      private static Dictionary<string, IConnection> Connections
      { get; set; }

      #endregion
   }
}
