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
        /// </param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> QueryMultiDbRowList(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, char? parameterNamePrefix = null)
        {
            DbCommandParameter[] commandParameters = connection.TranslateParametersFromObject(obj);
            IQuerySetting setting = connection.GetQuerySetting();
            string sql = DbCommandBuilder.RebuildQueryWithParamaters(sqlText,
                commandParameters, setting.ParameterPrefix, parameterNamePrefix);
            DbCommandDefinition commandDefinition = new DbCommandDefinition()
            {
                CommandText = sql,
                CommandType = commandType
            };
            commandDefinition.AddCommandParameters(commandParameters);
            IDbCommandResult<List<List<SimpleDbRow>>> dynamicList =
                connection.GetMultiDbRowListQuery(commandDefinition, transaction);
            return dynamicList.Result;
        }

        /// <summary>
        /// GetDbRowListQuery Gets query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="commandDefinition">Command Definition.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowListQuery(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null,
            CommandBehavior? behavior = null)
        {
            IDbCommandResult<List<SimpleDbRow>> result = new DbCommandResult<List<SimpleDbRow>>();

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                using (IDataReader reader = command.ExecuteDataReader(behavior))
                {
                    result.OutputParameters = command.GetOutParameters();
                    result.Result = reader.GetResultSetAsDbRow(closeAtFinal: true) ??
                        new List<SimpleDbRow>();
                }
            }

            return result;
        }

        /// <summary>
        /// GetMultiDbRowListQuery Gets query multi result set as multi SimpleDbRow list.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="commandDefinition">Command Definition.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> GetMultiDbRowListQuery(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null,
            CommandBehavior? behavior = null)
        {
            IDbCommandResult<List<List<SimpleDbRow>>> result = new DbCommandResult<List<List<SimpleDbRow>>>();

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                using (IDataReader reader = command.ExecuteDataReader(behavior))
                {
                    result.OutputParameters = command.GetOutParameters();
                    result.Result = reader.GetMultiDbRowList(closeAtFinal: true) ??
                        new List<List<SimpleDbRow>>();
                }
            }

            return result;
        }

        /// <summary>
        /// GetListAsDbRow Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> GetListAsDbRow(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                }).ToArray() ?? ArrayHelper.Empty<DbCommandParameter>();

            DbCommandDefinition commandDefinition =
                connection.BuildCommandDefinitionForTranslate(odbcSqlQuery, commandParameters, commandType, commandTimeout);

            IDbCommandResult<List<SimpleDbRow>> dynamicResult =
                connection.GetDbRowListQuery(commandDefinition, transaction);

            List<SimpleDbRow> resultSet = dynamicResult.Result;

            return resultSet;
        }
    }
}