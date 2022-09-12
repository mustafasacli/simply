using Simply.Data.Constants;
using Simply.Data.Interfaces;
using System;
using System.Data;
using System.Reflection;

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
            FieldInfo fieldInfo = database.GetType()
                .GetField(InternalAppValues.ConnectionParameterName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            IDbConnection connection = fieldInfo.GetValue(database) as IDbConnection;

            if (connection is null)
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
            FieldInfo fieldInfo = database.GetType()
                .GetField(InternalAppValues.TransactionParameterName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            IDbTransaction transaction = fieldInfo.GetValue(database) as IDbTransaction;

            return transaction;
        }
    }
}