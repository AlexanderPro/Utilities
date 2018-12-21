using System;
using System.Security.Principal;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Utilities.Security.Principal
{
    public class WindowsImpersonator : IDisposable
    {
        private WindowsImpersonationContext _impersonationContext;

        private const Int32 LOGON32_PROVIDER_DEFAULT = 0;
        private const Int32 LOGON32_LOGON_INTERACTIVE = 2;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern Int32 LogonUser(String userName, String domain, String password, Int32 logonType, Int32 logonProvider, ref IntPtr token);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 DuplicateToken(IntPtr token, Int32 impersonationLevel, ref IntPtr newToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        private static extern Boolean RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern Boolean CloseHandle(IntPtr handle);

        public Boolean Impersonate(String userName, String domainName, String password)
        {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;
            
            if (!RevertToSelf()) return false;
            var result = LogonUser(userName, domainName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token);
            if (result == 0)
            {
                result = Marshal.GetLastWin32Error();
                var message = "The call of LogonUser failed, GetLastError returned: " + result;
                throw new Win32Exception(result, message);
            }

            result = DuplicateToken(token, 2, ref tokenDuplicate);
            if (result == 0)
            {
                result = Marshal.GetLastWin32Error();
                var message = "The call of DuplicateToken failed, GetLastError returned: " + result;
                throw new Win32Exception(result, message);
            }

            var windowsIdentity = new WindowsIdentity(tokenDuplicate);
            _impersonationContext = windowsIdentity.Impersonate();
            if (token != IntPtr.Zero) CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);
            
            var impersonated = _impersonationContext != null;
            return impersonated;
        }

        public void UnImpersonate()
        {
            if (_impersonationContext != null) _impersonationContext.Undo();
        }

        public void Dispose()
        {
            UnImpersonate();
        }
    }
}