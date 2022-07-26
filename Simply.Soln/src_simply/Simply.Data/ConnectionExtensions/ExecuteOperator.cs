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
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns exection result as int.</returns>
        public static int Execute(this IDbConnection connection,
           string sqlQuery, object obj,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null)
        {
            int result = -1;

            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            DbCommandDefinition commandDefinition = new DbCommandDefinition()
            {
                CommandText = sqlQuery,
                CommandType = commandType
            };
            commandDefinition.AddCommandParameters(parameters);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                if (transaction == null)
                    connection.OpenIfNot();

                result = command.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Executes query and returns result as long.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execution result as long.</returns>
        public static long ExecuteAsLong(this IDbConnection connection,
           string sqlQuery, object obj, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null)
        {
            long value =
                Execute(connection, sqlQuery, obj, commandType, transaction);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as decimal.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execution result as decimal.</returns>
        public static decimal ExecuteAsDecimal(this IDbConnection connection,
           string sqlQuery, object obj, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null)
        {
            decimal value =
                Execute(connection, sqlQuery, obj, commandType, transaction);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="commandDefinition">Db Command Definition <see cref="DbCommandDefinition"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns execution result as int. <see cref="IDbCommandResult{System.Int32}"/></returns>
        public static IDbCommandResult<int> ExecuteQuery(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null)
        {
            IDbCommandResult<int> result = new DbCommandResult<int>(-1);

            try
            {
                using (IDbCommand command =
                    connection.CreateCommandWithOptions(commandDefinition, transaction))
                {
                    if (transaction == null && commandDefinition.AutoOpen)
                        connection.OpenIfNot();

                    result.ExecutionResult = command.ExecuteNonQuery();
                    result.Result = result.ExecutionResult;
                    result.OutputParameters = command.GetOutParameters();
                }
            }
            finally
            {
                if (transaction == null && commandDefinition.CloseAtFinal)
                    connection.CloseIfNot();
            }

            return result;
        }

        /// <summary>
        /// Executes the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">command timeout</param>
        /// <returns>Returns execution result as int.</returns>
        public static int ExecuteAsOdbc(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int result = -1;
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            DbCommandDefinition commandDefinition =
                connection.BuildCommandDefinitionForTranslate(odbcSqlQuery, commandParameters, commandType, commandTimeout);

            using (IDbCommand command = connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                if (transaction == null)
                    connection.OpenIfNot();

                result = command.ExecuteNonQuery();
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
        /// <param name="commandType">(Optional) Type of the command.</param>
        /// <param name="transaction">(Optional) The transaction.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteAsync(this IDbConnection connection,
            string sqlQuery, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Execute(connection, sqlQuery, obj, commandType, transaction);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as long.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>.</returns>
        public static async Task<long> ExecuteAsLongAsync(this IDbConnection connection,
            string sqlQuery, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsLong(connection, sqlQuery, obj, commandType, transaction);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as decimal.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<decimal> ExecuteAsDecimalAsync(this IDbConnection connection,
            string sqlQuery, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsDecimal(connection, sqlQuery, obj, commandType, transaction);
            });
        }

        #endregion [ Task methods ]
    }
}