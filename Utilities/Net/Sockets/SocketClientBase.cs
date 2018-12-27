using System;
using System.Net;
using System.Net.Sockets;

namespace Utilities.Net.Sockets
{
    public abstract class SocketClientBase : SocketBase, IDisposable
    {
        public abstract Socket Client { get; }

        public abstract Boolean Connect(IPEndPoint serverEndPoint);
        public abstract Boolean Connect(IPAddress serverAddress, Int32 serverPort);
        public abstract Boolean Connect(String serverNameOrAddress, Int32 serverPort);
        public abstract Boolean Connect();

        public abstract void ConnectAsync(IPEndPoint serverEndPoint);
        public abstract void ConnectAsync(IPAddress serverAddress, Int32 serverPort);
        public abstract void ConnectAsync(String serverNameOrAddress, Int32 serverPort);
        public abstract void ConnectAsync();

        public abstract void Disconnect();
        public abstract void SendData(Byte[] data);

        public abstract void Dispose();
    }
}
