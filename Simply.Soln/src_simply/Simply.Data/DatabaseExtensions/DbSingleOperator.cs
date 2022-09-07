using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.DatabaseExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
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
        /// <returns>Returns single record as object instance.</returns>
        public static T QuerySingle<T>(this ISimpleDatabase database,
            string sqlText, object obj) where T : class, new()
        {
            SimpleDbRow row = database.QuerySingleAsDbRow(sqlText, obj);
            T instance = row.ConvertRowTo<T>();
            return instance;
        }

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
        /// Get Single Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static T GetSingle<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues) where T : class
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter { Value = p })
                .ToArray() ?? new DbCommandParameter[0];
            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(
                odbcSqlQuery, commandParameters, database.CommandSetting);

            IDbCommandResult<SimpleDbRow> simpleDbRowResult =
                connection.QuerySingleAsDbRow(simpleDbCommand, transaction, logSetting: database.LogSetting);
            T instance = simpleDbRowResult.Result.ConvertRowTo<T>();
            return instance;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlText">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>An asynchronous result that yields the single as dynamic.</returns>
        public static async Task<SimpleDbRow> QuerySingleAsDbRowAsync(
            this ISimpleDatabase database, string sqlText, object obj)
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QuerySingleAsDbRow(sqlText, obj);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlText">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> SingleAsync<T>(this ISimpleDatabase database,
            string sqlText, object obj) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QuerySingle<T>(sqlText, obj);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        #region [ SimpleDbRow methods ]

        /// <summary>
        /// Get Single Row of the Resultset as dynamic object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
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
        /// <returns>Returns single record as dynamic object.</returns>
        public static SimpleDbRow QuerySingleAsDbRow(this ISimpleDatabase database,
            string sqlText, object obj)
        {
            IDbConnection connection = database.GetDbConnection();

            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, database.CommandSetting?.ParameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = database.CommandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = database.CommandSetting?.CommandTimeout,
            };
            simpleDbCommand.AddCommandParameters(commandParameters);

            SimpleDbRow simpleDbRow =
                database.QuerySingleAsDbRow(simpleDbCommand).Result;
            return simpleDbRow;
        }

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
            IDataReader reader = null;

            try
            {
                IDbConnection connection = database.GetDbConnection();
                IDbTransaction transaction = database.GetDbTransaction();

                if (transaction == null)
                    connection.OpenIfNot();

                DbCommandParameter[] outputValues;

                reader = connection.ExecuteReaderQuery(
                    simpleDbCommand, out outputValues, transaction, commandBehavior: null, logSetting: database.LogSetting);

                simpleDbRowResult.OutputParameters = outputValues;
                simpleDbRowResult.Result = reader.SingleDbRow();
            }
            finally
            { reader?.CloseIfNot(); }

            return simpleDbRowResult;
        }

        #endregion [ SimpleDbRow methods ]
    }
}