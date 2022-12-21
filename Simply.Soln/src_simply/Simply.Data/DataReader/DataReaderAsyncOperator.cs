using Simply.Common.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DataReaderAsyncOperator" />.
    /// </summary>
    public static class DataReaderAsyncOperator
    {
        /// <summary>
        /// Gets First Row Async.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <returns>An asynchronous result that yields the first row.</returns>
        public static async Task<SimpleDbRow> FirstDbRowAsync(this IDataReader reader)
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.FirstDbRow(reader, closeAtFinal: true);
            });

            return await resultTask;
        }

        /// <summary>
        /// Gets Last Row Async.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <returns>An asynchronous result that yields the last row.</returns>
        public static async Task<SimpleDbRow> LastDbRowAsync(this IDataReader reader)
        {
            Task<SimpleDbRow> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.LastDbRow(reader, closeAtFinal: true);
            });

            return await resultTask;
        }

        /// <summary> GetDynamicResultSetAsync
        /// Gets Dynamic ResultSet Async.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>Returns SimpleDbRow object list.</returns>
        public static async Task<List<SimpleDbRow>> GetDbRowListAsync(
            this IDataReader reader, bool closeAtFinal = false)
        {
            Task<List<SimpleDbRow>> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.GetResultSetAsDbRow(reader, closeAtFinal).ToList();
            });

            return await resultTask;
        }

        /// <summary> GetMultiDynamicResultSetAsync
        /// An IDataReader extension method that gets multi SimpleDbRow result set asynchronous.
        /// </summary>
        /// <param name="reader">IDataReader object.</param>
        /// <returns>An asynchronous result that yields the multi SimpleDbRow result set.</returns>
        public static async Task<List<List<SimpleDbRow>>> GetMultiDbRowListAsync(
           this IDataReader reader)
        {
            Task<List<List<SimpleDbRow>>> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.GetMultiDbRowList(reader);
            });

            return await resultTask;
        }
    }
}