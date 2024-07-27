using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Shuttler.Artery
{
    public class ShuttlerAgentManager
    {
        #region Fields

        private static readonly int InitCapacity = 1000;
        private static object _syncRoot = new object();

        private Dictionary<string, ShuttlerAgent> _shuttlerAgents;
        private IListener _listener;

        #endregion

        #region Methods

        public ShuttlerAgentManager(IListener listener)
        {
            try
            {
                _shuttlerAgents = new Dictionary<string, ShuttlerAgent>(InitCapacity);
                _listener = listener;
                _listener.OnShuttlerInComing += (sender, e) =>
                {
                    ConnectionEventArgs args = e;
                    if (OnShuttlerCreated != null && args.ShuttlerConnection != null)
                    {
                        ShuttlerEventArgs shuttlerArgs = new ShuttlerEventArgs(CreateShuttlerAgent(e.ShuttlerConnection), e.Socket.RemoteEndPoint.ToString());
                        OnShuttlerCreated.OnEvent<ShuttlerEventArgs>(this, shuttlerArgs);
                    }
                };
            }
            catch
            {
                throw new InvalidOperationException("Config error!");
            }
        }

        public ShuttlerAgent CreateShuttlerAgent(IConnection conn)
        {
            ShuttlerAgent shuttlerAgent = new ShuttlerAgent(this, conn);
            return shuttlerAgent;
        }


        private void TransactionCreated(object sender, TransationEventArgs e)
        {
            //OnTransactionCreated.OnEvent<TransationEventArgs>(this,e);
            if (OnTransactionCreated != null)
                OnTransactionCreated(this, e);
        }

        internal void Register(ShuttlerAgent shuttlerAgent)
        {
            lock (_syncRoot)
            {
                string agentHash = shuttlerAgent.ToString();
                ShuttlerAgent shuttler;
                if (!_shuttlerAgents.TryGetValue(agentHash, out shuttler))
                {
                    _shuttlerAgents.Add(agentHash, shuttlerAgent);

                    if (OnTransactionCreated != null)
                        shuttlerAgent.OnTransactionCreated += TransactionCreated;
                }
            }
        }

        protected void Find(string key)
        {
        }

        internal void UnRegister(ShuttlerAgent shuttlerAgent)
        {
            lock (_syncRoot)
            {
                string agentHash = shuttlerAgent.ToString();
                if (_shuttlerAgents.TryGetValue(agentHash, out shuttlerAgent))
                {
                    _shuttlerAgents.Remove(agentHash);
                    if (OnTransactionCreated != null)
                        shuttlerAgent.OnTransactionCreated -= OnTransactionCreated;
                }
            }
        }

        #endregion

        #region EventHandler

        public event EventHandler<TransationEventArgs> OnTransactionCreated;
        public event EventHandler<ShuttlerEventArgs> OnShuttlerCreated;

        #endregion

    }
}
