using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Diagnostics
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID
    {
        internal UInt32 m_nLowPart;
        internal UInt32 m_nHighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_PRIVILEGES
    {
        internal Int32 m_nPrivilegeCount;
        internal LUID m_oLUID;
        internal Int32 m_nAttributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROFILEINFO
    {
        internal Int32 dwSize;
        internal Int32 dwFlags;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal String lpUserName;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal String lpProfilePath;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal String lpDefaultPath;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal String lpServerName;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal String lpPolicyPath;
        internal IntPtr hProfile;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct STARTUPINFO
    {
        internal Int32 cb;
        internal String lpReserved;
        internal String lpDesktop;
        internal String lpTitle;
        internal Int32 dwX;
        internal Int32 dwY;
        internal Int32 dwXSize;
        internal Int32 dwXCountChars;
        internal Int32 dwYCountChars;
        internal Int32 dwFillAttribute;
        internal Int32 dwFlags;
        internal Int16 wShowWindow;
        internal Int16 cbReserved2;
        internal IntPtr lpReserved2;
        internal IntPtr hStdInput;
        internal IntPtr hStdOutput;
        internal IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_INFORMATION
    {
        internal IntPtr hProcess;
        internal IntPtr hThread;
        internal Int32 dwProcessID;
        internal Int32 dwThreadID;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES
    {
        internal Int32 Length;
        internal IntPtr lpSecurityDescriptor;
        internal Boolean bInheritHandle;
    }

    [Flags]
    internal enum CreationFlags
    {
        CREATE_SUSPENDED = 0x00000004,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000
    }

    [Flags]
    internal enum LogonFlags
    {
        LOGON_WITH_PROFILE = 0x00000001,
        LOGON_NETCREDENTIALS_ONLY = 0x00000002
    }

    internal enum WTS_INFO_CLASS
    {
        WTSInitialProgram,
        WTSApplicationName,
        WTSWorkingDirectory,
        WTSOEMId,
        WTSSessionId,
        WTSUserName,
        WTSWinStationName,
        WTSDomainName,
        WTSConnectState,
        WTSClientBuildNumber,
        WTSClientName,
        WTSClientDirectory,
        WTSClientProductId,
        WTSClientHardwareId,
        WTSClientAddress,
        WTSClientDisplay,
        WTSClientProtocolType
    }

    internal enum SECURITY_IMPERSONATION_LEVEL
    {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
    }

    internal enum TOKEN_TYPE
    {
        TokenPrimary = 1,
        TokenImpersonation
    }

    [Flags]
    internal enum TokenAccess : uint
    {
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        STANDARD_RIGHTS_READ = 0x00020000,
        TOKEN_ASSIGN_PRIMARY = 0x0001,
        TOKEN_DUPLICATE = 0x0002,
        TOKEN_IMPERSONATE = 0x0004,
        TOKEN_QUERY = 0x0008,
        TOKEN_QUERY_SOURCE = 0x0010,
        TOKEN_ADJUST_PRIVILEGES = 0x0020,
        TOKEN_ADJUST_GROUPS = 0x0040,
        TOKEN_ADJUST_DEFAULT = 0x0080,
        TOKEN_ADJUST_SESSIONID = 0x0100,
        TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY,
        TOKEN_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID
    }

    internal delegate Boolean EnumWindowsCallbackDelegate(IntPtr hWnd, UInt32 lParam);
}
