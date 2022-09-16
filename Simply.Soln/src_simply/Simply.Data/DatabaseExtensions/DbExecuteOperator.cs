using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbExecuteOperator"/>.
    /// </summary>
    public static class DbExecuteOperator
    {
        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns exection result as int.</returns>
        public static int Execute(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            int executionResult = database.Execute(simpleDbCommand);
            return executionResult;
        }

        /// <summary>
        /// Executes db command and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        public static int Execute(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            int executionResult;

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            {
                executionResult = command.ExecuteNonQuery();
            }

            return executionResult;
        }

        /// <summary>
        /// Executes the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execution result as int.</returns>
        public static int ExecuteOdbc(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            int executionResult = database.Execute(simpleDbCommand);
            return executionResult;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteAsync(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.Execute(sqlQuery, parameterObject, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteAsync(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.Execute(simpleDbCommand);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteOdbcAsync(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteOdbc(odbcSqlQuery, parameterValues, commandSetting);
            });
        }

        #endregion [ Task methods ]

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns exection result as int.</returns>
        public static IDbCommandResult<int> ExecuteResult(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
            IDbCommandResult<int> commandResult = database.ExecuteResult(simpleDbCommand);
            return commandResult;
        }

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        public static IDbCommandResult<int> ExecuteResult(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbCommandResult<int> commandResult = new DbCommandResult<int>(-1);

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            {
                commandResult.ExecutionResult = command.ExecuteNonQuery();
                commandResult.Result = commandResult.ExecutionResult;
                commandResult.OutputParameters = command.GetOutParameters();
            }

            return commandResult;
        }

        /// <summary>
        /// Executes the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execution result as int.</returns>
        public static IDbCommandResult<int> ExecuteResultOdbc(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            IDbCommandResult<int> commandResult = database.ExecuteResult(simpleDbCommand);
            return commandResult;
        }

        /// <summary>
        /// Executes db command list and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="dbCommands">Db database command list. <see cref="SimpleDbCommand"/></param>
        /// <returns>Returns execution result as int.</returns>
        public static int Execute(this ISimpleDatabase database, List<SimpleDbCommand> dbCommands)
        {
            int executionResult = 0;

            dbCommands.ForEach(command =>
            {
                executionResult += database.Execute(command);
            });

            return executionResult;
        }
    }
}