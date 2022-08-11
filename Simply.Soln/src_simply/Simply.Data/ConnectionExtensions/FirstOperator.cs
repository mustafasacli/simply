﻿using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="FirstOperator"/>.
    /// </summary>
    public static class FirstOperator
    {
        /// <summary>
        /// Get First Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T QueryFirst<T>(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char? parameterNamePrefix = null) where T : class, new()
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandType
            };
            simpleDbCommand.AddCommandParameters(commandParameters);

            IDbCommandResult<SimpleDbRow> commandResult = connection.QueryFirstAsDbRow(simpleDbCommand, transaction);
            T instance = commandResult.Result.ConvertRowTo<T>();

            return instance;
        }

        /// <summary>
        /// Get First Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns first record as dynamic object instance.</returns>
        public static IDbCommandResult<T> QueryFirst<T>(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null) where T : class, new()
        {
            IDbCommandResult<T> result = new DbCommandResult<T>();

            IDbCommandResult<SimpleDbRow> commandResult = connection.QueryFirstAsDbRow(simpleDbCommand, transaction);

            result.Result = commandResult.Result.ConvertRowTo<T>();
            result.AdditionalValues = commandResult.AdditionalValues;

            return result;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T GetFirst<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter { Value = p, ParameterDbType = p.ToDbType() })
                .ToArray() ?? new DbCommandParameter[0];
            SimpleDbCommand simpleDbCommand = connection.BuildsimpleDbCommandForTranslate(
                odbcSqlQuery, commandParameters, commandType, commandTimeout);

            IDbCommandResult<SimpleDbRow> dbRow = QueryFirstAsDbRow(connection, simpleDbCommand, transaction);

            T result = dbRow.Result.ConvertRowTo<T>();

            return result;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get First Row of the Resultset as dynamic object instance with async operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>An asynchronous result that yields the first as dynamic.</returns>
        public static async Task<SimpleDbRow> QueryFirstAsDbRowAsync(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char? parameterNamePrefix = null)
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryFirstAsDbRow(sqlText, obj, commandType, transaction, parameterNamePrefix);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get First Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> FirstAsync<T>(this IDbConnection connection,
           string sqlText, object obj, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, char? parameterNamePrefix = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryFirst<T>(sqlText, obj, commandType, transaction, parameterNamePrefix);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        #region [ DbRow methods ]

        /// <summary>
        /// Get First Row of the Resultset as dynamic object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns first record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QueryFirstAsDbRow(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IDbCommandResult<SimpleDbRow> result = new DbCommandResult<SimpleDbRow>();

            IDataReader reader = null;

            try
            {
                try
                {
                    DbCommandParameter[] outputValues;

                    if (transaction == null && simpleDbCommand.AutoOpen)
                        connection.OpenIfNot();

                    reader = connection.ExecuteReaderQuery(
                        simpleDbCommand, out outputValues, transaction, commandBehavior: CommandBehavior.SingleRow);

                    result.OutputParameters = outputValues;
                    result.Result = reader.FirstDbRow(closeAtFinal: true);
                }
                finally
                { reader?.CloseIfNot(); }
            }
            finally
            {
                if (transaction == null && simpleDbCommand.CloseAtFinal)
                    connection.CloseIfNot();
            }

            return result;
        }

        /// <summary>
        /// Get First Row of the Resultset as SimpleDbRow object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns first record as dynamic object.</returns>
        public static SimpleDbRow QueryFirstAsDbRow(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char? parameterNamePrefix = null)
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandType
            };
            simpleDbCommand.AddCommandParameters(commandParameters);
            SimpleDbRow instance = connection.QueryFirstAsDbRow(simpleDbCommand, transaction).Result;

            return instance;
        }

        #endregion [ DbRow methods ]
    }
}