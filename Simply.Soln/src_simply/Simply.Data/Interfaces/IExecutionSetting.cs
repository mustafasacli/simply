using System.Data;

namespace Simply.Data.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IExecutionSetting
    {
        /// <summary>
        /// Gets auto open. if it is true connection will be opened before operation.
        /// </summary>
        bool? AutoOpen { get; }

        /// <summary>
        /// Gets close at final. if it is true connection will be closed after operation.
        /// </summary>
        bool? CloseAtFinal { get; }

        /// <summary>
        /// Gets Command Type.
        /// </summary>
        CommandType? CommandType { get; }
        
        /// <summary>
        /// Gets Command Timeout.
        /// </summary>
        int? CommandTimeout { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoOpen">connection auto open setting.</param>
        /// <returns>Returns object instance</returns>
        IExecutionSetting SetAutoOpen(bool?  autoOpen = null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="closeAtFinal">connection auto close setting.</param>
        /// <returns>Returns object instance</returns>
        IExecutionSetting SetCloseAtFinal(bool? closeAtFinal = null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandType">command type.</param>
        /// <returns>Returns object instance</returns>
        IExecutionSetting SetCommandType(CommandType? commandType = null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandTimeout">command timeout.</param>
        /// <returns>Returns object instance</returns>
        IExecutionSetting SetCommandTimeout(int? commandTimeout = null);
    }
}