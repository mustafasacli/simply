using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Base Db Command.
    /// </summary>
    [DataContract]
    public class BaseDbCommand
    {
        private static readonly object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbCommand"/> class.
        /// </summary>
        public BaseDbCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbCommand"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        public BaseDbCommand(string commandText, IEnumerable<object> parameters)
        {
            this.CommandText = commandText;
            AddCommandParameters(parameters);
        }

        /// <summary>
        /// Gets or sets the CommandText.
        /// </summary>
        [DataMember]
        public string CommandText
        { get; set; }

        /// <summary>
        /// Gets, sets command text is odbc query. if it is true database command will be recompile.
        /// as query, else not.
        /// </summary>
        [DataMember]
        public bool IsOdbc
        { get; set; }

        /// <summary>
        /// command parameters.
        /// </summary>
        protected List<object> commandParameters = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbCommand"/> class.
        /// </summary>
        /// <param name="commandParameters">The command parameters.</param>
        public BaseDbCommand(List<object> commandParameters)
        {
            this.commandParameters = commandParameters;
        }

        /// <summary>
        /// Gets, sets command parameters.
        /// </summary>
        public virtual List<object> CommandParameters
        {
            get { CheckCommandParameters(); return commandParameters; }
            protected set { commandParameters = value; }
        }

        /// <summary>
        /// Checks the command parameters.
        /// </summary>
        protected virtual void CheckCommandParameters()
        {
            if (commandParameters == null)
            {
                lock (lockObject)
                {
                    if (commandParameters == null)
                        commandParameters = new List<object>();
                }
            }
        }

        /// <summary>
        /// Adds the command parameter.
        /// </summary>
        /// <param name="dbCommandParameter">The db command parameter.</param>
        public virtual void AddCommandParameter(object dbCommandParameter)
        {
            CheckCommandParameters();
            commandParameters.Add(dbCommandParameter);
        }

        /// <summary>
        /// Adds the command parameters.
        /// </summary>
        /// <param name="dbCommandParameters">The db command parameters.</param>
        public virtual void AddCommandParameters(IEnumerable<object> dbCommandParameters)
        {
            CheckCommandParameters();
            commandParameters.AddRange(dbCommandParameters);
        }

        /// <summary>
        /// Adds the command parameters.
        /// </summary>
        /// <param name="dbCommandParameters">The db command parameters.</param>
        public virtual void AddCommandParameters(IEnumerable<DbCommandParameter> dbCommandParameters)
        {
            CheckCommandParameters();
            commandParameters.AddRange(dbCommandParameters);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="dbParameter">db parameter</param>
        public virtual void AddDbParameter<TParameter>(TParameter dbParameter) where TParameter : DbParameter
        {
            if (dbParameter == null)
                throw new ArgumentNullException(nameof(dbParameter));

            AddCommandParameter(dbParameter);
        }
    }
}