using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbExecuteScalarOperator"/>.
    /// </summary>
    public static class DbExecuteScalarOperator
    {
        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static IDbCommandResult<object> ExecuteScalarQuery(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbCommandResult<object> commandResult;

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
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
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static IDbCommandResult<T> ExecuteScalarQueryAs<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : struct
        {
            IDbCommandResult<object> commandResult =
                database.ExecuteScalarQuery(simpleDbCommand)
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
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalar(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            IDbCommandResult<object> commandResult = database.ExecuteScalarQuery(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar as operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as T instance.</returns>
        public static T ExecuteScalarAs<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : struct
        {
            object value = database.ExecuteScalar(sqlQuery, parameterObject, commandSetting);
            return !value.IsNullOrDbNull() ? (T)value : default;
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalarOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            IDbCommandResult<object> commandResult = database.ExecuteScalarQuery(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static T ExecuteScalarOdbcAs<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            object value = database.ExecuteScalarOdbc(odbcSqlQuery, parameterValues, commandSetting);
            return !value.IsNullOrDbNull() ? (T)value : default;
        }

        #region [ Task methods ]

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static async Task<T> ExecuteScalarOdbcAsnc<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteScalarOdbcAs<T>(odbcSqlQuery, parameterValues, commandSetting);
            });
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object as async operation.
        /// </summary>
        /// <typeparam name="T">T struct.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static async Task<T> ExecuteScalarAsAsync<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteScalarAs<T>(sqlQuery, parameterObject, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>An asynchronous result that yields the execute scalar.</returns>
        public static async Task<T> ExecuteScalarQueryAsync<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteScalarQueryAs<T>(simpleDbCommand).Result;
            });
        }

        #endregion [ Task methods ]
    }
}