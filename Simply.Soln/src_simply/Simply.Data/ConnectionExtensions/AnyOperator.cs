using Simply.Common;
using Simply.Data.Objects;
using System.Data;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="AnyOperator"/>.
    /// </summary>
    public static class AnyOperator
    {
        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="connection">The connection <see cref="IDbConnection"/>.</param>
        /// <param name="sqlText">The sqlText <see cref="string"/>.</param>
        /// <param name="obj">The obj <see cref="object"/>.</param>
        /// <param name="commandType">The commandType <see cref="CommandType"/>.</param>
        /// <param name="transaction">The transaction <see cref="IDbTransaction"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this IDbConnection connection,
            string sqlText, object obj, CommandType commandType = CommandType.Text,
            IDbTransaction transaction = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            SimpleDbCommand commandDefinition = new SimpleDbCommand
            {
                CommandText = sqlText,
                CommandType = commandType
            };
            commandDefinition.AddCommandParameters(parameters);

            bool result = connection.Any(commandDefinition, transaction);

            return result;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="connection">The connection <see cref="IDbConnection"/>.</param>
        /// <param name="commandDefinition">The commandDefinition <see cref="SimpleDbCommand"/>.</param>
        /// <param name="transaction">The transaction <see cref="IDbTransaction"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this IDbConnection connection,
            SimpleDbCommand commandDefinition, IDbTransaction transaction = null)
        {
            bool result;
            DbCommandParameter[] outputValues;

            IDataReader reader = connection.ExecuteReaderQuery(commandDefinition, out outputValues,
                transaction, commandBehavior: CommandBehavior.SingleRow);

            result = reader.Any(closeAtFinal: true);

            return result;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="connection">The connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The odbcSqlQuery <see cref="string"/>.</param>
        /// <param name="parameterValues">The parameterValues <see cref="object[]"/>.</param>
        /// <param name="commandType">The commandType <see cref="CommandType"/>.</param>
        /// <param name="transaction">The transaction <see cref="IDbTransaction"/>.</param>
        /// <param name="commandTimeout">The commandTimeout <see cref="System.Nullable{int}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any(this IDbConnection connection, string odbcSqlQuery,
            object[] parameterValues, CommandType commandType = CommandType.Text,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter { Value = p, ParameterDbType = p.ToDbType() })
                .ToArray();
            SimpleDbCommand commandDefinition = connection.BuildCommandDefinitionForTranslate(
                odbcSqlQuery, commandParameters, commandType, commandTimeout);
            bool result = connection.Any(commandDefinition, transaction);

            return result;
        }
    }
}