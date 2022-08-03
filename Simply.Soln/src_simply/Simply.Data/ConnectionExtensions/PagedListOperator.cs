using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
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
        /// </param>
        /// <param name="obj">object which has contains parameters as properties.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns as object list.</returns>
        public static List<T> QueryList<T>(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, IPageInfo pageInfo = null,
            char? parameterNamePrefix = null) where T : class, new()
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandType
            };

            simpleDbCommand.AddCommandParameters(commandParameters);

            IDbCommandResult<List<SimpleDbRow>> rowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);

            List<T> result = rowListResult.Result.ConvertRowsToList<T>();
            return result;
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
        public static IDbCommandResult<List<T>> GetList<T>(
            this IDbConnection connection, SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null, IPageInfo pageInfo = null)
            where T : class, new()
        {
            IDbCommandResult<List<T>> result = new DbCommandResult<List<T>>();

            IDbCommandResult<List<SimpleDbRow>> rowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);

            result.Result =
                rowListResult.Result.ConvertRowsToList<T>()
                ?? new List<T>();
            result.AdditionalValues = rowListResult.AdditionalValues;

            return result;
        }

        /// <summary>
        /// Get List the specified ODBC SQL query with paging option.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="values">The parameters.</param>
        /// <param name="pageInfo">page info for skip and take counts.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns as object list.</returns>
        public static List<T> SelectList<T>(this IDbConnection connection,
           string odbcSqlQuery, object[] values,
            IPageInfo pageInfo = null,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            DbCommandParameter[] commandParameters = (values ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildsimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, commandType, commandTimeout);

            IDbCommandResult<List<SimpleDbRow>> rowListResult =
                PagedRowListOperator.GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);

            List<T> result = rowListResult.Result.ConvertRowsToList<T>();
            return result;
        }

        #endregion [ Page Info methods ]
    }
}