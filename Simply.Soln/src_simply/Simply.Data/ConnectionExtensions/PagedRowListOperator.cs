using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="PagedRowListOperator"/>.
    /// </summary>
    public static class PagedRowListOperator
    {
        #region [ Page Info methods ]

        /// <summary>
        /// QueryDbRowList Gets query resultset as simpledbrow list with paging option.
        /// </summary>
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
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.
        /// </param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        [Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static List<SimpleDbRow> QueryDbRowList(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IPageInfo pageInfo = null)
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
                GetDbRowList(connection, simpleDbCommand,
                transaction, pageInfo);
            List<SimpleDbRow> simpleDbRowList = simpleDbRowListResult.Result;

            return simpleDbRowList;
        }

        /// <summary>
        /// GetDbRowList Gets query resultset as SimpleDbRow object list with skip and take.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        [Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowList(
            this IDbConnection connection, SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null, IPageInfo pageInfo = null)
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = new DbCommandResult<List<SimpleDbRow>>();

            bool isPageableAndSkipAndTakeFormatEmpty = false;

            if (pageInfo != null)
            {
                if (!pageInfo.IsPageable)
                    return simpleDbRowListResult;

                IQuerySetting querySetting = connection.GetQuerySetting();
                isPageableAndSkipAndTakeFormatEmpty = querySetting.SkipAndTakeFormat.IsNullOrSpace();
                if (!isPageableAndSkipAndTakeFormatEmpty)
                {
                    string format = querySetting.SkipAndTakeFormat.CopyValue();
                    format = format.Replace(InternalAppValues.SkipFormat, pageInfo.Skip.ToString());
                    format = format.Replace(InternalAppValues.TakeFormat, pageInfo.Take.ToString());
                    format = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                    simpleDbCommand.CommandText = format.CopyValue();
                }
            }

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    simpleDbRowListResult.OutputParameters = command.GetOutParameters();
                    simpleDbRowListResult.Result =
                        (isPageableAndSkipAndTakeFormatEmpty ?
                        reader.GetSimpleRowListSkipAndTake(
                            skip: pageInfo.Skip, take: pageInfo.Take, closeAtFinal: true)
                        : reader.GetResultSetAsDbRow(closeAtFinal: true))
                        ?? new List<SimpleDbRow>();
                }
            }

            return simpleDbRowListResult;
        }

        /// <summary>
        /// Get List the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="values">The parameters.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns as SimpleDbRow object list.</returns>
        [Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static List<SimpleDbRow> SelectDbRowList(this IDbConnection connection,
           string odbcSqlQuery, object[] values, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null)
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
                GetDbRowList(connection, simpleDbCommand, transaction,
                pageInfo);

            List<SimpleDbRow> simpleDbRowList = simpleDbRowListResult.Result;
            return simpleDbRowList;
        }

        #endregion [ Page Info methods ]
    }
}