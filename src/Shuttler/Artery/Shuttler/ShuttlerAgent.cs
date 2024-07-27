using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public class ShuttlerAgent
   {
      #region Fields

      private static IDebugger _debug = new Debugger<ShuttlerAgent>(ArterySettings.Instance.Level);

      #endregion

      #region  Methods

      public ShuttlerAgent(ShuttlerAgentManager manager, IConnection connection)
      {
         TxManager = new TransactionManager();
         //--------shuttlerconnection--------------------
         ShuttlerConnection = connection;
         ShuttlerConnection.OnInnerRequestReceived += OnRequestReceived;
         ShuttlerConnection.OnInnerDisconnected += Disconnected;
         //--------agentmanager--------------------
         AgentManager = manager;
         AgentManager.Register(this);
      }

      public void BeginCreateTransaction(Request request, Action<Transaction> action)
      {
         //Server connection
         if (!string.IsNullOrEmpty(request.Uri))
         {
            IConnection conntemp = ConnectionManager.Instance.Find(request.Uri);
            if (conntemp == null)
            {
               conntemp = conntemp == null ? ConnectionManager.Instance.CreateNewConnection(request.Uri) : conntemp;
               conntemp.OnInnerRequestReceived += OnRequestReceived;
               conntemp.OnInnerDisconnected += Disconnected;
               action.BeginInvoke(new Transaction(conntemp, request, TxManager),null,null);
            }
            action.BeginInvoke(new Transaction(conntemp, request, TxManager),null,null);
         }
         action.BeginInvoke(new Transaction(ShuttlerConnection, request, TxManager),null,null); ;
      }

      public Transaction CreateTransaction(Request request)
      {
         //Server connection
         if (!string.IsNullOrEmpty(request.Uri)) {
            IConnection conntemp = ConnectionManager.Instance.Find(request.Uri);
            if (conntemp == null) {
               conntemp = conntemp == null ? ConnectionManager.Instance.CreateNewConnection(request.Uri) : conntemp;
               conntemp.OnInnerRequestReceived += OnRequestReceived;
               conntemp.OnInnerDisconnected += Disconnected;
               return new Transaction(conntemp, request, TxManager);
            }
            return new Transaction(conntemp, request, TxManager);
         }

         return new Transaction(ShuttlerConnection, request, TxManager);
      }

      private void OnRequestReceived(object sender, StreamEventArgs e)
      {
         EventHandler<TransationEventArgs> handler = OnTransactionCreated;
         if (handler != null)
         {

            Request request = e.PacketContext as Request;
            /*
             * If the CallId non-found that :
             * Transaction via the server(server mode)
             * The Client will be Raise event:OnTransactionCreated
             */
            if (!TxManager.Find(request.CallID))
            {
               Transaction tran = new Transaction(ShuttlerConnection, request, TxManager);
               TransationEventArgs txe = new TransationEventArgs(this, tran);
               handler(this, txe);
            }
            /*
            * If the CallId found that :
            * Transaction via the Client
            * The Client will be Raise event:ResponseRececived
            */
            else
            {
               ResponseEventArgs response = new ResponseEventArgs();
               response.PacketContext = request;
               TxManager.OnResponseReceived(response);
            }
         }
         else
            e.ShuttlerConnection.Close();
      }

      private void Disconnected(object sender, ConnectionEventArgs e)
      {
         AgentManager.UnRegister(this);
         OnDisconnected.OnEvent<ConnectionEventArgs>(this,e);
      }

      public override string ToString()
      {
         return GetHashCode().ToString();
      }

      #endregion

      #region Attributes

      private TransactionManager TxManager
      { get; set; }

      private ShuttlerAgentManager AgentManager
      { get; set; }

      private IConnection ShuttlerConnection
      { get; set; }

      #endregion

      #region EventHandler

      public event EventHandler<TransationEventArgs> OnTransactionCreated;
      public event EventHandler<ConnectionEventArgs> OnDisconnected;

      #endregion

   }
}
