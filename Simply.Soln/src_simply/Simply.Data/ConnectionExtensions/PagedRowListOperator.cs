using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
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
        public static List<SimpleDbRow> QueryDbRowList(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IPageInfo pageInfo = null)
        {
            List<SimpleDbRow> result;

            try
            {
                DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
                IQuerySetting setting = connection.GetQuerySetting();
                string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                    commandParameters, setting.ParameterPrefix, commandSetting.ParameterNamePrefix);

                SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
                {
                    CommandText = sql,
                    CommandType = commandSetting?.CommandType ?? CommandType.Text,
                    CommandTimeout = commandSetting?.CommandTimeout,
                };
                simpleDbCommand.AddCommandParameters(commandParameters);

                IDbCommandResult<List<SimpleDbRow>> dynamicResultSet =
                    GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);
                result = dynamicResultSet.Result;
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }

            return result;
        }

        /// <summary>
        /// GetDbRowList Gets query resultset as SimpleDbRow object list with skip and take.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowList(
            this IDbConnection connection, SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null, IPageInfo pageInfo = null)
        {
            DbCommandResult<List<SimpleDbRow>> result = new DbCommandResult<List<SimpleDbRow>>();

            bool isPageableAndSkipAndTakeFormatEmpty = false;

            if (pageInfo != null)
            {
                if (pageInfo.Take == 0)
                    return result;

                IQuerySetting setting = connection.GetQuerySetting();
                isPageableAndSkipAndTakeFormatEmpty = setting.SkipAndTakeFormat.IsNullOrSpace();
                if (!isPageableAndSkipAndTakeFormatEmpty)
                {
                    string format = setting.SkipAndTakeFormat.CopyValue();
                    format = format.Replace(InternalAppValues.SkipFormat, pageInfo.Skip.ToString());
                    format = format.Replace(InternalAppValues.TakeFormat, pageInfo.Take.ToString());
                    format = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                    simpleDbCommand.CommandText = format.CopyValue();
                }
            }

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                try
                {
                    if (transaction == null)
                        connection.OpenIfNot();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        result.OutputParameters = command.GetOutParameters();
                        result.Result =
                            (isPageableAndSkipAndTakeFormatEmpty ?
                            reader.GetSimleRowListSkipAndTake(
                                skip: pageInfo.Skip, take: pageInfo.Take, closeAtFinal: true)
                            : reader.GetResultSetAsDbRow(closeAtFinal: true))
                            ?? new List<SimpleDbRow>();
                    }
                }
                finally
                {
                    if (transaction == null)
                        connection.CloseIfNot();
                }
            }

            return result;
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
        public static List<SimpleDbRow> SelectDbRowList(this IDbConnection connection,
           string odbcSqlQuery, object[] values, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null)
        {
            try
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

                IDbCommandResult<List<SimpleDbRow>> dynamicResultSet =
                    GetDbRowList(connection, simpleDbCommand, transaction, pageInfo);

                List<SimpleDbRow> result = dynamicResultSet.Result;
                return result;
            }
            finally
            {
                if (commandSetting?.CloseAtFinal ?? false)
                    connection.CloseIfNot();
            }
        }

        #endregion [ Page Info methods ]
    }
}