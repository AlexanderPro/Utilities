using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Data.Odbc;

namespace Utilities.SQL
{
    public class SQLHelper : IDisposable
    {
        #region Fields.Private

        private DbProviderFactory _factory;

        #endregion


        #region Properties.Public

        public DbConnection Connection { get; private set; }

        public DbCommand Command { get; private set; }

        public Boolean HandleExceptions { get; set; }

        public Exception LastException { get; private set; }

        #endregion


        #region Constructors

        public SQLHelper(String connectionString, Provider provider)
            : this(connectionString, provider, true)
        {

        }

        public SQLHelper(String connectionString, String providerInvariantName)
            : this(connectionString, providerInvariantName, true)
        {

        }

        public SQLHelper(String connectionString, Provider provider, Boolean handleExceptions)
        {
            switch (provider)
            {
                case Provider.SqlServer:
                    {
                        _factory = SqlClientFactory.Instance;
                    }
                    break;

                case Provider.Oracle:
                    {
#pragma warning disable 0618
                        _factory = OracleClientFactory.Instance;
#pragma warning restore 0618
                    }
                    break;

                case Provider.OleDb:
                    {
                        _factory = OleDbFactory.Instance;
                    }
                    break;

                case Provider.ODBC:
                    {
                        _factory = OdbcFactory.Instance;
                    }
                    break;
            }

            Connection = _factory.CreateConnection();
            Command = _factory.CreateCommand();
            Connection.ConnectionString = connectionString;
            Command.Connection = Connection;
            HandleExceptions = handleExceptions;
        }

        public SQLHelper(String connectionString, String providerInvariantName, Boolean handleExceptions)
        {
            _factory = DbProviderFactories.GetFactory(providerInvariantName);
            Connection = _factory.CreateConnection();
            Command = _factory.CreateCommand();
            Connection.ConnectionString = connectionString;
            Command.Connection = Connection;
            HandleExceptions = handleExceptions;
        }

        #endregion


        #region Methods.Public

        public void BeginTransaction()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Command.Transaction = Connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Command.Transaction.Commit();
            Connection.Close();
        }

        public void RollbackTransaction()
        {
            Command.Transaction.Rollback();
            Connection.Close();
        }


        public Int32 AddParameter(DbParameter parameter)
        {
            return Command.Parameters.Add(parameter);
        }

        public Int32 AddParameter(String name, ParameterDirection direction)
        {
            DbParameter parameter = _factory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            return Command.Parameters.Add(parameter);
        }

        public Int32 AddParameter(String name, ParameterDirection direction, Object value)
        {
            DbParameter parameter = _factory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            parameter.Value = value;
            return Command.Parameters.Add(parameter);
        }


        public Int32 ExecuteNonQuery(String query)
        {
            return ExecuteNonQuery(query, CommandType.Text, ConnectionMode.CloseOnExit, true);
        }

        public Int32 ExecuteNonQuery(String query, Boolean autoClearParameters)
        {
            return ExecuteNonQuery(query, CommandType.Text, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public Int32 ExecuteNonQuery(String query, ConnectionMode connectionMode)
        {
            return ExecuteNonQuery(query, CommandType.Text, connectionMode, true);
        }

        public Int32 ExecuteNonQuery(String query, CommandType commandType)
        {
            return ExecuteNonQuery(query, commandType, ConnectionMode.CloseOnExit, true);
        }

        public Int32 ExecuteNonQuery(String query, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            return ExecuteNonQuery(query, CommandType.Text, connectionMode, autoClearParameters);
        }

        public Int32 ExecuteNonQuery(String query, CommandType commandType, Boolean autoClearParameters)
        {
            return ExecuteNonQuery(query, commandType, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public Int32 ExecuteNonQuery(String query, CommandType commandType, ConnectionMode connectionMode)
        {
            return ExecuteNonQuery(query, commandType, connectionMode, true);
        }

        public Int32 ExecuteNonQuery(String query, CommandType commandType, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            Command.CommandText = query;
            Command.CommandType = commandType;
            LastException = null;
            Int32 i = -1;
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                i = Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                if (autoClearParameters == true)
                {
                    Command.Parameters.Clear();
                }
                if (connectionMode == ConnectionMode.CloseOnExit)
                {
                    Connection.Close();
                }
            }
            return i;
        }


        public Object ExecuteScalar(String query)
        {
            return ExecuteScalar(query, CommandType.Text, ConnectionMode.CloseOnExit, true);
        }

        public Object ExecuteScalar(String query, Boolean autoClearParameters)
        {
            return ExecuteScalar(query, CommandType.Text, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public Object ExecuteScalar(String query, ConnectionMode connectionMode)
        {
            return ExecuteScalar(query, CommandType.Text, connectionMode, true);
        }

        public Object ExecuteScalar(String query, CommandType commandType)
        {
            return ExecuteScalar(query, commandType, ConnectionMode.CloseOnExit, true);
        }

        public Object ExecuteScalar(String query, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            return ExecuteScalar(query, CommandType.Text, connectionMode, autoClearParameters);
        }

        public Object ExecuteScalar(String query, CommandType commandType, Boolean autoClearParameters)
        {
            return ExecuteScalar(query, commandType, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public Object ExecuteScalar(String query, CommandType commandType, ConnectionMode connectionMode)
        {
            return ExecuteScalar(query, commandType, connectionMode, true);
        }

        public Object ExecuteScalar(String query, CommandType commandType, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            Command.CommandText = query;
            Command.CommandType = commandType;
            LastException = null;
            Object o = null;
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                o = Command.ExecuteScalar();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                if (autoClearParameters == true)
                {
                    Command.Parameters.Clear();
                }
                if (connectionMode == ConnectionMode.CloseOnExit)
                {
                    Connection.Close();
                }
            }
            return o;
        }


        public DbDataReader ExecuteReader(String query)
        {
            return ExecuteReader(query, CommandType.Text, ConnectionMode.CloseOnExit, true);
        }

        public DbDataReader ExecuteReader(String query, Boolean autoClearParameters)
        {
            return ExecuteReader(query, CommandType.Text, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public DbDataReader ExecuteReader(String query, ConnectionMode connectionMode)
        {
            return ExecuteReader(query, CommandType.Text, connectionMode, true);
        }

        public DbDataReader ExecuteReader(String query, CommandType commandType)
        {
            return ExecuteReader(query, commandType, ConnectionMode.CloseOnExit, true);
        }

        public DbDataReader ExecuteReader(String query, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            return ExecuteReader(query, CommandType.Text, connectionMode, autoClearParameters);
        }

        public DbDataReader ExecuteReader(String query, CommandType commandType, Boolean autoClearParameters)
        {
            return ExecuteReader(query, commandType, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public DbDataReader ExecuteReader(String query, CommandType commandType, ConnectionMode connectionMode)
        {
            return ExecuteReader(query, commandType, connectionMode, true);
        }

        public DbDataReader ExecuteReader(String query, CommandType commandType, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            Command.CommandText = query;
            Command.CommandType = commandType;
            LastException = null;
            DbDataReader reader = null;
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }

                if (connectionMode == ConnectionMode.CloseOnExit)
                {
                    reader = Command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    reader = Command.ExecuteReader();
                }
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                if (autoClearParameters == true)
                {
                    Command.Parameters.Clear();
                }
            }
            return reader;
        }


        public DataSet ExecuteDataSet(String query)
        {
            return ExecuteDataSet(query, CommandType.Text, ConnectionMode.CloseOnExit, true);
        }

        public DataSet ExecuteDataSet(String query, Boolean autoClearParameters)
        {
            return ExecuteDataSet(query, CommandType.Text, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public DataSet ExecuteDataSet(String query, ConnectionMode connectionMode)
        {
            return ExecuteDataSet(query, CommandType.Text, connectionMode, true);
        }

        public DataSet ExecuteDataSet(String query, CommandType commandType)
        {
            return ExecuteDataSet(query, commandType, ConnectionMode.CloseOnExit, true);
        }

        public DataSet ExecuteDataSet(String query, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            return ExecuteDataSet(query, CommandType.Text, connectionMode, autoClearParameters);
        }

        public DataSet ExecuteDataSet(String query, CommandType commandType, Boolean autoClearParameters)
        {
            return ExecuteDataSet(query, commandType, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public DataSet ExecuteDataSet(String query, CommandType commandType, ConnectionMode connectionMode)
        {
            return ExecuteDataSet(query, commandType, connectionMode, true);
        }

        public DataSet ExecuteDataSet(String query, CommandType commandType, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            DbDataAdapter adapter = _factory.CreateDataAdapter();
            Command.CommandText = query;
            Command.CommandType = commandType;
            adapter.SelectCommand = Command;
            LastException = null;
            DataSet ds = new DataSet();
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                if (autoClearParameters == true)
                {
                    Command.Parameters.Clear();
                }
                if (connectionMode == ConnectionMode.CloseOnExit)
                {
                    Connection.Close();
                }
            }
            return ds;
        }


        public DataTable ExecuteDataTable(String query)
        {
            return ExecuteDataTable(query, CommandType.Text, ConnectionMode.CloseOnExit, true);
        }

        public DataTable ExecuteDataTable(String query, Boolean autoClearParameters)
        {
            return ExecuteDataTable(query, CommandType.Text, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public DataTable ExecuteDataTable(String query, ConnectionMode connectionMode)
        {
            return ExecuteDataTable(query, CommandType.Text, connectionMode, true);
        }

        public DataTable ExecuteDataTable(String query, CommandType commandType)
        {
            return ExecuteDataTable(query, commandType, ConnectionMode.CloseOnExit, true);
        }

        public DataTable ExecuteDataTable(String query, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            return ExecuteDataTable(query, CommandType.Text, connectionMode, autoClearParameters);
        }

        public DataTable ExecuteDataTable(String query, CommandType commandType, Boolean autoClearParameters)
        {
            return ExecuteDataTable(query, commandType, ConnectionMode.CloseOnExit, autoClearParameters);
        }

        public DataTable ExecuteDataTable(String query, CommandType commandType, ConnectionMode connectionMode)
        {
            return ExecuteDataTable(query, commandType, connectionMode, true);
        }

        public DataTable ExecuteDataTable(String query, CommandType commandType, ConnectionMode connectionMode, Boolean autoClearParameters)
        {
            DbDataAdapter adapter = _factory.CreateDataAdapter();
            Command.CommandText = query;
            Command.CommandType = commandType;
            adapter.SelectCommand = Command;
            LastException = null;
            DataTable dt = new DataTable();
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                adapter.Fill(dt);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                if (autoClearParameters == true)
                {
                    Command.Parameters.Clear();
                }
                if (connectionMode == ConnectionMode.CloseOnExit)
                {
                    Connection.Close();
                }
            }
            return dt;
        }


        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
            Command.Dispose();
        }

        #endregion


        #region Methods.Private

        private void HandleException(Exception e)
        {
            if (HandleExceptions)
            {
                LastException = e;
            }
            else
            {
                throw e;
            }
        }

        #endregion
    }

    public enum Provider
    {
        SqlServer = 0x01,
        Oracle = 0x02,
        OleDb = 0x03,
        ODBC = 0x04
    }

    public enum ConnectionMode
    {
        KeepOpen = 0x01,
        CloseOnExit = 0x02
    }
}