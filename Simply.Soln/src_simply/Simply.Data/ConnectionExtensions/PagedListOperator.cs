using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="PagedListOperator"/>.
    /// </summary>
    public static class PagedListOperator
    {
        #region [ Page Info methods ]

        /// <summary>
        /// Gets query resultset as object list with paging option.
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
        /// <param name="obj">object which has contains parameters as properties.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <returns>Returns as object list.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static List<T> QueryList<T>(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IPageInfo pageInfo = null) where T : class, new()
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, commandSetting?.ParameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
                ParameterNamePrefix = commandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(commandParameters);

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);

            List<T> instanceList = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return instanceList;
        }

        /// <summary>
        /// Gets query resultset as object list with paging option.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <returns>Returns as object list.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static IDbCommandResult<List<T>> GetList<T>(
            this IDbConnection connection, SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null, IPageInfo pageInfo = null)
            where T : class, new()
        {
            IDbCommandResult<List<T>> instanceListResult = new DbCommandResult<List<T>>();

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction,
                pageInfo);

            instanceListResult.Result =
                simpleDbRowListResult.Result.ConvertRowsToList<T>()
                ?? new List<T>();
            instanceListResult.AdditionalValues = simpleDbRowListResult.AdditionalValues;

            return instanceListResult;
        }

        /// <summary>
        /// Get List the specified ODBC SQL query with paging option.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="values">The parameters.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="transaction">Database transaction.</param>
        /// <returns>Returns as object list.</returns>
        //[Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static List<T> SelectList<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] values, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null) where T : class
        {
            DbCommandParameter[] commandParameters = (values ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, commandSetting);

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand,
                transaction, pageInfo);

            List<T> instanceList = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return instanceList;
        }

        #endregion [ Page Info methods ]
    }
}