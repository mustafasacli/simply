using Simply.Common;
using Simply.Common.Interfaces;
using Simply.Data.Constants;
using Simply.Data.DbCommandExtensions;
using Simply.Data.DbTransactionExtensions;
using Simply.Data.Enums;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using Simply.Data.QuerySettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.String;

namespace Simply.Data.Database
{
    /// <summary>
    /// Simple Database object for database operations.
    /// </summary>
    /// <seealso cref="ISimpleDatabase" />
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
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
        /// Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        /// <exception cref="{System.NotImplementedException}" >Empty contructor can not be implemented.</exception>
        public SimpleDatabase()
        {
            throw new NotImplementedException("Empty contructor can not be implemented.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        /// <param name="definitorFactory">definitor factory instance for create definitor.</param>
        public SimpleDatabase(IDbConnection connection, IDbTransaction transaction = null,
            IQuerySetting querySetting = null, ISimpleDefinitorFactory definitorFactory = null)
        {
            this.connection = connection;
            this.transaction = transaction;
            ConnectionType = connection.GetDbConnectionType();
            QuerySetting = querySetting ?? connection.GetQuerySetting();
            DefinitorFactory = definitorFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDatabase"/> class.
        /// </summary>
        /// <param name="providerFactory">The provider factory.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="querySetting">Query Setting instance.</param>
        /// <param name="definitorFactory">definitor factory instance for create definitor.</param>
        public SimpleDatabase(DbProviderFactory providerFactory, string connectionString,
            IQuerySetting querySetting = null, ISimpleDefinitorFactory definitorFactory = null)
        {
            this.connection = providerFactory.CreateConnection();
            this.connection.ConnectionString = connectionString;
            ConnectionType = connection.GetDbConnectionType();
            QuerySetting = querySetting ?? connection.GetQuerySetting();
            DefinitorFactory = definitorFactory;
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
        /// Gets the action for command logging.
        /// </summary>
        public Action<SimpleDbCommand> CommandLogAction
        { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<IDbCommand> DbCommandLogAction
        { get; set; }

        /// <summary>
        /// Gets or sets the internal exception handler.
        /// </summary>
        public Action<Exception> InternalExceptionHandler
        { get; set; }

        /// <summary>
        /// Gets, sets value for SimpleDbCommand logging.
        /// </summary>
        public bool LogCommand
        { get; set; }

        /// <summary>
        /// Gets, sets value for IDbCommand logging.
        /// </summary>
        public bool LogDbCommand
        { get; set; }

        /// <summary>
        /// Gets or sets connection auto close.
        /// </summary>
        public bool AutoClose
        { get; set; }

        /// <summary>
        /// Gets the definitor factory.
        /// </summary>
        public ISimpleDefinitorFactory DefinitorFactory
        { get; protected set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (disposing)
                    {
                        // Dispose managed resources.
                    }

                    if (transactionState == 1)
                    {
                        try
                        {
                            transaction?.CommitAndDispose();
                            transactionState = 0;
                        }
                        catch (Exception ex1)
                        {
                            transactionState = 0;
                            try
                            {
                                if (InternalExceptionHandler != null)
                                {
                                    InternalExceptionHandler(ex1);
                                }
                            }
                            catch (Exception ee)
                            {
                                Trace.WriteLine(ee.ToString());
                            }
                            throw;
                        }
                    }
                }
                catch (Exception ex2)
                {
                    try
                    {
                        if (InternalExceptionHandler != null)
                        {
                            InternalExceptionHandler(ex2);
                        }
                    }
                    catch (Exception ee2)
                    {
                        Trace.WriteLine(ee2.ToString());
                    }
                    throw;
                }
                finally
                {
                    connection?.CloseAndDispose();
                }
            }
            disposed = true;
        }

        /// <summary>
        /// disposes both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            { Dispose(true); }
            finally
            { GC.SuppressFinalize(this); }
        }

        /// <summary>
        /// the finalizer
        /// </summary>
        ~SimpleDatabase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public void Begin(IsolationLevel? isolationLevel = null)
        {
            if (transaction is null)
                transaction = connection.OpenAndBeginTransaction(isolationLevel);

            transactionState = 1;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <param name="closeConnectionAtFinal">If true, close connection at final.</param>
        public void Commit(bool closeConnectionAtFinal = true)
        {
            try
            {
                if (transactionState == 1)
                {
                    try
                    { transaction.CommitAndDispose(); }
                    catch (Exception ex1)
                    {
                        try
                        {
                            if (InternalExceptionHandler != null)
                            {
                                InternalExceptionHandler(ex1);
                            }
                        }
                        catch (Exception ee)
                        {
                            Trace.WriteLine(ee.ToString());
                        }
                        throw;
                    }
                    finally
                    {
                        transactionState = 2;
                        transaction = null;
                    }
                }
            }
            finally
            {
                if (closeConnectionAtFinal)
                    connection?.CloseIfNot();
            }
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        /// <param name="closeConnectionAtFinal">If true, close connection at final.</param>
        public void Rollback(bool closeConnectionAtFinal = true)
        {
            try
            {
                if (transactionState == 1)
                {
                    try
                    { transaction.RollbackAndDispose(); }
                    catch (Exception ex1)
                    {
                        try
                        {
                            if (InternalExceptionHandler != null)
                            {
                                InternalExceptionHandler(ex1);
                            }
                        }
                        catch (Exception ee)
                        {
                            Trace.WriteLine(ee.ToString());
                        }
                        throw;
                    }
                    finally
                    {
                        transactionState = 2;
                        transaction = null;
                    }
                }
            }
            finally
            {
                if (closeConnectionAtFinal)
                    connection?.CloseIfNot();
            }
        }

        /// <summary>
        /// Closes the database connection if there is no alive transaction.
        /// </summary>
        /// <exception cref="System.Exception">if there is alive transaction.</exception>
        public void Close()
        {
            if (transactionState == 1)
                throw new Exception("There is alive transaction.");

            connection?.CloseIfNot();
        }

        /// <summary>
        /// Translates the parameters from object.
        /// </summary>
        /// <param name="obj">object that contains parameters as property.</param>
        /// <returns></returns>
        protected virtual List<DbCommandParameter> TranslateParametersFromObject(object obj)
        {
            List<DbCommandParameter> parameters = ArrayHelper.EmptyList<DbCommandParameter>();
            if (obj is null)
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
                    ParameterName = string.Concat(InternalAppValues.ParameterChar.ToString(), parameterCounter.ToString()),
                    Value = obj,
                    Direction = ParameterDirection.Input,
                    ParameterDbType = DbType.String,
                });

                return parameters;
            }

            bool isOdbc = this.ConnectionType.IsOdbcConn();
            IEnumerable objects = obj as IEnumerable;

            if (objects is null)
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
                    parameterName = string.Concat(querySetting.ParameterPrefix,
                        InternalAppValues.ParameterChar.ToString(), parameterCounter.ToString());
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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns database command object instance <see cref="SimpleDbCommand" />.</returns>
        protected virtual SimpleDbCommand BuildSimpleDbCommandForTranslate(string odbcSqlQuery,
            DbCommandParameter[] commandParameters, ICommandSetting commandSetting = null, bool setOverratedParametersToOutput = false)
        {
            string[] queryAndParameters = TranslateOdbcQuery(odbcSqlQuery);
            commandParameters = commandParameters ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand
            {
                CommandText = queryAndParameters[0],
                CommandType = commandSetting?.CommandType,
                CommandTimeout = commandSetting?.CommandTimeout
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
                int count = paramStringArray.Count - commandParameters.Length;
                for (int counter = 0; counter < count; counter++)
                {
                    simpleDbCommand.AddParameter(
                        new DbCommandParameter
                        {
                            Direction = ParameterDirection.Output,
                            ParameterName = paramStringArray[commandParameters.Length + counter],
                            ParameterDbType = DbType.Object
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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        public virtual SimpleDbCommand BuildSimpleDbCommandForOdbcQuery(string odbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null, bool setOverratedParametersToOutput = false)
        {
            DbCommandParameter[] commandParameters = (parameterValues ?? ArrayHelper.Empty<object>())
            .Select(p => new DbCommandParameter
            {
                Value = p,
                ParameterDbType = p.ToDbType()
            }).ToArray() ?? ArrayHelper.Empty<DbCommandParameter>();

            SimpleDbCommand simpleDbCommand =
               BuildSimpleDbCommandForTranslate(odbcSqlQuery, commandParameters, commandSetting,
               setOverratedParametersToOutput: setOverratedParametersToOutput);

            return simpleDbCommand;
        }

        /// <summary>
        /// Builds the simple database command for SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        public virtual SimpleDbCommand BuildSimpleDbCommandForQuery(string sqlQuery,
            object parameterObject, ICommandSetting commandSetting = null)
        {
            List<DbCommandParameter> parameters = TranslateParametersFromObject(parameterObject);

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = sqlQuery,
                CommandTimeout = commandSetting?.CommandTimeout,
                ParameterNamePrefix = commandSetting?.ParameterNamePrefix,
                CommandType = commandSetting?.CommandType ?? CommandType.Text
            };

            simpleDbCommand.RecompileQuery(this.QuerySetting, parameters);
            simpleDbCommand.AddParameters(parameters);

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
            if (simpleDbCommand is null || string.IsNullOrWhiteSpace(simpleDbCommand.CommandText))
                throw new ArgumentNullException(nameof(simpleDbCommand));

            CommandLog(simpleDbCommand);

            IDbCommand command = connection.CreateCommand()
                .SetCommandType(simpleDbCommand.CommandType)
                .SetCommandText(simpleDbCommand.CommandText)
                .SetCommandTimeout(simpleDbCommand.CommandTimeout)
                .SetTransaction(transaction)
                .IncludeCommandParameters(simpleDbCommand.CommandParameters, this.QuerySetting);

            DbCommandLog(command);

            if (connectionShouldBeOpened && transaction is null)
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
            if (pageInfo is null) return dbCommand;

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

        /// <summary>
        /// Logs the command.
        /// </summary>
        /// <param name="simpleDbCommand">The simple db command.</param>
        protected virtual void CommandLog(SimpleDbCommand simpleDbCommand)
        {
            if (simpleDbCommand == null)
                return;

            if (this.LogCommand && this.CommandLogAction != null)
                this.CommandLogAction(simpleDbCommand);
        }

        /// <summary>
        /// Logs the database command.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        protected virtual void DbCommandLog(IDbCommand dbCommand)
        {
            if (dbCommand == null)
                return;

            if (this.LogDbCommand && this.DbCommandLogAction != null)
                this.DbCommandLogAction(dbCommand);
        }

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns>A string.</returns>
        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        /// <summary>
        /// Builds SimpleDbCommand instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="jdbcSqlQuery">Jdbc Sql query <see cref="string"/>
        /// like #SELECT T1.* FROM TABLE T1 WHERE T1.INT_COLUMN = ?1 AND T2.DATE_COLUMN = ?2 #.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        public virtual SimpleDbCommand BuildSimpleDbCommandForJdbcQuery(string jdbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null, bool setOverratedParametersToOutput = false)
        {
            int questionMarkCount = jdbcSqlQuery.Count(s => s == InternalAppValues.QuestionMark);
            int parameterCount = parameterValues?.Length ?? 0;
            if ((!setOverratedParametersToOutput && questionMarkCount != parameterCount) || questionMarkCount < parameterCount)
                throw new ArgumentException(DbAppMessages.ParameterMismatchCompiledQueryAndCommand);

            char parameterNamePrefix = commandSetting?.ParameterNamePrefix ?? InternalAppValues.ParameterChar;
            string compiledSqlQuery = jdbcSqlQuery.Replace(InternalAppValues.QuestionMark.ToString(),
                Concat(QuerySetting.ParameterPrefix, parameterNamePrefix, this.QuerySetting.ParameterSuffix));

            SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandText = compiledSqlQuery,
                CommandTimeout = commandSetting?.CommandTimeout,
                CommandType = commandSetting?.CommandType ?? CommandType.Text,
                ParameterNamePrefix = parameterNamePrefix
            };

            List<DbCommandParameter> commandParameters = new List<DbCommandParameter>();

            for (int counter = 0; counter < parameterCount; counter++)
            {
                DbCommandParameter parameter = new DbCommandParameter
                {
                    ParameterName = Concat(QuerySetting.ParameterPrefix, parameterNamePrefix,
                    (counter + 1).ToString(), QuerySetting.ParameterSuffix),
                    Value = parameterValues[counter],
                    Direction = ParameterDirection.Input,
                    DbType = parameterValues[counter].ToDbType() ?? DbType.Object
                };
                commandParameters.Add(parameter);
            }

            if (setOverratedParametersToOutput && questionMarkCount > parameterCount)
            {
                int count = questionMarkCount - parameterCount;
                for (int counter = 0; counter < count; counter++)
                {
                    DbCommandParameter parameter = new DbCommandParameter
                    {
                        ParameterName = Concat(QuerySetting.ParameterPrefix, parameterNamePrefix,
                        (counter + 1).ToString(), QuerySetting.ParameterSuffix),
                        Direction = ParameterDirection.Input,
                        DbType = parameterValues[counter].ToDbType() ?? DbType.Object
                    };
                    commandParameters.Add(parameter);
                }
            }

            simpleDbCommand.AddCommandParameters(commandParameters);

            return simpleDbCommand;
        }

        /// <summary>
        /// Creates Db connection.
        /// </summary>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="connectionString">database connection string</param>
        /// <returns>TDbConnection object instance</returns>
        public static TDbConnection Create<TDbConnection>(string connectionString) where TDbConnection : IDbConnection
        {
            TDbConnection dbConnection = (TDbConnection)Activator.CreateInstance(typeof(TDbConnection));
            dbConnection.ConnectionString = connectionString;
            return dbConnection;
        }

        /// <summary>
        /// Creates the simple database instance.
        /// </summary>
        /// <param name="logCommand">If true, log command.</param>
        /// <param name="logDbCommand">If true, log db command.</param>
        /// <param name="autoClose">If true, auto close.</param>
        /// <param name="commandLogAction">The command log action.</param>
        /// <param name="dbCommandLogAction">The db command log action.</param>
        /// <param name="internalExceptionHandler">The internal exception handler.</param>
        /// <returns>A TSimpleDatabase instance.</returns>
        public static TSimpleDatabase CreateDb<TSimpleDatabase>(bool logCommand = false, bool logDbCommand = false, bool autoClose = false,
            Action<SimpleDbCommand> commandLogAction = null, Action<IDbCommand> dbCommandLogAction = null,
            Action<Exception> internalExceptionHandler = null) where TSimpleDatabase : ISimpleDatabase
        {
            TSimpleDatabase database = (TSimpleDatabase)Activator.CreateInstance(typeof(TSimpleDatabase));

            database.LogCommand = logCommand;
            database.LogDbCommand = logDbCommand;
            database.AutoClose = autoClose;
            database.CommandLogAction = commandLogAction;
            database.DbCommandLogAction = dbCommandLogAction;
            database.InternalExceptionHandler = internalExceptionHandler;

            return database;
        }
    }
}