﻿using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Linq;

namespace Simply.Data.ConnectionExtensions
{
    /// <summary>
    /// Defines the <see cref="CountOperator"/>.
    /// </summary>
    public static class CountOperator
    {
        /// <summary>
        /// counts rows for given database command.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="simpleDbCommand">database command<see cref="SimpleDbCommand"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static int Count(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            simpleDbCommand.CommandText =
                format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
            IDbCommandResult<int> commandResult = connection.ExecuteScalarQueryAs<int>(simpleDbCommand, transaction);
            return commandResult.Result;
        }

        /// <summary>
        /// counts rows as long for given database command.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static long CountLong(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            simpleDbCommand.CommandText =
                format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);

            IDbCommandResult<long> commandResult = connection.ExecuteScalarQueryAs<long>(simpleDbCommand, transaction);
            return commandResult.Result;
        }

        /// <summary>
        /// counts rows for given sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="sql">Sql query <see cref="string"/>.</param>
        /// <param name="obj">object which has parameters as property <see cref="object"/>.</param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/>.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static int Count(this IDbConnection connection, string sql, object obj,
            IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            string sqlText = format.Replace(InternalAppValues.SqlScriptFormat, sql);

            int result = connection.ExecuteScalarAs<int>(sqlText,
                obj, transaction, commandSetting);
            return result;
        }

        /// <summary>
        /// counts rows as long for given sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="sql">Sql query <see cref="string"/>.</param>
        /// <param name="obj">object which has parameters as property <see cref="object"/>.</param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/>.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static long CountLong(this IDbConnection connection, string sql, object obj,
            IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            string sqlText = format.Replace(InternalAppValues.SqlScriptFormat, sql);

            long result = connection.ExecuteScalarAs<long>(sqlText, obj,
                transaction, commandSetting);
            return result;
        }

        /// <summary>
        /// counts rows for given odbc sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static int Count(this IDbConnection connection, string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, commandSetting);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            simpleDbCommand.CommandText = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);

            IDbCommandResult<int> commandResult = connection.ExecuteScalarQueryAs<int>(simpleDbCommand, transaction);
            return commandResult.Result;
        }

        /// <summary>
        /// counts rows as long value for given odbc sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="transaction">Database transaction(optional).</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>Returns count value as long.</returns>
        [Obsolete("Method is deprecated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static long CountLong(this IDbConnection connection, string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, commandSetting);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            simpleDbCommand.CommandText = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);

            IDbCommandResult<long> commandResult = connection.ExecuteScalarQueryAs<long>(simpleDbCommand, transaction);
            return commandResult.Result;
        }
    }
}