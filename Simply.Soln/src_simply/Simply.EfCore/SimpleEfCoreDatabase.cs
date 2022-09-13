using Microsoft.EntityFrameworkCore;
using Simply.Data;
using Simply.Data.Database;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;

namespace Simply.EfCore
{
    /// <summary>
    /// Entity Framework içinde kullanılacak sınıftır.
    /// </summary>
    /// <seealso cref="Simply.Data.Database.SimpleDatabase" />
    public class SimpleEfCoreDatabase : SimpleDatabase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfCoreDatabase"/> class.
        /// </summary>
        public SimpleEfCoreDatabase()
        {
            throw new NotImplementedException("Empty contructor can not be implemented.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfCoreDatabase"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext instance.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleEfCoreDatabase(DbContext dbContext,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = dbContext.Database.GetDbConnection();
            ConnectionType = connection.GetDbConnectionType();
            CommandSetting = commandSetting;
            QuerySetting = querySetting ?? connection.GetQuerySetting();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfCoreDatabase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleEfCoreDatabase(IDbConnection connection, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
            : base(connection, transaction, commandSetting, querySetting)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = connection;
            this.transaction = transaction;
            CommandSetting = commandSetting;
            ConnectionType = connection.GetDbConnectionType();
            QuerySetting = querySetting ?? connection.GetQuerySetting();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfCoreDatabase"/> class.
        /// </summary>
        /// <param name="providerFactory">The provider factory.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleEfCoreDatabase(DbProviderFactory providerFactory, string connectionString,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
            : base(providerFactory, connectionString, commandSetting, querySetting)
        { }
    }
}