using Simply.Data;
using Simply.Data.Database;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace Simply.Ef
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Simply.Data.Database.SimpleDatabase" />
    public class SimpleEfDatabase : SimpleDatabase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfDatabase"/> class.
        /// </summary>
        public SimpleEfDatabase()
        {
            throw new NotImplementedException("Empty contructor can not be implemented.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfDatabase"/> class.
        /// </summary>
        /// <param name="dbContext">DbContext instance.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleEfDatabase(DbContext dbContext,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = dbContext.Database.Connection;
            ConnectionType = connection.GetDbConnectionType();
            CommandSetting = commandSetting;
            QuerySetting = querySetting ?? connection.GetQuerySetting();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfDatabase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleEfDatabase(IDbConnection connection, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
            : base(connection, transaction, commandSetting, querySetting)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEfDatabase"/> class.
        /// </summary>
        /// <param name="providerFactory">The provider factory.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleEfDatabase(DbProviderFactory providerFactory, string connectionString,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
            : base(providerFactory, connectionString, commandSetting, querySetting)
        { }
    }
}