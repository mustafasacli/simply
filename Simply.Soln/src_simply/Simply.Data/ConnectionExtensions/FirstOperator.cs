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
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T QueryFirst<T>(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null) where T : class, new()
        {
            try
            {
                DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
                IQuerySetting setting = connection.GetQuerySetting();
                string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                    commandParameters, setting.ParameterPrefix, commandSetting.ParameterNamePrefix);

                SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
                {
                    CommandText = sqlQuery,
                    CommandType = commandSetting?.CommandType ?? CommandType.Text,
                    CommandTimeout = commandSetting?.CommandTimeout,
                };
                simpleDbCommand.AddCommandParameters(commandParameters);

                IDbCommandResult<SimpleDbRow> commandResult = connection.QueryFirstAsDbRow(simpleDbCommand, transaction);
                T instance = commandResult.Result.ConvertRowTo<T>();
                return instance;
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }
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
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T GetFirst<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null) where T : class
        {
            try
            {
                DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                    .Select(p => new DbCommandParameter { Value = p, ParameterDbType = p.ToDbType() })
                    .ToArray() ?? new DbCommandParameter[0];
                SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(
                    odbcSqlQuery, commandParameters, commandSetting);

                IDbCommandResult<SimpleDbRow> dbRow = QueryFirstAsDbRow(connection, simpleDbCommand, transaction);

                T result = dbRow.Result.ConvertRowTo<T>();
                return result;
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }
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
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields the first as dynamic.</returns>
        public static async Task<SimpleDbRow> QueryFirstAsDbRowAsync(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryFirstAsDbRow(sqlText, obj, transaction, commandSetting);
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
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> FirstAsync<T>(this IDbConnection connection,
           string sqlText, object obj,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryFirst<T>(sqlText, obj,
                 transaction, commandSetting);
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

                    if (transaction == null)
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
                if (transaction == null)
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
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns first record as dynamic object.</returns>
        public static SimpleDbRow QueryFirstAsDbRow(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            try
            {
                DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
                IQuerySetting setting = connection.GetQuerySetting();
                string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                    commandParameters, setting.ParameterPrefix, commandSetting.ParameterNamePrefix);

                SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
                {
                    CommandText = sqlQuery,
                    CommandType = commandSetting?.CommandType ?? CommandType.Text,
                    CommandTimeout = commandSetting?.CommandTimeout,
                };
                simpleDbCommand.AddCommandParameters(commandParameters);
                SimpleDbRow instance = connection.QueryFirstAsDbRow(simpleDbCommand, transaction).Result;
                return instance;
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }
        }

        #endregion [ DbRow methods ]
    }
}