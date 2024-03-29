﻿using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="ExecuteOperator"/>.
    /// </summary>
    public static class ExecuteOperator
    {
        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns exection result as int.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static int Execute(this IDbConnection connection, string sqlQuery,
            object obj, IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
                ParameterNamePrefix = commandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(parameters);

            int result;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                result = command.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Executes query and returns result as long.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execution result as long.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static long ExecuteAsLong(this IDbConnection connection, string sqlQuery,
            object obj, IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            long value =
                Execute(connection, sqlQuery, obj, transaction, commandSetting);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as decimal.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execution result as decimal.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static decimal ExecuteAsDecimal(this IDbConnection connection,
           string sqlQuery, object obj, IDbTransaction transaction = null,
           ICommandSetting commandSetting = null)
        {
            decimal value =
                Execute(connection, sqlQuery, obj, transaction, commandSetting);

            return value;
        }

        /// <summary>
        /// Executes query and returns result as int.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="simpleDbCommand">Db database command <see cref="SimpleDbCommand"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns execution result as int. <see cref="IDbCommandResult{System.Int32}"/></returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static IDbCommandResult<int> ExecuteQuery(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IDbCommandResult<int> commandResult = new DbCommandResult<int>(-1);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
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
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns execution result as int.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static int ExecuteAsOdbc(this IDbConnection connection,
            string odbcSqlQuery, object[] parameterValues,
            IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter
                {
                    Value = p,
                    ParameterDbType = p.ToDbType()
                })
                .ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery,
                commandParameters, commandSetting);

            int executeResult;
            using (IDbCommand command =
                connection.CreateCommandWithOptions(simpleDbCommand, transaction))
            {
                executeResult = command.ExecuteNonQuery();
            }

            return executeResult;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) The transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static async Task<int> ExecuteAsync(this IDbConnection connection,
            string sqlQuery, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Execute(connection,
                    sqlQuery, obj, transaction, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as long.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static async Task<long> ExecuteAsLongAsync(this IDbConnection connection,
            string sqlQuery, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsLong(connection, sqlQuery, obj, transaction, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the asynchronous operation result
        /// returns as decimal.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>An asynchronous result that yields the execute.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static async Task<decimal> ExecuteAsDecimalAsync(this IDbConnection connection,
            string sqlQuery, object obj, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return
                ExecuteAsDecimal(connection, sqlQuery, obj, transaction, commandSetting);
            });
        }

        #endregion [ Task methods ]
    }
}