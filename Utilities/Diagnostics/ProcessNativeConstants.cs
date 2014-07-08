using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Diagnostics
{
    internal static class ProcessNativeConstants
    {
        public const Int32 SE_PRIVILEGE_ENABLED = 0x02;
        public const Int32 ERROR_NO_TOKEN = 1008;
        public const Int32 RPC_S_INVALID_BINDING = 1702;
        public const UInt32 WM_QUIT = 0x12;
        public const UInt32 WM_CLOSE = 0x10;
        public const UInt32 WM_DESTROY = 0x02;
    }
}
