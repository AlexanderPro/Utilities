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
        private static int StartProcessAsCurrentUser(string fileName, string arguments, string workingDirectory)
        {
            var procInfo = new PROCESS_INFORMATION();
            var userToken = IntPtr.Zero;
            var primaryToken = IntPtr.Zero;
            var ptrEnvironment = IntPtr.Zero;

            try
            {
                var activeUserSessionId = ProcessNativeMethods.WTSGetActiveConsoleSessionId();
                if (activeUserSessionId == 0xFFFFFFFF)
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("StartProcessAsCurrentUser: WTSGetActiveConsoleSessionId failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                var buffer = IntPtr.Zero;
                var bytesReturned = (UInt32)0;
                if (!ProcessNativeMethods.WTSQuerySessionInformation(IntPtr.Zero, (int)activeUserSessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out bytesReturned))
                {
                    var error = Marshal.GetLastWin32Error();
                    //On earlier operating systems from Vista, when no one is logged in, you get RPC_S_INVALID_BINDING which is ok, we just won't impersonate
                    if (error != ProcessNativeConstants.RPC_S_INVALID_BINDING)
                    {
                        var message = string.Format("StartProcessAsCurrentUser: WTSQuerySessionInformation failed, Error: {0}", error);
                        throw new Win32Exception(error, message);
                    }

                    //No one logged in so let's just do this the simple way
                    return StartProcess(fileName, arguments, workingDirectory);
                }

                var activeUserName = Marshal.PtrToStringAnsi(buffer);
                ProcessNativeMethods.WTSFreeMemory(buffer);

                //We are supposedly running as a service so we're going to be running in session 0 so get a user token from the active user session
                if (!ProcessNativeMethods.WTSQueryUserToken(activeUserSessionId, out userToken))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("StartProcessAsCurrentUser: WTSQueryUserToken failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                // Convert the impersonation token to a primary token
                if (!ProcessNativeMethods.DuplicateTokenEx(userToken, TokenAccess.TOKEN_ASSIGN_PRIMARY | TokenAccess.TOKEN_ALL_ACCESS, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out primaryToken))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("StartProcessAsCurrentUser: DuplicateTokenEx failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                //Create an appropriate environment block for this user token (if we have one)
                if (!ProcessNativeMethods.CreateEnvironmentBlock(out ptrEnvironment, primaryToken, false))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("StartProcessAsCurrentUser: CreateEnvironmentBlock failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                var profileInfo = new PROFILEINFO();
                profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                profileInfo.lpUserName = activeUserName;

                //Remember, sometimes nobody is logged in (especially when we're set to Automatically startup)
                if (!ProcessNativeMethods.LoadUserProfile(primaryToken, ref profileInfo))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("StartProcessAsCurrentUser: LoadUserProfile failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                var startInfo = new STARTUPINFO();
                startInfo.cb = Marshal.SizeOf(startInfo);
                //DO NOT set this to "winsta0\\default" (even though many online resources say to do so)
                //startInfo.lpDesktop = string.Empty;

                var commandLine = "\"" + fileName + "\"";
                commandLine = !string.IsNullOrEmpty(arguments) ? (commandLine + " " + arguments) : commandLine;
                if (!ProcessNativeMethods.CreateProcessAsUser(primaryToken, null, commandLine, IntPtr.Zero, IntPtr.Zero, false, CreationFlags.CREATE_NO_WINDOW | CreationFlags.NORMAL_PRIORITY_CLASS | CreationFlags.CREATE_UNICODE_ENVIRONMENT, ptrEnvironment, workingDirectory, ref startInfo, ref procInfo))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("StartProcessAsCurrentUser: CreateProcessAsUser failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                return procInfo.dwProcessID;
            }
            finally
            {
                if (userToken != IntPtr.Zero)
                {
                    ProcessNativeMethods.CloseHandle(userToken);
                }

                if (primaryToken != IntPtr.Zero)
                {
                    ProcessNativeMethods.CloseHandle(primaryToken);
                }

                if (ptrEnvironment != IntPtr.Zero)
                {
                    ProcessNativeMethods.DestroyEnvironmentBlock(ptrEnvironment);
                }

                if (procInfo.hProcess != IntPtr.Zero)
                {
                    ProcessNativeMethods.CloseHandle(procInfo.hProcess);
                }

                if (procInfo.hThread != IntPtr.Zero)
                {
                    ProcessNativeMethods.CloseHandle(procInfo.hThread);
                }
            }
        }

        public static void SetProcessTokenPrivileges(int processId, string tokenPrivilege)
        {
            var process = Process.GetProcessById(processId);
            SetProcessTokenPrivileges(process.Handle, tokenPrivilege);
        }

        public static void SetProcessTokenPrivileges(IntPtr processHandle, string tokenPrivilege)
        {
            var hToken = IntPtr.Zero;
            try
            {
                if (!ProcessNativeMethods.OpenProcessToken(processHandle, TokenAccess.TOKEN_ADJUST_PRIVILEGES | TokenAccess.TOKEN_QUERY, out hToken))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("SetProcessTokenPrivileges: OpenProcessToken failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                LUID restoreLUID;
                TOKEN_PRIVILEGES tokenPrivileges;
                if (!ProcessNativeMethods.LookupPrivilegeValue(string.Empty, tokenPrivilege, out restoreLUID))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("SetProcessTokenPrivileges: LookupPrivilegeValue failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }

                tokenPrivileges.m_nPrivilegeCount = 1;
                tokenPrivileges.m_oLUID = restoreLUID;
                tokenPrivileges.m_nAttributes = ProcessNativeConstants.SE_PRIVILEGE_ENABLED;
                if (!ProcessNativeMethods.AdjustTokenPrivileges(hToken, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
                {
                    var error = Marshal.GetLastWin32Error();
                    var message = string.Format("SetProcessTokenPrivileges: AdjustTokenPrivileges failed, Error: {0}", error);
                    throw new Win32Exception(error, message);
                }
            }
            finally
            {
                if (hToken != IntPtr.Zero)
                {
                    ProcessNativeMethods.CloseHandle(hToken);
                }
            }
        }

        public static int StartProcess(string fileName, string arguments, string workingDirectory)
        {
            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            return process.Start() ? process.Id : -1;
        }

        public static int SetPrivilegesAndStartProcessAsCurrentUser(string fileName, string arguments, string workingDirectory)
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity.Name != "NT AUTHORITY\\SYSTEM")
            {
                SetProcessTokenPrivileges(Process.GetCurrentProcess().Handle, "SeTcbPrivilege");
            }
            return StartProcessAsCurrentUser(fileName, arguments, workingDirectory);
        }

        public static void CloseAllWindowsOfProcess(int processId)
        {
            EnumWindowsCallbackDelegate d = delegate(IntPtr hWnd, UInt32 lParam)
            {
                UInt32 pid;
                ProcessNativeMethods.GetWindowThreadProcessId(hWnd, out pid);
                if ((int)pid == processId) ProcessNativeMethods.PostMessage(hWnd, ProcessNativeConstants.WM_CLOSE, 0, 0);
                return true;
            };

            ProcessNativeMethods.EnumWindows(d, 0);
        }

        public static void KillProcess(int processId)
        {
            var process = Process.GetProcessById(processId);
            process.Kill();
        }

        public static void KillProcessSafely(int processId, int waitTimeout)
        {
            var process = Process.GetProcessById(processId);
            CloseAllWindowsOfProcess(process.Id);
            process.WaitForExit(waitTimeout);
            if (!process.HasExited) process.Kill();
        }
    }
}
