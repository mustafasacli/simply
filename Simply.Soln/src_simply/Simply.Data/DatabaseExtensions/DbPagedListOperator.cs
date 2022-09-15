using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbPagedListOperator"/>.
    /// </summary>
    public static class DbPagedListOperator
    {
        #region [ Task methods ]

        /// <summary>
        /// GetDbRowListQuery Gets query result set as object instance list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static async Task<List<T>> GetListAsync<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            Task<List<T>> resultTask = Task.Factory.StartNew(() =>
            {
                return database.List<T>(simpleDbCommand, pageInfo, behavior);
            });

            return await resultTask;
        }

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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns as object list async.</returns>
        public static async Task<List<T>> QueryListAsync<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, IPageInfo pageInfo = null,
            ICommandSetting commandSetting = null, CommandBehavior? behavior = null) where T : class, new()
        {
            Task<List<T>> resultTask = Task.Factory.StartNew(() =>
            {
                return database.List<T>(sqlQuery, parameterObject, commandSetting, pageInfo, behavior);
            });

            return await resultTask;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static async Task<List<T>> GetListAsync<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            Task<List<T>> resultTask = Task.Factory.StartNew(() =>
            {
                return database.ListOdbc<T>(odbcSqlQuery, parameterValues, commandSetting, pageInfo, behavior);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}