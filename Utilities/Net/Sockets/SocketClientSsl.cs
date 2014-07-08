using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace Utilities.Net.Sockets
{
    /// <summary>
    /// Implements the socket client interface with support SSL. It operates with Berkeley socket.
    /// </summary>
    public class SocketClientSsl : SocketClientBase, IDisposable
    {
        #region Fields.Private

        private Socket _socket = null;
        private SslStream _stream = null;
        private IPEndPoint _remoteEndPoint = null;
        private const Boolean LEAVE_INNER_STREAM_OPEN = false;
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
        /// A Boolean value that specifies whether the certificate revocation list is checked during authentication.
        /// </summary>
        public Boolean CheckCertificateRevocation { get; set; }

        /// <summary>
        /// Containes client certificates.
        /// </summary>
        public X509CertificateCollection ClientCertificates { get; set; }

        /// <summary>
        /// Responsible for validating the certificate supplied by the remote party.
        /// </summary>
        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

        /// <summary>
        /// The name of the server that will share the SslStream.
        /// </summary>
        public String RemoteHostName { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the security protocol used to authenticate this connection. 
        /// </summary>
        public SslProtocols SslProtocol { get; set; }

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
        public SocketClientSsl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class with specified TCP socket.
        /// </summary>
        /// <param name="socket">The <see cref="Socket"/> class.</param>
        public SocketClientSsl(Socket socket)
        {
            _socket = socket;
            _remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            ClientCertificates = new X509CertificateCollection();
            SslProtocol = SslProtocols.None;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SocketClient"/> class. It is called by GC 
        /// befor the instance will delete from manage heap.
        /// </summary>
        ~SocketClientSsl()
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
                        _stream.Close();
                        _socket.Close();
                        ((IDisposable)_stream).Dispose();
                        ((IDisposable)_socket).Dispose();
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
                _stream = CertificateValidationCallback == null ? new SslStream(new NetworkStream(_socket), LEAVE_INNER_STREAM_OPEN) : new SslStream(new NetworkStream(_socket), LEAVE_INNER_STREAM_OPEN, CertificateValidationCallback);
                _stream.AuthenticateAsClient(RemoteHostName, ClientCertificates, SslProtocol, CheckCertificateRevocation);
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, ReceiveDataCallBack, null);
                OnConnected(new SocketEventArgs(serverEndPoint));
                OnSocketSecurity(new SocketSecurityEventArgs(serverEndPoint, _stream));
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
                _stream.Close();
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
                _stream.BeginWrite(data, 0, data.Length, SendDataCallBack, data);
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        #endregion


        #region Methods.Private

        /// <summary>
        /// Begins authenticating and reading data. It is necessity for working with socket server internaly.
        /// </summary>
        internal void BeginAuthenticateAsServer(Boolean clientCertificateRequired, Boolean checkCertificateRevocation, X509Certificate serverCertificate, RemoteCertificateValidationCallback certificateValidationCallback, SslProtocols sslProtocol)
        {
            try
            {
                _stream = certificateValidationCallback == null ? new SslStream(new NetworkStream(_socket), LEAVE_INNER_STREAM_OPEN) : new SslStream(new NetworkStream(_socket), LEAVE_INNER_STREAM_OPEN, certificateValidationCallback);
                _stream.BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, sslProtocol, checkCertificateRevocation, AuthenticateAsServerCallBack, null);
                OnConnected(new SocketEventArgs(_remoteEndPoint));
            }
            catch (SocketException e)
            {
                OnSocketError(new SocketErrorEventArgs(_remoteEndPoint, e));
            }
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
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
                _stream = CertificateValidationCallback == null ? new SslStream(new NetworkStream(_socket), LEAVE_INNER_STREAM_OPEN) : new SslStream(new NetworkStream(_socket), LEAVE_INNER_STREAM_OPEN, CertificateValidationCallback);
                _stream.BeginAuthenticateAsClient(RemoteHostName, ClientCertificates, SslProtocol, CheckCertificateRevocation, AuthenticateAsClientCallBack, null);
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        private void AuthenticateAsClientCallBack(IAsyncResult ar)
        {
            try
            {
                _stream.EndAuthenticateAsClient(ar);
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, ReceiveDataCallBack, null);
                OnSocketSecurity(new SocketSecurityEventArgs(_remoteEndPoint, _stream));
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        private void AuthenticateAsServerCallBack(IAsyncResult ar)
        {
            try
            {
                _stream.EndAuthenticateAsServer(ar);
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, ReceiveDataCallBack, null);
                OnSocketSecurity(new SocketSecurityEventArgs(_remoteEndPoint, _stream));
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
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

                Int32 bytesRead = _stream.EndRead(ar);
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
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
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, ReceiveDataCallBack, null);
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
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
                _stream.EndWrite(ar);
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
            catch (IOException e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
                Disconnect();
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(_remoteEndPoint, e));
            }
        }

        #endregion
    }
}
