using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utilities.Diagnostics
{
    internal static class WindowsServiceNativeMethods
    {
        /// <summary>
        /// The OpenSCManager function establishes a connection to the service control manager on the specified computer and opens the specified service control manager database.
        /// </summary>
        /// <param name="lpMachineName">[in] Pointer to a null-terminated string that specifies the name of the target computer. If the pointer is NULL or points to an empty string, the function connects to the service control manager on the local computer.</param>
        /// <param name="lpDatabaseName">[in] Pointer to a null-terminated string that specifies the name of the service control manager database to open. This parameter should be set to SERVICES_ACTIVE_DATABASE. If it is NULL, the SERVICES_ACTIVE_DATABASE database is opened by default.</param>
        /// <param name="dwDesiredAccess">[in] Access to the service control manager.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified service control manager database. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenSCManager(String lpMachineName, String lpDatabaseName, UInt32 dwDesiredAccess);


        /// <summary>
        /// The OpenService function opens an existing service.
        /// </summary>
        /// <param name="hSCManager">[in] Handle to the service control manager database.</param>
        /// <param name="lpServiceName">[in] Pointer to a null-terminated string that specifies the name of the service to open. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</param>
        /// <param name="dwDesiredAccess">[in] Access to the service.</param>
        /// <returns>If the function succeeds, the return value is a handle to the service. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr hSCManager, String lpServiceName, Int32 dwDesiredAccess);


        /// <summary>
        /// The QueryServiceConfig function retrieves the configuration parameters of the specified service.
        /// </summary>
        /// <param name="hService">[in] Handle to the service.</param>
        /// <param name="lpServiceConfig">[out] Pointer to a buffer that receives the service configuration information. The format of the data is a QUERY_SERVICE_CONFIG structure.</param>
        /// <param name="cbBufSize">[in] Size of the buffer pointed to by the lpServiceConfig parameter, in bytes.</param>
        /// <param name="pcbBytesNeeded">[out] Pointer to a variable that receives the number of bytes needed to store all the configuration information, if the function fails with ERROR_INSUFFICIENT_BUFFER.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr lpServiceConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);


        /// <summary>
        /// The CloseServiceHandle function closes a handle to a service control manager or service object.
        /// </summary>
        /// <param name="hSCObject">[in] Handle to the service control manager object or the service object to close.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean CloseServiceHandle(IntPtr hSCObject);


        /// <summary>
        /// The ChangeServiceConfig function changes the configuration parameters of a service.
        /// </summary>
        /// <param name="hService">[in] Handle to the service.</param>
        /// <param name="dwServiceType">[in] Type of service. Specify SERVICE_NO_CHANGE if you are not changing the existing service type; otherwise, specify one of the following service types.</param>
        /// <param name="dwStartType">[in] Service start options. Specify SERVICE_NO_CHANGE if you are not changing the existing start type; otherwise, specify one of the following values.</param>
        /// <param name="dwErrorControl">[in] Severity of the error, and action taken, if this service fails to start. Specify SERVICE_NO_CHANGE if you are not changing the existing error control; otherwise, specify one of the following values.</param>
        /// <param name="lpBinaryPathName">[in] Pointer to a null-terminated string that contains the fully qualified path to the service binary file. Specify NULL if you are not changing the existing path.</param>
        /// <param name="lpLoadOrderGroup">[in] Pointer to a null-terminated string that names the load ordering group of which this service is a member. Specify NULL if you are not changing the existing group. Specify an empty string if the service does not belong to a group.</param>
        /// <param name="lpdwTagId">[out] Pointer to a variable that receives a tag value that is unique in the group specified in the lpLoadOrderGroup parameter. Specify NULL if you are not changing the existing tag.</param>
        /// <param name="lpDependencies">[in] Pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service can be started.</param>
        /// <param name="lpServiceStartName">[in] Pointer to a null-terminated string that specifies the name of the account under which the service should run. Specify NULL if you are not changing the existing account name.</param>
        /// <param name="lpPassword">[in] Pointer to a null-terminated string that contains the password to the account name specified by the lpServiceStartName parameter. Specify NULL if you are not changing the existing password.</param>
        /// <param name="lpDisplayName">in] Pointer to a null-terminated string that contains the display name to be used by applications to identify the service for its users. Specify NULL if you are not changing the existing display name; otherwise, this string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean ChangeServiceConfig(IntPtr hService, UInt32 dwServiceType, UInt32 dwStartType, UInt32 dwErrorControl, String lpBinaryPathName, String lpLoadOrderGroup, IntPtr lpdwTagId, String lpDependencies, String lpServiceStartName, String lpPassword, String lpDisplayName);


        /// <summary>
        /// The ChangeServiceConfig2 function changes the optional configuration parameters of a service.
        /// </summary>
        /// <param name="hService">[in] Handle to the service.</param>
        /// <param name="dwInfoLevel">in] Configuration information to be changed.</param>
        /// <param name="lpInfo">[in] Pointer to the new value to be set for the configuration information. The format of this data depends on the value of the dwInfoLevel parameter. If this value is NULL, the information remains unchanged.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean ChangeServiceConfig2(IntPtr hService, UInt32 dwInfoLevel, IntPtr lpInfo);
    }
}