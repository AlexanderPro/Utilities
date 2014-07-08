﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace Utilities.Net.Sockets
{
    /// <summary>
    /// Implements the socket server interface. It operates with TcpListener and socket client collection.
    /// </summary>
    public class SocketServerSslAsync : SocketServerBase
    {
        #region Fields.Private

        private TcpListener _listener = null;
        private SynchronizedCollection<SocketClientBase> _socketClients;
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
        public override IList<SocketClientBase> SocketClients { get { return _socketClients; } }

        /// <summary>
        /// Gets the status of server.
        /// </summary>
        public SocketServerStatus ServerStatus { get; private set; }

        /// <summary>
        /// A Boolean value that specifies whether the client must supply a certificate for authentication.
        /// </summary>
        public Boolean ClientCertificateRequired { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the security protocol used to authenticate this connection. 
        /// </summary>
        public SslProtocols SslProtocol { get; set; }

        /// <summary>
        /// A Boolean value that specifies whether the certificate revocation list is checked during authentication.
        /// </summary>
        public Boolean CheckCertificateRevocation { get; set; }

        /// <summary>
        /// Responsible for validating the certificate supplied by the remote party.
        /// </summary>
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

        /// <summary>
        /// The X509Certificate used to authenticate the server.
        /// </summary>
        public X509Certificate ServerCertificate { get; set; }


        #endregion


        #region Methods.Public

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServerAsync"/> class with specified local point.
        /// </summary>
        /// <param name="localEndPoint">The <see cref="IPEndPoint"/> class.</param>
        public SocketServerSslAsync(IPEndPoint localEndPoint)
        {
            LocalEndPoint = localEndPoint;
            _listener = new TcpListener(localEndPoint);
            _socketClients = new SynchronizedCollection<SocketClientBase>();
            ServerStatus = SocketServerStatus.Stop;
            SslProtocol = SslProtocols.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServerAsync"/> class with specified ip address and port.
        /// </summary>
        /// <param name="localIP">The string ip address of local host.</param>
        /// <param name="localPort">The port of local host.</param>
        public SocketServerSslAsync(String localIP, Int32 localPort) : this(new IPEndPoint(IPAddress.Parse(localIP), localPort))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServerAsync"/> class with specified ip address and port.
        /// </summary>
        /// <param name="localIP">The ip address of local host.</param>
        /// <param name="localPort">The port of local host.</param>
        public SocketServerSslAsync(IPAddress localIP, Int32 localPort) : this(new IPEndPoint(localIP, localPort))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServerAsync"/> class with specified port.
        /// </summary>
        /// <param name="port">The port of local host.</param>
        public SocketServerSslAsync(Int32 localPort) : this(new IPEndPoint(IPAddress.Any, localPort))
        {
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketServerAsync"/> class. It is called by GC 
        /// befor the instance will delete from manage heap.
        /// </summary>
        ~SocketServerSslAsync()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketClient"/> class. This method is called by 
        /// programmist as distinct from destructor which is called by GC.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //Говорим сборщику муссора, что деструктор для объекта данного класса вызывать не нужно
            GC.SuppressFinalize(this);
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
            if (ServerStatus != SocketServerStatus.Stop) return;

            ServerStatus = SocketServerStatus.Start;
            AcceptConnections();
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public override void Stop()
        {
            if (ServerStatus == SocketServerStatus.Stop) return;

            ServerStatus = SocketServerStatus.Stop;
            _listener.Stop();
            while (SocketClients.Count > 0)
            {
                SocketClientSsl socketClient = (SocketClientSsl)SocketClients[0];
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
                if (!((SocketClientSsl)socketClient).SocketDisposed)
                {
                    socketClient.SendData(data);
                }
            }
        }

        /// <summary>
        /// Allows to find <see cref="SocketClient"/> in the collection of clients.
        /// </summary>
        /// <param name="clientEndPoint">The <see cref="IPEndPoint"/> of seing client.</param>
        /// <returns>The <see cref="SocketClient"/> class.</returns>
        public override SocketClientBase FindClient(IPEndPoint clientEndPoint)
        {
            foreach (SocketClientBase socketClient in SocketClients)
            {
                var remoteEndPoint = (IPEndPoint) socketClient.Client.RemoteEndPoint;
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
        /// Begins to start listener and to accept new clients by an asynchronous method.
        /// </summary>
        private void AcceptConnections()
        {
            try
            {
                _listener.Start();
                _listener.BeginAcceptSocket(AcceptCallBack, _listener);
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

        /// <summary>
        /// Ends the pending of asynchronous accept of new clients.
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                var listener = (TcpListener)ar.AsyncState;
                if (ServerStatus != SocketServerStatus.Start || !listener.Server.IsBound) return;

                Socket socket = listener.EndAcceptSocket(ar);
                var client = new SocketClientSsl(socket);
                client.Context = this.Context;
                client.Connected += (s, e) => { OnConnected(e); };
                client.Disconnected += SocketClientDisconnected;
                client.ReceivedData += (s, e) => { OnReceivedData(e); };
                client.SentData += (s, e) => { OnSentData(e); };
                client.Error += (s, e) => { OnError(e); };
                client.SocketError += (s, e) => { OnSocketError(e); };
                client.SocketSecurity += (s, e) => { OnSocketSecurity(e); };
                SocketClients.Add(client);
                client.BeginAuthenticateAsServer(ClientCertificateRequired, CheckCertificateRevocation, ServerCertificate, CertificateValidationCallback, SslProtocol);
                ContinueAcceptConnections();
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

        /// <summary>
        /// Continues the asynchronous accept of new clients.
        /// </summary>
        private void ContinueAcceptConnections()
        {
            try
            {
                if (ServerStatus == SocketServerStatus.Pause || ServerStatus == SocketServerStatus.Stop) return;
                _listener.BeginAcceptSocket(AcceptCallBack, _listener);
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
