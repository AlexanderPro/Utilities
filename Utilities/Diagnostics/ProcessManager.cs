using System;
using System.Reflection;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;

namespace Utilities.Diagnostics
{
    public static class ProcessManager
    {
        private static Int32 CreateUIProcessForServiceRunningAsLocalSystem(String fileName, String arguments, String workingDirectory)
        {
            var pi = new PROCESS_INFORMATION();
            var sa = new SECURITY_ATTRIBUTES();
            var si = new STARTUPINFO();
            var profileInfo = new PROFILEINFO();
            var userToken = new IntPtr(0);
            var primaryToken = new IntPtr(0);
            var activeUserSessionId = (UInt32)0xFFFFFFFF;
            var activeUserName = "";
            var processID = -1;
            var ptrBuffer = IntPtr.Zero;
            var nBytes = (UInt32)0;
            var message = "";
            var methodName = "";
            var error = 0;

            try
            {
                methodName = MethodInfo.GetCurrentMethod().Name;
                activeUserSessionId = ProcessNativeMethods.WTSGetActiveConsoleSessionId();
                if (activeUserSessionId == 0xFFFFFFFF)
                {
                    error = Marshal.GetLastWin32Error();
                    message = String.Format("ProcessManager -> {0} -> The call to WTSGetActiveConsoleSessionId failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                if (!ProcessNativeMethods.WTSQuerySessionInformation(IntPtr.Zero, (Int32)activeUserSessionId, WTS_INFO_CLASS.WTSUserName, out ptrBuffer, out nBytes))
                {
                    error = Marshal.GetLastWin32Error();
                    //On earlier operating systems from Vista, when no one is logged in, you get RPC_S_INVALID_BINDING which is ok, we just won't impersonate
                    if (error != ProcessNativeConstants.RPC_S_INVALID_BINDING)
                    {
                        message = String.Format("ProcessManager -> {0} -> The call to WTSQuerySessionInformation failed, GetLastError returns: {1}", methodName, error);
                        throw new Win32Exception(error, message);
                    }

                    //No one logged in so let's just do this the simple way
                    return StartProcess(fileName, arguments, workingDirectory);
                }

                activeUserName = Marshal.PtrToStringAnsi(ptrBuffer);
                ProcessNativeMethods.WTSFreeMemory(ptrBuffer);

                //We are supposedly running as a service so we're going to be running in session 0 so get a user token from the active user session
                if (!ProcessNativeMethods.WTSQueryUserToken((uint)activeUserSessionId, out userToken))
                {
                    //Int32 lastError = Marshal.GetLastWin32Error();
                    //Remember, sometimes nobody is logged in (especially when we're set to Automatically startup) you should get error code 1008 (no user token available)
                    //if (ERROR_NO_TOKEN != lastError)
                    //{
                    //   //Ensure we're running under the local system account
                    //   WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();

                    //   if ("NT AUTHORITY\\SYSTEM" != identity.Name)
                    //   {
                    //      message = String.Format("ProcessManager -> {0} -> The call to WTSQueryUserToken failed and querying the process' account identity results in an identity which does not match 'NT AUTHORITY\\SYSTEM' but instead returns the name: {1} GetLastError returns: {2}", MethodInfo.GetCurrentMethod().Name, identity.Name, lastError);
                    //      throw new Exception(message);
                    //   }

                    error = Marshal.GetLastWin32Error();
                    message = String.Format("ProcessManager -> {0} -> The call to WTSQueryUserToken failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                    //}

                    //No one logged in so let's just do this the simple way
                    //return SimpleProcessStart(fileName, arguments, workingDirectory);
                }

                if (!ProcessNativeMethods.DuplicateTokenEx(userToken, TokenAccess.TOKEN_ASSIGN_PRIMARY | TokenAccess.TOKEN_ALL_ACCESS, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out primaryToken))
                {
                    error = Marshal.GetLastWin32Error();
                    message = String.Format("ProcessManager -> {0} -> The call to DuplicateTokenEx failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                //Create an appropriate environment block for this user token (if we have one)
                IntPtr ptrEnvironment = IntPtr.Zero;
                if (!ProcessNativeMethods.CreateEnvironmentBlock(out ptrEnvironment, primaryToken, false))
                {
                    error = Marshal.GetLastWin32Error();
                    message = String.Format("ProcessManager -> {0} -> The call to CreateEnvironmentBlock failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                sa.Length = Marshal.SizeOf(sa);
                si.cb = Marshal.SizeOf(si);

                //DO NOT set this to "winsta0\\default" (even though many online resources say to do so)
                //si.lpDesktop = String.Empty;
                profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                profileInfo.lpUserName = activeUserName;


                //Remember, sometimes nobody is logged in (especially when we're set to Automatically startup)
                if (!ProcessNativeMethods.LoadUserProfile(primaryToken, ref profileInfo))
                {
                    error = Marshal.GetLastWin32Error();
                    message = String.Format("ProcessManager -> {0} -> The call to LoadUserProfile failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                String commandLine = "\"" + fileName + "\"";
                commandLine = (!String.IsNullOrEmpty(arguments)) ? (commandLine + " " + arguments) : commandLine;
                if (!ProcessNativeMethods.CreateProcessAsUser(primaryToken, null, commandLine, ref sa, ref sa, false, CreationFlags.CREATE_NO_WINDOW | CreationFlags.NORMAL_PRIORITY_CLASS | CreationFlags.CREATE_UNICODE_ENVIRONMENT, ptrEnvironment, workingDirectory, ref si, ref pi))
                {
                    error = Marshal.GetLastWin32Error();
                    message = String.Format("ProcessManager -> {0} -> The call to CreateProcessAsUser failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                //if (!ProcessNativeMethods.CreateProcessWithTokenW(primaryToken, LogonFlags.LOGON_WITH_PROFILE, null, commandLine, CreationFlags.CREATE_NO_WINDOW | CreationFlags.NORMAL_PRIORITY_CLASS | CreationFlags.CREATE_UNICODE_ENVIRONMENT, ptrEnvironment, workingDirectory, ref si, ref pi))
                //{
                //   error = Marshal.GetLastWin32Error();
                //   message = String.Format("ProcessManager -> {0} -> The call to CreateProcessWithTokenW failed, GetLastError returns: {1}", methodName, error);
                //   throw new Win32Exception(error, message);
                //}

                processID = pi.dwProcessID;
            }
            finally
            {
                if (pi.hProcess != IntPtr.Zero) ProcessNativeMethods.CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero) ProcessNativeMethods.CloseHandle(pi.hThread);
            }

            return processID;
        }

        public static void SetProcessTokenPrivileges(Int32 processId, String tokenPrivilege)
        {
            var process = Process.GetProcessById(processId);
            SetProcessTokenPrivileges(process.Handle, tokenPrivilege);
        }

        public static void SetProcessTokenPrivileges(IntPtr processHandle, String tokenPrivilege)
        {
            var hToken = IntPtr.Zero;
            try
            {
                var methodName = MethodInfo.GetCurrentMethod().Name;
                if (!ProcessNativeMethods.OpenProcessToken(processHandle, TokenAccess.TOKEN_ADJUST_PRIVILEGES | TokenAccess.TOKEN_QUERY, out hToken))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = String.Format("ProcessManager -> {0} -> The call to OpenProcessToken failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                LUID restoreLUID;
                TOKEN_PRIVILEGES tokenPrivileges;
                if (!ProcessNativeMethods.LookupPrivilegeValue(String.Empty, tokenPrivilege, out restoreLUID))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = String.Format("ProcessManager -> {0} -> The call to LookupPrivilegeValue failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }

                tokenPrivileges.m_nPrivilegeCount = 1;
                tokenPrivileges.m_oLUID = restoreLUID;
                tokenPrivileges.m_nAttributes = ProcessNativeConstants.SE_PRIVILEGE_ENABLED;

                if (!ProcessNativeMethods.AdjustTokenPrivileges(hToken, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = String.Format("ProcessManager -> {0} -> The call to AdjustTokenPrivileges failed, GetLastError returns: {1}", methodName, error);
                    throw new Win32Exception(error, message);
                }
            }
            finally
            {
                if (hToken != IntPtr.Zero) ProcessNativeMethods.CloseHandle(hToken);
            }
        }

        public static Int32 StartProcess(String fileName, String arguments, String workingDirectory)
        {
            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            return process.Start() ? process.Id : -1;
        }

        public static Int32 StartProcessAsActiveUser(String fileName, String arguments, String workingDirectory)
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (identity.Name != "NT AUTHORITY\\SYSTEM") SetProcessTokenPrivileges(Process.GetCurrentProcess().Handle, "SeTcbPrivilege");
            return CreateUIProcessForServiceRunningAsLocalSystem(fileName, arguments, workingDirectory);
        }

        public static void CloseAllWindowsOfProcess(Int32 processId)
        {
            EnumWindowsCallbackDelegate d = delegate(IntPtr hWnd, UInt32 lParam)
            {
                UInt32 pid;
                ProcessNativeMethods.GetWindowThreadProcessId(hWnd, out pid);
                if ((Int32)pid == processId) ProcessNativeMethods.PostMessage(hWnd, ProcessNativeConstants.WM_CLOSE, 0, 0);
                return true;
            };

            ProcessNativeMethods.EnumWindows(d, 0);
        }

        public static void KillProcess(Int32 processId)
        {
            var process = Process.GetProcessById(processId);
            process.Kill();
        }

        public static void KillProcessSafely(Int32 processId, Int32 waitTimeout)
        {
            var process = Process.GetProcessById(processId);
            CloseAllWindowsOfProcess(process.Id);
            process.WaitForExit(waitTimeout);
            if (!process.HasExited) process.Kill();
        }
    }
}
