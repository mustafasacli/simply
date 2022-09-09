using Simply.Common.Objects;
using Simply.Data.DatabaseExtensions;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Helpers;
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
        /// QueryMultiDbRowList Gets query multi result set as multi expando object list.
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
        /// <returns>Returns list of SimpleDbRow object list.</returns>
        public static List<List<SimpleDbRow>> QueryMultiDbRowList(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowListResult =
                connection.GetMultiDbRowListQuery(simpleDbCommand, transaction, logSetting: database.LogSetting);
            return multiSimpleDbRowListResult.Result;
        }

        /// <summary>
        /// GetDbRowListQuery Gets query result set as SimpleDbRow object list.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static IDbCommandResult<List<SimpleDbRow>> GetDbRowListQuery(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, CommandBehavior? behavior = null)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<List<SimpleDbRow>> simpleDbRowListResult;

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);

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
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <param name="behavior">The behavior <see cref="System.Nullable{CommandBehavior}"/>.</param>
        /// <returns>Returns multi SimpleDbRow list.</returns>
        public static IDbCommandResult<List<List<SimpleDbRow>>> GetMultiDbRowListQuery(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand, CommandBehavior? behavior = null)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IDbCommandResult<List<List<SimpleDbRow>>> multiSimpleDbRowListResult;

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);

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
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static List<SimpleDbRow> GetListAsDbRow(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<List<SimpleDbRow>> commandResult = database.GetDbRowListQuery(simpleDbCommand);
            return commandResult.Result;
        }
    }
}