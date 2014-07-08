using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Utilities.Net.Sockets
{
    public abstract class SocketServerBase : SocketBase, IDisposable
    {
        public abstract Socket Server { get; }
        public abstract IPEndPoint LocalEndPoint { get; protected set; }
        public abstract IList<SocketClientBase> SocketClients { get; }

        public abstract void Start();
        public abstract void Stop();
        public abstract void SendAllClients(Byte[] data);
        public abstract SocketClientBase FindClient(IPEndPoint clientAddress);
        public abstract void Dispose();
    }
}