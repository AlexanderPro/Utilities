#if NETFULL
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;

namespace Utilities.Reflection
{
    /// <summary>
    /// Provides access to System.Web.HttpContext.Current properties.
    /// </summary>
    public static class HttpContext
    {
        private static readonly PropertyInfo _httpContextPropertyInfo;
        private static readonly PropertyInfo _httpRequestPropertyInfo;
        private static readonly PropertyInfo _httpServerPropertyInfo;
        private static readonly PropertyInfo _httpRequestUrlPropertyInfo;
        private static readonly PropertyInfo _httpRequestServerVariablesPropertyInfo;
        private static readonly MethodInfo _httpMapPathMethodInfo;


        /// <summary>
        /// Attempts to load and cache System.Web.HttpContext.Current properties.
        /// </summary>
        static HttpContext()
        {
            try
            {
                var systemWebAssembly = Assembly.Load("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                if (systemWebAssembly == null) return;

                var httpContextType = systemWebAssembly.GetType("System.Web.HttpContext", false, true);
                if (httpContextType == null) return;

                _httpContextPropertyInfo = httpContextType.GetProperty("Current", (BindingFlags.Public | BindingFlags.Static));
                if (_httpContextPropertyInfo == null) return;

                var currentHttpContext = _httpContextPropertyInfo.GetValue(null, null);
                _httpRequestPropertyInfo = httpContextType.GetProperty("Request", (BindingFlags.Public | BindingFlags.Instance));
                var requestType = _httpRequestPropertyInfo.GetValue(currentHttpContext, null).GetType();

                _httpServerPropertyInfo = httpContextType.GetProperty("Server", (BindingFlags.Public | BindingFlags.Instance));
                var serverType = _httpServerPropertyInfo.GetValue(currentHttpContext, null).GetType();

                _httpRequestUrlPropertyInfo = requestType.GetProperty("Url", (BindingFlags.Public | BindingFlags.Instance));
                if (_httpRequestUrlPropertyInfo == null) return;

                _httpRequestServerVariablesPropertyInfo = requestType.GetProperty("ServerVariables", (BindingFlags.Public | BindingFlags.Instance));
                if (_httpRequestServerVariablesPropertyInfo == null) return;

                _httpMapPathMethodInfo = serverType.GetMethod("MapPath", (BindingFlags.Public | BindingFlags.Instance));
            }
            catch (Exception ex)
            {
                var message = String.Format("An error occurred attempting to get the current HttpContext. Exception: {0}", ex.Message);
                Trace.WriteLine(message);
            }
        }

        /// <summary>
        /// Gets the current HttpContext.
        /// </summary>
        /// <returns>Reference to the current HttpContext.</returns>
        public static Object GetCurrentHttpContext()
        {
            if (_httpContextPropertyInfo != null)
            {
                var currentHttpContext = _httpContextPropertyInfo.GetValue(null, null);
                return currentHttpContext;
            }

            return null;
        }

        /// <summary>
        /// Retrieves information about the URL of the current request from the current HttpContext.
        /// </summary>
        /// <returns>A <see cref="Uri"/> object containing information regarding the URL of the current request.</returns>
        public static Uri GetRequestUrl()
        {
            if (_httpContextPropertyInfo != null && _httpRequestPropertyInfo != null && _httpRequestUrlPropertyInfo != null)
            {
                var currentHttpContext = _httpContextPropertyInfo.GetValue(null, null);
                if (currentHttpContext != null)
                {
                    var currentHttpRequest = _httpRequestPropertyInfo.GetValue(currentHttpContext, null);
                    if (currentHttpRequest != null)
                    {
                        var url = _httpRequestUrlPropertyInfo.GetValue(currentHttpRequest, null) as Uri;
                        return url;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a Web server variable value.
        /// </summary>
        /// <returns>Server variable value.</returns>
        public static String GetRequestServerVariableValue(String key)
        {
            if (_httpContextPropertyInfo != null && _httpRequestUrlPropertyInfo != null && _httpRequestServerVariablesPropertyInfo != null)
            {
                var currentHttpContext = _httpContextPropertyInfo.GetValue(null, null);
                if (currentHttpContext != null)
                {
                    var currentHttpRequest = _httpRequestPropertyInfo.GetValue(currentHttpContext, null);
                    if (currentHttpRequest != null)
                    {
                        var items = _httpRequestServerVariablesPropertyInfo.GetValue(currentHttpRequest, null) as NameValueCollection;
                        if (items != null)
                        {
                            var value = items[key];
                            if (value != null)
                            {
                                return value;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the physical file path that corresponds to the specified virtual path.
        /// </summary>
        /// <param name="path">The physical file path on the Web server that corresponds to path.</param>
        public static String MapPath(String path)
        {
            if (_httpContextPropertyInfo != null && _httpServerPropertyInfo != null && _httpMapPathMethodInfo != null)
            {
                var currentHttpContext = _httpContextPropertyInfo.GetValue(null, null);
                if (currentHttpContext != null)
                {
                    var currentServer = _httpServerPropertyInfo.GetValue(currentHttpContext, null);
                    if (currentServer != null)
                    {
                        var physicalPath = _httpMapPathMethodInfo.Invoke(currentServer, new Object[] { path }) as String;
                        return physicalPath;
                    }
                }
            }
            return null;
        }
    }
}
#endif