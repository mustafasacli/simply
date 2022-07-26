using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
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
        /// counts rows for given sql query and command definition.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="commandDefinition">Command Definition <see cref="DbCommandDefinition"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null)
        {
            commandDefinition.CommandText = string.Format(InternalAppValues.CountFormat, commandDefinition.CommandText);
            IDbCommandResult<int> result = connection.ExecuteScalarQueryAs<int>(commandDefinition, transaction);
            return result.Result;
        }

        /// <summary>
        /// counts rows as long for given sql query and command definition.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/></param>
        /// <param name="commandDefinition">Command Definition <see cref="DbCommandDefinition"/></param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/></param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        public static long CountLong(this IDbConnection connection,
            DbCommandDefinition commandDefinition, IDbTransaction transaction = null)
        {
            commandDefinition.CommandText = string.Format(InternalAppValues.CountFormat, commandDefinition.CommandText);
            IDbCommandResult<long> result = connection.ExecuteScalarQueryAs<long>(commandDefinition, transaction);
            return result.Result;
        }

        /// <summary>
        /// counts rows for given sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="sql">Sql query <see cref="string"/>.</param>
        /// <param name="obj">object which has parameters as property <see cref="object"/>.</param>
        /// <param name="commandType">Command Type <see cref="CommandType"/>.</param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/>.</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            string sqlText = string.Format(InternalAppValues.CountFormat, sql);
            int result = connection.ExecuteScalarAs<int>(sqlText, obj, commandType, transaction);
            return result;
        }

        /// <summary>
        /// counts rows as long for given sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="sql">Sql query <see cref="string"/>.</param>
        /// <param name="obj">object which has parameters as property <see cref="object"/>.</param>
        /// <param name="commandType">Command Type <see cref="CommandType"/>.</param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/>.</param>
        /// <returns>Returns row count as long value <see cref="long"/>.</returns>
        public static long CountLong(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            string sqlText = string.Format(InternalAppValues.CountFormat, sql);
            long result = connection.ExecuteScalarAs<long>(sqlText, obj, commandType, transaction);
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
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns row count as int value <see cref="int"/>.</returns>
        public static int Count(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray();

            DbCommandDefinition commandDefinition =
                connection.BuildCommandDefinitionForTranslate(odbcSqlQuery, commandParameters, commandType, commandTimeout);
            commandDefinition.CommandText = string.Format(InternalAppValues.CountFormat, commandDefinition.CommandText);
            IDbCommandResult<int> result = connection.ExecuteScalarQueryAs<int>(commandDefinition, transaction);
            return result.Result;
        }

        /// <summary>
        /// counts rows as long value for given odbc sql query and parameters.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="odbcSqlQuery">
        /// The ODBC SQL query.Like SELECT * FROM TABLE_NAME WHERE COLUMN2 &gt; ? AND COLUMN3 = TRUNC(?)
        /// </param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="transaction">Database transaction(optional).</param>
        /// <param name="commandTimeout">command timeout(optional).</param>
        /// <returns>Returns count value as long.</returns>
        public static long CountLong(this IDbConnection connection,
           string odbcSqlQuery, object[] parameterValues,
           CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray();

            DbCommandDefinition commandDefinition =
                connection.BuildCommandDefinitionForTranslate(odbcSqlQuery, commandParameters, commandType, commandTimeout);
            commandDefinition.CommandText = string.Format(InternalAppValues.CountFormat, commandDefinition.CommandText);
            IDbCommandResult<long> result = connection.ExecuteScalarQueryAs<long>(commandDefinition, transaction);
            return result.Result;
        }
    }
}