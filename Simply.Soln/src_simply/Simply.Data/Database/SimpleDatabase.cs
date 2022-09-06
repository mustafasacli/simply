using Simply.Data.DbTransactionExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;

namespace Simply.Data.Database
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Simply.Data.Interfaces.ISimpleDatabase" />
    public class SimpleDatabase : ISimpleDatabase
    {
        /// <summary>
        /// The disposed for disposing.
        /// </summary>
        protected bool disposed = false;

        /// <summary>
        /// The is transaction handled.
        /// 0; transaction state empty.
        /// 1; transaction can be committed/rollbacked.
        /// 2; transaction has been committed/rollbacked.
        /// </summary>
        protected sbyte isTransactionHandled = 0;

        /// <summary>
        /// The connection
        /// </summary>
        protected IDbConnection connection;

        /// <summary>
        /// The transaction
        /// </summary>
        protected IDbTransaction transaction;

        /// <summary>
        /// The log setting
        /// </summary>
        protected ILogSetting logSetting = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandSetting">The command setting.</param>
        public SimpleDatabase(IDbConnection connection, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = connection;
            this.transaction = transaction;
            CommandSetting = commandSetting;
        }

        public SimpleDatabase(DbProviderFactory providerFactory,
            string connectionString,
            ICommandSetting commandSetting = null)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = providerFactory.CreateConnection();
            this.connection.ConnectionString = connectionString;
            CommandSetting = commandSetting;
        }

        /// <summary>
        /// Gets, sets command setting.
        /// </summary>
        public ICommandSetting CommandSetting
        { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<SimpleDbCommand> CommandLogAction
        {
            get { return logSetting.CommandLogAction; }
            set { logSetting.SetCommandLogAction(value); }
        }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<IDbCommand> DbCommandLogAction
        {
            get { return logSetting.DbCommandLogAction; }
            set { logSetting.SetDbCommandLogAction(value); }
        }
        /// <summary>
        /// Gets the log setting.
        /// </summary>
        public ILogSetting LogSetting
        { get { return this.logSetting; } }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                if (isTransactionHandled == 1)
                {
                    transaction?.CommitAndDispose();
                    isTransactionHandled = 0;
                }
                connection?.CloseAndDispose();

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            disposed = true;
        }

        /// <summary>
        /// disposes both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public void BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            if (transaction == null)
                transaction = connection.OpenAndBeginTransaction(isolationLevel);

            isTransactionHandled = 1;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (isTransactionHandled == 1)
            {
                transaction.Commit();
                isTransactionHandled = 2;
            }
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (isTransactionHandled == 1)
            {
                transaction.Rollback();
                isTransactionHandled = 2;
            }
        }

        /// <summary>
        /// Begins the transction.
        /// </summary>
        ~SimpleDatabase()
        {
            Dispose(false);
        }
    }
}