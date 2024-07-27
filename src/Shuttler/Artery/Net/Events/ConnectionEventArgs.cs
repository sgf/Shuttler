using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Shuttler.Artery
{
    [Serializable]
    public class ConnectionEventArgs : EventArgs
    {

        public ConnectionEventArgs(Socket socket, IConnection shuttlerConnection)
        {
            Socket = socket;
            ShuttlerConnection = shuttlerConnection;
        }

        public Socket Socket { get; private set; }
        public IConnection ShuttlerConnection { get; private set; }
    }

}
