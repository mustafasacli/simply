using Simply.Common;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
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
        /// <param name="transaction">The transaction <see cref="IDbTransaction"/>.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>The <see cref="bool"/>.</returns>
        // [Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static bool Any(this IDbConnection connection, string sqlText, object obj,
            IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] parameters = connection.TranslateParametersFromObject(obj);
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand
            {
                CommandText = sqlText,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout,
                ParameterNamePrefix = commandSetting?.ParameterNamePrefix
            };

            simpleDbCommand.RecompileQuery(connection.GetQuerySetting(), obj);
            simpleDbCommand.AddCommandParameters(parameters);

            bool any = connection.Any(simpleDbCommand, transaction);
            return any;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="connection">The connection <see cref="IDbConnection"/>.</param>
        /// <param name="simpleDbCommand">The simpleDbCommand <see cref="SimpleDbCommand"/>.</param>
        /// <param name="transaction">The transaction <see cref="IDbTransaction"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        // [Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static bool Any(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand, IDbTransaction transaction = null)
        {
            DbCommandParameter[] outputValues;

            IDataReader reader = connection.ExecuteReaderQuery(simpleDbCommand, out outputValues,
                transaction, commandBehavior: CommandBehavior.SingleRow);

            bool any = reader.Any(closeAtFinal: true);
            return any;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <param name="connection">The connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The odbcSqlQuery <see cref="string"/>.</param>
        /// <param name="parameterValues">The parameterValues <see cref="object[]"/>.</param>
        /// <param name="transaction">The transaction <see cref="IDbTransaction"/>.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <returns>The <see cref="bool"/>.</returns>
        // [Obsolete("Method is depreated. it will be removed later versions. Please, use ISimpleDatabase extension methods. You can check the github.com/mustafasacli/simply repo.")]
        public static bool Any(this IDbConnection connection,
            string odbcSqlQuery, object[] parameterValues,
           IDbTransaction transaction = null, ICommandSetting commandSetting = null)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
                .Select(p => new DbCommandParameter { Value = p, ParameterDbType = p.ToDbType() })
                .ToArray();
            SimpleDbCommand simpleDbCommand = connection.BuildSimpleDbCommandForTranslate(
                odbcSqlQuery, commandParameters, commandSetting);

            bool any = connection.Any(simpleDbCommand, transaction);
            return any;
        }
    }
}