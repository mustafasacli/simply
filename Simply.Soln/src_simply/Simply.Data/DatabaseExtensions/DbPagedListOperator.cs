using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.DatabaseExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbPagedListOperator"/>.
    /// </summary>
    public static class DbPagedListOperator
    {
        #region [ Page Info methods ]

        /// <summary>
        /// Gets query resultset as object list with paging option.
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
        /// <param name="parameterObject">object which has contains parameters as properties.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns as object list.</returns>
        public static List<T> QueryList<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, IPageInfo pageInfo = null, CommandType? commandType = null) where T : class, new()
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction, pageInfo, logSetting: database.LogSetting);

            List<T> instanceList = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return instanceList;
        }

        /// <summary>
        /// Gets query resultset as object list with paging option.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <returns>Returns as object list.</returns>
        public static IDbCommandResult<List<T>> GetList<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null)
            where T : class, new()
        {
            IDbCommandResult<List<T>> instanceListResult = new DbCommandResult<List<T>>();

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                database.GetDbRowList(simpleDbCommand, pageInfo);

            instanceListResult.Result =
                simpleDbRowListResult.Result.ConvertRowsToList<T>()
                ?? new List<T>();
            instanceListResult.AdditionalValues = simpleDbRowListResult.AdditionalValues;

            return instanceListResult;
        }

        /// <summary>
        /// Get List the specified ODBC SQL query with paging option.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">The parameters.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns as object list.</returns>
        public static List<T> SelectList<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           IPageInfo pageInfo = null, CommandType? commandType = null) where T : class
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand,
                transaction, pageInfo, logSetting: database.LogSetting);

            List<T> instanceList = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return instanceList;
        }

        #endregion [ Page Info methods ]
    }
}