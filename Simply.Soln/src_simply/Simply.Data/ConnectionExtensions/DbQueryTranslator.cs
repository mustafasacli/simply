using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.Enums;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using Simply.Data.QuerySettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbQueryTranslator"/>.
    /// </summary>
    public static class DbQueryTranslator
    {
        /// <summary>
        /// Defines the empty.
        /// </summary>
        private static readonly string empty = string.Empty;

        /// <summary>
        /// The Translate Odbc Query to parametrized query.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <returns>
        /// The <see cref="string[]"/> Returns translated query and parameters in same array. First
        /// element of array is translated query and other elements are query parameters. Query :
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = ?"
        /// For Sql Server Result;
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = @p0", "@p0"
        /// For Oracle Result;
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = :p0", ":p0"
        /// </returns>
        public static string[] TranslateOdbcQuery(this IDbConnection connection, string odbcSqlQuery)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            string[] queryAndParameters = TranslateOdbcQuery(connection.GetDbConnectionType(), odbcSqlQuery);

            return queryAndParameters.ToArray();
        }

        /// <summary>
        /// The Translate Odbc Query to parametrized query.
        /// </summary>
        /// <param name="connectionType">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <returns>
        /// The <see cref="string[]"/> Returns translated query and parameters in same array. First
        /// element of array is translated query and other elements are query parameters. Query :
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = ?"
        /// For Sql Server Result;
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = @p0", "@p0"
        /// For Oracle Result;
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = :p0", ":p0"
        /// </returns>
        public static string[] TranslateOdbcQuery(this DbConnectionTypes connectionType, string odbcSqlQuery)
        {
            if (string.IsNullOrWhiteSpace(odbcSqlQuery))
                throw new ArgumentNullException(nameof(odbcSqlQuery));

            List<string> queryAndParameters = new List<string>();
            odbcSqlQuery = odbcSqlQuery.Trim();
            if (!odbcSqlQuery.Contains(InternalAppValues.QuestionMark) || connectionType.IsOdbcConn())
            { queryAndParameters.Add(odbcSqlQuery); return queryAndParameters.ToArray(); }

            List<string> queryParts = odbcSqlQuery
                .Split(new char[] { InternalAppValues.QuestionMark }, StringSplitOptions.None)
                .ToList() ?? ArrayHelper.EmptyList<string>();
            IQuerySetting querySetting = QuerySettingsFactory.GetQuerySetting(connectionType);

            StringBuilder sqlBuilder = new StringBuilder();
            int parameterCounter = 0;
            queryAndParameters.Add(empty);

            for (int counter = 0; counter < queryParts.Count - 1; counter++)
            {
                string parameterName = empty;
                string item = queryParts[counter];
                sqlBuilder.Append(item);

                do
                {
                    parameterName = querySetting.ParameterPrefix + InternalAppValues.ParameterChar.ToString() + parameterCounter.ToString();
                    parameterCounter++;
                } while (!(queryParts.All(q => !q.Contains(parameterName)) && queryAndParameters.IndexOf(parameterName) == -1));

                sqlBuilder.Append(parameterName);
                queryAndParameters.Add(parameterName);
            }

            sqlBuilder.Append(queryParts[queryParts.Count - 1]);
            queryAndParameters[0] = sqlBuilder.ToString();

            return queryAndParameters.ToArray();
        }

        /// <summary>
        /// Builds SimpleDbCommand instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <param name="commandParameters">The commandParameters <see cref="DbCommandParameter[]"/>.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns database command object instance <see cref="SimpleDbCommand" />.</returns>
        public static SimpleDbCommand BuildSimpleDbCommandForTranslate(
            this IDbConnection connection, string odbcSqlQuery, DbCommandParameter[] commandParameters, CommandType? commandType,
            int? commandTimeout = null, bool setOverratedParametersToOutput = false)
        {
            string[] queryAndParameters = connection.TranslateOdbcQuery(odbcSqlQuery);
            commandParameters = commandParameters ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand
            {
                CommandText = queryAndParameters[0],
                CommandType = commandType,
                CommandTimeout = commandTimeout
            };
            List<string> paramStringArray = queryAndParameters.Skip(1).ToList() ?? ArrayHelper.EmptyList<string>();

            if ((!setOverratedParametersToOutput && paramStringArray.Count != commandParameters.Length) || paramStringArray.Count < commandParameters.Length)
                throw new ArgumentException(DbAppMessages.ParameterMismatchCompiledQueryAndCommand);

            for (int counter = 0; counter < commandParameters.Length; counter++)
            {
                simpleDbCommand.AddParameter(
                    new DbCommandParameter
                    {
                        ParameterDbType = commandParameters[counter].ParameterDbType,
                        ParameterColumnName = commandParameters[counter].ParameterColumnName,
                        Direction = commandParameters[counter].Direction,
                        ParameterName = paramStringArray[counter],
                        ParameterPrecision = commandParameters[counter].ParameterPrecision,
                        ParameterScale = commandParameters[counter].ParameterScale,
                        ParameterSize = commandParameters[counter].ParameterSize,
                        Value = commandParameters[counter].Value
                    });
            }

            if (setOverratedParametersToOutput && paramStringArray.Count > commandParameters.Length)
            {
                int cnt = paramStringArray.Count - commandParameters.Length;
                for (int counter = 0; counter < cnt; counter++)
                {
                    simpleDbCommand.AddParameter(
                        new DbCommandParameter
                        {
                            Direction = ParameterDirection.Output,
                            ParameterName = paramStringArray[commandParameters.Length + counter]
                        });
                }
            }

            return simpleDbCommand;
        }

        /// <summary>
        /// Builds SimpleDbCommand instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <param name="commandParameters">The commandParameters <see cref="DbCommandParameter[]"/>.</param>
        /// <param name="commandSetting">Command setting</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns database command object instance <see cref="SimpleDbCommand" />.</returns>
        public static SimpleDbCommand BuildSimpleDbCommandForTranslate(
            this IDbConnection connection, string odbcSqlQuery, DbCommandParameter[] commandParameters,
            ICommandSetting commandSetting = null, bool setOverratedParametersToOutput = false)
        {
            string[] queryAndParameters = connection.TranslateOdbcQuery(odbcSqlQuery);
            commandParameters = commandParameters ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand
            {
                CommandText = queryAndParameters[0],
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                CommandTimeout = commandSetting?.CommandTimeout
            };
            List<string> paramStringArray = queryAndParameters
                .Skip(1)
                .ToList() ?? ArrayHelper.EmptyList<string>();

            if ((!setOverratedParametersToOutput && paramStringArray.Count != commandParameters.Length) || paramStringArray.Count < commandParameters.Length)
                throw new ArgumentException(DbAppMessages.ParameterMismatchCompiledQueryAndCommand);

            for (int counter = 0; counter < commandParameters.Length; counter++)
            {
                simpleDbCommand.AddParameter(
                    new DbCommandParameter
                    {
                        ParameterDbType = commandParameters[counter].ParameterDbType,
                        ParameterColumnName = commandParameters[counter].ParameterColumnName,
                        Direction = commandParameters[counter].Direction,
                        ParameterName = paramStringArray[counter],
                        ParameterPrecision = commandParameters[counter].ParameterPrecision,
                        ParameterScale = commandParameters[counter].ParameterScale,
                        ParameterSize = commandParameters[counter].ParameterSize,
                        Value = commandParameters[counter].Value
                    });
            }

            if (setOverratedParametersToOutput && paramStringArray.Count > commandParameters.Length)
            {
                int outputParameterCount = paramStringArray.Count - commandParameters.Length;
                for (int counter = 0; counter < outputParameterCount; counter++)
                {
                    simpleDbCommand.AddParameter(
                        new DbCommandParameter
                        {
                            Direction = ParameterDirection.Output,
                            ParameterName = paramStringArray[commandParameters.Length + counter]
                        });
                }
            }

            return simpleDbCommand;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionType">db connacteion type.</param>
        /// <param name="tempsimpleDbCommand">odbc database command</param>
        /// <param name="setOverratedParamsToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns></returns>
        public static SimpleDbCommand RebuildSimpleDbCommandForTranslate(this DbConnectionTypes connectionType,
            SimpleDbCommand tempsimpleDbCommand, bool setOverratedParamsToOutput = false)
        {
            SimpleDbCommand simpleDbCommand = new SimpleDbCommand();

            string[] queryAndParameters = connectionType.TranslateOdbcQuery(tempsimpleDbCommand.CommandText);

            simpleDbCommand.CommandText = queryAndParameters[0];
            simpleDbCommand.CommandType = tempsimpleDbCommand.CommandType;
            simpleDbCommand.CommandTimeout = tempsimpleDbCommand.CommandTimeout;
            List<string> paramStringArray = queryAndParameters.Skip(1).ToList() ?? ArrayHelper.EmptyList<string>();

            if ((!setOverratedParamsToOutput && paramStringArray.Count != tempsimpleDbCommand.CommandParameters.Count)
                || paramStringArray.Count < tempsimpleDbCommand.CommandParameters.Count)
                throw new ArgumentException(DbAppMessages.ParameterMismatchCompiledQueryAndCommand);

            for (int counter = 0; counter < tempsimpleDbCommand.CommandParameters.Count; counter++)
            {
                DbCommandParameter parameter = tempsimpleDbCommand.CommandParameters[counter] as DbCommandParameter;
                object cmdParameter = new DbCommandParameter
                {
                    DbType = parameter.DbType,
                    ParameterColumnName = parameter.ParameterColumnName,
                    Direction = parameter.Direction,
                    ParameterName = paramStringArray[counter],
                    Precision = parameter.Precision,
                    Scale = parameter.Scale,
                    Size = parameter.Size,
                    Value = parameter.Value
                };
                simpleDbCommand.AddCommandParameter(cmdParameter);
            }

            if (setOverratedParamsToOutput && paramStringArray.Count > tempsimpleDbCommand.CommandParameters.Count)
            {
                int outputParameterCount = paramStringArray.Count - tempsimpleDbCommand.CommandParameters.Count;
                for (int counter = 0; counter < outputParameterCount; counter++)
                {
                    simpleDbCommand.AddParameter(
                        new DbCommandParameter
                        {
                            Direction = ParameterDirection.Output,
                            ParameterName =
                            paramStringArray[tempsimpleDbCommand.CommandParameters.Count + counter]
                        });
                }
            }

            return simpleDbCommand;
        }

        /// <summary>
        /// Builds DbCommand instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">Sql query <see cref="string"/>.</param>
        /// <param name="parameterValues">Database command parameters <see cref="object[]"/>.</param>
        /// <param name="commandType">Command Type.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as ouput parameters.</param>
        /// <param name="transaction">Database transaction.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <returns>Returns DbCommand object instance <see cref="DbCommand"/>.</returns>
        public static DbCommand BuildDbCommandForTranslate(
            this IDbConnection connection, string odbcSqlQuery, object[] parameterValues,
            CommandType commandType = CommandType.Text, bool setOverratedParametersToOutput = false,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DbCommand command = connection.CreateCommand() as DbCommand;
            command.Connection = (DbConnection)connection;

            string[] queryAndParamters = connection.TranslateOdbcQuery(odbcSqlQuery);
            parameterValues = parameterValues ?? ArrayHelper.Empty<object>();

            command.CommandText = queryAndParamters[0];
            command.CommandType = commandType;

            if (commandTimeout != null) command.CommandTimeout = commandTimeout.Value;

            List<string> paramStringArray = queryAndParamters.Skip(1).ToList() ?? ArrayHelper.EmptyList<string>();

            if ((!setOverratedParametersToOutput && paramStringArray.Count != parameterValues.Length)
                || paramStringArray.Count < parameterValues.Length)
                throw new ArgumentException(DbAppMessages.ParameterMismatchCompiledQueryAndCommand);

            for (int counter = 0; counter < parameterValues.Length; counter++)
            {
                DbParameter parameter = command.CreateDbParameter(paramStringArray[counter],
                    parameterValues[counter], parameterValues[counter].ToDbType(), ParameterDirection.Input);
                command.Parameters.Add(parameter);
            }

            if (setOverratedParametersToOutput && paramStringArray.Count > parameterValues.Length)
            {
                int count = paramStringArray.Count - parameterValues.Length;
                for (int counter = 0; counter < count; counter++)
                {
                    DbParameter parameter = command.CreateDbParameter(paramStringArray[parameterValues.Length + counter],
                        value: parameterValues[counter - 1], direction: ParameterDirection.Output);
                    command.Parameters.Add(parameter);
                }
            }

            command.SetCommandTransaction(transaction);
            return command;
        }

        /// <summary>
        /// Gets parameters from object.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="obj">object contains db parameters as property.</param>
        /// <returns>Returns DbCommandParameter instance array.</returns>
        public static DbCommandParameter[] TranslateParametersFromObject(this IDbConnection connection, object obj)
        {
            DbCommandParameter[] parameters = ArrayHelper.Empty<DbCommandParameter>();
            if (obj == null)
                return parameters;

            DbConnectionTypes connectionType = connection.GetDbConnectionType();
            bool isOdbc = connectionType.IsOdbcConn();

            parameters = obj.GetType()
                .GetProperties()
                .Select(p => new DbCommandParameter
                {
                    ParameterName = isOdbc ? string.Empty : p.Name,
                    Value = p.GetValue(obj, null),
                    Direction = ParameterDirection.Input,
                    ParameterDbType = p.GetValue(obj, null).ToDbType(),
                }).ToArray() ?? ArrayHelper.Empty<DbCommandParameter>();

            return parameters;
        }
    }
}