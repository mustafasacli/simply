using Simply.Data.Objects;
using System;
using System.Data;

namespace Simply.Data.Interfaces
{
    /// <summary>
    /// Logging Settings for Database Operations(command logging and error logging).
    /// </summary>
    public interface ILogSetting
    {
        /// <summary>
        /// Gets a value indicating whether logs the command.
        /// </summary>
        bool LogCommand { get; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<SimpleDbCommand> CommandLogAction { get; }

        /// <summary>
        /// Gets a value indicating whether logs the command.
        /// </summary>
        bool LogDbCommand { get; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        Action<IDbCommand> DbCommandLogAction { get; }

        /// <summary>
        /// Sets the log command.
        /// </summary>
        /// <param name="logCommand">If true, log command.</param>
        /// <returns>A ILogSetting.</returns>
        ILogSetting SetLogCommand(bool logCommand);

        /// <summary>
        /// Sets the command log action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A ILogSetting.</returns>
        ILogSetting SetCommandLogAction(Action<SimpleDbCommand> action);

        /// <summary>
        /// Sets the log command.
        /// </summary>
        /// <param name="logDbCommand">If true, log dbcommand.</param>
        /// <returns>A ILogSetting.</returns>
        ILogSetting SetLogDbCommand(bool logDbCommand);

        /// <summary>
        /// Sets the command log action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A ILogSetting.</returns>
        ILogSetting SetDbCommandLogAction(Action<IDbCommand> action);
    }
}