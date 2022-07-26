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
    /// Defines the <see cref="ExecuteScalarOperator"/>.
    /// </summary>
    public static class ExecuteScalarOperator
    {
        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandTimeout">Db Command timeout.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalar(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            DbCommandDefinition commandDefinition = new DbCommandDefinition()
            {
                CommandText = sql,
                CommandType = commandType,
                CommandTimeout = commandTimeout
            };

            commandDefinition.AddCommandParameters(parameters);

            object result = null;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                if (transaction == null)
                    connection.OpenIfNot();

                result = command.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar as operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as T instance.</returns>
        public static T ExecuteScalarAs<T>(this IDbConnection connection,
           string sqlText, object obj, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null) where T : struct
        {
            object value =
                ExecuteScalar(connection, sqlText, obj, commandType, transaction);

            return !value.IsNullOrDbNull() ? (T)value : default;
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object QueryExecuteScalar(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            object result = null;

            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            DbCommandDefinition commandDefinition = new DbCommandDefinition()
            {
                CommandText = sql,
                CommandType = commandType
            };
            commandDefinition.AddCommandParameters(parameters);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                if (transaction == null)
                    connection.OpenIfNot();

                result = command.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="commandDefinition">Command Definition.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static IDbCommandResult<object> ExecuteScalarQuery(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null)
        {
            IDbCommandResult<object> result = new DbCommandResult<object>(-1);

            try
            {
                using (IDbCommand command =
                    connection.CreateCommandWithOptions(commandDefinition, transaction))
                {
                    if (transaction == null && commandDefinition.AutoOpen)
                        connection.OpenIfNot();

                    result.Result = command.ExecuteScalar();
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
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="commandDefinition">Command Definition.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static IDbCommandResult<T> ExecuteScalarQueryAs<T>(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null)
        {
            IDbCommandResult<object> result =
                ExecuteScalarQuery(connection, commandDefinition, transaction)
                ?? new DbCommandResult<object>();

            T value = !result.Result.IsNullOrDbNull() ? (T)result : default;

            return new DbCommandResult<T>()
            {
                Result = value,
                AdditionalValues = result.AdditionalValues,
                ExecutionResult = result.ExecutionResult
            };
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalarOdbc(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            object result = null;

            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();
            DbCommandDefinition commandDefinition =
                connection.BuildCommandDefinitionForTranslate(odbcSqlQuery,
                commandParameters, commandType, commandTimeout);
            commandDefinition.CommandTimeout = commandTimeout;

            using (IDbCommand command = connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                result = command.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static T ExecuteScalarOdbcAs<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null) where T : struct
        {
            object result = null;
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();
            DbCommandDefinition commandDefinition = connection.BuildCommandDefinitionForTranslate(odbcSqlQuery, commandParameters, commandType, commandTimeout);

            using (IDbCommand command = connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                result = command.ExecuteScalar();
            }

            return (T)result;
        }

        #region [ Task methods ]

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object as async operation.
        /// </summary>
        /// <typeparam name="T">T struct.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static async Task<T> ExecuteScalarAsAsync<T>(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteScalarAs<T>(connection, sqlText, obj, commandType, transaction);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar asynchronous operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>An asynchronous result that yields the execute scalar.</returns>
        public static async Task<object> ExecuteScalarAsync(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            Task<object> resultTask = Task.Factory.StartNew(() =>
            {
                return
                ExecuteScalar(connection, sql, obj, commandType, transaction);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}