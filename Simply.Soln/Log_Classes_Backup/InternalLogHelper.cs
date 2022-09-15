using Simply.Data.Constants;
using Simply.Data.Interfaces;
using Simply.Data.Objects;
using System;
using System.Data;

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

            if (logSetting.CommandLogAction == null)
                throw new Exception(DbAppMessages.CommandLogActionNotDefined);

            if (logSetting.LogCommand)
                logSetting.CommandLogAction(simpleDbCommand);
        }

        /// <summary>
        /// Logs the database command.
        /// </summary>
        /// <param name="dbCommand">The database command.</param>
        /// <param name="logSetting">The log setting.</param>
        /// <exception cref="System.Exception"></exception>
        public static void LogDbCommand(IDbCommand dbCommand, ILogSetting logSetting = null)
        {
            if (logSetting == null || dbCommand == null)
                return;

            if (logSetting.DbCommandLogAction == null)
                throw new Exception(DbAppMessages.CommandLogActionNotDefined);

            if (logSetting.LogCommand)
                logSetting.DbCommandLogAction(dbCommand);
        }
    }
}