using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Diagnostics
{
    [Flags]
    internal enum SCM_ACCESS : uint
    {
        SC_MANAGER_CONNECT = 0x00001,
        SC_MANAGER_CREATE_SERVICE = 0x00002,
        SC_MANAGER_ENUMERATE_SERVICE = 0x00004,
        SC_MANAGER_LOCK = 0x00008,
        SC_MANAGER_QUERY_LOCK_STATUS = 0x00010,
        SC_MANAGER_MODIFY_BOOT_CONFIG = 0x00020,
        SC_MANAGER_ALL_ACCESS = 0xF003F
    }

    [Flags]
    internal enum SERVICE_ACCESS : uint
    {
        STANDARD_RIGHTS_REQUIRED = 0xF0000,
        SERVICE_QUERY_CONFIG = 0x00001,
        SERVICE_CHANGE_CONFIG = 0x00002,
        SERVICE_QUERY_STATUS = 0x00004,
        SERVICE_ENUMERATE_DEPENDENTS = 0x00008,
        SERVICE_START = 0x00010,
        SERVICE_STOP = 0x00020,
        SERVICE_PAUSE_CONTINUE = 0x00040,
        SERVICE_INTERROGATE = 0x00080,
        SERVICE_USER_DEFINED_CONTROL = 0x00100,
        SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS |
        SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_DESCRIPTION
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public String lpDescription;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct QUERY_SERVICE_CONFIG
    {
        public UInt32 dwServiceType;
        public UInt32 dwStartType;
        public UInt32 dwErrorControl;
        [MarshalAs(UnmanagedType.LPTStr)]
        public String lpBinaryPathName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public String lpLoadOrderGroup;
        public UInt32 dwTagId;
        [MarshalAs(UnmanagedType.LPTStr)]
        public String lpDependencies;
        [MarshalAs(UnmanagedType.LPTStr)]
        public String lpServiceStartName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public String lpDisplayName;
    }
}
