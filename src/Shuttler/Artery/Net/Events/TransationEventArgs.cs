using System;
using System.Collections.Generic;
using System.Net;

namespace Shuttler.Artery
{
    [Serializable]
    public class TransationEventArgs : EventArgs
    {
        public TransationEventArgs(ShuttlerAgent shuttlerAgent, Transaction tran)
        {
            ShuttlerAgent = shuttlerAgent;
            Transaction = tran;
        }

        public Transaction Transaction { get; private set; }
        public ShuttlerAgent ShuttlerAgent { get; private set; }

    }

}
