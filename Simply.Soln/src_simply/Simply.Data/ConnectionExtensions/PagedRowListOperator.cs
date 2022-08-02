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
        /// QueryDbRowList Gets query resultset as expando object list with paging option.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlText">Sql query.
        /// if parameterNamePrefix is ? and Query: Select * From TableName Where Column1 = ?p1?
        /// Then;
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// if parameterNamePrefix is null and Query: Select * From TableName Where Column1 = :p1 (for PostgreSql)
        /// no conversion occured.
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.
        /// </param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> QueryDbRowList(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null,
            IPageInfo pageInfo = null, char? parameterNamePrefix = null)
        {
            List<SimpleDbRow> result;

            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);

            SimpleDbCommand commandDefinition = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = commandType
            };
            commandDefinition.AddCommandParameters(commandParameters);

            IDbCommandResult<List<SimpleDbRow>> dynamicResultSet =
                GetDbRowList(connection, commandDefinition, transaction, pageInfo);
            result = dynamicResultSet.Result;

            return result;
        }

        /// <summary>
        /// GetDbRowList Gets query resultset as SimpleDbRow object list with skip and take.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="commandDefinition">Command Definition.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowList(
            this IDbConnection connection, SimpleDbCommand commandDefinition,
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
                    format = format.Replace(InternalAppValues.SqlScriptFormat, commandDefinition.CommandText);
                    commandDefinition.CommandText = format.CopyValue();
                }
            }

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                try
                {
                    if (transaction == null && commandDefinition.AutoOpen)
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
                    if (transaction == null && commandDefinition.CloseAtFinal)
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
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns as SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> SelectDbRowList(this IDbConnection connection,
           string odbcSqlQuery, object[] values,
           IPageInfo pageInfo = null, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommandParameter[] commandParameters = (values ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();

            SimpleDbCommand commandDefinition =
                connection.BuildCommandDefinitionForTranslate(odbcSqlQuery,
                commandParameters, commandType, commandTimeout);

            IDbCommandResult<List<SimpleDbRow>> dynamicResultSet =
                GetDbRowList(connection, commandDefinition, transaction, pageInfo);

            List<SimpleDbRow> result = dynamicResultSet.Result;
            return result;
        }

        #endregion [ Page Info methods ]
    }
}