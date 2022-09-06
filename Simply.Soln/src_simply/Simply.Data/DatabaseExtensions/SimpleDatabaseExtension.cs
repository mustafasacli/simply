using Simply.Data.Constants;
using Simply.Data.Interfaces;
using System;
using System.Data;

namespace Simply.Data.DatabaseExtensions
{
    /// <summary>
    /// Defines the <see cref="SimpleDatabaseExtension"/>.
    /// </summary>
    public static class SimpleDatabaseExtension
    {
        /// <summary>
        /// Gets the db connection.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <returns>A IDbConnection.</returns>
        public static IDbConnection GetDbConnection<TSimpleDatabase>(this TSimpleDatabase database)
            where TSimpleDatabase : class, ISimpleDatabase
        {
            var field = database.GetType().GetField(InternalAppValues.ConnectionParameterName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            IDbConnection connection = field.GetValue(database) as IDbConnection;

            if (connection == null)
                throw new Exception(DbAppMessages.DbConnectionNotDefined);

            return connection;
        }

        /// <summary>
        /// Gets the db transaction.
        /// </summary>
        /// <param name="database">The simple database object instance.</param>
        /// <returns>A IDbTransaction.</returns>
        public static IDbTransaction GetDbTransaction<TSimpleDatabase>(this TSimpleDatabase database)
            where TSimpleDatabase : class, ISimpleDatabase
        {
            var field = database.GetType().GetField(InternalAppValues.TransactionParameterName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            IDbTransaction transaction = field.GetValue(database) as IDbTransaction;

            //if (transaction == null)
            //    throw new Exception(DbAppMessages.DbTransactionNotDefined);

            return transaction;
        }
    }
}