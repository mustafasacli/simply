using System.Data;

namespace Simply.Data.Interfaces
{
    /// <summary>
    /// Command Settings for execution.
    /// </summary>
    public interface ICommandSetting
    {
        /// <summary>
        /// Gets close at final. if it is true connection will be closed after operation.
        /// </summary>
        bool? CloseAtFinal { get; }

        /// <summary>
        /// Gets Command Type.
        /// </summary>
        CommandType CommandType { get; }

        /// <summary>
        /// Gets Command Timeout.
        /// </summary>
        int? CommandTimeout { get; }

        /// <summary>
        /// Gets Parameter Name Prefix for Rebuild Query.
        /// </summary>
        char? ParameterNamePrefix { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="closeAtFinal">connection auto close setting.</param>
        /// <returns>Returns object instance</returns>
        ICommandSetting SetCloseAtFinal(bool? closeAtFinal = null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandType">command type.</param>
        /// <returns>Returns object instance</returns>
        ICommandSetting SetCommandType(CommandType commandType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandTimeout">command timeout.</param>
        /// <returns>Returns object instance</returns>
        ICommandSetting SetCommandTimeout(int? commandTimeout = null);

        /// <summary>
        /// Sets the parameter name prefix.
        /// </summary>
        /// <param name="parameterNamePrefix">The parameter name prefix.</param>
        /// <returns>A ICommandSetting.</returns>
        ICommandSetting SetParameterNamePrefix(char? parameterNamePrefix = null);
    }
}