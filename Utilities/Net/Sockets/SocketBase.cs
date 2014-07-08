using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Utilities.Net.Sockets
{
    public abstract class SocketBase
    {
        #region Properties.Public

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that are issued when callback function was called.
        /// </summary>
        public SynchronizationContext Context { get; set; }

        #endregion


        #region Events.Public

        /// <summary>
        /// Occurs when socket connection to remote host will be established.
        /// </summary>
        public event EventHandler<SocketEventArgs> Connected;

        /// <summary>
        /// Occurs when socket connection to remote host will be broken.
        /// </summary>
        public event EventHandler<SocketEventArgs> Disconnected;

        /// <summary>
        /// Occurs when socket receives data.
        /// </summary>
        public event EventHandler<SocketReceiveEventArgs> ReceivedData;

        /// <summary>
        /// Occurs when socket will complete to send data.
        /// </summary>
        public event EventHandler<SocketSendEventArgs> SentData;

        /// <summary>
        /// Occurs when arise some exception.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Occurs when arise soсket exception.
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> SocketError;

        /// <summary>
        /// Occurs when SSL authentication is changed.
        /// </summary>
        public event EventHandler<SocketSecurityEventArgs> SocketSecurity;

        #endregion


        #region Methods.Protected

        protected void OnConnected(SocketEventArgs e)
        {
            RaiseEvent(Connected, e);
        }

        protected void OnDisconnected(SocketEventArgs e)
        {
            RaiseEvent(Disconnected, e);
        }

        protected void OnSentData(SocketSendEventArgs e)
        {
            RaiseEvent(SentData, e);
        }

        protected void OnReceivedData(SocketReceiveEventArgs e)
        {
            RaiseEvent(ReceivedData, e);
        }

        protected void OnError(ErrorEventArgs e)
        {
            RaiseEvent(Error, e);
        }

        protected void OnSocketError(SocketErrorEventArgs e)
        {
            RaiseEvent(SocketError, e);
        }

        protected void OnSocketSecurity(SocketSecurityEventArgs e)
        {
            RaiseEvent(SocketSecurity, e);
        }

        protected void RaiseEvent<T>(EventHandler<T> eventHandler, T e) where T : EventArgs
        {
            EventHandler<T> handler = eventHandler;
            if (handler != null)
            {
                if(Context != null)
                {
                    Context.Post((s) => { handler(this, e); }, null);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Resolves host name by parsing string with address in format of IP or machine DNS name.
        /// </summary>
        /// <param name="hostNameOrAddress">A string that contains an IP (for IPv4 or for IPv6) address or DNS address.</param>
        /// <returns>An IPAddress instance.</returns>
        protected IPAddress ResolveHostName(String hostNameOrAddress)
        {
            IPAddress address;
            if (!IPAddress.TryParse(hostNameOrAddress, out address))
            {
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);
                    if (hostEntry.AddressList.Length > 0)
                    {
                        address = hostEntry.AddressList[0];
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Impossible to resolve host or address.", e);
                }
            }

            return address;
        }

        #endregion
    }
}
