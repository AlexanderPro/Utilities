using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Net.RAS
{
    public class RasDialer
    {
        private IntPtr _connection = IntPtr.Zero;

        public void Connect(String connectionName, String userName, String password)
        {
            var rdp = new RASDIALPARAMS();
            rdp.dwSize = Marshal.SizeOf(rdp);
            rdp.szEntryName = connectionName;
            rdp.szUserName = userName;
            rdp.szPassword = password;
            UInt32 result = RasNativeMethods.RasDial(null, null, rdp, 0xFFFFFFFF, null, ref _connection);
            if (result != 0) throw new RasException(result);
        }

        public void Disconnect()
        {
            if (_connection == IntPtr.Zero) throw new RasException("The connection is not established.");
            UInt32 result = RasNativeMethods.RasHangUp(_connection);
            if (result != 0) throw new RasException(result);
            _connection = IntPtr.Zero;
        }

        public String GetIPAddress()
        {
            if (_connection == IntPtr.Zero) throw new RasException("The connection is not established.");
            var pppip = new RASPPPIP();
            var size = (UInt32)Marshal.SizeOf(typeof(RASPPPIP));
            pppip.dwSize = size;
            UInt32 result = RasNativeMethods.RasGetProjectionInfo(_connection, RASPROJECTION.RASP_PppIp, pppip, ref size);
            if (result != 0) throw new RasException(result);
            return pppip.szIpAddress;
        }

        public RASCONNSTATE GetStatus()
        {
            if (_connection == IntPtr.Zero) throw new RasException("The connection is not established.");
            var status = new RASCONNSTATUS();
            UInt32 result = RasNativeMethods.RasGetConnectStatus(_connection, status);
            if (result != 0) throw new RasException(result);
            return status.rasconnstate;
        }
    }
}