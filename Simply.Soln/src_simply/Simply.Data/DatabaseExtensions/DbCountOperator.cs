using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbCountOperator"/>.
    /// </summary>
    public static class DbCountOperator
    {
        /// <summary>
        /// counts rows for given database command.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command<see cref="SimpleDbCommand"/></param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            try
            {
                string format = database.QuerySetting.CountFormat;
                simpleDbCommand.CommandText =
                    format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                object commandResult = database.ExecuteScalar(simpleDbCommand);
                int result = commandResult.ToInt();
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows as long for given database command.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/></param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        public static long CountLong(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            try
            {
                string format = database.QuerySetting.CountFormat;
                simpleDbCommand.CommandText =
                    format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);
                object commandResult = database.ExecuteScalar(simpleDbCommand);
                long result = commandResult.ToLong();
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows for given sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query <see cref="string"/>.</param>
        /// <param name="parameterObject">object which has parameters as property <see cref="object"/>.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                int result = database.Count(simpleDbCommand);
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows as long for given sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql query <see cref="string"/>.</param>
        /// <param name="parameterObject">object which has parameters as property <see cref="object"/>.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        public static long CountLong(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
                long result = database.CountLong(simpleDbCommand);
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows for given odbc sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int CountOdbc(this ISimpleDatabase database, string odbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                int result = database.Count(simpleDbCommand);
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows as long value for given odbc sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns count value as long.</returns>
        public static long CountLongOdbc(this ISimpleDatabase database, string odbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
                long result = database.CountLong(simpleDbCommand);
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows for given jdbc sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">
        /// The JDBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ?1 AND COLUMN3 = TRUNC(?2)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int CountJdbc(this ISimpleDatabase database, string jdbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                int result = database.Count(simpleDbCommand);
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }

        /// <summary>
        /// counts rows as long value for given jdbc sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="jdbcSqlQuery">
        /// The JDBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ?1 AND COLUMN3 = TRUNC(?2)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns count value as long.</returns>
        public static long CountLongJdbc(this ISimpleDatabase database, string jdbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null)
        {
            try
            {
                SimpleDbCommand simpleDbCommand =
                    database.BuildSimpleDbCommandForJdbcQuery(jdbcSqlQuery, parameterValues, commandSetting);
                long result = database.CountLong(simpleDbCommand);
                return result;
            }
            finally
            {
                if (database.AutoClose)
                    database.Close();
            }
        }
    }
}