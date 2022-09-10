using Simply.Common.Objects;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="SimpleDbRowListOperator"/>.
    /// </summary>
    public static class SimpleDbRowListOperator
    {
        /// <summary>
        /// GetDbRowListQuery Gets query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowListWithResult(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = new DbCommandResult<List<SimpleDbRow>>();

            if (!(pageInfo?.IsPageable ?? true))
                return simpleDbRowListResult;

            SimpleDbCommand dbCommand = database.ApplyPageInfo(simpleDbCommand, pageInfo);

            using (IDbCommand command = database.CreateCommand(dbCommand))
            using (IDataReader dataReader = command.ExecuteDataReader(behavior))
            {
                try
                {
                    simpleDbRowListResult.OutputParameters = command.GetOutParameters();
                    simpleDbRowListResult.ExecutionResult = dataReader.RecordsAffected;
                    simpleDbRowListResult.Result = dataReader.GetResultSetAsDbRow(closeAtFinal: true);
                }
                finally
                { dataReader?.CloseIfNot(); }
            }

            return simpleDbRowListResult;
        }

        /// <summary>
        /// GetDbRowListQuery Gets query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="pageInfo">page info for skip and take counts. it is optional.
        /// if it is null then paging will be disabled.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> GetDbRowList(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            List<SimpleDbRow> simpleDbRowList = new List<SimpleDbRow>();

            if (!(pageInfo?.IsPageable ?? true))
                return simpleDbRowList;

            SimpleDbCommand dbCommand = database.ApplyPageInfo(simpleDbCommand, pageInfo);

            using (IDbCommand command = database.CreateCommand(dbCommand))
            using (IDataReader dataReader = command.ExecuteDataReader(behavior))
            {
                try
                {
                    simpleDbRowList = dataReader.GetResultSetAsDbRow(closeAtFinal: true);
                }
                finally
                { dataReader?.CloseIfNot(); }
            }

            return simpleDbRowList;
        }

        /// <summary>
        /// GetMultiDbRowListQuery Gets query multi result set as multi SimpleDbRow list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> GetMultiDbRowListWithResult(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, CommandBehavior? behavior = null)
        {
            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowListResult = new DbCommandResult<List<List<SimpleDbRow>>>();

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            using (IDataReader dataReader = command.ExecuteDataReader(behavior))
            {
                try
                {
                    multiSimpleDbRowListResult.OutputParameters = command.GetOutParameters();
                    multiSimpleDbRowListResult.ExecutionResult = dataReader.RecordsAffected;
                    multiSimpleDbRowListResult.Result = dataReader.GetMultiDbRowList(closeAtFinal: true);
                }
                finally
                { dataReader?.CloseIfNot(); }
            }

            return multiSimpleDbRowListResult;
        }

        /// <summary>
        /// Gets query multi result set as multi SimpleDbRow list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static List<List<SimpleDbRow>> GetMultiDbRowList(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, CommandBehavior? behavior = null)
        {
            List<List<SimpleDbRow>> multiSimpleDbRowList = new List<List<SimpleDbRow>>();

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            using (IDataReader dataReader = command.ExecuteDataReader(behavior))
            {
                try
                {
                    multiSimpleDbRowList = dataReader.GetMultiDbRowList(closeAtFinal: true);
                }
                finally
                { dataReader?.CloseIfNot(); }
            }

            return multiSimpleDbRowList;
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
        public static List<SimpleDbRow> QueryDbRowList(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            List<SimpleDbRow> simpleDbRowList = database.GetDbRowList(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowList;
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
        public static List<SimpleDbRow> GetDbRowList(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           CommandType? commandType = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            List<SimpleDbRow> simpleDbRowList = database.GetDbRowList(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowList;
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
        public static IDbCommandResult<List<SimpleDbRow>> QueryDbRowListWithResult(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = database.GetDbRowListWithResult(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowListResult;
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
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowListWithResult(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           CommandType? commandType = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = database.GetDbRowListWithResult(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowListResult;
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
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> QueryMultiDbRowList(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            List<List<SimpleDbRow>> multiSimpleDbRowList = database.GetMultiDbRowList(simpleDbCommand, behavior);
            return multiSimpleDbRowList;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> GetMultiDbRowList(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           CommandType? commandType = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            List<List<SimpleDbRow>> multiSimpleDbRowList = database.GetMultiDbRowList(simpleDbCommand, behavior);
            return multiSimpleDbRowList;
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
        /// parameterNamePrefix will be set in ICommandSetting property of database instance.
        /// </param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> QueryMultiDbRowListWithResult(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowListResult = database.GetMultiDbRowListWithResult(simpleDbCommand, behavior);
            return multiSimpleDbRowListResult;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> GetMultiDbRowListWithResult(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           CommandType? commandType = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowList = database.GetMultiDbRowListWithResult(simpleDbCommand, behavior);
            return multiSimpleDbRowList;
        }
    }
}