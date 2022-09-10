using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbPagedRowListOperator"/>.
    /// </summary>
    public static class DbPagedRowListOperator
    {
        #region [ Page Info methods ]

        /// <summary>
        /// QueryDbRowList Gets query resultset as simpledbrow list with paging option.
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
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.
        /// </param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> QueryDbRowList(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, IPageInfo pageInfo = null, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<List<SimpleDbRow>> result = database.GetDbRowList(simpleDbCommand, pageInfo);
            return result.Result;
        }

        /// <summary>
        /// GetDbRowList Gets query resultset as SimpleDbRow object list with skip and take.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowList(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null)
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = new DbCommandResult<List<SimpleDbRow>>();

            if (pageInfo != null)
            {
                if (pageInfo.Take == 0)
                    return simpleDbRowListResult;

                IQuerySetting querySetting = database.QuerySetting;
                bool isPageableAndSkipAndTakeFormatEmpty = querySetting.SkipAndTakeFormat.IsNullOrSpace();
                if (!isPageableAndSkipAndTakeFormatEmpty)
                {
                    string format = querySetting.SkipAndTakeFormat.CopyValue();
                    format = format.Replace(InternalAppValues.SkipFormat, pageInfo.Skip.ToString());
                    format = format.Replace(InternalAppValues.TakeFormat, pageInfo.Take.ToString());
                    format = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                    simpleDbCommand.CommandText = format.CopyValue();
                }
            }

            simpleDbRowListResult = database.GetDbRowListQuery(simpleDbCommand);
            return simpleDbRowListResult;
        }

        /// <summary>
        /// Get List the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">The parameters.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns as SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> SelectDbRowList(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, IPageInfo pageInfo = null, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<List<SimpleDbRow>> result = database.GetDbRowList(simpleDbCommand, pageInfo);
            return result.Result;
        }

        #endregion [ Page Info methods ]
    }
}