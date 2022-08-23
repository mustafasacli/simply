using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbRowListOperator"/>.
    /// </summary>
    public static class DbRowListOperator
    {
        /// <summary>
        /// QueryMultiDbRowList Gets query multi result set as multi expando object list.
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
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> QueryMultiDbRowList(this IDbConnection connection,
            string sqlText, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
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
            };
            simpleDbCommand.AddCommandParameters(commandParameters);

            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowListResult = 
                connection.GetMultiDbRowListQuery(simpleDbCommand, transaction);
            return multiSimpleDbRowListResult.Result;
        }

        /// <summary>
        /// GetDbRowListQuery Gets query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowListQuery(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null,
            CommandBehavior? behavior = null)
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult;

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                using (IDataReader reader = command.ExecuteDataReader(behavior))
                {
                    simpleDbRowListResult = new DbCommandResult<List<SimpleDbRow>>();
                    simpleDbRowListResult.OutputParameters = command.GetOutParameters();
                    simpleDbRowListResult.Result = reader.GetResultSetAsDbRow(closeAtFinal: true) ??
                        new List<SimpleDbRow>();
                }
            }

            return simpleDbRowListResult;
        }

        /// <summary>
        /// GetMultiDbRowListQuery Gets query multi result set as multi SimpleDbRow list.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> GetMultiDbRowListQuery(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null,
            CommandBehavior? behavior = null)
        {
            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowListResult;

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                using (IDataReader reader = command.ExecuteDataReader(behavior))
                {
                    multiSimpleDbRowListResult = new DbCommandResult<List<List<SimpleDbRow>>>();
                    multiSimpleDbRowListResult.OutputParameters = command.GetOutParameters();
                    multiSimpleDbRowListResult.Result = reader.GetMultiDbRowList(closeAtFinal: true) ??
                        new List<List<SimpleDbRow>>();
                }
            }

            return multiSimpleDbRowListResult;
        }

        /// <summary>
        /// GetListAsDbRow Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> GetListAsDbRow(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray() ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, commandSetting);

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
                connection.GetDbRowListQuery(simpleDbCommand, transaction);

            List<SimpleDbRow> simpleDbRowList = simpleDbRowListResult.Result;
            return simpleDbRowList;
        }
    }
}