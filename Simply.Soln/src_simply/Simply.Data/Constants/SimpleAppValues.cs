using System;

namespace Coddie.Data.Constants
{
    /// <summary>
    /// Defines the <see cref="SimpleAppValues" />.
    /// </summary>
    internal static class SimpleAppValues
    {
        /// <summary>
        /// General Command Timeout value..
        /// </summary>
        internal static readonly int CommandTimeout = 1000000;

        /// <summary>
        /// means DbNull.Value..
        /// </summary>
        internal static readonly object NullValue = (object)DBNull.Value;

        /// <summary>
        /// Time Format: yyyy-MM-dd-HH-mm-ss.
        /// </summary>
        internal static readonly string ErrorFileDateFormat = "yyyy-MM-dd-HH-mm-ss";

        /// <summary>
        /// Time Format: yyyy-MM-dd, HH:mm:ss ffffff.
        /// </summary>
        internal static readonly string GeneralDateFormat = "yyyy-MM-dd, HH:mm:ss ffffff";

        /// <summary>
        /// Defines the ServerVersionProperty.
        /// </summary>
        internal static readonly string ServerVersionProperty = "ServerVersion";

        /// <summary>
        /// char '|'.
        /// </summary>
        internal static readonly char ParameterQueryDelimiter = '|';

        /// <summary>
        /// char ';'.
        /// </summary>
        internal static readonly char ParameterPropertyDelimiter = ';';
    }
}