using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ProtoBuf;
using System.IO;

namespace Shuttler.Artery
{
   public class TcpRpcClientTransport : IClientTransport
   {
      #region Fields

      private static IDebugger _debug = new Debugger<TcpRpcClientTransport>(ArterySettings.Instance.Level);
      private ShuttlerAgent _agent;
      private Timer _healthCheck;
      private long _callID = 0;

      private string _uri;

      #endregion

      #region Methods

      public TcpRpcClientTransport(ShuttlerArtery artery,string uri)
      {
         _uri = uri;
         HealthCheck(artery);
         //_healthCheck = new Timer(new TimerCallback(HealthCheck), artery, 1000 * 100, 1000 * 100);
      }

      private void HealthCheck(object obj)
      {
         IConnection conntemp = ConnectionManager.Instance.Find(_uri);
         if (conntemp != null) return;

         conntemp=ConnectionManager.Instance.CreateNewConnection(_uri);
         ShuttlerArtery artery = obj as ShuttlerArtery;
         _agent = artery.ShuttlerAgentManager.CreateShuttlerAgent(conntemp);
         _agent.OnTransactionCreated += (sender, e) => { };
      }

      public void BeginInvoke<TArgs>(string method, TArgs args, Action<RpcContext<TArgs>> action)
      {
         try
         {
             RpcContext<TArgs> context = new RpcContext<TArgs>();
             context.Value = args;

            Request request = RequestAssemble<TArgs>(method,context);
            Transaction tran = _agent.CreateTransaction(request);
            tran.ResponseRececived += (sender, e) =>
            {
               if (action != null)
               {
                   RpcContext<TArgs> innerContext = SerializerExt.DeserializeBytesToT<RpcContext<TArgs>>(e.PacketContext.Body.BufferTrimEnd());
                   if (innerContext == default(RpcContext<TArgs>))
                  {
                     _debug.Error("DeserializeBytesToT Faild!!" + ByteUtil.BytesToHex(e.PacketContext.Body));
                     action = null;
                     return;
                  }
                  if (!string.IsNullOrEmpty(innerContext.RpcException))
                     _debug.Error(innerContext.RpcException);
                  action.BeginInvoke(innerContext, null, null);
               }
            };
            tran.SendRequest();
         }
         finally
         {
            ArteryPerfCounter.Instance.RateOfRpcClientInvoke.Increment();
         }
      }

      private Request RequestAssemble<TArgs>(string method, RpcContext<TArgs> context)
      {
         Request request = new Request();
         request.Header = Global.TOAD_HEADER;

         byte[] bytes = new byte[Global.TOAD_TOADIDCHARS_LENGTH];
         request.From = bytes;
         request.To = bytes;
         request.Command = 0x002;
         request.CallID = Interlocked.Increment(ref _callID)&0x7FFFFFF;
         request.Uri = _uri;
         context.MethodName = method;

         request.Body = SerializerExt.SerializerStreamToBytes<RpcContext<TArgs>>(context);
         request.Tail = Global.TOAD_TAIL;
         return request;
      }

      public void Dispose()
      { }

      #endregion

   }
}
