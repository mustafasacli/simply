using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.DatabaseExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;
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
        /// <returns>Returns last record as object instance.</returns>
        public static T QueryLast<T>(this ISimpleDatabase database,
            string sqlText, object obj) where T : class, new()
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, database.CommandSetting?.ParameterNamePrefix);
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = database.CommandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = database.CommandSetting?.CommandTimeout,
                ParameterNamePrefix = database.CommandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(parameters);

            SimpleDbRow simpleDbRow = connection.QueryLastAsDbRow(simpleDbCommand, transaction, logSetting: database.LogSetting).Result;
            T instance = simpleDbRow.ConvertRowTo<T>();
            return instance;
        }

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
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<SimpleDbRow> simpleDbRowResult =
                connection.QueryLastAsDbRow(simpleDbCommand, transaction, logSetting: database.LogSetting);

            IDbCommandResult<T> instanceResult = new DbCommandResult<T>();
            instanceResult.Result = simpleDbRowResult.Result.ConvertRowTo<T>();
            instanceResult.AdditionalValues = simpleDbRowResult.AdditionalValues;
            return instanceResult;
        }

        /// <summary>
        /// Get Last Row of the Odbc Sql query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <returns>Returns last record as object instance.</returns>
        public static T GetLast<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues) where T : class
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(
                odbcSqlQuery, commandParameters, database.CommandSetting);

            IDbCommandResult<SimpleDbRow> simpleDbRowResult =
                connection.QueryLastAsDbRow(simpleDbCommand, transaction, logSetting: database.LogSetting);

            T instance = simpleDbRowResult.Result.ConvertRowTo<T>();
            return instance;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get Last Row of the Resultset as dynamic object instance with async operation.
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
        /// <returns>An asynchronous result that yields the last as dynamic.</returns>
        public static async Task<SimpleDbRow> LastAsDynamicAsync(this ISimpleDatabase database,
            string sqlText, object obj)
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryLastDbRow(sqlText, obj);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Last Row of the Resultset as object instance with async operation.
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
        public static async Task<T> LastAsync<T>(this ISimpleDatabase database,
           string sqlText, object obj) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryLast<T>(sqlText, obj);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        /// <summary>
        /// Get Last Row of the Resultset as SimpleDbRow object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns last record as dynamic object instance.</returns>
        public static IDbCommandResult<SimpleDbRow> QueryLastAsDbRow(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = new DbCommandResult<SimpleDbRow>();
            IDataReader reader = null;

            try
            {
                IDbConnection connection = database.GetDbConnection();
                IDbTransaction transaction = database.GetDbTransaction();

                if (transaction == null)
                    connection.OpenIfNot();

                IQuerySetting querySetting = connection.GetQuerySetting();
                CommandBehavior? commandBehavior = null;

                if (!querySetting.LastFormat.IsNullOrSpace())
                {
                    string format = querySetting.LastFormat;
                    simpleDbCommand.CommandText =
                        format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                    commandBehavior = CommandBehavior.SingleRow;
                }

                DbCommandParameter[] outputValues;
                reader =
                    connection.ExecuteReaderQuery(simpleDbCommand, out outputValues,
                    transaction, commandBehavior, logSetting: database.LogSetting);

                simpleDbRowResult.Result = reader.LastDbRow();
                simpleDbRowResult.OutputParameters = outputValues;
            }
            finally
            { reader?.CloseIfNot(); }

            return simpleDbRowResult;
        }

        /// <summary>
        /// Get Last Row of the Resultset as SimpleDbRow object instance.
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
        /// <returns>Returns last record as SimpleDbRow instance.</returns>
        public static SimpleDbRow QueryLastDbRow(this ISimpleDatabase database,
            string sqlText, object obj)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, database.CommandSetting?.ParameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = database.CommandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = database.CommandSetting?.CommandTimeout,
                ParameterNamePrefix = database.CommandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(commandParameters);

            IDbCommandResult<SimpleDbRow> simpleDbRowResult =
                connection.QueryLastAsDbRow(simpleDbCommand, transaction, logSetting: database.LogSetting);
            return simpleDbRowResult.Result;
        }
    }
}