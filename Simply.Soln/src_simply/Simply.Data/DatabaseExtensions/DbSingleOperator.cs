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
    /// Defines the <see cref="DbSingleOperator"/>.
    /// </summary>
    public static class DbSingleOperator
    {
        /// <summary>
        /// Get Single Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns single record as dynamic object instance.</returns>
        public static IDbCommandResult<T> QuerySingle<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = database.QuerySingleAsDbRow(simpleDbCommand);

            IDbCommandResult<T> instanceResult = new DbCommandResult<T>();
            instanceResult.Result = simpleDbRowResult.Result.ConvertRowTo<T>();
            instanceResult.AdditionalValues = simpleDbRowResult.AdditionalValues;

            return instanceResult;
        }

        /// <summary>
        /// Get Single Row of the Resultset as object instance.
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
        /// <returns>Returns single record as object instance.</returns>
        public static T QuerySingle<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<T> commandResult = database.QuerySingle<T>(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Get Single Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static T GetSingle<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<T> commandResult = database.QuerySingle<T>(simpleDbCommand);
            return commandResult.Result;
        }

        #region [ SimpleDbRow methods ]

        /// <summary>
        /// Get Single Row of the Resultset as dynamic object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>Returns single record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QuerySingleAsDbRow(this ISimpleDatabase database,
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
                        simpleDbRowResult.Result = dataReader.SingleDbRow();
                    }
                    finally
                    { dataReader?.CloseIfNot(); }
                }
            }

            return simpleDbRowResult;
        }

        /// <summary>
        /// Get Single Row of the Resultset as dynamic object instance.
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
        /// <returns>Returns single record as dynamic object.</returns>
        public static SimpleDbRow QuerySingleAsDbRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            SimpleDbRow simpleDbRow = database.QuerySingleAsDbRow(simpleDbCommand).Result;
            return simpleDbRow;
        }

        /// <summary>
        /// Get Single Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static SimpleDbRow GetSingleAsDbRow(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<SimpleDbRow> commandResult = database.QuerySingleAsDbRow(simpleDbCommand);
            return commandResult.Result;
        }

        #endregion [ SimpleDbRow methods ]

        #region [ Task methods ]

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>An asynchronous result that yields the single as object instance.</returns>
        public static async Task<T> QuerySingleAsync<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QuerySingle<T>(simpleDbCommand).Result;
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as object instance with async operation.
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
        public static async Task<T> SingleAsync<T>(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, CommandType? commandType = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QuerySingle<T>(sqlQuery, parameterObject, commandType);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>s
        /// <returns>An asynchronous result that yields the single as object instance.</returns>
        public static async Task<T> GetSingleAsync<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.GetSingle<T>(odbcSqlQuery, parameterValues, commandType);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}