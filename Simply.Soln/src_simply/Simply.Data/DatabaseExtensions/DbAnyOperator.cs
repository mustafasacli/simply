using Simply.Data.DbCommandExtensions;
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
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this ISimpleDatabase database, string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForQuery(sqlQuery, parameterObject, commandSetting);
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
            bool any;

            using (IDbCommand command = database.CreateCommand(simpleDbCommand))
            using (IDataReader dataReader = command.ExecuteDataReader())
            {
                try
                { any = dataReader.Any(closeAtFinal: true); }
                finally
                { dataReader?.CloseIfNot(); }
            }

            return any;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <param name="odbcSqlQuery">The odbcSqlQuery <see cref="string"/>.</param>
        /// <param name="parameterValues">The parameterValues <see cref="object[]"/>.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool AnyOdbc(this ISimpleDatabase database, string odbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null)
        {
            SimpleDbCommand simpleDbCommand =
                database.BuildSimpleDbCommandForOdbcQuery(odbcSqlQuery, parameterValues, commandSetting);
            bool any = database.Any(simpleDbCommand);
            return any;
        }
    }
}