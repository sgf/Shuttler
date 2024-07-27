using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public class Transaction
   {
      #region Fields

      private static IDebugger _debug = new Debugger<Transaction>(ArterySettings.Instance.Level);

      #endregion

      #region Methods

      public Transaction(IConnection conn, Request request, TransactionManager transactionManager)
      {
         ShuttlerConnection = conn;
         Request = request;
         TxManager = transactionManager;

         TxManager.Register(this);
      }

      public Transaction(Request request, TransactionManager transactionManager)
      {
         Request = request;
         TxManager = transactionManager;

         TxManager.Register(this);
      }

      public void SendRequest()
      {
         ShuttlerConnection.Send(Request);
      }

      public void SendRequest(Request request)
      {
         ShuttlerConnection.Send(request);
      }

      public void SendResponse(Response response)
      {
         ShuttlerConnection.Send(response);
      }

      internal void OnResponseReceived(ResponseEventArgs e)
      {
         ResponseRececived.OnEvent<ResponseEventArgs>(this, e);
      }

      internal void OnTransactionTimeOut()
      {
         TransactionTimeOut.OnEvent<EventArgs>(this,EventArgs.Empty);
      }

      void OnServerRequestReceived(object sender, StreamEventArgs e)
      {
         ResponseEventArgs response = new ResponseEventArgs();
         response.PacketContext = e.PacketContext;

         if (!TxManager.OnResponseReceived(response))
            _debug.Info("Transaction key not found!");
      }

      #endregion

      #region Attributes

      public Request InitialRequest
      {
         get { return Request; }
      }

      private TransactionManager TxManager
      { get; set; }

      private IConnection ShuttlerConnection
      { get; set; }

      private Request Request
      { get; set; }

      #endregion

      #region EventHandler

      public event EventHandler<ResponseEventArgs> ResponseRececived;
      public event EventHandler<EventArgs> TransactionTimeOut;

      #endregion
   }
}
