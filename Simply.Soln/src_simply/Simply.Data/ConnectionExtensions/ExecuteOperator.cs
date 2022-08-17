using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="ExecuteOperator"/>.
    /// </summary>
    public static class ExecuteOperator
    {
        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns exection result as int.</returns>
        public static int Execute(this IDbConnection connection, string sqlQuery,
            object obj, IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            int result = -1;

            try
            {
                DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
                SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
                {
                    CommandText = sqlQuery,
                    CommandType = commandSetting?.CommandType ?? CommandType.Text,
                    CommandTimeout = commandSetting?.CommandTimeout,
                };
                simpleDbCommand.AddCommandParameters(parameters);

                using (IDbCommand command =
                    connection.CreateCommandWithOptions(simpleDbCommand, transaction))
                {
                    result = command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }

            return result;
        }

        /// <summary>
        /// Executes query and returns result as long.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execution result as long.</returns>
        public static long ExecuteAsLong(this IDbConnection connection, string sqlQuery,
            object obj, IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            long value =
                Execute(connection, sqlQuery, obj, transaction, commandSetting);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as decimal.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execution result as decimal.</returns>
        public static decimal ExecuteAsDecimal(this IDbConnection connection,
           string sqlQuery, object obj, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null)
        {
            decimal value =
                Execute(connection, sqlQuery, obj, transaction, commandSetting);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns execution result as int. <see cref="IDbCommandResult{System.Int32}"/></returns>
        public static IDbCommandResult<int> ExecuteQuery(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IDbCommandResult<int> result = new DbCommandResult<int>(-1);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                result.ExecutionResult = command.ExecuteNonQuery();
                result.Result = result.ExecutionResult;
                result.OutputParameters = command.GetOutParameters();
            }

            return result;
        }

        /// <summary>
        /// Executes the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execution result as int.</returns>
        public static int ExecuteAsOdbc(this IDbConnection connection,
            string odbcSqlQuery, object[] parameterValues,
            IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            int result = -1;

            try
            {
                DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                    .Select(p => new DbCommandParameter
                    {
                        Value = p,
                        ParameterDbType = p.ToDbType()
                    })
                    .ToArray();

                SimpleDbCommand simpleDbCommand =
                    connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                    commandParameters, commandSetting);

                using (IDbCommand command =
                    connection.CreateCommandWithOptions(simpleDbCommand, transaction))
                {
                    result = command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }

            return result;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) The transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteAsync(this IDbConnection connection,
            string sqlQuery, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Execute(connection,
                    sqlQuery, obj, transaction, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as long.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>.</returns>
        public static async Task<long> ExecuteAsLongAsync(this IDbConnection connection,
            string sqlQuery, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsLong(connection, sqlQuery, obj, transaction, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as decimal.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<decimal> ExecuteAsDecimalAsync(this IDbConnection connection,
            string sqlQuery, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsDecimal(connection, sqlQuery, obj, transaction, commandSetting);
            });
        }

        #endregion [ Task methods ]
    }
}