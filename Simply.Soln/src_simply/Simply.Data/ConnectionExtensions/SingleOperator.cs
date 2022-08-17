using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="SingleOperator"/>.
    /// </summary>
    public static class SingleOperator
    {
        /// <summary>
        /// Get Single Row of the Resultset as object instance.
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
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns single record as object instance.</returns>
        public static T QuerySingle<T>(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, char? parameterNamePrefix = null) where T : class, new()
        {
            try
            {
                DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
                IQuerySetting setting = connection.GetQuerySetting();
                string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                    commandParameters, setting.ParameterPrefix, parameterNamePrefix);

                SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
                {
                    CommandText = sqlQuery,
                    CommandType = commandSetting?.CommandType ?? CommandType.Text,
                    CommandTimeout = commandSetting?.CommandTimeout,
                };
                simpleDbCommand.AddCommandParameters(commandParameters);

                IDbCommandResult<SimpleDbRow> commandResult = connection.QuerySingleAsDbRow(simpleDbCommand, transaction);
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
        /// Get Single Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns single record as dynamic object instance.</returns>
        public static IDbCommandResult<T> QuerySingle<T>(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null) where T : class, new()
        {
            IDbCommandResult<T> result = new DbCommandResult<T>();

            IDbCommandResult<SimpleDbRow> commandResult = connection.QuerySingleAsDbRow(simpleDbCommand, transaction);

            result.Result = commandResult.Result.ConvertRowTo<T>();
            result.AdditionalValues = commandResult.AdditionalValues;

            return result;
        }

        /// <summary>
        /// Get Single Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns single record as object instance.</returns>
        public static T GetSingle<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null) where T : class
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter { Value = p })
                .ToArray() ?? new DbCommandParameter[0];
            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(
                odbcSqlQuery, commandParameters, commandSetting);

            IDbCommandResult<SimpleDbRow> expando = QuerySingleAsDbRow(connection, simpleDbCommand, transaction);

            T result = expando.Result.ConvertRowTo<T>();
            return result;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>An asynchronous result that yields the single as dynamic.</returns>
        public static async Task<SimpleDbRow> QuerySingleDynamicAsync(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, char parameterNamePrefix = '?')
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QuerySingleAsDbRow(sqlText, obj, transaction, commandSetting, parameterNamePrefix);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> SingleAsync<T>(this IDbConnection connection,
           string sqlText, object obj, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null, char parameterNamePrefix = '?') where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QuerySingle<T>(sqlText, obj, transaction, commandSetting, parameterNamePrefix);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        #region [ SimpleDbRow methods ]

        /// <summary>
        /// Get Single Row of the Resultset as dynamic object instance.
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
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns single record as dynamic object.</returns>
        public static SimpleDbRow QuerySingleAsDbRow(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, char? parameterNamePrefix = null)
        {
            try
            {
                DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
                IQuerySetting setting = connection.GetQuerySetting();
                string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                    commandParameters, setting.ParameterPrefix, parameterNamePrefix);

                SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
                {
                    CommandText = sql,
                    CommandType = commandSetting?.CommandType ?? CommandType.Text,
                    CommandTimeout = commandSetting?.CommandTimeout,
                };
                simpleDbCommand.AddCommandParameters(commandParameters);
                SimpleDbRow instance = connection.QuerySingleAsDbRow(simpleDbCommand, transaction).Result;
                return instance;
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }
        }

        /// <summary>
        /// Get Single Row of the Resultset as dynamic object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns single record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QuerySingleAsDbRow(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IDbCommandResult<SimpleDbRow> result = new DbCommandResult<SimpleDbRow>();

            IDataReader reader = null;

            try
            {
                DbCommandParameter[] outputValues;

                reader = connection.ExecuteReaderQuery(
                    simpleDbCommand, out outputValues, transaction, commandBehavior: null);

                result.OutputParameters = outputValues;
                result.Result = reader.SingleDbRow();
            }
            finally
            { reader?.CloseIfNot(); }

            return result;
        }

        #endregion [ SimpleDbRow methods ]
    }
}