using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Linq;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbCommandBuilder"/>.
    /// </summary>
    public static class DbCommandBuilder
    {
        /// <summary>
        /// Create IDbCommand instance with database command and db transaction for given db connection.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <param name="transaction">Database transaction <see cref="IDbTransaction"/>.</param>
        /// <param name="querySetting">Query setting object instance.</param>
        /// <returns>Returns A Database command instance. <see cref="IDbCommand"/>.</returns>
        public static IDbCommand CreateCommandWithOptions(this IDbConnection connection,
            SimpleDbCommand simpleDbCommand,
          IDbTransaction transaction = null, IQuerySetting querySetting = null)
        {
            if (simpleDbCommand is null || string.IsNullOrWhiteSpace(simpleDbCommand.CommandText))
                throw new ArgumentNullException(nameof(simpleDbCommand));

            // TODO : WILL BE CHECKED AND TESTED.
            // LIST AND BULK INSERT TEST OK.

            IDbCommand command = connection.CreateCommand()
                .SetCommandType(simpleDbCommand.CommandType)
                .SetCommandText(simpleDbCommand.CommandText)
                .SetCommandTimeout(simpleDbCommand.CommandTimeout)
                .SetTransaction(transaction)
                .IncludeCommandParameters(simpleDbCommand.CommandParameters, querySetting: querySetting);

            return command;
        }

        /// <summary>
        /// Rebuilds the query with paramaters.
        /// </summary>
        /// <param name="sqlText">The sql text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterPrefix">The parameter prefix.</param>
        /// <param name="parameterNamePrefix">The parameter name prefix.</param>
        /// <returns>A string.</returns>
        internal static string RebuildQueryWithParamaters(string sqlText,
            DbCommandParameter[] parameters, string parameterPrefix, char? parameterNamePrefix = null)
        {
            string sql = sqlText.CopyValue(true);

            if (parameters.IsNullOrEmpty())
                return sql;

            char spaceChar = InternalAppValues.OneSpace[0];
            char prefixChar = parameterNamePrefix ?? spaceChar;

            if (prefixChar != spaceChar)
            {
                parameters.ToList()
                      .ForEach(p =>
                      {
                          if (!sql.Contains(string.Format("{0}{1}", p.ParameterName.StartsWith(parameterPrefix) ? "" : parameterPrefix, p.ParameterName)))
                          {
                              sql = sql.Replace(string.Format("{0}{1}{0}", prefixChar, p.ParameterName),
                                    string.Format("{0}{1}", parameterPrefix, p.ParameterName)).CopyValue(true);
                          }
                      });
            }

            return sql;
        }
    }
}