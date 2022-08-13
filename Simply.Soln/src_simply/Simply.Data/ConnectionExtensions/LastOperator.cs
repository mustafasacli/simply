using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="LastOperator"/>.
    /// </summary>
    public static class LastOperator
    {
        /// <summary>
        /// Get Last Row of the Resultset as object instance.
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
        /// <returns>Returns last record as object instance.</returns>
        public static T QueryLast<T>(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char? parameterNamePrefix = null) where T : class, new()
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandType
            };
            simpleDbCommand.AddCommandParameters(parameters);
            SimpleDbRow row = connection.QueryLastAsDbRow(simpleDbCommand, transaction).Result;

            T instance = row.ConvertRowTo<T>();
            return instance;
        }

        /// <summary>
        /// Get Last Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns last record as object instance.</returns>
        public static IDbCommandResult<T> QueryLast<T>(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null) where T : class, new()
        {
            IDbCommandResult<T> result = new DbCommandResult<T>();

            IDbCommandResult<SimpleDbRow> dbCommandResult =
                connection.QueryLastAsDbRow(simpleDbCommand, transaction);

            result.Result = dbCommandResult.Result.ConvertRowTo<T>();
            result.AdditionalValues = dbCommandResult.AdditionalValues;

            return result;
        }

        /// <summary>
        /// Get Last Row of the Odbc Sql query Resultset as object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns last record as object instance.</returns>
        public static T GetLast<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();
            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, commandType, commandTimeout);

            IDbCommandResult<SimpleDbRow> dbCommandResult = connection.QueryLastAsDbRow(simpleDbCommand, transaction);

            T result = dbCommandResult.Result.ConvertRowTo<T>();

            return result;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get Last Row of the Resultset as dynamic object instance with async operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>An asynchronous result that yields the last as dynamic.</returns>
        public static async Task<SimpleDbRow> LastAsDynamicAsync(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char parameterNamePrefix = '?')
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryLastDbRow(sqlText, obj, commandType, transaction, parameterNamePrefix);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Last Row of the Resultset as object instance with async operation.
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
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> LastAsync<T>(this IDbConnection connection,
           string sqlText, object obj, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, char parameterNamePrefix = '?') where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryLast<T>(sqlText, obj, commandType, transaction, parameterNamePrefix);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        /// <summary>
        /// Get Last Row of the Resultset as SimpleDbRow object instance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns last record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QueryLastAsDbRow(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null)
        {
            IDbCommandResult<SimpleDbRow> result = new DbCommandResult<SimpleDbRow>();
            IDataReader reader = null;

            try
            {
                try
                {
                    IQuerySetting setting = connection.GetQuerySetting();
                    CommandBehavior? commandBehavior = null;

                    if (!setting.LastFormat.IsNullOrSpace())
                    {
                        string format = setting.LastFormat;
                        simpleDbCommand.CommandText =
                            format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                        commandBehavior = CommandBehavior.SingleRow;
                    }

                    DbCommandParameter[] outputValues;

                    if (transaction == null && simpleDbCommand.AutoOpen)
                        connection.OpenIfNot();

                    reader =
                        connection.ExecuteReaderQuery(simpleDbCommand, out outputValues, transaction, commandBehavior);

                    result.Result = reader.LastDbRow();
                    result.OutputParameters = outputValues;
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
        /// Get Last Row of the Resultset as SimpleDbRow object instance.
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
        /// <returns>Returns last record as dynamic object instance.</returns>
        public static SimpleDbRow QueryLastDbRow(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char? parameterNamePrefix = null)
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandType
            };
            simpleDbCommand.AddCommandParameters(commandParameters);
            IDbCommandResult<SimpleDbRow> instance =
                connection.QueryLastAsDbRow(simpleDbCommand, transaction);

            return instance.Result;
        }
    }
}