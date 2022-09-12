using Simply.Common;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.DbTransactionExtensions;
using Simply.Data.Enums;
using Simply.Data.Helpers;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using Simply.Data.QuerySettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Simply.Data.Database
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Simply.Data.Interfaces.ISimpleDatabase" />
    public class SimpleDatabase : ISimpleDatabase
    {
        /// <summary>
        /// Defines the empty.
        /// </summary>
        private static readonly string empty = string.Empty;

        /// <summary>
        /// The disposed for disposing.
        /// </summary>
        protected bool disposed = false;

        /// <summary>
        /// The transaction state.
        /// 0; transaction state empty.
        /// 1; transaction can be committed/rollbacked.
        /// 2; transaction has been committed/rollbacked.
        /// </summary>
        protected sbyte transactionState = 0;

        /// <summary>
        /// The connection
        /// </summary>
        protected IDbConnection connection;

        /// <summary>
        /// The transaction
        /// </summary>
        protected IDbTransaction transaction;

        /// <summary>
        /// The log setting
        /// </summary>
        protected ILogSetting logSetting = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleDatabase(IDbConnection connection, IDbTransaction transaction = null,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = connection;
            this.transaction = transaction;
            CommandSetting = commandSetting;
            ConnectionType = connection.GetDbConnectionType();
            QuerySetting = querySetting ?? connection.GetQuerySetting();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        /// <param name="providerFactory">The provider factory.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        public SimpleDatabase(DbProviderFactory providerFactory, string connectionString,
            ICommandSetting commandSetting = null, IQuerySetting querySetting = null)
        {
            logSetting = SimpleLogSetting.New();
            this.connection = providerFactory.CreateConnection();
            this.connection.ConnectionString = connectionString;
            CommandSetting = commandSetting;
            ConnectionType = connection.GetDbConnectionType();
            QuerySetting = querySetting ?? connection.GetQuerySetting();
        }

        /// <summary>
        /// Gets the query setting.
        /// </summary>
        /// <value>
        /// The query setting.
        /// </value>
        public IQuerySetting QuerySetting
        { get; protected set; }

        /// <summary>
        /// Gets the type of the database connection.
        /// </summary>
        /// <value>
        /// The type of the database connection.
        /// </value>
        public DbConnectionTypes ConnectionType
        { get; protected set; }

        /// <summary>
        /// Gets, sets command setting.
        /// </summary>
        public ICommandSetting CommandSetting
        { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<SimpleDbCommand> CommandLogAction
        {
            get { return logSetting.CommandLogAction; }
            set { logSetting.SetCommandLogAction(value); }
        }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<IDbCommand> DbCommandLogAction
        {
            get { return logSetting.DbCommandLogAction; }
            set { logSetting.SetDbCommandLogAction(value); }
        }

        /// <summary>
        /// Gets the log setting.
        /// </summary>
        public ILogSetting LogSetting
        { get { return this.logSetting; } }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                if (transactionState == 1)
                {
                    transaction?.CommitAndDispose();
                    transactionState = 0;
                }
                connection?.CloseAndDispose();

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            disposed = true;
        }

        /// <summary>
        /// disposes both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public void Begin(IsolationLevel? isolationLevel = null)
        {
            if (transaction == null)
                transaction = connection.OpenAndBeginTransaction(isolationLevel);

            transactionState = 1;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            if (transactionState == 1)
            {
                transaction.CommitAndDispose();
                transactionState = 2;
                transaction = null;
            }
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public void Rollback()
        {
            if (transactionState == 1)
            {
                transaction.RollbackAndDispose();
                transactionState = 2;
                transaction = null;
            }
        }

        /// <summary>
        /// Begins the transction.
        /// </summary>
        ~SimpleDatabase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Translates the parameters from object.
        /// </summary>
        /// <param name="obj">object that contains parameters as property.</param>
        /// <returns></returns>
        protected virtual List<DbCommandParameter> TranslateParametersFromObject(object obj)
        {
            List<DbCommandParameter> parameters = ArrayHelper.EmptyList<DbCommandParameter>();
            if (obj == null)
                return parameters;

            //
            // TODO: Bu method SimpleDatabase içine alınacak.
            // obj nesnesi IEnumerable olursa ona göre tekrar düzenleme yapılacak.
            // DONE

            // TODO : CODE SHOULD BE REVIEWED.

            int parameterCounter = 0;
            if (obj is string)
            {
                parameters.Add(new DbCommandParameter
                {
                    ParameterName = string.Concat(InternalAppValues.ParameterChar.ToString(), parameterCounter.ToString()),//string.Empty,
                    Value = obj,
                    Direction = ParameterDirection.Input,
                    ParameterDbType = DbType.String,
                });

                return parameters;
            }

            bool isOdbc = this.ConnectionType.IsOdbcConn();
            IEnumerable objects = obj as IEnumerable;

            if (objects == null)
            {
                parameters = obj.GetType()
                    .GetProperties()
                    .Select(p => new DbCommandParameter
                    {
                        ParameterName = isOdbc ? string.Empty : p.Name,
                        Value = p.GetValue(obj, null),
                        Direction = ParameterDirection.Input,
                        ParameterDbType = p.GetValue(obj, null).ToDbType(),
                    }).ToList() ?? ArrayHelper.EmptyList<DbCommandParameter>();

                return parameters;
            }

            int counter = 0;
            foreach (object item in objects)
            {
                if (item is null)
                    continue;

                if (item.GetType().IsSimpleTypeV2())
                {
                    parameters.Add(new DbCommandParameter
                    {
                        ParameterName = string.Concat(InternalAppValues.ParameterChar.ToString(), (parameterCounter++).ToString()),
                        Value = item,
                        Direction = ParameterDirection.Input,
                        ParameterDbType = DbType.String,
                    });
                }
                else
                {
                    List<DbCommandParameter> tempParameters =
                 item.GetType()
                .GetProperties()
                .Select(p => new DbCommandParameter
                {
                    ParameterName = isOdbc ? string.Empty : string.Concat(p.Name, (counter++).ToString()),
                    Value = p.GetValue(obj, null),
                    Direction = ParameterDirection.Input,
                    ParameterDbType = p.GetValue(obj, null).ToDbType(),
                }).ToList() ?? ArrayHelper.EmptyList<DbCommandParameter>();
                    parameters.AddRange(tempParameters);
                }

                counter++;
            }

            return parameters;
        }

        /// <summary>
        /// The Translate Odbc Query to parametrized query.
        /// </summary>
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
        protected virtual string[] TranslateOdbcQuery(string odbcSqlQuery)
        {
            if (string.IsNullOrWhiteSpace(odbcSqlQuery))
                throw new ArgumentNullException(nameof(odbcSqlQuery));

            List<string> queryAndParameters = new List<string>();
            odbcSqlQuery = odbcSqlQuery.Trim();
            if (!odbcSqlQuery.Contains(InternalAppValues.QuestionMark) || this.ConnectionType.IsOdbcConn())
            { queryAndParameters.Add(odbcSqlQuery); return queryAndParameters.ToArray(); }

            List<string> queryParts = odbcSqlQuery
                .Split(new char[] { InternalAppValues.QuestionMark }, StringSplitOptions.None)
                .ToList() ?? ArrayHelper.EmptyList<string>();
            IQuerySetting querySetting = QuerySettingsFactory.GetQuerySetting(this.ConnectionType);

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
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <param name="commandParameters">The commandParameters <see cref="DbCommandParameter[]"/>.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns database command object instance <see cref="SimpleDbCommand" />.</returns>
        protected virtual SimpleDbCommand BuildSimpleDbCommandForTranslate(
            string odbcSqlQuery, DbCommandParameter[] commandParameters,
            CommandType? commandType = null, bool setOverratedParametersToOutput = false)
        {
            string[] queryAndParameters = TranslateOdbcQuery(odbcSqlQuery);
            commandParameters = commandParameters ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand
            {
                CommandText = queryAndParameters[0],
                CommandType = commandType,
                CommandTimeout = this.CommandSetting?.CommandTimeout
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
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        public virtual SimpleDbCommand BuildSimpleDbCommandForOdbcQuery(string odbcSqlQuery,
            object[] parameterValues, CommandType? commandType = null, bool setOverratedParametersToOutput = false)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray() ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand =
               BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, commandType: commandType,
               setOverratedParametersToOutput: setOverratedParametersToOutput);

            return simpleDbCommand;
        }

        /// <summary>
        /// Builds the simple database command for SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        public virtual SimpleDbCommand BuildSimpleDbCommandForQuery(string sqlQuery,
            object parameterObject, CommandType? commandType = null)
        {
            List<DbCommandParameter> parameters = TranslateParametersFromObject(parameterObject);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandTimeout = this.CommandSetting?.CommandTimeout,
                ParameterNamePrefix = this.CommandSetting?.ParameterNamePrefix,
                CommandType = commandType ?? CommandType.Text,
            };

            simpleDbCommand.RecompileQuery(this.QuerySetting, parameters);
            return simpleDbCommand;
        }

        /// <summary>
        /// Create IDbCommand instance with database command.
        /// </summary>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <param name="connectionShouldBeOpened">if it is true database connection will be opened, else not.</param>
        /// <returns>Returns DbCommand object instance <see cref="IDbCommand"/>.</returns>
        public virtual IDbCommand CreateCommand(SimpleDbCommand simpleDbCommand, bool connectionShouldBeOpened = true)
        {
            if (simpleDbCommand == null || string.IsNullOrWhiteSpace(simpleDbCommand.CommandText))
                throw new ArgumentNullException(nameof(simpleDbCommand));

            InternalLogHelper.LogCommand(simpleDbCommand, this.LogSetting);

            // TODO : WILL BE CHECKED AND TESTED.
            // LIST AND BULK INSERT TEST OK.

            IDbCommand command = connection.CreateCommand()
                .SetCommandType(simpleDbCommand.CommandType)
                .SetCommandText(simpleDbCommand.CommandText)
                .SetCommandTimeout(simpleDbCommand.CommandTimeout)
                .SetTransaction(transaction)
                .IncludeCommandParameters(simpleDbCommand.CommandParameters);

            InternalLogHelper.LogDbCommand(command, this.LogSetting);

            if (connectionShouldBeOpened && transaction == null)
                connection.OpenIfNot();

            return command;
        }

        /// <summary>
        /// Gets DbDataAdapter instance of database connection.
        /// </summary>
        /// <returns>Returns DbDataAdapter instance.</returns>
        public virtual DbDataAdapter CreateDataAdapter()
        {
            // Alternative
            // connection.CreateAdapter();
            Type adapterType = null;

            IEnumerable<Type> adapterTypes =
                 connection.GetType().Assembly.GetExportedTypes().Where(
                     type => type.IsClass && type.GetInterfaces().Contains(typeof(IDbDataAdapter))
                            && !type.IsAbstract && typeof(DbDataAdapter).IsAssignableFrom(type));

            if (adapterTypes.Count() > 1)
            {
                string connectionTypeName = connection.GetType().Name;
                connectionTypeName = connectionTypeName.Substring(0,
                    connectionTypeName.Length - InternalAppValues.ConnectionName.Length).ToLower();
                adapterType = adapterTypes.First(typ => typ.Name.ToLower().StartsWith(connectionTypeName));
            }
            else if (adapterTypes.Count() == 1)
            {
                adapterType = adapterTypes.First();
            }

            DbDataAdapter dataAdapter = null;
            if (adapterType != null)
            {
                dataAdapter = Activator.CreateInstance(adapterType) as DbDataAdapter;
            }

            return dataAdapter;
        }

        /// <summary>
        /// Applies the paging info into simple db command.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        /// <param name="pageInfo">The page information.</param>
        /// <returns>Applies paging and return simpledbcommand instance.</returns>
        public virtual SimpleDbCommand ApplyPageInfo(SimpleDbCommand dbCommand, IPageInfo pageInfo = null)
        {
            if (pageInfo == null) return dbCommand;

            if (!pageInfo.IsPageable) return dbCommand;

            string skipAndTakeFormat = this.QuerySetting.SkipAndTakeFormat;
            bool isPageableAndSkipAndTakeFormatEmpty = skipAndTakeFormat.IsNullOrSpace();
            if (!isPageableAndSkipAndTakeFormatEmpty)
            {
                string format = skipAndTakeFormat.CopyValue();
                format = format.Replace(InternalAppValues.SkipFormat, pageInfo.Skip.ToString());
                format = format.Replace(InternalAppValues.TakeFormat, pageInfo.Take.ToString());
                format = format.Replace(InternalAppValues.SqlScriptFormat, dbCommand.CommandText);
                dbCommand.CommandText = format.CopyValue();
            }

            return dbCommand;
        }
    }
}