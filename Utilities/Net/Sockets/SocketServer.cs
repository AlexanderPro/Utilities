using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Utilities.Net.Sockets
{
    public class SocketServer : SocketServerBase
    {
        #region Fields.Private

        private TcpListener _listener = null;
        private Thread _acceptThread = null;
        private ConcurrentBag<SocketClientBase> _socketClients;
        private Boolean _disposed = false;

        #endregion


        #region Properties.Public

        /// <summary>
        /// Gets the TCP local socket.
        /// </summary>
        public override Socket Server { get { return _listener.Server; } }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        public override IPEndPoint LocalEndPoint { get; protected set; }

        /// <summary>
        /// Gets the collecton of socket clients
        /// </summary>
        public override IList<SocketClientBase> SocketClients { get { return _socketClients.ToList(); } }

        /// <summary>
        /// Gets a value indicating whether server is started.
        /// </summary>
        public Boolean IsStarted { get; private set; }

        #endregion


        #region Methods.Public

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class with specified local point.
        /// </summary>
        /// <param name="localEndPoint">The <see cref="IPEndPoint"/> class.</param>
        public SocketServer(IPEndPoint localEndPoint)
        {
            LocalEndPoint = localEndPoint;
            _listener = new TcpListener(localEndPoint);
            _socketClients = new ConcurrentBag<SocketClientBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class with specified ip address and port.
        /// </summary>
        /// <param name="localIP">The string ip address of local host.</param>
        /// <param name="localPort">The port of local host.</param>
        public SocketServer(String localIP, Int32 localPort)
            : this(new IPEndPoint(IPAddress.Parse(localIP), localPort))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class with specified ip address and port.
        /// </summary>
        /// <param name="localIP">The ip address of local host.</param>
        /// <param name="localPort">The port of local host.</param>
        public SocketServer(IPAddress localIP, Int32 localPort)
            : this(new IPEndPoint(localIP, localPort))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class with specified port.
        /// </summary>
        /// <param name="port">The port of local host.</param>
        public SocketServer(Int32 port)
            : this(new IPEndPoint(IPAddress.Any, port))
        {
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketServer"/> class. It is called by GC 
        /// befor the instance will delete from manage heap.
        /// </summary>
        ~SocketServer()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketClient"/> class. This method is called by 
        /// programmist as distinct from destructor which is called by GC.
        /// </summary>
        public override void Dispose()
        {
            //Говорим сборщику муссора, что деструктор для объекта данного класса вызывать не нужно
            GC.SuppressFinalize(this);

            Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketClient"/> class. This method is called by 
        /// programmist as distinct from destructor which is called by GC.
        /// </summary>
        /// <param name="disposing">The value indicating whether do disposing.</param>
        public void Dispose(Boolean disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();
                        _listener.Server.Close();
                    }
                    catch
                    {
                    }
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Runs server with specified befor IP and Port.
        /// </summary>
        public override void Start()
        {
            if (IsStarted) return;

            _acceptThread = new Thread(AcceptConnections);
            _acceptThread.IsBackground = true;
            _acceptThread.Start();
            IsStarted = true;
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public override void Stop()
        {
            if (!IsStarted) return;

            _listener.Stop();
            if (_acceptThread != null && _acceptThread.IsAlive)
            {
                _acceptThread.Interrupt();
                _acceptThread.Join();
            }

            IsStarted = false;
            while (SocketClients.Count > 0)
            {
                SocketClientBase socketClient = SocketClients[0];
                socketClient.Disconnect();
                if (SocketClients.Contains(socketClient))
                {
                    SocketClients.Remove(socketClient);
                }
            }
        }

        /// <summary>
        /// Sends data to all clients of this server.
        /// </summary>
        /// <param name="data">An array of type Byte that contains the data to be sent.</param>
        public override void SendAllClients(Byte[] data)
        {
            foreach (SocketClientBase socketClient in SocketClients)
            {
                if (!((SocketClient)socketClient).SocketDisposed)
                {
                    socketClient.SendData(data);
                }
            }
        }

        /// <summary>
        /// Allows to find <see cref="ISocket"/> in the collection of clients.
        /// </summary>
        /// <param name="clientEndPoint">The <see cref="IPEndPoint"/> of seing client.</param>
        /// <returns>The <see cref="SocketClient"/> class.</returns>
        public override SocketClientBase FindClient(IPEndPoint clientEndPoint)
        {
            foreach (SocketClientBase socketClient in SocketClients)
            {
                var remoteEndPoint = (IPEndPoint)socketClient.Client.RemoteEndPoint;
                if (remoteEndPoint.Port == clientEndPoint.Port && remoteEndPoint.Address.ToString() == clientEndPoint.Address.ToString())
                {
                    return socketClient;
                }
            }
            return null;
        }

        #endregion


        #region Methods.Private

        /// <summary>
        /// Accepts clients in the separate thread.
        /// </summary>
        private void AcceptConnections()
        {
            try
            {
                _listener.Start();
                while (true)
                {
                    Socket socket = _listener.AcceptSocket();  //Waiting
                    var client = new SocketClient(socket);
                    client.Context = this.Context;
                    client.Connected += (s, e) => { OnConnected(e); };
                    client.Disconnected += SocketClientDisconnected;
                    client.ReceivedData += (s, e) => { OnReceivedData(e); };
                    client.SentData += (s, e) => { OnSentData(e); };
                    client.Error += (s, e) => { OnError(e); };
                    client.SocketError += (s, e) => { OnSocketError(e); };
                    SocketClients.Add(client);
                    client.BeginReceive();
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10004)
                {
                    OnSocketError(new SocketErrorEventArgs(LocalEndPoint, e));
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(LocalEndPoint, e));
            }
        }

        private void SocketClientDisconnected(object sender, SocketEventArgs e)
        {
            var socketClient = sender as SocketClientBase;
            if (socketClient != null && SocketClients.Contains(socketClient))
            {
                SocketClients.Remove(socketClient);
            }
            OnDisconnected(e);
        }

        #endregion
    }
}
