using Simply.Data.Interfaces;
using System.Data;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Execution Settings for Database Operations.
    /// </summary>
    public class SimpleCommandSetting : ICommandSetting
    {
        /// <summary>
        /// Default instance for IExecutionSetting.
        /// </summary>
        public static readonly ICommandSetting DefaultInstance = new SimpleCommandSetting();

        /// <summary>
        /// Default instance
        /// </summary>
        private SimpleCommandSetting()
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SimpleCommandSetting"/> class from being created.
        /// </summary>
        /// <param name="closeAtFinal">If true, close at final.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">The command type.</param>
        private SimpleCommandSetting(bool? closeAtFinal, int? commandTimeout,
            CommandType commandType = System.Data.CommandType.Text)
        {
            CloseAtFinal = closeAtFinal;
            CommandTimeout = commandTimeout;
            CommandType = commandType;
        }

        /// <summary>
        /// Default instance for IExecutionSetting with given parameters.
        /// </summary>
        /// <param name="closeAtFinal">If true, close at final.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">The command type.</param>
        /// <returns>Returns IExecutionSetting object instance.</returns>
        public static ICommandSetting Create(bool? closeAtFinal = null,
            int? commandTimeout = null, CommandType commandType = System.Data.CommandType.Text)
        {
            return new SimpleCommandSetting(closeAtFinal, commandTimeout, commandType);
        }

        /// <summary>
        /// Gets auto open. if it is true connection will be opened before operation.
        /// </summary>
        public bool? AutoOpen
        { get; private set; }

        /// <summary>
        /// Gets close at final. if it is true connection will be closed after operation.
        /// </summary>
        public bool? CloseAtFinal
        { get; private set; }

        /// <summary>
        /// Gets Command Type.
        /// </summary>
        public CommandType CommandType
        { get; private set; }

        /// <summary>
        /// Gets Command Timeout.
        /// </summary>
        public int? CommandTimeout
        { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="closeAtFinal">connection auto close setting.</param>
        /// <returns>Returns object instance</returns>
        public ICommandSetting SetCloseAtFinal(bool? closeAtFinal = null)
        {
            CloseAtFinal = closeAtFinal;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandType">command type.</param>
        /// <returns>Returns object instance</returns>
        public ICommandSetting SetCommandType(CommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandTimeout">command timeout.</param>
        /// <returns>Returns object instance</returns>
        public ICommandSetting SetCommandTimeout(int? commandTimeout = null)
        {
            CommandTimeout = commandTimeout;
            return this;
        }
    }
}