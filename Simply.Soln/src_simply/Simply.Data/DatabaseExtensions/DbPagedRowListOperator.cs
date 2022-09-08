using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Constants;
using Simply.Data.DatabaseExtensions;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Helpers;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.
        /// </param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> QueryDbRowList(this ISimpleDatabase database,
            string sqlText, object obj, IPageInfo pageInfo = null)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, querySetting.ParameterPrefix, database.CommandSetting?.ParameterNamePrefix);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sql,
                CommandType = database.CommandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = database.CommandSetting?.CommandTimeout,
                ParameterNamePrefix = database.CommandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(commandParameters);

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                connection.GetDbRowList(simpleDbCommand,
                transaction, pageInfo, logSetting: database.LogSetting);
            List<SimpleDbRow> simpleDbRowList = simpleDbRowListResult.Result;

            return simpleDbRowList;
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
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = new DbCommandResult<List<SimpleDbRow>>();

            bool isPageableAndSkipAndTakeFormatEmpty = false;

            if (pageInfo != null)
            {
                if (pageInfo.Take == 0)
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

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);

                using (IDataReader reader = command.ExecuteReader())
                {
                    simpleDbRowListResult.OutputParameters = command.GetOutParameters();
                    simpleDbRowListResult.Result =
                        (isPageableAndSkipAndTakeFormatEmpty ?
                        reader.GetSimleRowListSkipAndTake(
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
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="values">The parameters.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <returns>Returns as SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> SelectDbRowList(this ISimpleDatabase database,
           string odbcSqlQuery, object[] values, IPageInfo pageInfo = null)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = (values ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, database.CommandSetting);

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                connection.GetDbRowList(simpleDbCommand, transaction,
                pageInfo, logSetting: database.LogSetting);

            List<SimpleDbRow> simpleDbRowList = simpleDbRowListResult.Result;
            return simpleDbRowList;
        }

        #endregion [ Page Info methods ]
    }
}