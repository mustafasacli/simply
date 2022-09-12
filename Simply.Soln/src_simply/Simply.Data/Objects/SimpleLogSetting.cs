using Simply.Data.Interfaces;
using System;
using System.Data;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Logging Settings for Database Operations(command logging and error logging).
    /// </summary>
    public class SimpleLogSetting : ILogSetting
    {
        /// <summary>
        /// Default instance
        /// </summary>
        private SimpleLogSetting()
        { }

        /// <summary>
        /// Create instance for ILogSetting with given parameters.
        /// </summary>
        /// <param name="logCommand">If true, log command.</param>
        /// <param name="logCommandAction">The log command action.</param>
        /// <param name="logDbCommand">If true, log idbcommand object.</param>
        /// <param name="dbCommandLogAction">The log idbcommand action.</param>
        /// <returns>Returns ILogSetting object instance.</returns>
        public static ILogSetting Create(bool logCommand = false,
            Action<SimpleDbCommand> logCommandAction = null,
            bool logDbCommand = false, Action<IDbCommand> dbCommandLogAction = null)
        {
            return new SimpleLogSetting()
            {
                LogCommand = logCommand,
                CommandLogAction = logCommandAction,
                LogDbCommand = logDbCommand,
                DbCommandLogAction = dbCommandLogAction
            };
        }

        /// <summary>
        /// Create empty instance for ILogSetting.
        /// </summary>
        /// <returns>Returns ILogSetting object instance.</returns>
        public static ILogSetting New()
        {
            return new SimpleLogSetting();
        }

        /// <summary>
        /// Gets a value indicating whether logs the command.
        /// </summary>
        public bool LogCommand
        { get; protected set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<SimpleDbCommand> CommandLogAction
        { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether logs the command.
        /// </summary>
        public bool LogDbCommand
        { get; protected set; }

        /// <summary>
        /// Gets the action for command logging.
        /// </summary>
        public Action<IDbCommand> DbCommandLogAction
        { get; protected set; }

        /// <summary>
        /// Sets the log command.
        /// </summary>
        /// <param name="logCommand">If true, log command.</param>
        /// <returns>A ILogSetting.</returns>
        public ILogSetting SetLogCommand(bool logCommand)
        {
            LogCommand = logCommand;
            return this;
        }

        /// <summary>
        /// Sets the command log action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A ILogSetting.</returns>
        public ILogSetting SetCommandLogAction(Action<SimpleDbCommand> action)
        {
            CommandLogAction = action;
            LogCommand = true;
            return this;
        }

        /// <summary>
        /// Sets the log command.
        /// </summary>
        /// <param name="logDbCommand">If true, log dbcommand.</param>
        /// <returns>A ILogSetting.</returns>
        public ILogSetting SetLogDbCommand(bool logDbCommand)
        {
            LogDbCommand = logDbCommand;
            return this;
        }

        /// <summary>
        /// Sets the command log action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A ILogSetting.</returns>
        public ILogSetting SetDbCommandLogAction(Action<IDbCommand> action)
        {
            DbCommandLogAction = action;
            LogDbCommand = true;
            return this;
        }
    }
}