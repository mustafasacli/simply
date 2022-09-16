﻿using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbListOperator"/>.
    /// </summary>
    public static class DbListOperator
    {
        /// <summary>
        /// GetDbRowListQuery Gets query result set as object instance list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<T> List<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            List<SimpleDbRow> simpleDbRowListResult =
            database.ListRow(simpleDbCommand, pageInfo, behavior);
            List<T> instanceList = simpleDbRowListResult.ConvertRowsToList<T>();
            return instanceList;
        }

        /// <summary>
        /// Gets query result set as object instance list.
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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<T> List<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            List<SimpleDbRow> simpleDbRowListResult =
            database.ListRow(sqlQuery, parameterObject, commandSetting, pageInfo, behavior);
            List<T> instanceList = simpleDbRowListResult.ConvertRowsToList<T>();
            return instanceList;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<T> ListOdbc<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            List<SimpleDbRow> simpleDbRowListResult =
            database.ListRowOdbc(odbcSqlQuery, parameterValues, commandSetting, pageInfo, behavior);
            List<T> instanceList = simpleDbRowListResult.ConvertRowsToList<T>();
            return instanceList;
        }

        /// <summary>
        /// GetDbRowListQuery Gets query result set as object instance list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<T>> ListResult<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
            database.ListRowResult(simpleDbCommand, pageInfo, behavior);
            IDbCommandResult<List<T>> commandResult = new DbCommandResult<List<T>>();

            commandResult.OutputParameters = simpleDbRowListResult.OutputParameters;
            commandResult.AdditionalValues = simpleDbRowListResult.AdditionalValues;
            commandResult.ExecutionResult = simpleDbRowListResult.ExecutionResult;
            commandResult.Result = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return commandResult;
        }

        /// <summary>
        /// Gets query multi result set as multi simpledbrow list.
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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<T>> ListResult<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
            database.MultiListRowResult(sqlQuery, parameterObject, commandSetting, pageInfo, behavior);
            IDbCommandResult<List<T>> commandResult = new DbCommandResult<List<T>>();

            commandResult.OutputParameters = simpleDbRowListResult.OutputParameters;
            commandResult.AdditionalValues = simpleDbRowListResult.AdditionalValues;
            commandResult.ExecutionResult = simpleDbRowListResult.ExecutionResult;
            commandResult.Result = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return commandResult;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<T>> ListOdbcResult<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
            database.MultiListRowOdbcResult(odbcSqlQuery, parameterValues, commandSetting, pageInfo, behavior);
            IDbCommandResult<List<T>> commandResult = new DbCommandResult<List<T>>();

            commandResult.OutputParameters = simpleDbRowListResult.OutputParameters;
            commandResult.AdditionalValues = simpleDbRowListResult.AdditionalValues;
            commandResult.ExecutionResult = simpleDbRowListResult.ExecutionResult;
            commandResult.Result = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return commandResult;
        }
    }
}