﻿using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbExecuteScalarOperator"/>.
    /// </summary>
    public static class DbExecuteScalarOperator
    {
        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalar(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            try
            {
                object commandResult;

                using (IDbCommand command = database.CreateCommand(simpleDbCommand))
                {
                    commandResult = command.ExecuteScalar();
                }

                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static T ExecuteScalarAs<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : struct
        {
            try
            {
                object commandResult = database.ExecuteScalar(simpleDbCommand);
                T instance = !commandResult.IsNullOrDbNull() ? (T)commandResult : default;
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalar(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                object commandResult = database.ExecuteScalar(simpleDbCommand);
                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar as operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as T instance.</returns>
        public static T ExecuteScalarAs<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : struct
        {
            try
            {
                object commandResult = database.ExecuteScalar(sqlQuery, parameterObject, commandSetting);
                T instance = !commandResult.IsNullOrDbNull() ? (T)commandResult : default;
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalarOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                object commandResult = database.ExecuteScalar(simpleDbCommand);
                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static T ExecuteScalarOdbcAs<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            try
            {
                object commandResult = database.ExecuteScalarOdbc(odbcSqlQuery, parameterValues, commandSetting);
                T instance = !commandResult.IsNullOrDbNull() ? (T)commandResult : default;
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        #region [ Task methods ]

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object as async operation.
        /// </summary>
        /// <typeparam name="T">T struct.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static async Task<T> ExecuteScalarAsAsync<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteScalarAs<T>(sqlQuery, parameterObject, commandSetting);
            });
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar asynchronous operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>An asynchronous result that yields the execute scalar.</returns>
        public static async Task<T> ExecuteScalarAsync<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteScalarAs<T>(simpleDbCommand);
            });
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static async Task<T> ExecuteScalarOdbcAsync<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            return await Task.Factory.StartNew(() =>
            {
                return database.ExecuteScalarOdbcAs<T>(odbcSqlQuery, parameterValues, commandSetting);
            });
        }

        #endregion [ Task methods ]

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static IDbCommandResult<object> ExecuteScalarResult(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            try
            {
                IDbCommandResult<object> commandResult;

                using (IDbCommand command = database.CreateCommand(simpleDbCommand))
                {
                    commandResult = new DbCommandResult<object>();
                    commandResult.Result = command.ExecuteScalar();
                    commandResult.OutputParameters = command.GetOutParameters();
                }

                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static IDbCommandResult<T> ExecuteScalarResultAs<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : struct
        {
            try
            {
                IDbCommandResult<object> commandResult =
                    database.ExecuteScalarResult(simpleDbCommand)
                    ?? new DbCommandResult<object>();

                T instance = !commandResult.Result.IsNullOrDbNull() ? (T)commandResult.Result : default;

                return new DbCommandResult<T>()
                {
                    Result = instance,
                    AdditionalValues = commandResult.AdditionalValues,
                    ExecutionResult = commandResult.ExecutionResult
                };
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// ExecuteScalar query with parameters and returns result object.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static IDbCommandResult<object> ExecuteScalarResult(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                   database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                IDbCommandResult<object> commandResult = database.ExecuteScalarResult(simpleDbCommand);
                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// An IDbConnection extension method that executes the scalar as operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">The SQL text.</param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as T instance.</returns>
        public static IDbCommandResult<T> ExecuteScalarResultAs<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : struct
        {
            try
            {
                IDbCommandResult<object> commandResult = database.ExecuteScalarResult(sqlQuery, parameterObject, commandSetting);
                T instance = !commandResult.Result.IsNullOrDbNull() ? (T)commandResult.Result : default;
                return new DbCommandResult<T>()
                {
                    Result = instance,
                    AdditionalValues = commandResult.AdditionalValues,
                    ExecutionResult = commandResult.ExecutionResult
                };
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static IDbCommandResult<object> ExecuteScalarResultOdbc(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                IDbCommandResult<object> commandResult = database.ExecuteScalarResult(simpleDbCommand);
                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified ODBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static IDbCommandResult<T> ExecuteScalarResultOdbcAs<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            try
            {
                IDbCommandResult<object> commandResult = database.ExecuteScalarResultOdbc(odbcSqlQuery, parameterValues, commandSetting);

                T instance = !commandResult.Result.IsNullOrDbNull() ? (T)commandResult.Result : default;
                return new DbCommandResult<T>()
                {
                    Result = instance,
                    AdditionalValues = commandResult.AdditionalValues,
                    ExecutionResult = commandResult.ExecutionResult
                };
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Executes db command list and returns result as object array.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="dbCommands">Db database command list. <see cref="SimpleDbCommand" /></param>
        /// <returns>Returns object array.</returns>
        public static IEnumerable<object> ExecuteScalar(this ISimpleDatabase database, List<SimpleDbCommand> dbCommands)
        {
            bool autoClose = false;

            try
            {
                List<object> valuesList = new List<object>();
                autoClose = database.AutoClose;
                database.AutoClose = false;
                dbCommands.ForEach(command =>
                {
                    object value = database.ExecuteScalar(command);
                    valuesList.Add(value);
                });
                database.AutoClose = autoClose;
                return valuesList.AsEnumerable();
            }
            finally
            {
                if (database.AutoClose || autoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified JDBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static object ExecuteScalarJdbc(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                   database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                object commandResult = database.ExecuteScalar(simpleDbCommand);
                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified JDBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static T ExecuteScalarJdbcAs<T>(this ISimpleDatabase database,
            string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            try
            {
                object commandResult = database.ExecuteScalarJdbc(jdbcSqlQuery, parameterValues, commandSetting);
                T instance = !commandResult.IsNullOrDbNull() ? (T)commandResult : default;
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified JDBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object.</returns>
        public static IDbCommandResult<object> ExecuteScalarResultJdbc(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                IDbCommandResult<object> commandResult = database.ExecuteScalarResult(simpleDbCommand);
                return commandResult;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Execute Scalar the specified JDBC SQL query.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query.</param>
        /// <param name="parameterValues">Sql command parameters.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns execute scalar result as object instance.</returns>
        public static IDbCommandResult<T> ExecuteScalarResultJdbcAs<T>(this ISimpleDatabase database,
            string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : struct
        {
            try
            {
                IDbCommandResult<object> commandResult = database.ExecuteScalarResultJdbc(jdbcSqlQuery, parameterValues, commandSetting);

                T instance = !commandResult.Result.IsNullOrDbNull() ? (T)commandResult.Result : default;
                return new DbCommandResult<T>()
                {
                    Result = instance,
                    AdditionalValues = commandResult.AdditionalValues,
                    ExecutionResult = commandResult.ExecutionResult
                };
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }
    }
}