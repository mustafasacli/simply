using Simply.Common.Objects;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
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
        public static T First<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            SimpleDbRow simpleRow = database.FirstRow(simpleDbCommand);
            T instance = simpleRow.ConvertRowTo<T>();
            return instance;
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
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T First<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            T instance = database.First<T>(simpleDbCommand);
            return instance;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T FirstOdbc<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            T instance = database.First<T>(simpleDbCommand);
            return instance;
        }

        /// <summary>
        /// Get First Row of the Jdbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ?1 ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static T FirstJdbc<T>(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
            T instance = database.First<T>(simpleDbCommand);
            return instance;
        }

        #region [ Task methods ]

        /// <summary>
        /// Get First Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns first record as T object instance as async.</returns>
        public static async Task<T> FirstAsync<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.First<T>(simpleDbCommand);
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
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> FirstAsync<T>(this ISimpleDatabase database,
           string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.First<T>(sqlQuery, parameterObject, commandSetting);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>An asynchronous result that yields the first as T.</returns>
        public static async Task<T> FirstOdbcAsync<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.FirstOdbc<T>(odbcSqlQuery, parameterValues, commandSetting);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get First Row of the Jdbc Sql Query Resultset as object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ?1 ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>An asynchronous result that yields the first as T.</returns>
        public static async Task<T> FirstJdbcAsync<T>(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.FirstJdbc<T>(jdbcSqlQuery, parameterValues, commandSetting);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        #region [ DbRow methods ]

        /// <summary>
        /// Get First Row of the Resultset as simple db row instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>Returns first record as simple db row instance.</returns>
        public static SimpleDbRow FirstRow(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand)
        {
            SimpleDbRow simpleRow = SimpleDbRow.NewRow();

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            using (IDataReader dataReader = command.ExecuteDataReader(CommandBehavior.SingleRow))
            {
                try
                {
                    simpleRow = dataReader.FirstDbRow(closeAtFinal: true);
                }
                finally
                { dataReader?.CloseIfNot(); }
            }

            return simpleRow;
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
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns first record as dynamic object.</returns>
        public static SimpleDbRow FirstRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            SimpleDbRow simpleRow = database.FirstRow(simpleDbCommand);
            return simpleRow;
        }

        /// <summary>
        /// Get First Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static SimpleDbRow FirstRowOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            SimpleDbRow simpleRow = database.FirstRow(simpleDbCommand);
            return simpleRow;
        }

        /// <summary>
        /// Get First Row of the Jdbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ?1 ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns first record as object instance.</returns>
        public static SimpleDbRow FirstRowJdbc(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
            SimpleDbRow simpleRow = database.FirstRow(simpleDbCommand);
            return simpleRow;
        }

        #endregion [ DbRow methods ]
    }
}