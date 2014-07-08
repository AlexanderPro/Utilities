using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Net.Sockets
{
    /// <summary>
    /// Specifies the status of socket server.
    /// </summary>
    public enum SocketServerStatus
    {
        /// <summary>
        /// Server is started.
        /// </summary>
        Start,

        /// <summary>
        /// Server is stopped.
        /// </summary>
        Stop,

        /// <summary>
        /// Server is paused.
        /// </summary>
        Pause
    }
}
