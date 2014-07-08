using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;

namespace Utilities.Net.Sockets
{
    /// <summary>
    /// Provides data for <see cref="Connected"/> and <see cref="Disconnected"/> events.
    /// </summary>
    public class SocketEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the remote endpoint.
        /// </summary>
        public IPEndPoint RemotePoint { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketEventArgs"/> class with specified endpoint.
        /// </summary>
        /// <param name="remoteEndPoint">The <see cref="IPEndPoint"/> of remote host.</param>
        public SocketEventArgs(IPEndPoint remoteEndPoint)
        {
            RemotePoint = remoteEndPoint;
        }
    }


    /// <summary>
    /// Provides data for <see cref="SendedData"/> event.
    /// </summary>
    public class SocketSendEventArgs : SocketEventArgs
    {
        /// <summary>
        /// Gets the array of sended data.
        /// </summary>
        public Byte[] SendedData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketSendEventArgs"/> class with specified endpoint and
        /// sended data.
        /// </summary>
        /// <param name="remoteEndPoint">The <see cref="IPEndPoint"/> of remote host.</param>
        /// <param name="sendedData">The array of sended data.</param>
        public SocketSendEventArgs(IPEndPoint remoteEndPoint, byte[] sendedData) : base(remoteEndPoint)
        {
            SendedData = sendedData;
        }
    }


    /// <summary>
    /// Provides data for <see cref="ReceivedData"/> event.
    /// </summary>
    public class SocketReceiveEventArgs : SocketEventArgs
    {
        /// <summary>
        /// Gets the array of received data.
        /// </summary>
        public Byte[] ReceivedData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketReceiveEventArgs"/> class with specified endpoint and
        /// received data.
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of remote host.</param>
        /// <param name="receivedData">The array of received data.</param>
        public SocketReceiveEventArgs(IPEndPoint endPoint, Byte[] receivedData) : base(endPoint)
        {
            ReceivedData = receivedData;
        }
    }


    /// <summary>
    /// Provides data for <see cref="Error"/> event.
    /// </summary>
    public class ErrorEventArgs : SocketEventArgs
    {
        /// <summary>
        /// Gets the arised exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventArgs"/> class with specified endpoint and
        /// arised exception.
        /// </summary>
        /// <param name="remoteEndPoint">The <see cref="IPEndPoint"/> of remote host.</param>
        /// <param name="exception">The arised <see cref="Exception"/>.</param>
        public ErrorEventArgs(IPEndPoint remoteEndPoint, Exception exception) : base(remoteEndPoint)
        {
            Exception = exception;
        }
    }


    /// <summary>
    /// Provides data for <see cref="SocketError"/> event.
    /// </summary>
    public class SocketErrorEventArgs : SocketEventArgs
    {
        /// <summary>
        /// Gets the arised socket exception.
        /// </summary>
        public SocketException SocketException { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketErrorEventArgs"/> class with specified endpoint and
        /// arised socket exception.
        /// </summary>
        /// <param name="remoteEndPoint">The <see cref="IPEndPoint"/> of remote host.</param>
        /// <param name="socketException">The arised <see cref="SocketException"/>.</param>
        public SocketErrorEventArgs(IPEndPoint remoteEndPoint, SocketException socketException) : base(remoteEndPoint)
        {
            SocketException = socketException;
        }
    }


    /// <summary>
    /// Provides data for <see cref="SocketSecurity"/> event.
    /// </summary>
    public class SocketSecurityEventArgs : SocketEventArgs
    {
        /// <summary>
        /// Gets the Secure Socket Layer stream.
        /// </summary>
        public SslStream Stream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketSecurityEventArgs"/> class with specified endpoint and ssl stream.
        /// </summary>
        /// <param name="remoteEndPoint">The <see cref="IPEndPoint"/> of host.</param>
        /// <param name="stream">The <see cref="SslStream"/>.</param>
        public SocketSecurityEventArgs(IPEndPoint endPoint, SslStream stream) : base(endPoint)
        {
            Stream = stream;
        }
    }
}