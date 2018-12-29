#if NETFULL
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ServiceProcess;

namespace Utilities.Diagnostics
{
    public static class WindowsServiceManager
    {
        /// <summary>
        /// Adds a service the interactive functionality.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="makeInteractive">Parameter indicating whether service must be interactive.</param>
        public static void ChangeInteractiveState(String serviceName, Boolean makeInteractive)
        {
            ChangeService(serviceName, false, makeInteractive, String.Empty);
        }

        /// <summary>
        /// Sets a service description.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="serviceDescription">Service description.</param>
        public static void ChangeDescription(String serviceName, String serviceDescription)
        {
            ChangeService(serviceName, true, false, serviceDescription);
        }

        private static void ChangeService(String serviceName, Boolean setDescription, Boolean makeInteractive, String serviceDescription)
        {
            IntPtr handleScm = WindowsServiceNativeMethods.OpenSCManager(null, null, (UInt32)SCM_ACCESS.SC_MANAGER_ALL_ACCESS);
            if (handleScm == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
            try
            {
                IntPtr handleService = WindowsServiceNativeMethods.OpenService(handleScm, serviceName, (Int32)(SERVICE_ACCESS.SERVICE_QUERY_CONFIG | SERVICE_ACCESS.SERVICE_CHANGE_CONFIG));
                if (handleService != IntPtr.Zero)
                {
                    try
                    {
                        if (setDescription) ChangeDescriptionInternal(handleService, serviceDescription);
                        ChangeInteractiveStateInternal(handleService, makeInteractive);
                    }
                    finally
                    {
                        WindowsServiceNativeMethods.CloseServiceHandle(handleService);
                    }
                }
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                WindowsServiceNativeMethods.CloseServiceHandle(handleScm);
            }
        }

        private static void ChangeInteractiveStateInternal(IntPtr serviceHandle, Boolean makeInteractive)
        {
            UInt32 bytesNeaded = 0;
            if (!WindowsServiceNativeMethods.QueryServiceConfig(serviceHandle, IntPtr.Zero, 0, out bytesNeaded) && Marshal.GetLastWin32Error() != WindowsServiceNativeConstants.ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var ptrBuffer = IntPtr.Zero;
            ptrBuffer = Marshal.AllocHGlobal((Int32)bytesNeaded);
            if (!WindowsServiceNativeMethods.QueryServiceConfig(serviceHandle, ptrBuffer, bytesNeaded, out bytesNeaded))
            {
                if (ptrBuffer != IntPtr.Zero) Marshal.FreeHGlobal(ptrBuffer);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var serviceConf = (QUERY_SERVICE_CONFIG)Marshal.PtrToStructure(ptrBuffer, typeof(QUERY_SERVICE_CONFIG));
            if (ptrBuffer != IntPtr.Zero) Marshal.FreeHGlobal(ptrBuffer);
            if (serviceConf.lpServiceStartName != ServiceAccount.LocalSystem.ToString())
            {
                if (!WindowsServiceNativeMethods.ChangeServiceConfig(serviceHandle, WindowsServiceNativeConstants.SERVICE_NO_CHANGE, WindowsServiceNativeConstants.SERVICE_NO_CHANGE, WindowsServiceNativeConstants.SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, ServiceAccount.LocalSystem.ToString(), String.Empty, null))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            serviceConf.dwServiceType = makeInteractive ? serviceConf.dwServiceType | WindowsServiceNativeConstants.SERVICE_INTERACTIVE_PROCESS : serviceConf.dwServiceType & ~WindowsServiceNativeConstants.SERVICE_INTERACTIVE_PROCESS;
            if (!WindowsServiceNativeMethods.ChangeServiceConfig(serviceHandle, serviceConf.dwServiceType, WindowsServiceNativeConstants.SERVICE_NO_CHANGE, WindowsServiceNativeConstants.SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, null, null, null))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static void ChangeDescriptionInternal(IntPtr serviceHandle, String serviceDescription)
        {
            var serviceDescrition = new SERVICE_DESCRIPTION();
            serviceDescrition.lpDescription = serviceDescription;
            var ptrDescription = IntPtr.Zero;
            try
            {
                ptrDescription = Marshal.AllocHGlobal(Marshal.SizeOf(serviceDescrition));
                Marshal.StructureToPtr(serviceDescrition, ptrDescription, false);
                if (!WindowsServiceNativeMethods.ChangeServiceConfig2(serviceHandle, WindowsServiceNativeConstants.SERVICE_CONFIG_DESCRIPTION, ptrDescription)) throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (ptrDescription != IntPtr.Zero) Marshal.FreeHGlobal(ptrDescription);
            }
        }
    }
}
#endif