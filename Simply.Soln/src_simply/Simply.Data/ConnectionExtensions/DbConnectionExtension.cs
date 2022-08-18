using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Enums;
using Simply.Data.Interfaces;
using Simply.Data.QuerySettings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbConnectionExtension"/>.
    /// </summary>
    public static class DbConnectionExtension
    {
        /// <summary>
        /// The database comand pairs
        /// </summary>
        private static readonly ConcurrentDictionary<byte, Type> dbCommandParameterTypePairs = new ConcurrentDictionary<byte, Type>();

        /// <summary>
        /// Gets Server Version of database connection.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <returns>The server version of db connection.</returns>
        public static string GetServerVersion(this IDbConnection connection)
        {
            CheckConnectionIsNull(connection);

            PropertyInfo property = connection.GetType().GetProperty(InternalAppValues.ServerVersion);
            object versionValue = property?.GetValue(connection, null);
            string version = versionValue.ToStr();
            return version;
        }

        /// <summary>
        /// Gets Connection String Builder of database connection.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <returns>The connection string builder.</returns>
        public static DbConnectionStringBuilder GetConnectionStringBuilder(this IDbConnection connection)
        {
            CheckConnectionIsNull(connection);

            string connectionTypeName = connection.GetType().Name;

            if (connectionTypeName.EndsWith(InternalAppValues.ConnectionName))
                connectionTypeName =
                    connectionTypeName.Substring(0, connectionTypeName.Length - InternalAppValues.ConnectionName.Length).ToLower();

            Type builderType = connection
                  .GetType()
                  .Assembly
                  .GetExportedTypes()
                  .FirstOrDefault(type =>
                  type.IsClass
                  && !type.IsAbstract
                  && type.Name.ToLower().StartsWith(connectionTypeName)
                  && typeof(DbConnectionStringBuilder).IsAssignableFrom(type));

            DbConnectionStringBuilder builder = null;
            if (builderType != null)
            {
                builder = Activator.CreateInstance(builderType) as DbConnectionStringBuilder;
            }

            return builder;
        }

        /// <summary>
        /// Closes DbConnection if not closed.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        public static void CloseIfNot(this IDbConnection connection)
        {
            if (!IsClosed(connection))
                connection.Close();
        }

        /// <summary>
        /// Opens DbConnection if not opened.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <exception cref="ArgumentNullException">if connection parameter is null, throws exception, else not.</exception>
        public static void OpenIfNot(this IDbConnection connection)
        {
            if (!IsOpen(connection))
                connection.Open();
        }

        /// <summary>
        /// Opens the database connection if not opened and return this instance.
        /// </summary>
        /// <param name="connection">The database connection instance.</param>
        /// <returns>A IDbConnection instance.</returns>
        public static IDbConnection OpenAnd(this IDbConnection connection)
        {
            OpenIfNot(connection);
            return connection;
        }

        /// <summary>
        /// Opens connection and Begins a DbTransaction.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <param name="isolationLevel">(Optional) Transaction isolation level.</param>
        /// <returns>An IDbTransaction instance.</returns>
        /// <exception cref="ArgumentNullException">if connection parameter is null, throws exception, else not.</exception>
        public static IDbTransaction OpenAndBeginTransaction(
            this IDbConnection connection, IsolationLevel? isolationLevel = null)
        {
            OpenIfNot(connection);

            IDbTransaction transaction;
            if (isolationLevel != null)
                transaction = connection.BeginTransaction(isolationLevel.Value);
            else
                transaction = connection.BeginTransaction();

            return transaction;
        }

        /// <summary>
        /// Gets Query Setting.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>.</returns>
        public static IQuerySetting GetQuerySetting(this IDbConnection connection)
        {
            CheckConnectionIsNull(connection);
            DbConnectionTypes connectionType = connection.GetDbConnectionType();
            IQuerySetting querySetting = QuerySettingsFactory.GetQuerySetting(connectionType);
            return querySetting;
        }

        /// <summary>
        /// Gets DbDataAdapter instance of database connection.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <returns>Returns DbDataAdapter instance.</returns>
        public static DbDataAdapter CreateAdapter(this IDbConnection connection)
        {
            Type adapterType = null;

            IEnumerable<Type> adapterTypes =
                 connection.GetType().Assembly.GetExportedTypes().Where(
                     type => type.IsClass && type.GetInterfaces().Contains(typeof(IDbDataAdapter))
                            && !type.IsAbstract && typeof(DbDataAdapter).IsAssignableFrom(type));

            if (adapterTypes.Count() > 1)
            {
                string connectionTypeName = connection.GetType().Name;
                connectionTypeName = connectionTypeName.Substring(0,
                    connectionTypeName.Length - InternalAppValues.ConnectionName.Length).ToLower();
                adapterType = adapterTypes.First(typ => typ.Name.ToLower().StartsWith(connectionTypeName));
            }
            else if (adapterTypes.Count() == 1)
            {
                adapterType = adapterTypes.First();
            }

            DbDataAdapter dataAdapter = null;
            if (adapterType != null)
            {
                dataAdapter = Activator.CreateInstance(adapterType) as DbDataAdapter;
            }

            return dataAdapter;
        }

        /// <summary>
        /// Creates Db Parameter.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="dbType">Parameter Db Type</param>
        /// <param name="direction">Parameter direction</param>
        /// <returns>Returns DbCommand instance.</returns>
        public static DbParameter CreateDbCommandParameter(this IDbConnection connection,
            string parameterName = null, object value = null,
            DbType? dbType = null, ParameterDirection direction = ParameterDirection.Input)
        {
            CheckConnectionIsNull(connection);

            DbConnectionTypes connectionType = connection.GetDbConnectionType();
            Type commandParameterType =
            dbCommandParameterTypePairs.GetOrAdd((byte)connectionType, (connection_type) =>
            {
                return GetCommandParameterFromConnection(connection);
            });

            if (commandParameterType == null)
                throw new Exception(DbAppMessages.AdapterNotContainsDbParameterType);

            DbParameter parameter = Activator.CreateInstance(commandParameterType) as DbParameter;

            IQuerySetting querySetting = QuerySettingsFactory.GetQuerySetting(connectionType);
            if (!string.IsNullOrWhiteSpace(parameterName))
                parameter.ParameterName =
                    parameterName.TrimStart().StartsWith(querySetting.ParameterPrefix) ?
                    parameterName.TrimStart() : querySetting.ParameterPrefix + parameterName.TrimStart();

            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = direction;

            if (dbType != null)
            {
                parameter.DbType = dbType.Value;
            }
            else
            {
                DbType? db_type = value.ToDbType();
                if (db_type != null)
                    parameter.DbType = db_type.Value;
            }

            return parameter;
        }

        /// <summary>
        /// Gets the command parameter from connection.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Returns Type of DbParameter class of this DbConnection class.</returns>
        private static Type GetCommandParameterFromConnection(IDbConnection connection)
        {
            IEnumerable<Type> commandTypes =
                 connection.GetType().Assembly.GetExportedTypes().Where(
                     type => type.IsClass && type.GetInterfaces().Contains(typeof(IDbDataParameter))
                            && !type.IsAbstract && typeof(DbParameter).IsAssignableFrom(type));

            Type commandParameterType = null;
            if (commandTypes.Count() > 1)
            {
                string connectionTypeName = connection.GetType().Name;
                connectionTypeName = connectionTypeName.Substring(0,
                    connectionTypeName.Length - InternalAppValues.ConnectionName.Length).ToLower();
                commandParameterType = commandTypes.First(typ => typ.Name.ToLower().StartsWith(connectionTypeName));
            }
            else if (commandTypes.Count() == 1)
            {
                commandParameterType = commandTypes.First();
            }

            return commandParameterType;
        }

        /// <summary>
        /// Builds the database command.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql Query.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">command Timeout (value as second)</param>
        /// <returns>Returns DbCommand instance.</returns>
        public static DbCommand BuildDbCommand(this IDbConnection connection, string sql,
            DbParameter[] parameters = null, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommand command = (DbCommand)connection.CreateCommand();

            command.CommandText = sql;
            command.CommandType = commandType;

            if (commandTimeout != null) command.CommandTimeout = commandTimeout.Value;

            if (parameters != null && parameters.Length > 0)
            {
                IQuerySetting querySetting = connection.GetQuerySetting();

                for (int counter = 0; counter < parameters.Length; counter++)
                {
                    if (!parameters[counter].ParameterName.StartsWith(querySetting.ParameterPrefix))
                    {
                        parameters[counter].ParameterName =
                            querySetting.ParameterPrefix + parameters[counter].ParameterName.TrimStart();
                    }

                    command.Parameters.Add(parameters[counter]);
                }
            }

            command.SetCommandTransaction(transaction);
            return command;
        }

        /// <summary>
        /// Builds the database command.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql Query.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns DbCommand instance.</returns>
        public static DbCommand BuildDbCommand(this IDbConnection connection,
            string sql, DbParameter[] parameters = null,
            IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommand command = (DbCommand)connection.CreateCommand();

            command.CommandText = sql;
            command.CommandType = commandSetting?.CommandType ?? CommandType.Text;

            if (commandSetting?.CommandTimeout != null) command.CommandTimeout = (commandSetting?.CommandTimeout).Value;

            if (parameters != null && parameters.Length > 0)
            {
                IQuerySetting querySetting = connection.GetQuerySetting();

                for (int counter = 0; counter < parameters.Length; counter++)
                {
                    if (!parameters[counter].ParameterName.StartsWith(querySetting.ParameterPrefix))
                    {
                        parameters[counter].ParameterName =
                            querySetting.ParameterPrefix + parameters[counter].ParameterName.TrimStart();
                    }

                    command.Parameters.Add(parameters[counter]);
                }
            }

            command.SetCommandTransaction(transaction);
            return command;
        }

        /// <summary>
        /// Checks DbConnection is open.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <returns>Returns true if connection is open returns true, else returns false.</returns>
        /// <exception cref="ArgumentNullException">if connection parameter is null, throws exception, else not.</exception>
        public static bool IsOpen(this IDbConnection connection)
        {
            CheckConnectionIsNull(connection);
            bool isOpen = connection.State == ConnectionState.Open;
            return isOpen;
        }

        /// <summary>
        /// Checks DbConnection is closed.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <returns>Returns true if connection is closed returns true, else returns false.</returns>
        /// <exception cref="ArgumentNullException">if connection parameter is null, throws exception, else not.</exception>
        public static bool IsClosed(this IDbConnection connection)
        {
            CheckConnectionIsNull(connection);
            bool isClosed = connection.State == ConnectionState.Closed;
            return isClosed;
        }

        /// <summary>
        /// Checks DbConnection is null.
        /// </summary>
        /// <param name="connection">Database Connection.</param>
        /// <exception cref="ArgumentNullException">if connection parameter is null, throws exception, else not.</exception>
        private static void CheckConnectionIsNull(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
        }
    }
}