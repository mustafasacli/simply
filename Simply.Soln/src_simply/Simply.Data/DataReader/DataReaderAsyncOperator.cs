using System.Collections.Generic;
using System.Data;
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
        /// <param name="reader">   The dataReader to act on.</param>
        /// <returns>   An asynchronous result that yields the first row.</returns>
        public static async Task<dynamic> FirstDynamicRowAsync(this IDataReader reader)
        {
            Task<dynamic> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.FirstDynamicRow(reader, closeAtFinal: true);
            });

            return await resultTask;
        }

        /// <summary>
        /// Gets Last Row Async.
        /// </summary>
        /// <param name="reader">   The dataReader to act on.</param>
        /// <returns>   An asynchronous result that yields the last row.</returns>
        public static async Task<dynamic> LastDynamicRowAsync(this IDataReader reader)
        {
            Task<dynamic> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.LastDynamicRow(reader, closeAtFinal: true);
            });

            return await resultTask;
        }

        /// <summary> GetDynamicResultSetAsync
        /// Gets Dynamic ResultSet Async.
        /// </summary>
        /// <param name="reader">       .</param>
        /// <param name="closeAtFinal"> (Optional) .</param>
        /// <returns>Returns dynamic object list.</returns>
        public static async Task<List<dynamic>> GetDynamicListAsync(
            this IDataReader reader, bool closeAtFinal = false)
        {
            Task<List<dynamic>> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.GetResultSetAsDynamic(reader, closeAtFinal);
            });

            return await resultTask;
        }

        /// <summary> GetMultiDynamicResultSetAsync
        /// An IDataReader extension method that gets multi dynamic result set asynchronous.
        /// </summary>
        /// <param name="reader">IDataReader object.</param>
        /// <returns>   An asynchronous result that yields the multi dynamic result set.</returns>
        public static async Task<List<List<dynamic>>> GetMultiDynamicListAsync(
           this IDataReader reader)
        {
            Task<List<List<dynamic>>> resultTask = Task.Factory.StartNew(() =>
            {
                return DataReaderOperator.GetMultiDynamicList(reader);
            });

            return await resultTask;
        }
    }
}