﻿using Simply.Common.Objects;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbSingleOperator"/>.
    /// </summary>
    public static class DbSingleOperator
    {
        /// <summary>
        /// Get Single Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command.</param>
        /// <returns>Returns single record as dynamic object instance.</returns>
        public static T Single<T>(
            this ISimpleDatabase database, SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            try
            {
                SimpleDbRow simpleRow = database.SingleRow(simpleDbCommand);
                T instance = simpleRow.ConvertRowTo<T>();
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Single Row of the Resultset as object instance.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
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
        /// <returns>Returns single record as object instance.</returns>
        public static T Single<T>(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null) where T : class, new()
        {
            try
            {
                SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                T instance = database.Single<T>(simpleDbCommand);
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Single Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static T SingleOdbc<T>(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            try
            {
                SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                T instance = database.Single<T>(simpleDbCommand);
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Single Row of the Jdbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ?1 ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static T SingleJdbc<T>(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            try
            {
                SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                T instance = database.Single<T>(simpleDbCommand);
                return instance;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        #region [ SimpleDbRow methods ]

        /// <summary>
        /// Get Single Row of the Resultset as simple db row instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>Returns single record as simple db row instance.</returns>
        public static SimpleDbRow SingleRow(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand)
        {
            SimpleDbRow simpleRow;

            try
            {
                using (IDbCommand command = database.CreateCommand(simpleDbCommand))
                using (IDataReader dataReader = command.ExecuteDataReader())
                {
                    try
                    {
                        simpleRow = dataReader.SingleDbRow();
                    }
                    finally
                    { dataReader?.CloseIfNot(); }
                }
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }

            return simpleRow;
        }

        /// <summary>
        /// Get Single Row of the Resultset as dynamic object instance.
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
        /// <returns>Returns single record as dynamic object.</returns>
        public static SimpleDbRow SingleRow(this ISimpleDatabase database,
            string sqlQuery, object parameterObject, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                SimpleDbRow simpleDbRow = database.SingleRow(simpleDbCommand);
                return simpleDbRow;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Single Row of the Odbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static SimpleDbRow SingleOdbcRow(this ISimpleDatabase database,
           string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                SimpleDbRow simpleRow = database.SingleRow(simpleDbCommand);
                return simpleRow;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// Get Single Row of the Jdbc Sql Query Resultset as object instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ?1 ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns single record as object instance.</returns>
        public static SimpleDbRow SingleJdbcRow(this ISimpleDatabase database,
           string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                SimpleDbRow simpleRow = database.SingleRow(simpleDbCommand);
                return simpleRow;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        #endregion [ SimpleDbRow methods ]

        #region [ Task methods ]

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>An asynchronous result that yields the single as object instance.</returns>
        public static async Task<T> SingleAsync<T>(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.Single<T>(simpleDbCommand);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as object instance with async operation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query.
        /// Select * From TableName Where Column1 = ?p1?
        /// parameterNamePrefix : ?
        /// Query For Oracle ==> Select * From TableName Where Column1 = :p1
        /// Query For Sql Server ==> Select * From TableName Where Column1 = @p1
        /// parameterNamePrefix will be set in ICommandSetting instance.
        /// </param>
        /// <param name="parameterObject">object contains db parameters as property.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>An asynchronous result that yields a T.</returns>
        public static async Task<T> SingleAsync<T>(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.Single<T>(sqlQuery, parameterObject, commandSetting);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The ODBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ? ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>s
        /// <returns>An asynchronous result that yields the single as object instance.</returns>
        public static async Task<T> SingleOdbcAsync<T>(this ISimpleDatabase database,
            string odbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.SingleOdbc<T>(odbcSqlQuery, parameterValues, commandSetting);
            });

            return await resultTask;
        }

        /// <summary>
        /// Get Single Row of the Resultset as simple db row object instance with async operation.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">The JDBC SQL query ( Example: SELECT * FROM TABLE_NAME WHERE ID_COLUMN = ?1 ).</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>s
        /// <returns>An asynchronous result that yields the single as object instance.</returns>
        public static async Task<T> SingleJdbcAsync<T>(this ISimpleDatabase database,
            string jdbcSqlQuery, object[] parameterValues, ICommandSetting commandSetting = null) where T : class, new()
        {
            Task<T> resultTask = Task.Factory.StartNew(() =>
            {
                return database.SingleJdbc<T>(jdbcSqlQuery, parameterValues, commandSetting);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]

        /// <summary>
        /// Get Single Row of the Resultset as simple db row instance.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <returns>Returns single record as simple db row instance.</returns>
        public static IDbCommandResult<SimpleDbRow> SingleRowResult(this ISimpleDatabase database,
            SimpleDbCommand simpleDbCommand)
        {
            IDbCommandResult<SimpleDbRow> simpleDbRowResult = new DbCommandResult<SimpleDbRow>();

            try
            {
                using (IDbCommand command = database.CreateCommand(simpleDbCommand))
                using (IDataReader dataReader = command.ExecuteDataReader())
                {
                    try
                    {
                        simpleDbRowResult.OutputParameters = command.GetOutParameters();
                        simpleDbRowResult.ExecutionResult = dataReader.RecordsAffected;
                        simpleDbRowResult.Result = dataReader.SingleDbRow();
                    }
                    finally
                    { dataReader?.CloseIfNot(); }
                }
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }

            return simpleDbRowResult;
        }
    }
}