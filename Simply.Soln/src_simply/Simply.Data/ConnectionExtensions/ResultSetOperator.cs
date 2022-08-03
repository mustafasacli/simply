using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="ResultSetOperator"/>.
    /// </summary>
    public static class ResultSetOperator
    {
        /// <summary>
        /// Get Resultset of the Command definition.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static IDbCommandResult<DataSet> GetResultSetQuery(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            DbCommandResult<DataSet> result = new DbCommandResult<DataSet>();

            DbDataAdapter dataAdapter = connection.CreateAdapter();
            if (dataAdapter == null)
                throw new Exception("DataAdapter class not found in .NET Provider.");

            DataSet dataSet;
            try
            {
                using (IDbCommand command =
                    connection.CreateCommandWithOptions(simpleDbCommand, transaction))
                {
                    dataAdapter.SelectCommand = (DbCommand)command;
                    dataSet = new DataSet();
                    if (transaction == null && simpleDbCommand.AutoOpen)
                        connection.OpenIfNot();
                    int executionResult = dataAdapter.Fill(dataSet);
                    result.ExecutionResult = executionResult;
                    result.Result = dataSet;
                    result.OutputParameters = command.GetOutParameters();
                }
            }
            finally
            {
                if (transaction == null && simpleDbCommand.CloseAtFinal)
                    connection.CloseIfNot();
            }

            return result;
        }

        /// <summary>
        /// Get Resultset of the specified ODBC SQL query.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static DataSet GetOdbcResultSet(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildsimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, commandType);
            IDbCommandResult<DataSet> resultSet = GetResultSetQuery(connection, simpleDbCommand, transaction);

            return resultSet.Result;
        }

        /// <summary>
        /// GetDynamicResultSetSkipAndTake Gets query resultset as dynamic object list with skip and take.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns dynamic object list.</returns>
        public static IDbCommandResult<DataTable> GetResultSet(
            this IDbConnection connection, SimpleDbCommand simpleDbCommand,
            IDbTransaction transaction = null, IPageInfo pageInfo = null)
        {
            IDbCommandResult<DataTable> result = new DbCommandResult<DataTable>();

            bool isPageableAndSkipAndTakeFormatEmpty = false;

            if (pageInfo != null)
            {
                if (!pageInfo.IsPageable)
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

            IDbCommandResult<DataSet> tempResultSet = GetResultSetQuery(connection, simpleDbCommand, transaction);
            result = new DbCommandResult<DataTable>
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
                    result.Result = table.CopyDatatable(pageInfo);
                }
                else
                {
                    result.Result = table;
                }
            }
            else
            { result.Result = new DataTable(); }

            return result;
        }
    }
}