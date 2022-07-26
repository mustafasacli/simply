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
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = ?" For Sql Server Result; "SELECT T1.*
        /// FROM TABLE1 T1 WHERE T1.ID_COLUMN = @p0", "@p0" For Oracle Result; "SELECT T1.* FROM
        /// TABLE1 T1 WHERE T1.ID_COLUMN = :p0", ":p0" .
        /// </returns>
        public static string[] TranslateOdbcQuery(this IDbConnection connection, string odbcSqlQuery)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            string[] queryAndParameters = TranslateOdbcQuery(connection.GetDbConnectionType(), odbcSqlQuery);
            /*
            odbcSqlQuery = odbcSqlQuery.Trim();
            if (!odbcSqlQuery.Contains(InternalAppValues.QuestionMark) || connection.GetDbConnectionType().IsOdbcConn())
            { queryAndParameters.Add(odbcSqlQuery); return queryAndParameters.ToArray(); }

            List<string> queryParts = odbcSqlQuery.Split(new char[] { InternalAppValues.QuestionMark }, StringSplitOptions.None).ToList() ?? ArrayHelper.EmptyList<string>();
            IQuerySetting setting = connection.GetQuerySetting();

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
                    parameterName = setting.ParameterPrefix + InternalAppValues.ParameterChar.ToString() + parameterCounter.ToString();
                    parameterCounter++;
                } while (!(queryParts.All(q => !q.Contains(parameterName)) &&
                queryAndParameters.IndexOf(parameterName) == -1));

                sqlBuilder.Append(parameterName);
                queryAndParameters.Add(parameterName);
            }

            sqlBuilder.Append(queryParts[queryParts.Count - 1]);
            queryAndParameters[0] = sqlBuilder.ToString();
            */
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
        /// "SELECT T1.* FROM TABLE1 T1 WHERE T1.ID_COLUMN = ?" For Sql Server Result; "SELECT T1.*
        /// FROM TABLE1 T1 WHERE T1.ID_COLUMN = @p0", "@p0" For Oracle Result; "SELECT T1.* FROM
        /// TABLE1 T1 WHERE T1.ID_COLUMN = :p0", ":p0" .
        /// </returns>
        public static string[] TranslateOdbcQuery(this DbConnectionTypes connectionType, string odbcSqlQuery)
        {
            if (string.IsNullOrWhiteSpace(odbcSqlQuery))
                throw new ArgumentNullException(nameof(odbcSqlQuery));

            List<string> queryAndParameters = new List<string>();
            odbcSqlQuery = odbcSqlQuery.Trim();
            if (!odbcSqlQuery.Contains(InternalAppValues.QuestionMark) || connectionType.IsOdbcConn())
            { queryAndParameters.Add(odbcSqlQuery); return queryAndParameters.ToArray(); }

            List<string> queryParts = odbcSqlQuery.Split(new char[] { InternalAppValues.QuestionMark }, StringSplitOptions.None).ToList() ?? ArrayHelper.EmptyList<string>();
            IQuerySetting setting = QuerySettingsFactory.GetQuerySetting(connectionType);

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
                    parameterName = setting.ParameterPrefix + InternalAppValues.ParameterChar.ToString() + parameterCounter.ToString();
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
        /// Builds DbCommandDefinition instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <param name="commandParameters">The commandParameters <see cref="DbCommandParameter[]"/>.</param>
        /// <param name="cmdType">The cmdType <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="commandTimeout">DbCommand timeout</param>
        /// <param name="setOverratedParamsToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns Command Definition object instance <see cref="DbCommandDefinition"/>.</returns>
        public static DbCommandDefinition BuildCommandDefinitionForTranslate(
            this IDbConnection connection, string odbcSqlQuery, DbCommandParameter[] commandParameters, CommandType? cmdType,
            int? commandTimeout = null, bool setOverratedParamsToOutput = false)
        {
            DbCommandDefinition commandDefinition = new DbCommandDefinition();

            string[] queryAndParameters = connection.TranslateOdbcQuery(odbcSqlQuery);
            commandParameters = commandParameters ?? ArrayHelper.Empty<DbCommandParameter>();

            commandDefinition.CommandText = queryAndParameters[0];
            commandDefinition.CommandType = cmdType;
            commandDefinition.CommandTimeout = commandTimeout;
            List<string> paramStringArray = queryAndParameters.Skip(1).ToList() ?? ArrayHelper.EmptyList<string>();

            if ((!setOverratedParamsToOutput && paramStringArray.Count != commandParameters.Length) || paramStringArray.Count < commandParameters.Length)
                throw new ArgumentException("compiled query parameters did not match with command parameters. Please check query and parameters.");

            for (int counter = 0; counter < commandParameters.Length; counter++)
            {
                /*
                DbCommandParameter cmdParameter = new DbCommandParameter
                {
                    DbType = commandParameters[counter].DbType,
                    ParameterColumnName = commandParameters[counter].ParameterColumnName,
                    Direction = commandParameters[counter].Direction,
                    ParameterName = paramStringArray[counter],
                    Precision = commandParameters[counter].Precision,
                    Scale = commandParameters[counter].Scale,
                    Size = commandParameters[counter].Size,
                    Value = commandParameters[counter].Value
                };
                commandDefinition.AddDatabaseParameter(cmdParameter);
                */
                commandDefinition.AddParameter(
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

            if (setOverratedParamsToOutput && paramStringArray.Count > commandParameters.Length)
            {
                int cnt = paramStringArray.Count - commandParameters.Length;
                for (int counter = 0; counter < cnt; counter++)
                {
                    //object parameter = connection.CreateDbCommandParameter(parameterName: paramStringArray[commandParameters.Length + counter],direction: ParameterDirection.Output);
                    //commandDefinition.AddDatabaseParameter(parameter);

                    commandDefinition.AddParameter(
                        new DbCommandParameter
                        {
                            Direction = ParameterDirection.Output,
                            ParameterName = paramStringArray[commandParameters.Length + counter]
                        });
                }
            }

            return commandDefinition;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionType">db connacteion type.</param>
        /// <param name="tempCommandDefinition">odbc command definition</param>
        /// <param name="setOverratedParamsToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns></returns>
        public static DbCommandDefinition RebuildCommandDefinitionForTranslate(this DbConnectionTypes connectionType,
            DbCommandDefinition tempCommandDefinition, bool setOverratedParamsToOutput = false)
        {
            DbCommandDefinition commandDefinition = new DbCommandDefinition();

            string[] queryAndParameters = connectionType.TranslateOdbcQuery(tempCommandDefinition.CommandText);

            commandDefinition.CommandText = queryAndParameters[0];
            commandDefinition.CommandType = tempCommandDefinition.CommandType;
            commandDefinition.CommandTimeout = tempCommandDefinition.CommandTimeout;
            List<string> paramStringArray = queryAndParameters.Skip(1).ToList() ?? ArrayHelper.EmptyList<string>();

            if ((!setOverratedParamsToOutput && paramStringArray.Count != tempCommandDefinition.CommandParameters.Count) || paramStringArray.Count < tempCommandDefinition.CommandParameters.Count)
                throw new ArgumentException("compiled query parameters did not match with command parameters. Please check query and parameters.");

            for (int counter = 0; counter < tempCommandDefinition.CommandParameters.Count; counter++)
            {
                DbCommandParameter parameter = tempCommandDefinition.CommandParameters[counter] as DbCommandParameter;
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
                commandDefinition.AddCommandParameter(cmdParameter);
            }

            if (setOverratedParamsToOutput && paramStringArray.Count > tempCommandDefinition.CommandParameters.Count)
            {
                int count = paramStringArray.Count - tempCommandDefinition.CommandParameters.Count;
                for (int counter = 0; counter < count; counter++)
                {
                    commandDefinition.AddParameter(
                        new DbCommandParameter
                        {
                            Direction = ParameterDirection.Output,
                            ParameterName = paramStringArray[tempCommandDefinition.CommandParameters.Count + counter]
                        });
                }
            }

            return commandDefinition;
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
                throw new ArgumentException("compiled query parameters did not match with command parameters. Please check query and parameters.");

            // method for create db parameter.
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