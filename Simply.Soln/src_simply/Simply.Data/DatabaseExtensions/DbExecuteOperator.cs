using Simply.Data.DatabaseExtensions;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Helpers;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
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
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns exection result as int.</returns>
        public static int Execute(this ISimpleDatabase database, string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            IDbCommandResult<int> commandResult = database.ExecuteQuery(simpleDbCommand);
            return commandResult.Result;
        }

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        /// <returns>Returns execution result as int. <see cref="IDbCommandResult{System.Int32}"/></returns>
        public static IDbCommandResult<int> ExecuteQuery(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            InternalLogHelper.LogCommand(simpleDbCommand, database.LogSetting);
            IDbCommandResult<int> commandResult = new DbCommandResult<int>(-1);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                InternalLogHelper.LogDbCommand(command, database.LogSetting);
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
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>Returns execution result as int.</returns>
        public static int ExecuteAsOdbc(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            IDbCommandResult<int> commandResult = database.ExecuteQuery(simpleDbCommand);
            return commandResult.Result;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        public static async Task<int> ExecuteAsync(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.Execute(sqlQuery, parameterObject, commandType);
            });
        }

        #endregion [ Task methods ]
    }
}