using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbFirstOperator"/>.
    /// </summary>
    public static class DbFirstOperator
    {
        /// <summary>
        /// Get First Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns first record as dynamic object instance.</returns>
        public static IDbCommandResult<T> QueryFirst<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = database.QueryFirstAsDbRow(simpleDbCommand);

            IDbCommandResult<T> instanceResult = new DbCommandResult<T>();
            instanceResult.AdditionalValues = simpleDbRowResult.AdditionalValues;
            instanceResult.ExecutionResult = simpleDbRowResult.ExecutionResult;
            instanceResult.Result = simpleDbRowResult.Result.ConvertRowTo<T>();
            return instanceResult;
        }

        /// <summary>
        /// Get First Row of the Resultset as object instance.
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
        /// <returns>Returns first record as object instance.</returns>
        public static T QueryFirst<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<T> commandResult = database.QueryFirst<T>(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T GetFirst<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<T> commandResult = database.QueryFirst<T>(simpleDbCommand);
            return commandResult.Result;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get First Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns first record as T object instance as async.</returns>
        public static async Task<T> QueryFirstAsync<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryFirst<T>(simpleDbCommand).Result;
            });

            return await resultTask;
        }

        /// <summary>
        /// Get First Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
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
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> FirstAsync<T>(this ISimpleDatabase database,
           string sqlQuery, object parameterObject) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryFirst<T>(sqlQuery, parameterObject);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>An asynchronous result that yields the first as T.</returns>
        public static async Task<T> GetFirstAsync<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.GetFirst<T>(odbcSqlQuery, parameterValues, commandType);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        #region [ DbRow methods ]

        /// <summary>
        /// Get First Row of the Resultset as dynamic object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>Returns first record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QueryFirstAsDbRow(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand)
        {
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = new DbCommandResult<SimpleDbRow>();

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            {
                using (IDataReader dataReader = command.ExecuteDataReader(CommandBehavior.SingleRow))
                {
                    try
                    {
                        simpleDbRowResult.OutputParameters = command.GetOutParameters();
                        simpleDbRowResult.ExecutionResult = dataReader.RecordsAffected;
                        simpleDbRowResult.Result = dataReader.FirstDbRow(closeAtFinal: true);
                    }
                    finally
                    { dataReader?.CloseIfNot(); }
                }
            }

            return simpleDbRowResult;
        }

        /// <summary>
        /// Get First Row of the Resultset as SimpleDbRow object instance.
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
        /// <returns>Returns first record as dynamic object.</returns>
        public static SimpleDbRow QueryFirstAsDbRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<SimpleDbRow> commandResult = database.QueryFirstAsDbRow(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static SimpleDbRow GetFirstAsDbRow(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<SimpleDbRow> commandResult = database.QueryFirstAsDbRow(simpleDbCommand);
            return commandResult.Result;
        }

        #endregion [ DbRow methods ]
    }
}