using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="ListOperator"/>.
    /// </summary>
    public static class ListOperator
    {
        /// <summary>
        /// Get List the specified ODBC SQL query with skip and take.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Db Command parameter values.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <returns>Returns as object list.</returns>
        public static List<T> GetList<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null, IPageInfo pageInfo = null) where T : class
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

            IDbCommandResult<List<SimpleDbRow>> rowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);

            List<T> resultSet = rowListResult.Result.ConvertRowsToList<T>();
            return resultSet;
        }

        #region [ Task methods ]

        /// <summary>
        /// Gets Resultset of query as object instance list with async operation.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns as object list.</returns>
        public static async Task<List<T>> QueryListAsync<T>(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, IPageInfo pageInfo = null,
            char parameterNamePrefix = '?') where T : class, new()
        {
            Task<List<T>> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryList<T>(sqlText, obj, commandType,
                transaction, pageInfo, parameterNamePrefix);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}