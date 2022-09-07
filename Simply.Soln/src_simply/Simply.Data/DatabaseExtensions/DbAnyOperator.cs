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
        /// <param name="sqlText">The sqlText <see cref="string"/>.</param>
        /// <param name="obj">The obj <see cref="object"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this ISimpleDatabase database, string sqlText, object obj)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            bool any = connection.Any(sqlText, obj,
            transaction, database.CommandSetting, database.LogSetting);

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
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this ISimpleDatabase database, string odbcSqlQuery, object[] parameterValues)
        {
            IDbConnection connection = database.GetDbConnection();
            IDbTransaction transaction = database.GetDbTransaction();

            if (transaction == null)
                connection.OpenIfNot();

            bool any = connection.Any(odbcSqlQuery, parameterValues, transaction,
                database.CommandSetting, database.LogSetting);
            return any;
        }
    }
}