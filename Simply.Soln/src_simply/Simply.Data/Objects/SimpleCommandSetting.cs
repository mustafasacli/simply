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
        /// Default instance
        /// </summary>
        private SimpleCommandSetting()
        { }

        /// <summary>
        /// Prevents a default instance of the <see cref="SimpleCommandSetting"/> class from being created.
        /// </summary>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        private SimpleCommandSetting(int? commandTimeout,
            CommandType commandType = CommandType.Text, char? parameterNamePrefix = null)
        {
            CommandTimeout = commandTimeout;
            CommandType = commandType;
            ParameterNamePrefix = parameterNamePrefix;
        }

        /// <summary>
        /// Create instance for ICommandSetting with given parameters.
        /// </summary>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="parameterNamePrefix">Parameter Name Prefix for Rebuild Query</param>
        /// <returns>Returns ICommandSetting object instance.</returns>
        public static ICommandSetting Create(
            int? commandTimeout = null, CommandType commandType = CommandType.Text,
            char? parameterNamePrefix = null)
        {
            return new SimpleCommandSetting(commandTimeout, commandType, parameterNamePrefix);
        }

        /// <summary>
        /// Create empty instance for ICommandSetting.
        /// </summary>
        /// <returns>Returns ICommandSetting object instance.</returns>
        public static ICommandSetting New()
        {
            return new SimpleCommandSetting();
        }

        /// <summary>
        /// Gets Command Type.
        /// </summary>
        public CommandType CommandType
        { get; private set; }

        /// <summary>
        /// Gets Command Timeout. Value as second.
        /// </summary>
        public int? CommandTimeout
        { get; private set; }

        /// <summary>
        /// Gets Parameter Name Prefix for Rebuild Query.
        /// </summary>
        public char? ParameterNamePrefix
        { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandType">command type.</param>
        /// <returns>Returns ICommandSetting object instance.</returns>
        public ICommandSetting SetCommandType(CommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandTimeout">command timeout. Value as second.</param>
        /// <returns>Returns ICommandSetting object instance.</returns>
        public ICommandSetting SetCommandTimeout(int? commandTimeout = null)
        {
            CommandTimeout = commandTimeout;
            return this;
        }

        /// <summary>
        /// Sets the parameter name prefix.
        /// </summary>
        /// <param name="parameterNamePrefix">The parameter name prefix.</param>
        /// <returns>Returns ICommandSetting object instance.</returns>
        public ICommandSetting SetParameterNamePrefix(char? parameterNamePrefix = null)
        {
            ParameterNamePrefix = parameterNamePrefix;
            return this;
        }
    }
}