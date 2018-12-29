#if NETFULL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Diagnostics
{
    internal static class WindowsServiceNativeConstants
    {
        public const UInt32 SERVICE_CONFIG_DESCRIPTION = 0x00001;
        public const UInt32 ERROR_INSUFFICIENT_BUFFER = 122;
        public const UInt32 SERVICE_NO_CHANGE = 0xFFFFFFFF;
        public const UInt32 SERVICE_INTERACTIVE_PROCESS = 0x00100;
    }
}
#endif