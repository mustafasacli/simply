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
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, commandSetting?.ParameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
            };
            simpleDbCommand.AddCommandParameters(commandParameters);

            IDbCommandResult<SimpleDbRow> simpleDbRowResult = connection.QueryFirstAsDbRow(simpleDbCommand, transaction);
            T instance = simpleDbRowResult.Result.ConvertRowTo<T>();
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
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = connection.QueryFirstAsDbRow(simpleDbCommand, transaction);

            IDbCommandResult<T> instanceResult = new DbCommandResult<T>();
            instanceResult.Result = simpleDbRowResult.Result.ConvertRowTo<T>();
            instanceResult.AdditionalValues = simpleDbRowResult.AdditionalValues;
            return instanceResult;
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
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter { Value = p, ParameterDbType = p.ToDbType() })
                .ToArray() ?? new DbCommandParameter[0];
            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(
                odbcSqlQuery, commandParameters, commandSetting);

            IDbCommandResult<SimpleDbRow> simpleDbRowResult = QueryFirstAsDbRow(connection, simpleDbCommand, transaction);

            T instance = simpleDbRowResult.Result.ConvertRowTo<T>();
            return instance;
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
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = new DbCommandResult<SimpleDbRow>();

            IDataReader reader = null;

            try
            {
                //if (transaction == null)
                //    connection.OpenIfNot();

                DbCommandParameter[] outputValues;
                reader = connection.ExecuteReaderQuery(
                    simpleDbCommand, out outputValues, transaction, commandBehavior: CommandBehavior.SingleRow);

                simpleDbRowResult.OutputParameters = outputValues;
                simpleDbRowResult.Result = reader.FirstDbRow(closeAtFinal: true);
            }
            finally
            { reader?.CloseIfNot(); }

            return simpleDbRowResult;
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
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sqlQuery = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, commandSetting?.ParameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
            };
            simpleDbCommand.AddCommandParameters(commandParameters);

            SimpleDbRow simpleDbRow = connection.QueryFirstAsDbRow(simpleDbCommand, transaction).Result;
            return simpleDbRow;
        }

        #endregion [ DbRow methods ]
    }
}