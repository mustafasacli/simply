using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.DatabaseExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbListOperator"/>.
    /// </summary>
    public static class DbListOperator
    {
        /// <summary>
        /// Get List the specified ODBC SQL query with skip and take.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Db Command parameter values.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns as object list.</returns>
        public static List<T> GetList<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, IPageInfo pageInfo = null, CommandType? commandType = null) where T : class
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

        #region [ Task methods ]

        /// <summary>
        /// Gets Resultset of query as object instance list with async operation.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns as object list.</returns>
        public static async Task<List<T>> QueryListAsync<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, IPageInfo pageInfo = null, CommandType? commandType = null) where T : class, new()
        {
            Task<List<T>> resultTask = Task.Factory.StartNew(() =>
            {
                return database.QueryList<T>(sqlQuery, parameterObject, pageInfo, commandType);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}