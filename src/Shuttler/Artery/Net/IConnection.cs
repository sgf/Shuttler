using System;
using System.IO;
using System.Collections.Generic;

namespace Shuttler.Artery
{
    public interface IConnection
    {
        string Key { get; }
        void Send(PacketBase packet);
        void Close();
        event EventHandler<StreamEventArgs> OnInnerRequestReceived;
        event EventHandler<ConnectionEventArgs> OnInnerDisconnected;
    }
}
