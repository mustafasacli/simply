using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DatabaseExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;
using System.Linq;

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
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

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
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/></param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        public static long CountLong(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

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
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sql">Sql query <see cref="string"/>.</param>
        /// <param name="obj">object which has parameters as property <see cref="object"/>.</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this ISimpleDatabase database, string sql, object obj)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            string sqlText = format.Replace(InternalAppValues.SqlScriptFormat, sql);

            int result = connection.ExecuteScalarAs<int>(sqlText,
                obj, transaction, database.CommandSetting);
            return result;
        }

        /// <summary>
        /// counts rows as long for given sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sql">Sql query <see cref="string"/>.</param>
        /// <param name="obj">object which has parameters as property <see cref="object"/>.</param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        public static long CountLong(this ISimpleDatabase database, string sql, object obj)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            string sqlText = format.Replace(InternalAppValues.SqlScriptFormat, sql);

            long result = connection.ExecuteScalarAs<long>(sqlText, obj,
                transaction, database.CommandSetting);
            return result;
        }

        /// <summary>
        /// counts rows for given odbc sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this ISimpleDatabase database, string odbcSqlQuery, object[] parameterValues)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, database.CommandSetting);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            simpleDbCommand.CommandText = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);

            IDbCommandResult<int> commandResult = connection.ExecuteScalarQueryAs<int>(simpleDbCommand, transaction);
            return commandResult.Result;
        }

        /// <summary>
        /// counts rows as long value for given odbc sql query and parameters.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <returns>Returns count value as long.</returns>
        public static long CountLong(this ISimpleDatabase database, string odbcSqlQuery, object[] parameterValues)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray();

            SimpleDbCommand simpleDbCommand =
                connection.BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, database.CommandSetting);
            IQuerySetting querySetting = connection.GetQuerySetting();
            string format = querySetting.CountFormat;
            simpleDbCommand.CommandText = format.Replace(InternalAppValues.SqlScriptFormat, simpleDbCommand.CommandText);

            IDbCommandResult<long> commandResult = connection.ExecuteScalarQueryAs<long>(simpleDbCommand, transaction);
            return commandResult.Result;
        }
    }
}