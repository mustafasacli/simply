using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.Enums;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Simply.Data.DbCommandExtensions
{
    /// <summary>
    /// Defines the <see cref="CommandExtensions"/>.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// The Sets Transaction of IDbCommand instance.
        /// </summary>
        /// <param name="command">IDbCommand instance <see cref="IDbCommand"/>.</param>
        /// <param name="transaction">IDbTransaction instance <see cref="IDbTransaction"/>.</param>
        public static void SetCommandTransaction(
            this IDbCommand command, IDbTransaction transaction)
        {
            if (transaction != null && command != null)
                command.Transaction = transaction;
        }

        /// <summary>
        /// The Sets Value of Parameter with given parmeter index and value.
        /// </summary>
        /// <param name="command">IDbCommand instance<see cref="IDbCommand"/>.</param>
        /// <param name="parameterIndex">Index of parameter <see cref="int"/>.</param>
        /// <param name="parameterValue">Value of parameter <see cref="object"/>.</param>
        /// <exception cref="System.NullReferenceException">throws IDbCommand instance is null.</exception>
        /// <exception cref="System.IndexOutOfRangeException">throws parameter index less than zero or greater than parameter length.</exception>
        public static void SetParameterValue(this IDbCommand command,
            int parameterIndex, object parameterValue)
        {
            IDbDataParameter dbParameter = command.Parameters[parameterIndex] as IDbDataParameter;
            dbParameter.Value = parameterValue ?? InternalAppValues.NullValue;
            command.Parameters[parameterIndex] = dbParameter;
        }

        /// <summary>
        /// Set Value Parameter with given name and value.
        /// </summary>
        /// <param name="command">The command <see cref="IDbCommand"/>.</param>
        /// <param name="parameterName">The parameterName <see cref="string"/>.</param>
        /// <param name="parameterValue">The parameterValue <see cref="object"/>.</param>
        /// <exception cref="System.NullReferenceException">throws IDbCommand instance is null or parameter not exist with given name.</exception>
        public static void SetParameterValue(this IDbCommand command,
            string parameterName, object parameterValue)
        {
            IDbDataParameter dbParameter = command.Parameters[parameterName] as IDbDataParameter;
            dbParameter.Value = InternalAppValues.NullValue;
            dbParameter.Value = parameterValue ?? InternalAppValues.NullValue;
            command.Parameters[parameterName] = dbParameter;
        }

        /// <summary>
        /// Set Command Connection.
        /// </summary>
        /// <param name="command">The command <see cref="IDbCommand"/>.</param>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        public static void SetCommandConnection(
            this IDbCommand command, IDbConnection connection)
        {
            if (command != null && connection != null)
                command.Connection = connection;
        }

        /// <summary>
        /// Sets Command values to DbNull.Value.
        /// </summary>
        /// <param name="command">The command <see cref="IDbCommand"/>.</param>
        public static void SetCommandValuesToNull(this IDbCommand command)
        {
            if (command == null) return;

            int parameterCount = (command.Parameters?.Count).GetValueOrDefault();

            for (int counter = 0; counter < parameterCount; counter++)
            {
                IDbDataParameter parameter = command.Parameters[counter] as IDbDataParameter;
                parameter.Value = InternalAppValues.NullValue;
                command.Parameters[counter] = parameter;
            }
        }

        /// <summary>
        /// Adds Command Parameters.
        /// </summary>
        /// <param name="command">Db command <see cref="IDbCommand"/>.</param>
        /// <param name="commandParameters">Database command parameters<see cref="List{DbCommandParameter}"/>.</param>
        internal static void AddCommandParameters(this IDbCommand command,
            List<DbCommandParameter> commandParameters)
        {
            if (commandParameters == null || commandParameters.Count < 1)
                return;

            IQuerySetting setting = command.Connection.GetQuerySetting();
            string parameterPrefix = setting.ParameterPrefix;

            foreach (DbCommandParameter parameter in commandParameters)
            {
                DbParameter dbParameter = command.RegenerateDbParameter(parameter, parameterPrefix);
                command.Parameters.Add(dbParameter);
            }
        }

        /// <summary>
        /// Adds Command Parameters.
        /// </summary>
        /// <param name="command">Db command.</param>
        /// <param name="parameters">Db Command Parameters.</param>
        public static void AddCommandParameters(this IDbCommand command,
           object[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                return;

            foreach (object parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Gets Output Parameters Of Command.
        /// </summary>
        /// <param name="command">Db command instance<see cref="IDbCommand"/>.</param>
        /// <returns>Db command parameters<see cref="DbCommandParameter[]"/>.</returns>
        public static DbCommandParameter[] GetOutParameters(this IDbCommand command)
        {
            List<DbCommandParameter> parameters = ArrayHelper.EmptyList<DbCommandParameter>();

            if (command.Parameters == null || command.Parameters.Count < 1) return parameters.ToArray();

            foreach (object item in command.Parameters)
            {
                if (!(item is DbParameter parameterItem) || parameterItem.Direction == ParameterDirection.Input) continue;

                parameters.Add(new DbCommandParameter
                {
                    Direction = parameterItem.Direction,
                    ParameterDbType = parameterItem.DbType,
                    ParameterName = parameterItem.ParameterName,
                    IsNullable = parameterItem.IsNullable,
                    ParameterColumnName = parameterItem.SourceColumn,
                    ParameterPrecision = parameterItem.Precision,
                    ParameterScale = parameterItem.Scale,
                    ParameterSize = parameterItem.Size,
                    Value = parameterItem.Value
                });
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Creates the database command parameter.
        /// </summary>
        /// <param name="command">Database command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Parameter Db Type.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <returns>Returns DbParameterinstance.</returns>
        /// <exception cref="ArgumentNullException">command</exception>
        public static DbParameter CreateDbParameter(this IDbCommand command,
            string parameterName = null, object value = null,
            DbType? dbType = null, ParameterDirection direction = ParameterDirection.Input)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            DbParameter parameter = (DbParameter)command.CreateParameter();

            if (!string.IsNullOrWhiteSpace(parameterName)) parameter.ParameterName = parameterName;

            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = direction;

            if (dbType != null)
            {
                parameter.DbType = dbType.Value;
            }
            else
            {
                DbType? db_type = value.ToDbType();
                if (db_type != null) parameter.DbType = db_type.Value;
            }

            return parameter;
        }

        /// <summary>
        /// Creates DbDataParameter with given DbCommandParameter instance and parameterPrefix.
        /// </summary>
        /// <param name="command">IDbCommand instance.</param>
        /// <param name="parameter">DbCommandParameter instance</param>
        /// <param name="parameterPrefix">Parameter prefix.</param>
        /// <returns>Returns DbParameter object instance.</returns>
        public static DbParameter RegenerateDbParameter(this IDbCommand command, DbCommandParameter parameter, string parameterPrefix)
        {
            DbParameter dbParameter = command.Connection.CreateDbCommandParameter();
            dbParameter.ParameterName = parameter.ParameterName;

            if (!string.IsNullOrWhiteSpace(parameterPrefix) && !dbParameter.ParameterName.StartsWith(parameterPrefix))
            { dbParameter.ParameterName = string.Format("{0}{1}", parameterPrefix, dbParameter.ParameterName); }

            dbParameter.Value = parameter.Value ?? InternalAppValues.NullValue;
            dbParameter.Direction = parameter.Direction;

            if (parameter.ParameterPrecision != null)
            { dbParameter.Precision = parameter.ParameterPrecision.Value; }

            if (parameter.ParameterScale != null)
            { dbParameter.Scale = parameter.ParameterScale.Value; }

            if (parameter.ParameterSize != null)
            { dbParameter.Size = parameter.ParameterSize.Value; }

            DbType? dbType = parameter.ParameterDbType ?? parameter.Value.ToDbType();
            if (dbType != null)
            { dbParameter.DbType = dbType.Value; }

            return dbParameter;
        }

        /// <summary>
        /// Executes command and returns IDataReader object instance.
        /// </summary>
        /// <param name="command">database command</param>
        /// <param name="behavior">database command behavior</param>
        /// <returns>Returns IDataReader object instance.</returns>
        public static IDataReader ExecuteDataReader(this IDbCommand command, CommandBehavior? behavior = null)
        {
            IDataReader reader = behavior == null ? command.ExecuteReader() : command.ExecuteReader(behavior.Value);
            return reader;
        }

        /// <summary>
        /// The Sets Transaction of IDbCommand instance.
        /// </summary>
        /// <param name="command">IDbCommand instance <see cref="IDbCommand"/>.</param>
        /// <param name="transaction">IDbTransaction instance <see cref="IDbTransaction"/>.</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        public static IDbCommand SetTransaction(
            this IDbCommand command, IDbTransaction transaction)
        {
            if (transaction != null && command != null)
                command.Transaction = transaction;

            return command;
        }

        /// <summary>
        /// Sets sql command timeout and returns command.
        /// </summary>
        /// <param name="command">db command instance</param>
        /// <param name="timeout">command timeout value.</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        internal static IDbCommand SetCommandTimeout(this IDbCommand command, int? timeout)
        {
            if (timeout != null)
                command.CommandTimeout = timeout.Value;

            return command;
        }

        /// <summary>
        /// Sets sql command text and returns command.
        /// </summary>
        /// <param name="command">db command instance</param>
        /// <param name="sqlCommandText">sql Command Text</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        internal static IDbCommand SetCommandText(this IDbCommand command, string sqlCommandText)
        {
            command.CommandText = sqlCommandText;
            return command;
        }

        /// <summary>
        /// Sets sql command type and returns command.
        /// </summary>
        /// <param name="command">db command instance</param>
        /// <param name="commandType">Sql Command Type</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        internal static IDbCommand SetCommandType(this IDbCommand command, CommandType? commandType)
        {
            if (commandType != null)
                command.CommandType = commandType.Value;

            return command;
        }

        /// <summary>
        /// Includes Command Parameters.
        /// </summary>
        /// <param name="command">The command <see cref="IDbCommand"/>.</param>
        /// <param name="commandParameters">database command parameters<see cref="List{DbCommandParameter}"/>.</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        [Obsolete("Method is depreated. it will be removed later versions.")]
        internal static IDbCommand IncludeCommandParameters(this IDbCommand command,
            List<DbCommandParameter> commandParameters)
        {
            if (commandParameters == null || commandParameters.Count < 1)
                return command;

            IQuerySetting setting = command.Connection.GetQuerySetting();
            string parameterPrefix = setting.ParameterPrefix;

            foreach (DbCommandParameter parameter in commandParameters)
            {
                DbParameter dbParameter = command.RegenerateDbParameter(parameter, parameterPrefix);
                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        /// <summary>
        /// Includes Command Parameters.
        /// </summary>
        /// <param name="command">The command <see cref="IDbCommand"/>.</param>
        /// <param name="commandParameters">database command parameters<see cref="List{System.Object}"/>.</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        internal static IDbCommand IncludeCommandParameters(
            this IDbCommand command, List<object> commandParameters)
        {
            if (commandParameters == null || commandParameters.Count < 1)
                return command;

            IQuerySetting setting = command.Connection.GetQuerySetting();
            string parameterPrefix = setting.ParameterPrefix;

            foreach (object parameter in commandParameters)
            {
                if (parameter == null)
                    throw new ArgumentNullException($"{nameof(parameter)} can not be null.");

                DbParameter dbParameter = null;

                if (parameter == null && parameter.GetRealType().IsSimpleTypeV2())
                {
                    dbParameter = command.CreateDbParameter();
                    dbParameter.Value = parameter ?? (object)DBNull.Value;
                }
                else if (parameter is DbCommandParameter)
                {
                    dbParameter = command.RegenerateDbParameter(parameter as DbCommandParameter, parameterPrefix);
                }
                else if (parameter is DbParameter)
                {
                    dbParameter = (DbParameter)parameter;
                }
                else if (parameter is IDbDataParameter)
                {
                    dbParameter = (DbParameter)(parameter as IDbDataParameter);
                }
                else if (parameter is IDataParameter)
                {
                    dbParameter = (DbParameter)(parameter as IDataParameter);
                }
                else if (parameter.GetRealType().IsSimpleTypeV2())
                {
                    dbParameter = command.CreateDbParameter();
                    dbParameter.Value = parameter;
                }

                if (dbParameter == null)
                    throw new ArgumentNullException($"{nameof(dbParameter)} can not be null." + parameter.ToString());

                if (dbParameter.ParameterName.IsValid() && !dbParameter.ParameterName.StartsWith(parameterPrefix))
                {
                    dbParameter.ParameterName = string.Concat(parameterPrefix, dbParameter.ParameterName);
                }

                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        /// <summary>
        /// Adds Command Parameters.
        /// </summary>
        /// <param name="command">Db command.</param>
        /// <param name="parameters">Db Command Parameters.</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        [Obsolete("Method is depreated. it will be removed later versions.")]
        public static IDbCommand IncludeCommandParameters(this IDbCommand command, object[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                return command;

            foreach (object parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Adds parameter into command and return command.
        /// </summary>
        /// <param name="command">Db command.</param>
        /// <param name="parameter">Db Command Parameter.</param>
        /// <returns>Returns IDbCommand object instance.</returns>
        public static IDbCommand AddDbParameter(this IDbCommand command, DbParameter parameter)
        {
            if (parameter != null)
            { command.Parameters.Add(parameter); }

            return command;
        }
    }
}