using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Net.RAS
{
    internal static class RasNativeMethods
    {
        [DllImport("Rasapi32.dll", EntryPoint = "RasEnumConnectionsA", SetLastError = true)]
        public static extern UInt32 RasEnumConnections(ref RASCONN lprasconn, ref int lpcb, ref Int32 lpcConnections);

        [DllImport("Rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 RasGetConnectionStatistics(IntPtr hRasConn, [In, Out]RasStats lpStatistics);

        [DllImport("Rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 RasHangUp(IntPtr hrasconn);

        [DllImport("Wininet.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 InternetDial(IntPtr hwnd, [In]String lpszConnectoid, UInt32 dwFlags, ref Int32 lpdwConnection, UInt32 dwReserved);

        [DllImport("Wininet.dll", CharSet = CharSet.Auto)]
        public static extern Boolean InternetGetConnectedState(ref Int32 lpdwFlags, Int32 dwReserved);

        [DllImport("Rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 RasGetConnectStatus(IntPtr hrasconn, [In, Out]RASCONNSTATUS lprasconnstatus);

        [DllImport("Rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 RasGetProjectionInfo(IntPtr hRasConn, RASPROJECTION projection, [In, Out] RASPPPIP pppip, ref uint ppipSize);

        [DllImport("Rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 RasDial([In]RASDIALEXTENSIONS lpRasDialExtensions, [In]String lpszPhonebook, [In]RASDIALPARAMS lpRasDialParams, uint dwNotifierType, Delegate lpvNotifier, ref IntPtr lphRasConn);

        [DllImport("Rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern UInt32 RasGetErrorString(UInt32 uErrorValue, StringBuilder lpszErrorString, [In]Int32 cBufSize);
    }
}
