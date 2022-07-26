using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Command Definition.
    /// </summary>
    [DataContract]
    public class DbCommandDefinition : BaseDbCommand
    {
        /// <summary>
        /// Creates new DbCommandDefinition instance.
        /// </summary>
        public DbCommandDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandDefinition"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="oracleCommandBindByName">If true, oracle command bind by name.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public DbCommandDefinition(
            string commandText, CommandType? commandType = null,
            bool oracleCommandBindByName = false, int? commandTimeout = null)
        {
            CommandText = commandText;
            CommandType = commandType;
            OracleCommandBindByName = oracleCommandBindByName;
            CommandTimeout = commandTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandDefinition"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="oracleCommandBindByName">If true, oracle command bind by name.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public DbCommandDefinition(
            string commandText, IEnumerable<object> commandParameters, CommandType? commandType = null,
            bool oracleCommandBindByName = false, int? commandTimeout = null)
            : this(commandText, commandType, oracleCommandBindByName, commandTimeout)
        {
            if (commandParameters != null)
            { AddCommandParameters(commandParameters); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandDefinition"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="oracleCommandBindByName">If true, oracle command bind by name.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public DbCommandDefinition(
            string commandText, IEnumerable<DbCommandParameter> commandParameters, CommandType? commandType = null,
            bool oracleCommandBindByName = false, int? commandTimeout = null)
            : this(commandText, commandType, oracleCommandBindByName, commandTimeout)
        {
            if (commandParameters != null)
            { AddCommandParameters(commandParameters); }
        }

        /// <summary>
        /// Gets or sets the CommandType.
        /// </summary>
        [DataMember]
        public CommandType? CommandType
        { get; set; }

        /// <summary>
        /// Gets or sets the CommandTimeout
        /// Command Timeout.
        /// </summary>
        [DataMember]
        public int? CommandTimeout
        { get; set; }

        /// <summary>
        /// Adds command parameter to list.
        /// </summary>
        /// <param name="dbCommandParameter"></param>
        public void AddParameter(DbCommandParameter dbCommandParameter)
        {
            AddCommandParameter(dbCommandParameter);
        }

        /// <summary>
        /// Adds command parameter to list.
        /// </summary>
        /// <param name="dbCommandParameter"></param>
        public DbCommandDefinition AddParameterAndReturn(DbCommandParameter dbCommandParameter)
        {
            AddCommandParameter(dbCommandParameter);
            return this;
        }

        /// <summary>
        /// Adds command parameters to list.
        /// </summary>
        /// <param name="dbCommandParameters">Command Parameters</param>
        public void AddParameters(IEnumerable<DbCommandParameter> dbCommandParameters)
        {
            AddCommandParameters(dbCommandParameters);
        }

        /// <summary>
        /// clears parameters.
        /// </summary>
        public void ClearParameters()
        {
            commandParameters?.Clear();
        }

        /// <summary>
        /// Gets Sets OracleCommand BindByName value.
        /// </summary>
        [DataMember]
        public bool OracleCommandBindByName
        { get; set; }

        /// <summary>
        /// Auto Opens DbConnection before db operations.
        /// </summary>
        [DataMember]
        public bool AutoOpen
        { get; set; }

        /// <summary>
        /// Closes Connection after db operation.
        /// </summary>
        [DataMember]
        public bool CloseAtFinal
        { get; set; }

        /// <summary>
        /// Adds command parameter to list.
        /// </summary>
        /// <param name="dbCommandParameter">db command parameter</param>
        /// <returns>Returns DbCommandDefinition instance.</returns>
        public DbCommandDefinition AddDatabaseParameterReturn(object dbCommandParameter)
        {
            AddCommandParameter(dbCommandParameter);
            return this;
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>A DbParameter.</returns>
        public DbParameter CreateParameter()
        {
            return new DbCommandParameter();
        }
    }
}