using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;

namespace Simply.Data.Helpers
{
    /// <summary>
    /// The internal log helper.
    /// </summary>
    public class InternalLogHelper
    {
        /// <summary>
        /// Logs the command.
        /// </summary>
        /// <param name="simpleDbCommand">The simple db command.</param>
        /// <param name="logSetting">The log setting.</param>
        public static void LogCommand(SimpleDbCommand simpleDbCommand, ILogSetting logSetting = null)
        {
            if (logSetting == null || simpleDbCommand == null)
                return;

            if (!logSetting.LogCommand && logSetting.CommandLogAction == null)
                return;

            if (logSetting.CommandLogAction == null)
                throw new Exception(DbAppMessages.CommandLogActionNotDefined);

            logSetting.CommandLogAction(simpleDbCommand);
        }
    }
}