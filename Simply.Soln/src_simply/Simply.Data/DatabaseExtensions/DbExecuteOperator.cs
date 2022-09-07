using Simply.Common;
using Simply.Data.DatabaseExtensions;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Helpers;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbExecuteOperator"/>.
    /// </summary>
    public static class DbExecuteOperator
    {
        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>Returns exection result as int.</returns>
        public static int Execute(this ISimpleDatabase database, string sqlQuery, object obj)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = database.CommandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = database.CommandSetting?.CommandTimeout,
                ParameterNamePrefix = database.CommandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(parameters);

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);

            int result;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);
                result = command.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Executes query and returns result as long.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>Returns execution result as long.</returns>
        public static long ExecuteAsLong(this ISimpleDatabase database, string sqlQuery, object obj)
        {
            long value =
                Execute(database, sqlQuery, obj);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as decimal.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>Returns execution result as decimal.</returns>
        public static decimal ExecuteAsDecimal(this ISimpleDatabase database,
           string sqlQuery, object obj)
        {
            decimal value =
                Execute(database, sqlQuery, obj);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        /// <returns>Returns execution result as int. <see cref="IDbCommandResult{System.Int32}"/></returns>
        public static IDbCommandResult<int> ExecuteQuery(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);
            IDbCommandResult<int> commandResult = new DbCommandResult<int>(-1);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);
                commandResult.ExecutionResult = command.ExecuteNonQuery();
                commandResult.Result = commandResult.ExecutionResult;
                commandResult.OutputParameters = command.GetOutParameters();
            }

            return commandResult;
        }

        /// <summary>
        /// Executes the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <returns>Returns execution result as int.</returns>
        public static int ExecuteAsOdbc(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, database.CommandSetting);

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);

            int executeResult;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);
                executeResult = command.ExecuteNonQuery();
            }

            return executeResult;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteAsync(this ISimpleDatabase database,
            string sqlQuery, object obj)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Execute(database, sqlQuery, obj);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as long.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>.</returns>
        public static async Task<long> ExecuteAsLongAsync(this ISimpleDatabase database,
            string sqlQuery, object obj)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsLong(database, sqlQuery, obj);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as decimal.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<decimal> ExecuteAsDecimalAsync(
            this ISimpleDatabase database, string sqlQuery, object obj)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsDecimal(database, sqlQuery, obj);
            });
        }

        #endregion [ Task methods ]
    }
}