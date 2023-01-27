using Simply.Common.Interfaces;
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
        /// Gets the action for command logging.
        /// </summary>
        Action<SimpleDbCommand> CommandLogAction { get; set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<IDbCommand> DbCommandLogAction { get; set; }

        /// <summary>
        /// Gets, sets value for simpledbcommand logging.
        /// </summary>
        bool LogCommand { get; set; }

        /// <summary>
        /// Gets, sets value for IDbCommand logging.
        /// </summary>
        bool LogDbCommand { get; set; }

        /// <summary>
        /// Gets the definitor factory.
        /// </summary>
        ISimpleDefinitorFactory DefinitorFactory
        { get; }

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
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        SimpleDbCommand BuildSimpleDbCommandForOdbcQuery(string odbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null, bool setOverratedParametersToOutput = false);

        /// <summary>
        /// Builds the simple database command for SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        SimpleDbCommand BuildSimpleDbCommandForQuery(string sqlQuery, object parameterObject, ICommandSetting commandSetting = null);

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

        /// <summary>
        /// Builds SimpleDbCommand instance for Translate of Odbc Sql Query.
        /// </summary>
        /// <param name="jdbcSqlQuery">Jdbc Sql query <see cref="string"/> 
        /// like #SELECT T1.* FROM TABLE T1 WHERE T1.INT_COLUMN = ?1 AND T2.DATE_COLUMN = ?2 #.</param>
        /// <param name="parameterValues">Sql command parameter values.</param>
        /// <param name="commandSetting">The command setting.</param>
        /// <param name="setOverratedParametersToOutput">if it is true overrated parameters set as output else will be throw error.</param>
        /// <returns>Returns simple database command object instance <see cref="SimpleDbCommand" />.</returns>
        SimpleDbCommand BuildSimpleDbCommandForJdbcQuery(string jdbcSqlQuery,
            object[] parameterValues, ICommandSetting commandSetting = null, bool setOverratedParametersToOutput = false);
    }
}