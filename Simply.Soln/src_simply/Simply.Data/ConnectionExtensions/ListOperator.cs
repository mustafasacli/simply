using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
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
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <returns>Returns as object list.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static List<T> GetList<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null) where T : class
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, commandSetting);

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand,
                transaction, pageInfo);

            List<T> instanceList = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return instanceList;
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
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <returns>Returns as object list.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static async Task<List<T>> QueryListAsync<T>(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IPageInfo pageInfo = null) where T : class, new()
        {
            Task<List<T>> resultTask = Task.Factory.StartNew(() =>
            {
                return
                connection.QueryList<T>(sqlText, obj,
                transaction, commandSetting, pageInfo);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}