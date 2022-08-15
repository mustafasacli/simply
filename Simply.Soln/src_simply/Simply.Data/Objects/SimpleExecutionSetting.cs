using Simply.Data.Interfaces;
using System.Data;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Execution Settings for Database Operations.
    /// </summary>
    public class SimpleExecutionSetting : IExecutionSetting
    {
        /// <summary>
        /// Default instance for IExecutionSetting.
        /// </summary>
        public static readonly IExecutionSetting DefaultInstance = new SimpleExecutionSetting();

        /// <summary>
        /// Default instance
        /// </summary>
        private SimpleExecutionSetting()
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SimpleExecutionSetting"/> class from being created.
        /// </summary>
        /// <param name="autoOpen">If true, auto open.</param>
        /// <param name="closeAtFinal">If true, close at final.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">The command type.</param>
        private SimpleExecutionSetting(
            bool? autoOpen, bool? closeAtFinal,
            int? commandTimeout, CommandType? commandType)
        {
            AutoOpen = autoOpen;
            CloseAtFinal = closeAtFinal;
            CommandTimeout = commandTimeout;
            CommandType = commandType;
        }

        /// <summary>
        /// Default instance for IExecutionSetting with given parameters.
        /// </summary>
        /// <param name="autoOpen">If true, auto open.</param>
        /// <param name="closeAtFinal">If true, close at final.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">The command type.</param>
        /// <returns>Returns IExecutionSetting object instance.</returns>
        public static IExecutionSetting Create(bool? autoOpen = null, bool? closeAtFinal = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return new SimpleExecutionSetting(autoOpen, closeAtFinal, commandTimeout, commandType);
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
        public CommandType? CommandType
        { get; private set; }

        /// <summary>
        /// Gets Command Timeout.
        /// </summary>
        public int? CommandTimeout
        { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoOpen">connection auto open setting.</param>
        /// <returns>Returns object instance</returns>
        public IExecutionSetting SetAutoOpen(bool? autoOpen = null)
        {
            AutoOpen = autoOpen;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="closeAtFinal">connection auto close setting.</param>
        /// <returns>Returns object instance</returns>
        public IExecutionSetting SetCloseAtFinal(bool? closeAtFinal = null)
        {
            CloseAtFinal = closeAtFinal;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandType">command type.</param>
        /// <returns>Returns object instance</returns>
        public IExecutionSetting SetCommandType(CommandType? commandType = null)
        {
            CommandType = commandType;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandTimeout">command timeout.</param>
        /// <returns>Returns object instance</returns>
        public IExecutionSetting SetCommandTimeout(int? commandTimeout = null)
        {
            CommandTimeout = commandTimeout;
            return this;
        }
    }
}