using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Utilities.Net.Sockets
{
    /// <summary>
    /// The socket datagram server.
    /// </summary>
    public class SocketDatagramServer : SocketServerBase
    {
        #region Fields.Private

        private UdpClient _client = null;
        private Boolean _disposed = false;

        #endregion


        #region Properties.Public

        /// <summary>
        /// Gets the TCP local socket.
        /// </summary>
        public override Socket Server { get { return _client.Client; } }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        public override IPEndPoint LocalEndPoint { get; protected set; }

        /// <summary>
        /// Gets the collecton of socket clients
        /// </summary>
        public override IList<SocketClientBase> SocketClients { get { throw new NotSupportedException(); } }

        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        public SocketServerStatus ServerStatus { get; private set; }

        #endregion


        #region Methods.Public

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketDatagramServer"/> class with specified local point.
        /// </summary>
        /// <param name="port">The port of local host.</param>
        public SocketDatagramServer(Int32 port)
        {
            _client = new UdpClient(port);
            LocalEndPoint = (IPEndPoint)_client.Client.LocalEndPoint;
            ServerStatus = SocketServerStatus.Stop;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketDatagramServer"/> class. It is called by GC 
        /// befor the instance will delete from manage heap.
        /// </summary>
        ~SocketDatagramServer()
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
                        _client.Close();
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
            ReceiveData();
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public override void Stop()
        {
            if (ServerStatus == SocketServerStatus.Stop) return;
            ServerStatus = SocketServerStatus.Stop;
        }

        /// <summary>
        /// This method is not supported for datagram protocol.
        /// </summary>
        public override void SendAllClients(Byte[] data)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method is not supported for datagram protocol.
        /// </summary>
        public override SocketClientBase FindClient(IPEndPoint clientEndPoint)
        {
            throw new NotSupportedException();
        }

        #endregion


        #region Methods.Private

        private void ReceiveData()
        {
            try
            {
                _client.BeginReceive(ReceivedCallBack, _client);
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

        private void ReceivedCallBack(IAsyncResult ar)
        {
            try
            {
                var client = (UdpClient)ar.AsyncState;
                if (ServerStatus == SocketServerStatus.Pause || ServerStatus == SocketServerStatus.Stop) return;

                IPEndPoint remotePoint = null;
                Byte[] data = client.EndReceive(ar, ref remotePoint);
                var eventArgs = new SocketReceiveEventArgs(remotePoint, data);
                OnReceivedData(eventArgs);
                ContinueReceiveData();
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

        private void ContinueReceiveData()
        {
            try
            {
                if (ServerStatus == SocketServerStatus.Pause || ServerStatus == SocketServerStatus.Stop) return;
                _client.BeginReceive(ReceivedCallBack, _client);
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

        #endregion
    }
}
