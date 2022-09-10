using Simply.Common;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
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
        public static IDbCommandResult<List<T>> GetListWithResult<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
            database.GetDbRowListWithResult(simpleDbCommand, pageInfo, behavior);
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
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<T>> QueryListWithResult<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
            database.QueryDbRowListWithResult(sqlQuery, parameterObject, commandType, pageInfo, behavior);
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
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<T>> GetListWithResult<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           CommandType? commandType = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult =
            database.GetDbRowListWithResult(odbcSqlQuery, parameterValues, commandType, pageInfo, behavior);
            IDbCommandResult<List<T>> commandResult = new DbCommandResult<List<T>>();

            commandResult.OutputParameters = simpleDbRowListResult.OutputParameters;
            commandResult.AdditionalValues = simpleDbRowListResult.AdditionalValues;
            commandResult.ExecutionResult = simpleDbRowListResult.ExecutionResult;
            commandResult.Result = simpleDbRowListResult.Result.ConvertRowsToList<T>();
            return commandResult;
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
        public static List<T> GetList<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            List<SimpleDbRow> simpleDbRowListResult =
            database.GetDbRowList(simpleDbCommand, pageInfo, behavior);
            List<T> instanceList = simpleDbRowListResult.ConvertRowsToList<T>();
            return instanceList;
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
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<T> QueryList<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            List<SimpleDbRow> simpleDbRowListResult =
            database.QueryDbRowList(sqlQuery, parameterObject, commandType, pageInfo, behavior);
            List<T> instanceList = simpleDbRowListResult.ConvertRowsToList<T>();
            return instanceList;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<T> GetList<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           CommandType? commandType = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null) where T : class
        {
            List<SimpleDbRow> simpleDbRowListResult =
            database.GetDbRowList(odbcSqlQuery, parameterValues, commandType, pageInfo, behavior);
            List<T> instanceList = simpleDbRowListResult.ConvertRowsToList<T>();
            return instanceList;
        }
    }
}