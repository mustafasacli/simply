using Simply.Common.Objects;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
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
        public static List<SimpleDbRow> ListRow(this ISimpleDatabase database,
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
        public static List<SimpleDbRow> ListRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            List<SimpleDbRow> simpleDbRowList = database.ListRow(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowList;
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
        public static List<SimpleDbRow> ListRowOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            List<SimpleDbRow> simpleDbRowList = database.ListRow(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowList;
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
        public static IDbCommandResult<List<SimpleDbRow>> ListRowResult(this ISimpleDatabase database,
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

        /*******************************************************************/

        /// <summary>
        /// Gets query multi result set as multi SimpleDbRow list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static List<List<SimpleDbRow>> MultiListRow(this ISimpleDatabase database,
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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> MultiListRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            List<List<SimpleDbRow>> multiSimpleDbRowList = database.MultiListRow(simpleDbCommand, behavior);
            return multiSimpleDbRowList;
        }

        /// <summary>
        /// Gets odbc sql query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> MultiListRowOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           ICommandSetting commandSetting = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            List<List<SimpleDbRow>> multiSimpleDbRowList = database.MultiListRow(simpleDbCommand, behavior);
            return multiSimpleDbRowList;
        }

        /*******************************************************************/

        /// <summary>
        /// GetMultiDbRowListQuery Gets query multi result set as multi SimpleDbRow list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> MultiListRowResult(this ISimpleDatabase database,
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
        public static IDbCommandResult<List<SimpleDbRow>> MultiListRowResult(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null,
            IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = database.ListRowResult(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowListResult;
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
        public static IDbCommandResult<List<SimpleDbRow>> MultiListRowOdbcResult(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues,
           ICommandSetting commandSetting = null, IPageInfo pageInfo = null, CommandBehavior? behavior = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult = database.ListRowResult(simpleDbCommand, pageInfo, behavior);
            return simpleDbRowListResult;
        }
    }
}