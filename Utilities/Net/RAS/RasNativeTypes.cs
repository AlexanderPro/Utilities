using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Net.RAS
{
    internal enum RasFieldSizeConstants
    {
        RAS_MaxDeviceType = 16,
        RAS_MaxPhoneNumber = 128,
        RAS_MaxIpAddress = 15,
        RAS_MaxIpxAddress = 21,
        //#if WINVER4
        RAS_MaxEntryName = 256,
        RAS_MaxDeviceName = 128,
        RAS_MaxCallbackNumber = RAS_MaxPhoneNumber,
        /*#else
                RAS_MaxEntryName = 20,
                RAS_MaxDeviceName = 32,
                RAS_MaxCallbackNumber = 48,
        #endif*/

        RAS_MaxAreaCode = 10,
        RAS_MaxPadType = 32,
        RAS_MaxX25Address = 200,
        RAS_MaxFacilities = 200,
        RAS_MaxUserData = 200,
        RAS_MaxReplyMessage = 1024,
        RAS_MaxDnsSuffix = 256,
        UNLEN = 256,
        PWLEN = 256,
        DNLEN = 15,

        INTERNET_RAS_INSTALLED = 0x10,
        RAS_Connected = 0x2000
    }

    public enum RASCONNSTATE
    {
        RASCS_OpenPort = 0,
        RASCS_PortOpened,
        RASCS_ConnectDevice,
        RASCS_DeviceConnected,
        RASCS_AllDevicesConnected,
        RASCS_Authenticate,
        RASCS_AuthNotify,
        RASCS_AuthRetry,
        RASCS_AuthCallback,
        RASCS_AuthChangePassword,
        RASCS_AuthProject,
        RASCS_AuthLinkSpeed,
        RASCS_AuthAck,
        RASCS_ReAuthenticate,
        RASCS_Authenticated,
        RASCS_PrepareForCallback,
        RASCS_WaitForModemReset,
        RASCS_WaitForCallback,
        RASCS_Projected,

        //#if (WINVER4) 
        RASCS_StartAuthentication,    // Windows 95 only 
        RASCS_CallbackComplete,       // Windows 95 only 
        RASCS_LogonNetwork,           // Windows 95 only 
        //#endif
        RASCS_SubEntryConnected,
        RASCS_SubEntryDisconnected,

        RASCS_Interactive = RASCS_PAUSED,
        RASCS_RetryAuthentication,
        RASCS_CallbackSetByCaller,
        RASCS_PasswordExpired,
#if (WINVER5)
		RASCS_InvokeEapUI,
#endif
        RASCS_Connected = RASCS_DONE,
        RASCS_Disconnected,
        RASCS_PAUSED = 0x1000,
        RASCS_DONE = 0x2000
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct RASEAPINFO
    {
        public Int32 sizeOfEapData;
        public IntPtr eapData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class RASDIALEXTENSIONS
    {
        public readonly Int32 dwSize = Marshal.SizeOf(typeof(RASDIALEXTENSIONS));
        public UInt32 dwfOptions = 0;
        public Int32 hwndParent = 0;
        public Int32 reserved = 0;
        public Int32 reserved1 = 0;
        public RASEAPINFO RasEapInfo = new RASEAPINFO();
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class RASDIALPARAMS
    {
        public Int32 dwSize = Marshal.SizeOf(typeof(RASDIALPARAMS));
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (Int32)RasFieldSizeConstants.RAS_MaxEntryName + 1)]
        public String szEntryName = null;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (Int32)RasFieldSizeConstants.RAS_MaxPhoneNumber + 1)]
        public String szPhoneNumber = null;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (Int32)RasFieldSizeConstants.RAS_MaxCallbackNumber + 1)]
        public String szCallbackNumber = null;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (Int32)RasFieldSizeConstants.UNLEN + 1)]
        public String szUserName = null;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (Int32)RasFieldSizeConstants.PWLEN + 1)]
        public String szPassword = null;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (Int32)RasFieldSizeConstants.DNLEN + 1)]
        public String szDomain = null;
        public Int32 dwSubEntry = 0;
        public Int32 dwCallbackId = 0;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct GUID
    {
        public UInt32 Data1;
        public ushort Data2;
        public ushort Data3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] Data4;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct RASCONN
    {
        public Int32 dwSize;
        public IntPtr hrasconn;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)RasFieldSizeConstants.RAS_MaxEntryName + 1)]
        public String szEntryName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)RasFieldSizeConstants.RAS_MaxDeviceType + 1)]
        public String szDeviceType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)RasFieldSizeConstants.RAS_MaxDeviceName + 1)]
        public String szDeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]//MAX_PAPTH=260
        public String szPhonebook;
        public Int32 dwSubEntry;
        public GUID guidEntry;
#if (WINVER501)
		 int     dwFlags;
		 public LUID      luid;
#endif
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct LUID
    {
        Int32 LowPart;
        Int32 HighPart;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class RasStats
    {
        public Int32 dwSize = Marshal.SizeOf(typeof(RasStats));
        public Int32 dwBytesXmited;
        public Int32 dwBytesRcved;
        public Int32 dwFramesXmited;
        public Int32 dwFramesRcved;
        public Int32 dwCrcErr;
        public Int32 dwTimeoutErr;
        public Int32 dwAlignmentErr;
        public Int32 dwHardwareOverrunErr;
        public Int32 dwFramingErr;
        public Int32 dwBufferOverrunErr;
        public Int32 dwCompressionRatioIn;
        public Int32 dwCompressionRatioOut;
        public Int32 dwBps;
        public Int32 dwConnectDuration;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class RASCONNSTATUS
    {
        public readonly Int32 dwSize = Marshal.SizeOf(typeof(RASCONNSTATUS));
        public RASCONNSTATE rasconnstate = RASCONNSTATE.RASCS_OpenPort;
        public Int32 dwError = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)RasFieldSizeConstants.RAS_MaxDeviceType + 1)]
        public String szDeviceType = null;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)RasFieldSizeConstants.RAS_MaxDeviceName + 1)]
        public String szDeviceName = null;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class RASPPPIP
    {
        public UInt32 dwSize = 72;
        public UInt32 dwError;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String szIpAddress;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String szServerIpAddress;
    }

    internal enum RASPROJECTION : uint
    {
        RASP_AMB = 0x10000,
        RASP_PppNbf = 0x803F,
        RASP_PppIpx = 0x802B,
        RASP_PppIp = 0x8021,
        RASP_PppCcp = 0x80FD,
        RASP_PppLcp = 0xC021,
        RASP_Slip = 0x20000
    }
}