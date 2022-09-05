using Simply.Data.Objects;
using System;
using System.Data;

namespace Simply.Data.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ISimpleDatabase : IDisposable
    {
        /// <summary>
        /// Gets, sets command setting.
        /// </summary>
        ICommandSetting CommandSetting { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<SimpleDbCommand> CommandLogAction { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<IDbCommand> DbCommandLogAction { get; set; }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        void BeginTransaction(IsolationLevel? isolationLevel = null);

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        void RollbackTransaction();
    }
}