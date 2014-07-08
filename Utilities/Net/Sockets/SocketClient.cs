using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace Utilities.Net.Sockets
{
    /// <summary>
    /// Implements the socket client interface. It operates with Berkeley socket.
    /// </summary>
    public class SocketClient : SocketClientBase, IDisposable
    {
        #region Fields.Private

        private Socket _socket = null;
        private IPEndPoint _remoteEndPoint = null;
        private const Int32 MAX_READ_BUFFER_SIZE = 512 * 1024;
        private Byte[] _readBuffer = new Byte[MAX_READ_BUFFER_SIZE];

        #endregion


        #region Properties.Public


        /// <summary>
        /// Gets the TCP local socket.
        /// </summary>
        public override Socket Client
        {
            get
            {
                return _socket;
            }
        }

        /// <summary>
        /// Gets a value indicating whether socket was disposed.
        /// </summary>
        public Boolean SocketDisposed
        {
            get
            {
                try
                {
                    IPEndPoint localPoint = (IPEndPoint)_socket.LocalEndPoint;
                }
                catch (ObjectDisposedException)
                {
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                return false;
            }
        }


        #endregion


        #region Methods.Public

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        public SocketClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class with specified TCP socket.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> class.</param>
        public SocketClient(Socket socket)
        {
            _socket = socket;
            _remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketClient"/> class. It is called by GC 
        /// befor the instance will delete from manage heap.
        /// </summary>
        ~SocketClient()
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
            if (disposing)
            {
                if (!SocketDisposed)
                {
                    try
                    {
                        _socket.Close();
                        (_socket as IDisposable).Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Establishes a connection to a remote host. The host is specified by an IPEndPoint. 
        /// </summary>
        /// <param name="serverEndPoint">The <see cref="IPEndPoint"/> to which you intend to connect.</param>
        /// <returns>Returns a value that indicates whether the connect was done or wasn't.</returns>
        public override Boolean Connect(IPEndPoint serverEndPoint)
        {
            try
            {
                if (_socket == null || SocketDisposed)
                {
                    _socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }
                if (_socket.Connected)
                {
                    return false;
                }
                _remoteEndPoint = serverEndPoint;
                _socket.Connect(serverEndPoint);
                _socket.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, ReceiveDataCallBack, null);
                OnConnected(new SocketEventArgs(serverEndPoint));
                return true;
            }
            catch (SocketException e)
            {
                OnSocketError(new SocketErrorEventArgs(serverEndPoint, e));
                return false;
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(serverEndPoint, e));
                return false;
            }
        }

        /// <summary>
        /// Establishes a connection to a remote host. The host is specified by an IP address and a port number. 
        /// </summary>
        /// <param name="serverAddress">The IP address of the remote host.</param>
        /// <param name="serverPort">The port number of the remote host.</param>
        /// <returns>Returns a value that indicates whether the connect was done or wasn't.</returns>
        public override Boolean Connect(IPAddress serverAddress, Int32 serverPort)
        {
            return Connect(new IPEndPoint(serverAddress, serverPort));
        }

        /// <summary>
        /// Establishes a connection to a remote host. The host is specified by an host name or IP address and a port number.
        /// </summary>
        /// <param name="serverIP">The IP address of the remote host.</param>
        /// <param name="serverPort">The port number of the remote host.</param>
        /// <returns>Returns a value that indicates whether the connect was done or wasn't.</returns>
        public override Boolean Connect(String serverNameOrAddress, Int32 serverPort)
        {
            IPAddress address = ResolveHostName(serverNameOrAddress);
            return Connect(new IPEndPoint(address, serverPort));
        }

        /// <summary>
        /// Establishes a connection to a remote host. The host must be specified befor by properties ServerPoint or by
        /// ServerIP and ServerPort
        /// </summary>
        /// <returns>Returns a value that indicates whether the connect was done or wasn't.</returns>
        public override Boolean Connect()
        {
            if (_remoteEndPoint != null)
            {
                return Connect(_remoteEndPoint);
            }
            else
            {
                Exception e = new Exception("The ServerPoint property is not initialized.");
                OnError(new ErrorEventArgs(new IPEndPoint(-1, -1), e));
                return false;
            }
        }

        /// <summary>
        /// Establishes an asynchronous connection to a remote host. The host is specified by an IPEndPoint. 
        /// </summary>
        /// <param name="serverEndPoint">The <see cref="IPEndPoint"/> to which you intend to connect.</param>
        public override void ConnectAsync(IPEndPoint serverEndPoint)
        {
            try
            {
                if (_socket == null || SocketDisposed)
                {
                    _socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }
                if (_socket.Connected)
                {
                    return;
                }
                _remoteEndPoint = serverEndPoint;
                _socket.BeginConnect(serverEndPoint, ConnectCallBack, null);
            }
            catch (SocketException e)
            {
                OnSocketError(new SocketErrorEventArgs(serverEndPoint, e));
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(serverEndPoint, e));
            }
        }

        /// <summary>
        /// Establishes an asynchronous connection to a remote host. The host is specified by an IP address and a port number. 
        /// </summary>
        /// <param name="serverAddress">The IP address of the remote host.</param>
        /// <param name="serverPort">The port number of the remote host.</param>
        public override void ConnectAsync(IPAddress serverAddress, Int32 serverPort)
        {
            ConnectAsync(new IPEndPoint(serverAddress, serverPort));
        }

        /// <summary>
        /// Establishes an asynchronous  connection to a remote host. The host is specified by an IP address and a port number.
        /// </summary>
        /// <param name="serverIP">The IP address of the remote host.</param>
        /// <param name="serverPort">The port number of the remote host.</param>
        public override void ConnectAsync(String serverNameOrAddress, Int32 serverPort)
        {
            IPAddress address = ResolveHostName(serverNameOrAddress);
            ConnectAsync(new IPEndPoint(address, serverPort));
        }

        /// <summary>
        /// Establishes a asynchronous connection to a remote host. The host must be specified befor by properties ServerPoint or by
        /// ServerIP and ServerPort
        /// </summary>
        public override void ConnectAsync()
        {
            if (_remoteEndPoint != null)
            {
                ConnectAsync(_remoteEndPoint);
            }
            else
            {
                Exception e = new Exception("The ServerPoint property is not initialized.");
                OnError(new ErrorEventArgs(new IPEndPoint(-1, -1), e));
            }
        }

        /// <summary>
        /// Disconnects from remote host.
        /// </summary>
        public override void Disconnect()
        {
            if (SocketDisposed || (!_socket.Connected && !_socket.IsBound))
            {
                return;
            }
            else
            {
                _socket.Shutdown(SocketShutdown.Both);
                Dispose(true);
            }

            OnDisconnected(new SocketEventArgs(_remoteEndPoint));
        }

        /// <summary>
        /// Sends data to remote host.
        /// </summary>
        /// <param name="data">An array of type Byte that contains the data to be sent.</param>
        public override void SendData(Byte[] data)
        {
            try
            {
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendDataCallBack, data);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10053 || e.ErrorCode == 10054)
                {
                    Disconnect();
                }
                else
                {
                    OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        #endregion


        #region Methods.Private

        /// <summary>
        /// Begins reading data. It is necessity for working with <see cref="SocketServer"/> and <see cref="SocketServerAsync"/> internaly.
        /// </summary>
        internal void BeginReceive()
        {
            try
            {
                //Начинаем асинхронное чтение пакета
                _socket.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, ReceiveDataCallBack, null);
                OnConnected(new SocketEventArgs(_remoteEndPoint));
            }
            catch (SocketException e)
            {
                OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        /// <summary>
        /// Ends a pending asynchronous connection request.
        /// </summary>
        /// <param name="ar">An IAsyncResult object that stores state information and any user-defined data for this asynchronous operation.</param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                _socket.EndConnect(ar);
                _socket.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, ReceiveDataCallBack, null);
                OnConnected(new SocketEventArgs(_remoteEndPoint));
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10053 || e.ErrorCode == 10054)
                {
                    Disconnect();
                }
                else
                {
                    OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        /// <summary>
        /// Ends a pending asynchronous data read request.
        /// </summary>
        /// <param name="ar">An IAsyncResult object that stores state information and any user-defined data for this asynchronous operation.</param>
        private void ReceiveDataCallBack(IAsyncResult ar)
        {
            try
            {
                //This control is needed if method Disconnect was called befor and after that will called this method.
                if (!_socket.IsBound || !_socket.Connected) return;

                Int32 bytesRead = _socket.EndReceive(ar);
                if (bytesRead < 1)
                {
                    Disconnect();
                }
                else
                {
                    Byte[] data = new Byte[bytesRead];
                    Array.Copy(_readBuffer, 0, data, 0, data.Length);
                    OnReceivedData(new SocketReceiveEventArgs(_remoteEndPoint, data));
                    Array.Clear(_readBuffer, 0, _readBuffer.Length);
                    ContinueReceiveData();
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10053 || e.ErrorCode == 10054)
                {
                    Disconnect();
                }
                else
                {
                    OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        /// <summary>
        /// This method continues read data.
        /// </summary>
        private void ContinueReceiveData()
        {
            try
            {
                //This control is needed if method Disconnect was called befor and after that will called this method.
                if (!_socket.Connected) return;
                _socket.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, ReceiveDataCallBack, null);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10053 || e.ErrorCode == 10054)
                {
                    Disconnect();
                }
                else
                {
                    OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        /// <summary>
        /// Ends a pending asynchronous data send request.
        /// </summary>
        /// <param name="ar">An IAsyncResult object that stores state information and any user-defined data for this asynchronous operation.</param>
        private void SendDataCallBack(IAsyncResult ar)
        {
            try
            {
                //Проверка необходима если был вызван метод Disconnect и после этого была вызвана эта функция
                if (!_socket.Connected) return;
                
                Byte[] data = (Byte[])ar.AsyncState;
                _socket.EndSend(ar);
                OnSentData(new SocketSendEventArgs(_remoteEndPoint, data));
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10053 || e.ErrorCode == 10054)
                {
                    Disconnect();
                }
                else
                {
                    OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        #endregion
    }
}