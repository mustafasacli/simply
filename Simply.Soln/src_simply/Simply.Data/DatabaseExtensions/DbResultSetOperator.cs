﻿using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbResultSetOperator"/>.
    /// </summary>
    public static class DbResultSetOperator
    {
        /// <summary>
        /// Get Resultset of the specified SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="parameterObject">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static DataSet GetDataSet(this ISimpleDatabase database,
           string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                IDbCommandResult<DataSet> resultSet = database.GetResultSetQuery(simpleDbCommand);
                return resultSet.Result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Resultset of the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static DataSet GetDataSetOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                IDbCommandResult<DataSet> resultSet = database.GetResultSetQuery(simpleDbCommand);
                return resultSet.Result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Resultset of the specified JDBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static DataSet GetDataSetJdbc(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                IDbCommandResult<DataSet> resultSet = database.GetResultSetQuery(simpleDbCommand);
                return resultSet.Result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Resultset of the Command definition.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns result set in a dataset instance.</returns>
        public static IDbCommandResult<DataSet> GetResultSetQuery(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand)
        {
            try
            {
                DbCommandResult<DataSet> result = new DbCommandResult<DataSet>();

                DbDataAdapter dataAdapter = database.CreateDataAdapter();
                if (dataAdapter is null)
                    throw new Exception(DbAppMessages.DataAdapterNotFound);

                using (IDbCommand command = database.CreateCommand(simpleDbCommand, false))
                {
                    dataAdapter.SelectCommand = (DbCommand)command;
                    DataSet dataSet = new DataSet();
                    result.ExecutionResult = dataAdapter.Fill(dataSet);
                    result.Result = dataSet;
                    result.OutputParameters = command.GetOutParameters();
                }

                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// GetDynamicResultSetSkipAndTake Gets query resultset as dynamic object list with skip and take.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional. if it is null then paging will be disabled.</param>
        /// <returns>Returns dynamic object list.</returns>
        public static IDbCommandResult<DataTable> GetDataSetResult(this ISimpleDatabase database,
             SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null)
        {
            try
            {
                IDbCommandResult<DataTable> dataTableResult = new DbCommandResult<DataTable>();

                bool isPageableAndSkipAndTakeFormatEmpty = false;
                if (pageInfo != null)
                {
                    if (!pageInfo.IsPageable)
                        return dataTableResult;

                    string skipAndTakeFormat = database.QuerySetting.SkipAndTakeFormat;
                    isPageableAndSkipAndTakeFormatEmpty = skipAndTakeFormat.IsNullOrSpace();
                    if (!isPageableAndSkipAndTakeFormatEmpty)
                    {
                        string format = skipAndTakeFormat.CopyValue();
                        format = format.Replace(InternalAppValues.SkipFormat, pageInfo.Skip.ToString());
                        format = format.Replace(InternalAppValues.TakeFormat, pageInfo.Take.ToString());
                        format = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                        simpleDbCommand.CommandText = format.CopyValue();
                    }
                }

                IDbCommandResult<DataSet> tempResultSet = database.GetResultSetQuery(simpleDbCommand);

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
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }
    }
}