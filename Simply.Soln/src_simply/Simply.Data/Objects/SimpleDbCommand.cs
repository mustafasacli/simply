using Simply.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;

namespace Simply.Data.Objects
{
    /// <summary>
    /// database command.
    /// </summary>
    [DataContract]
    public class SimpleDbCommand : BaseDbCommand
    {
        /// <summary>
        /// Creates new SimpleDbCommand instance.
        /// </summary>
        public SimpleDbCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDbCommand"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="oracleCommandBindByName">If true, oracle command bind by name.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public SimpleDbCommand(
            string commandText, CommandType? commandType = null,
            bool oracleCommandBindByName = false, int? commandTimeout = null)
        {
            CommandText = commandText;
            CommandType = commandType;
            OracleCommandBindByName = oracleCommandBindByName;
            CommandTimeout = commandTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDbCommand"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="oracleCommandBindByName">If true, oracle command bind by name.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public SimpleDbCommand(
            string commandText, IEnumerable<object> commandParameters, CommandType? commandType = null,
            bool oracleCommandBindByName = false, int? commandTimeout = null)
            : this(commandText, commandType, oracleCommandBindByName, commandTimeout)
        {
            if (commandParameters != null)
            { AddCommandParameters(commandParameters); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDbCommand"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="oracleCommandBindByName">If true, oracle command bind by name.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        public SimpleDbCommand(
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
        public SimpleDbCommand AddParameterAndReturn(DbCommandParameter dbCommandParameter)
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
        [Obsolete("This property will be removed later versions.")]
        public bool OracleCommandBindByName
        { get; set; }

        /// <summary>
        /// Gets Parameter Name Prefix for Rebuild Query.
        /// </summary>
        public char? ParameterNamePrefix
        { get; set; }

        /// <summary>
        /// Adds command parameter to list.
        /// </summary>
        /// <param name="dbCommandParameter">db command parameter</param>
        /// <returns>Returns SimpleDbCommand instance.</returns>
        public SimpleDbCommand AddDatabaseParameterReturn(object dbCommandParameter)
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

        /// <summary>
        /// Recompiles the parameters.
        /// </summary>
        /// <param name="querySetting">The query setting.</param>
        /// <param name="obj">The value object.</param>
        public void RecompileQuery(IQuerySetting querySetting, object obj)
        {
            if (this.ParameterNamePrefix == null || obj == null)
                return;

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                this.CommandText = this.CommandText
                    .Replace(
                    string.Concat(this.ParameterNamePrefix, property.Name, this.ParameterNamePrefix),
                    string.Concat(querySetting.ParameterPrefix, property.Name, querySetting.ParameterSuffix)
                    );
            }
        }

        /// <summary>
        /// Recompiles the query.
        /// </summary>
        /// <param name="querySetting">The query setting.</param>
        /// <param name="parameters">The parameters.</param>
        public void RecompileQuery(IQuerySetting querySetting, List<DbCommandParameter> parameters)
        {
            if (this.ParameterNamePrefix == null || (parameters?.Count ?? 0) < 1)
                return;

            foreach (DbCommandParameter parameter in parameters)
            {
                this.CommandText = this.CommandText
                    .Replace(
                    string.Concat(this.ParameterNamePrefix, parameter.ParameterName, this.ParameterNamePrefix),
                    string.Concat(querySetting.ParameterPrefix, parameter.ParameterName, querySetting.ParameterSuffix)
                    );
            }
        }
    }
}