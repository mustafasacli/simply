using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbLastOperator"/>.
    /// </summary>
    public static class DbLastOperator
    {
        /// <summary>
        /// Get Last Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns last record as object instance.</returns>
        public static IDbCommandResult<T> QueryLast<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = database.QueryLastAsDbRow(simpleDbCommand);
            IDbCommandResult<T> instanceResult = new DbCommandResult<T>();
            instanceResult.Result = simpleDbRowResult.Result.ConvertRowTo<T>();
            instanceResult.AdditionalValues = simpleDbRowResult.AdditionalValues;
            return instanceResult;
        }

        /// <summary>
        /// Get Last Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns last record as object instance.</returns>
        public static T QueryLast<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<T> commandResult = database.QueryLast<T>(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Get Last Row of the Odbc Sql query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns last record as object instance.</returns>
        public static T GetLast<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<T> commandResult = database.QueryLast<T>(simpleDbCommand);
            return commandResult.Result;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get Last Row of the Resultset as dynamic object instance with async operation.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>An asynchronous result that yields the last record as object instance.</returns>
        public static async Task<T> QueryLastAsync<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryLast<T>(simpleDbCommand).Result;
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Last Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> QueryLastAsync<T>(this ISimpleDatabase database,
           string sqlQuery, object parameterObject, CommandType? commandType = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryLast<T>(sqlQuery, parameterObject, commandType);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Last Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).
        /// </param>
        /// <param name="parameterValues">object contains db parameters as property.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> GetLastAsync<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.GetLast<T>(odbcSqlQuery, parameterValues, commandType);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        #region [ DbRow methods ]

        /// <summary>
        /// Get Last Row of the Resultset as SimpleDbRow object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns last record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QueryLastAsDbRow(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IQuerySetting querySetting = database.QuerySetting;
            CommandBehavior? commandBehavior = null;

            if (!querySetting.LastFormat.IsNullOrSpace())
            {
                string format = querySetting.LastFormat;
                simpleDbCommand.CommandText =
                    format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                commandBehavior = CommandBehavior.SingleRow;
            }

            IDbCommandResult<SimpleDbRow> simpleDbRowResult = new DbCommandResult<SimpleDbRow>();

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            {
                using (IDataReader dataReader = command.ExecuteDataReader(commandBehavior))
                {
                    try
                    {
                        simpleDbRowResult.OutputParameters = command.GetOutParameters();
                        simpleDbRowResult.ExecutionResult = dataReader.RecordsAffected;
                        simpleDbRowResult.Result = dataReader.LastDbRow(closeAtFinal: true);
                    }
                    finally
                    { dataReader?.CloseIfNot(); }
                }
            }

            return simpleDbRowResult;
        }

        /// <summary>
        /// Get Last Row of the Resultset as SimpleDbRow object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns last record as SimpleDbRow instance.</returns>
        public static SimpleDbRow QueryLastDbRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<SimpleDbRow> commandResult = database.QueryLastAsDbRow(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Get Last Row of the Odbc Sql query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns last record as SimpleDbRow instance.</returns>
        public static SimpleDbRow GetLastAsDbRow(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<SimpleDbRow> commandResult = database.QueryLastAsDbRow(simpleDbCommand);
            return commandResult.Result;
        }

        #endregion [ DbRow methods ]
    }
}