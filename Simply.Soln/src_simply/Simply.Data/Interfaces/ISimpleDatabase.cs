using Simply.Data.Enums;
using Simply.Data.Objects;
using System;
using System.Data;
using System.Data.Common;

namespace Simply.Data.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ISimpleDatabase : IDisposable
    {
        /// <summary>
        /// Gets the query setting.
        /// </summary>
        /// <value>
        /// The query setting.
        /// </value>
        IQuerySetting QuerySetting { get; }

        /// <summary>
        /// Gets the type of the database connection.
        /// </summary>
        /// <value>
        /// The type of the database connection.
        /// </value>
        DbConnectionTypes ConnectionType { get; }

        /// <summary>
        /// Gets, sets command setting.
        /// </summary>
        [Obsolete("This property will be removed.")]
        ICommandSetting CommandSetting { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<SimpleDbCommand> CommandLogAction { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<IDbCommand> DbCommandLogAction { get; set; }

        /// <summary>
        /// Gets the log setting.
        /// </summary>
        ILogSetting LogSetting { get; }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        void Begin(IsolationLevel? isolationLevel = null);

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Builds SimpleDbCommand instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="odbcSqlQuery">The query <see cref="string"/>.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandType">The db command type <see cref="Nullable{CommandType}"/>.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        SimpleDbCommand BuildSimpleDbCommandForOdbcQuery(string odbcSqlQuery,
            object[] parameterValues, CommandType? commandType = null, bool setOverratedParametersToOutput = false);

        /// <summary>
        /// Builds the simple database command for SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        SimpleDbCommand BuildSimpleDbCommandForQuery(string sqlQuery, object parameterObject, CommandType? commandType = null);

        /// <summary>
        /// Create IDbCommand instance with database command and db transaction for given db connection.
        /// </summary>
        /// <param name="simpleDbCommand">database command <see cref="SimpleDbCommand"/>.</param>
        /// <param name="connectionShouldBeOpened">if it is true database connection will be opened, else not.</param>
        /// <returns>Returns DbCommand object instance <see cref="IDbCommand"/>.</returns>
        IDbCommand CreateCommand(SimpleDbCommand simpleDbCommand, bool connectionShouldBeOpened = true);

        /// <summary>
        /// Gets DbDataAdapter instance of database connection.
        /// </summary>
        /// <returns>Returns DbDataAdapter instance.</returns>
        DbDataAdapter CreateDataAdapter();

        /// <summary>
        /// Applies the paging info into simple db command.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        /// <param name="pageInfo">The page information.</param>
        /// <returns>Applies paging and return simpledbcommand instance.</returns>
        SimpleDbCommand ApplyPageInfo(SimpleDbCommand dbCommand, IPageInfo pageInfo = null);
    }
}