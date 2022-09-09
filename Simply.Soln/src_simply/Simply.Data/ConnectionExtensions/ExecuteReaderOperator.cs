using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Helpers;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="ExecuteReaderOperator"/>.
    /// </summary>
    public static class ExecuteReaderOperator
    {
        /// <summary>
        /// Executes query with parameters and returns DataReader object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="outputParameters"></param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">Db Command Behavior.</param>
        /// <param name="logSetting">Log Setting</param>
        /// <returns>Returns an IDataReader instance.</returns>
        public static IDataReader ExecuteReaderQuery(
            this IDbConnection connection, SimpleDbCommand simpleDbCommand,
            out DbCommandParameter[] outputParameters, IDbTransaction transaction = null,
            CommandBehavior? commandBehavior = null, ILogSetting logSetting = null)
        {
            outputParameters = ArrayHelper.Empty<DbCommandParameter>();
            IDataReader dataReader;

            InternalLogHelper.LogCommand(simpleDbCommand, logSetting);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, logSetting);

                if (!commandBehavior.HasValue)
                    dataReader = command.ExecuteReader();
                else
                    dataReader = command.ExecuteReader(commandBehavior.Value);

                outputParameters = command.GetOutParameters();
            }

            return dataReader;
        }

        /// <summary>
        /// Executes query with parameters and returns DataReader object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">CommandBehaviour for DataReader.</param>
        /// <param name="commandTimeout">db command timeout(optional).</param>
        /// <param name="logSetting">Log Setting</param>
        /// <returns>Returns an IDataReader instance.</returns>
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string sqlQuery, object parameterObject, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, CommandBehavior? commandBehavior = null,
            int? commandTimeout = null, ILogSetting logSetting = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(parameterObject);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandType,
                CommandTimeout = commandTimeout,
            };
            simpleDbCommand.AddCommandParameters(parameters);

            InternalLogHelper.LogCommand(simpleDbCommand, logSetting);

            IDataReader dataReader;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, logSetting);

                if (!commandBehavior.HasValue)
                    dataReader = command.ExecuteReader();
                else
                    dataReader = command.ExecuteReader(commandBehavior.Value);
            }

            return dataReader;
        }

        /// <summary>
        /// Executes query with parameters and returns DataReader object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">CommandBehaviour for DataReader.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="logSetting">Log Setting</param>
        /// <returns>Returns an IDataReader instance.</returns>
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string sqlQuery, object parameterObject, IDbTransaction transaction = null,
             ICommandSetting commandSetting = null, CommandBehavior? commandBehavior = null,
             ILogSetting logSetting = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(parameterObject);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
                ParameterNamePrefix = commandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), parameterObject);
            simpleDbCommand.AddCommandParameters(parameters);

            InternalLogHelper.LogCommand(simpleDbCommand, logSetting);

            IDataReader dataReader;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, logSetting);

                if (!commandBehavior.HasValue)
                    dataReader = command.ExecuteReader();
                else
                    dataReader = command.ExecuteReader(commandBehavior.Value);
            }

            return dataReader;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the reader asynchronous operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">Db Command Behavior</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="logSetting">Log Setting</param>
        /// <returns>An asynchronous result that yields the execute reader.</returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
            string sqlQuery, object parameterObject, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, CommandBehavior? commandBehavior = null,
            ILogSetting logSetting = null)
        {
            Task<IDataReader> resultTask = Task.Factory.StartNew(() =>
            {
                return
                ExecuteReader(connection, sqlQuery, parameterObject, transaction,
                commandSetting, commandBehavior, logSetting);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}