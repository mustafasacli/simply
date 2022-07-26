using Simply.Common;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Objects;
using System.Data;
using System.Threading.Tasks;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="ExecuteReaderOperator"/>.
    /// </summary>
    public static class ExecuteReaderOperator
    {
        /// <summary>
        /// Executes query with parameters and returns DataReader object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="commandDefinition">Command Definition.</param>
        /// <param name="outputParameters"></param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">Db Command Behavior.</param>
        /// <returns>Returns an IDataReader instance.</returns>
        public static IDataReader ExecuteReaderQuery(this IDbConnection connection,
            DbCommandDefinition commandDefinition, out DbCommandParameter[] outputParameters,
            IDbTransaction transaction = null, CommandBehavior? commandBehavior = null)
        {
            IDataReader dataReader = null;
            outputParameters = ArrayHelper.Empty<DbCommandParameter>();

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                if (transaction == null)
                    connection.OpenIfNot();

                if (!commandBehavior.HasValue)
                    dataReader = command.ExecuteReader();
                else
                    dataReader = command.ExecuteReader(commandBehavior.Value);

                outputParameters = command.GetOutParameters();
            }

            return dataReader;
        }

        /// <summary>
        /// Executes query with parameters and returns DataReader object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">CommandBehaviour for DataReader.</param>
        /// <returns>Returns an IDataReader instance.</returns>
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, CommandBehavior? commandBehavior = null)
        {
            IDataReader dataReader = null;

            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            DbCommandDefinition commandDefinition = new DbCommandDefinition()
            {
                CommandText = sql,
                CommandType = commandType
            };
            commandDefinition.AddCommandParameters(parameters);

            using (IDbCommand command =
                connection.CreateCommandWithOptions(commandDefinition, transaction))
            {
                if (!commandBehavior.HasValue)
                    dataReader = command.ExecuteReader();
                else
                    dataReader = command.ExecuteReader(commandBehavior.Value);
            }

            return dataReader;
        }

        #region [ Task methods ]

        /// <summary>
        /// An IDbConnection extension method that executes the reader asynchronous operation.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="sql">Sql query.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <param name="commandType">(Optional) Command type.</param>
        /// <param name="transaction">(Optional) Database transaction.</param>
        /// <param name="commandBehavior">Db Command Behavior</param>
        /// <returns>An asynchronous result that yields the execute reader.</returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
            string sql, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null, CommandBehavior? commandBehavior = null)
        {
            Task<IDataReader> resultTask = Task.Factory.StartNew(() =>
            {
                return
                ExecuteReader(connection, sql, obj, commandType, transaction,
            commandBehavior);
            });

            return await resultTask;
        }

        #endregion [ Task methods ]
    }
}