using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DatabaseExtensions;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Helpers;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbResultSetOperator"/>.
    /// </summary>
    public static class DbResultSetOperator
    {
        /// <summary>
        /// Get Resultset of the Command definition.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static IDbCommandResult<DataSet> GetResultSetQuery(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            DbCommandResult<DataSet> result = new DbCommandResult<DataSet>();

            DbDataAdapter dataAdapter = connection.CreateAdapter();
            if (dataAdapter == null)
                throw new Exception(DbAppMessages.DataAdapterNotFound);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);
                dataAdapter.SelectCommand = (DbCommand)command;
                DataSet dataSet = new DataSet();
                result.ExecutionResult = dataAdapter.Fill(dataSet);
                result.Result = dataSet;
                result.OutputParameters = command.GetOutParameters();
            }

            return result;
        }

        /// <summary>
        /// Get Resultset of the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static DataSet GetOdbcResultSet(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, database.CommandSetting);

            IDbCommandResult<DataSet> resultSet =
                connection.GetResultSetQuery(simpleDbCommand, transaction, logSetting: database.LogSetting);
            return resultSet.Result;
        }

        /// <summary>
        /// GetDynamicResultSetSkipAndTake Gets query resultset as dynamic object list with skip and take.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns dynamic object list.</returns>
        public static IDbCommandResult<DataTable> GetResultSet(this ISimpleDatabase database,
             SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null)
        {
            IDbCommandResult<DataTable> dataTableResult = new DbCommandResult<DataTable>();

            bool isPageableAndSkipAndTakeFormatEmpty = false;
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (pageInfo != null)
            {
                if (!pageInfo.IsPageable)
                    return dataTableResult;

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

            IDbCommandResult<DataSet> tempResultSet =
                connection.GetResultSetQuery(simpleDbCommand, transaction, logSetting: database.LogSetting);
            dataTableResult = new DbCommandResult<DataTable>
            {
                ExecutionResult = tempResultSet.ExecutionResult,
                AdditionalValues = tempResultSet.AdditionalValues,
                OutputParameters = tempResultSet.OutputParameters
            };

            if (tempResultSet.Result.Tables.Count > 0)
            {
                DataTable table = tempResultSet.Result.Tables[0];
                if (isPageableAndSkipAndTakeFormatEmpty)
                {
                    dataTableResult.Result = table.CopyDatatable(pageInfo);
                }
                else
                {
                    dataTableResult.Result = table;
                }
            }
            else
            { dataTableResult.Result = new DataTable(); }

            return dataTableResult;
        }
    }
}