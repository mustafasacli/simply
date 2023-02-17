using System.Data;

namespace Simply.Data.DbTransactionExtensions
{
    /// <summary>
    /// Defines the <see cref="TransactionExtensions" />.
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// Disposes Transaction.
        /// </summary>
        /// <param name="transaction">IDbTransaction instance<see cref="IDbTransaction"/>.</param>
        public static void DisposeTransaction(this IDbTransaction transaction)
        {
            transaction?.Dispose();
        }

        /// <summary>
        /// Commit And Dispose.
        /// </summary>
        /// <param name="transaction">IDbTransaction instance<see cref="IDbTransaction"/>.</param>
        public static void CommitAndDispose(this IDbTransaction transaction)
        {
            if (transaction != null)
            {
                try
                { transaction.Commit(); }
                finally
                { DisposeTransaction(transaction); }
            }
        }

        /// <summary>
        /// Rollback And Dispose.
        /// </summary>
        /// <param name="transaction">IDbTransaction instance<see cref="IDbTransaction"/>.</param>
        public static void RollbackAndDispose(this IDbTransaction transaction)
        {
            if (transaction != null)
            {
                try 
                { transaction.Rollback(); }
                finally 
                { DisposeTransaction(transaction); }
            }
        }
    }
}