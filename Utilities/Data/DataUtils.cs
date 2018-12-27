using System;
using System.Data;
using System.Data.Common;
using Utilities.Reflection;

namespace Utilities.Data
{
    public static class DataUtils
    {
        /// <summary>
        /// Returns a provider factory using the old Provider Model names from full framework .NET.
        /// Simply calls DbProviderFactories.
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(string providerName)
        {
#if NETFULL
            return DbProviderFactories.GetFactory(providerName);
#else
            var lowerProvider = providerName.ToLower();

            if (lowerProvider == "system.data.sqlclient")
                return GetDbProviderFactory(DataAccessProviderTypes.SqlServer);
            if (lowerProvider == "system.data.sqlite" || lowerProvider == "microsoft.data.sqlite")
                return GetDbProviderFactory(DataAccessProviderTypes.SqLite);
            if (lowerProvider == "mysql.data.mysqlclient" || lowerProvider == "mysql.data")
                return GetDbProviderFactory(DataAccessProviderTypes.MySql);
            if (lowerProvider == "npgsql")
                return GetDbProviderFactory(DataAccessProviderTypes.PostgreSql);

            throw new NotSupportedException(string.Format("Unsupported Provider Factory specified: {0}", providerName));
#endif
        }

        /// <summary>
        /// This method loads various providers dynamically similar to the 
        /// way that DbProviderFactories.GetFactory() works except that
        /// this API is not available on .NET Standard 2.0
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(DataAccessProviderTypes type)
        {
            if (type == DataAccessProviderTypes.SqlServer)
                return GetDbProviderFactory("System.Data.SqlClient.SqlClientFactory", "System.Data.SqlClient");

            if (type == DataAccessProviderTypes.SqLite)
            {
                // SqLite support in .NET Standard available now
                return GetDbProviderFactory("System.Data.SQLite.SQLiteFactory", "System.Data.SQLite");
                //#if NETFULL
                //#else
                //                return GetDbProviderFactory("Microsoft.Data.Sqlite.SqliteFactory", "Microsoft.Data.Sqlite");
                //#endif
            }
            if (type == DataAccessProviderTypes.MySql)
                return GetDbProviderFactory("MySql.Data.MySqlClient.MySqlClientFactory", "MySql.Data");
            if (type == DataAccessProviderTypes.PostgreSql)
                return GetDbProviderFactory("Npgsql.NpgsqlFactory", "Npgsql");
#if NETFULL
            if (type == DataAccessProviderTypes.OleDb)
                return System.Data.OleDb.OleDbFactory.Instance;
            if (type == DataAccessProviderTypes.SqlServerCompact)
                return DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");                
#endif

            throw new NotSupportedException(string.Format("Unsupported Provider Factory specified: {0}", type.ToString()));
        }


        /// <summary>
        /// Loads a SQL Provider factory based on the DbFactory type name and assembly.       
        /// </summary>
        /// <param name="dbProviderFactoryTypename">Type name of the DbProviderFactory</param>
        /// <param name="assemblyName">Short assembly name of the provider factory. Note: Host project needs to have a reference to this assembly</param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(string dbProviderFactoryTypename, string assemblyName)
        {
            var instance = ReflectionUtils.GetStaticProperty(dbProviderFactoryTypename, "Instance");
            if (instance == null)
            {
                var assembly = ReflectionUtils.LoadAssembly(assemblyName);
                if (assembly != null)
                {
                    instance = ReflectionUtils.GetStaticProperty(dbProviderFactoryTypename, "Instance");
                }
            }

            if (instance == null)
            {
                throw new InvalidOperationException(string.Format("Unable to retrieve DbProviderFactory for: {0}", dbProviderFactoryTypename));
            }

            return instance as DbProviderFactory;
        }
    }

    public enum DataAccessProviderTypes
    {
        SqlServer,
        SqLite,
        MySql,
        PostgreSql,

#if NETFULL
        OleDb,
        SqlServerCompact
#endif
    }
}
