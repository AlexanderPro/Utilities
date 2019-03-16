using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Diagnostics
{
    internal static class ProcessNativeMethods
    {
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", SetLastError = true)]
        public static extern Boolean AdjustTokenPrivileges(IntPtr tokenHandle, [MarshalAs(UnmanagedType.Bool)]Boolean disableAllPrivileges, ref TOKEN_PRIVILEGES newState, UInt32 bufferLength, IntPtr previousState, IntPtr returnLength);

        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", SetLastError = true)]
        public static extern Boolean OpenProcessToken(IntPtr processHandle, TokenAccess desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValue", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean LookupPrivilegeValue(String systemName, String name, out LUID lpLuid);

        [DllImport("userenv.dll", EntryPoint = "CreateEnvironmentBlock", SetLastError = true)]
        public static extern Boolean CreateEnvironmentBlock(out IntPtr environmentBlock, IntPtr tokenHandle, Boolean inheritProcessEnvironment);

        [DllImport("userenv.dll", EntryPoint = "DestroyEnvironmentBlock", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        public static extern Boolean CloseHandle(IntPtr handle);

        [DllImport("wtsapi32.dll", EntryPoint = "WTSQueryUserToken", SetLastError = true)]
        public static extern Boolean WTSQueryUserToken(UInt32 sessionId, out IntPtr tokenHandle);

        [DllImport("kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId", SetLastError = true)]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("Wtsapi32.dll", EntryPoint = "WTSQuerySessionInformation", SetLastError = true)]
        public static extern Boolean WTSQuerySessionInformation(IntPtr serverHandle, Int32 sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out UInt32 pBytesReturned);

        [DllImport("wtsapi32.dll", EntryPoint = "WTSFreeMemory", SetLastError = false)]
        public static extern void WTSFreeMemory(IntPtr memory);

        [DllImport("userenv.dll", EntryPoint = "LoadUserProfile", SetLastError = true)]
        public static extern Boolean LoadUserProfile(IntPtr tokenHandle, ref PROFILEINFO profileInfo);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean CreateProcessAsUser(IntPtr userTokenHandle, String applicationName, String commandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, Boolean inheritHandles, CreationFlags in_eCreationFlags, IntPtr environmentBlock, String currentDirectory, ref STARTUPINFO startupInfo, ref PROCESS_INFORMATION processInformation);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessWithTokenW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean CreateProcessWithTokenW(IntPtr userTokenHandle, LogonFlags logonFlags, String applicationName, String commandLine, CreationFlags creationFlags, IntPtr environmentBlock, String currentDirectory, ref STARTUPINFO startupInfo, ref PROCESS_INFORMATION processInformation);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx", SetLastError = true, CharSet = CharSet.Auto)]
        public extern static Boolean DuplicateTokenEx(IntPtr existingTokenHandle, TokenAccess desiredAccess, IntPtr tokenAttributes, SECURITY_IMPERSONATION_LEVEL impersonationLevel, TOKEN_TYPE tokenType, out IntPtr newTokenHandle);

        [DllImport("user32.dll", EntryPoint = "PostThreadMessage", SetLastError = true)]
        public static extern Boolean PostThreadMessage(Int32 threadId, UInt32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
        public static extern Int32 PostMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        public static extern void EnumWindows(EnumWindowsCallbackDelegate d, UInt32 lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 lpdwProcessId);
    }
}