using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
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
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execute scalar result as object.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static object ExecuteScalar(this IDbConnection connection,
            string sql, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
                ParameterNamePrefix = commandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(parameters);

            object result;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
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
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execute scalar result as T instance.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static T ExecuteScalarAs<T>(this IDbConnection connection,
           string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null) where T : struct
        {
            object value =
                ExecuteScalar(connection, sqlText, obj, transaction, commandSetting);

            return !value.IsNullOrDbNull() ? (T)value : default;
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execute scalar result as object.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static object QueryExecuteScalar(this IDbConnection connection,
            string sql, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout
            };
            simpleDbCommand.AddCommandParameters(parameters);

            object result;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                result = command.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static IDbCommandResult<object> ExecuteScalarQuery(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IDbCommandResult<object> commandResult;

            using (IDbCommand command =
                    connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                commandResult = new DbCommandResult<object>();
                commandResult.Result = command.ExecuteScalar();
                commandResult.OutputParameters = command.GetOutParameters();
            }

            return commandResult;
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static IDbCommandResult<T> ExecuteScalarQueryAs<T>(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IDbCommandResult<object> commandResult =
                ExecuteScalarQuery(connection, simpleDbCommand, transaction)
                ?? new DbCommandResult<object>();

            T instance = !commandResult.Result.IsNullOrDbNull() ? (T)commandResult.Result : default;

            return new DbCommandResult<T>()
            {
                Result = instance,
                AdditionalValues = commandResult.AdditionalValues,
                ExecutionResult = commandResult.ExecutionResult
            };
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execute scalar result as object.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static object ExecuteScalarOdbc(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();
            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, commandSetting);

            object result;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
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
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static T ExecuteScalarOdbcAs<T>(
            this IDbConnection connection, string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null) where T : struct
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();
            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(
                    odbcSqlQuery, commandParameters, commandSetting);

            object result;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
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
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static async Task<T> ExecuteScalarAsAsync<T>(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteScalarAs<T>(connection, sqlText, obj,
                transaction, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar asynchronous operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields the execute scalar.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static async Task<object> ExecuteScalarAsync(this IDbConnection connection,
            string sql, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            Task<object> resultTask = Task.Factory.StartNew(() =>
            {
                return
                ExecuteScalar(connection, sql, obj, transaction, commandSetting);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}