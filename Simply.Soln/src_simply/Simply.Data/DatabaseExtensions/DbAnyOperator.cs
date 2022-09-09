using Simply.Data.DatabaseExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System.Data;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbAnyOperator"/>.
    /// </summary>
    public static class DbAnyOperator
    {
        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="sqlQuery">Sql Query<see cref="string"/>.</param>
        /// <param name="parameterObject">parameter Value object <see cref="object"/>.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this ISimpleDatabase database, string sqlQuery, object parameterObject, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandType);
            bool any = database.Any(simpleDbCommand);
            return any;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="simpleDbCommand">The simpleDbCommand <see cref="SimpleDbCommand"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this ISimpleDatabase database, SimpleDbCommand simpleDbCommand)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            bool any = connection.Any(simpleDbCommand, transaction, database.LogSetting);
            return any;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The odbcSqlQuery <see cref="string"/>.</param>
        /// <param name="parameterValues">The parameterValues <see cref="object[]"/>.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this ISimpleDatabase database, string odbcSqlQuery, object[] parameterValues, CommandType? commandType = null)
        {
            SimpleDbCommand simpleDbCommand = database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandType);
            bool any = database.Any(simpleDbCommand);
            return any;
        }
    }
}