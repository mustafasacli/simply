using System;

namespace Simply.Data.Constants
{
    /// <summary>
    /// Defines the <see cref="InternalAppValues" />.
    /// </summary>
    internal static class InternalAppValues
    {
        /// <summary>
        /// Defines the NullValue.
        /// </summary>
        public static readonly object NullValue = (object)DBNull.Value;

        /// <summary>
        /// Time Format: yyyy-MM-dd-HH-mm-ss.
        /// </summary>
        internal static readonly string ErrorFileDateFormat = "yyyy-MM-dd-HH-mm-ss";

        /// <summary>
        /// Time Format: yyyy-MM-dd, HH:mm:ss ffffff.
        /// </summary>
        internal static readonly string GeneralDateFormat = "yyyy-MM-dd, HH:mm:ss ffffff";

        /// <summary>
        /// Defines the connectionName ==> "Connection";.
        /// </summary>
        internal static readonly string ConnectionName = "Connection";

        /// <summary>
        /// Defines the serverVersion ==> "ServerVersion".
        /// </summary>
        internal static readonly string ServerVersion = "ServerVersion";

        /// <summary>
        /// Defines the atChar ==> '@'.
        /// </summary>
        internal static readonly char AtChar = '@';

        /// <summary>
        /// Defines the questionChar ==> '?'.
        /// </summary>
        internal static readonly char QuestionMark = '?';

        /// <summary>
        /// Defines the colonChar ==> ':'.
        /// </summary>
        internal static readonly char ColonChar = ':';

        /// <summary>
        /// Defines the ParameterChar.
        /// </summary>
        internal static readonly char ParameterChar = 'p';

        /// <summary>
        /// Defines the OpeningSquareBracket ==> '['.
        /// </summary>
        internal static readonly char OpeningSquareBracket = '[';

        /// <summary>
        /// Defines the ClosingSquareBracket ==> ']'.
        /// </summary>
        internal static readonly char ClosingSquareBracket = ']';

        /// <summary>
        /// Defines the QuoteChar ==> '"'.
        /// </summary>
        internal static readonly char QuoteChar = '"';

        /// <summary>
        /// Defines the Backquote ==> '`'.
        /// </summary>
        internal const char Backquote = '`';

        /// <summary>
        /// Defines the Emtpy string.
        /// </summary>
        internal static readonly string Empty = string.Empty;

        /// <summary>
        /// Defines sql count format ==> SELECT COUNT(1) AS CNT FROM ( {0} )
        /// </summary>
        internal static readonly string CountFormat = "SELECT COUNT(1) AS CNT FROM ( {0} )";

        /// <summary>
        /// Defines the CommandTextSqlScriptFormaty ==> "#SQL_SCRIPT#".
        /// </summary>
        internal static readonly string SqlScriptFormat = "#SQL_SCRIPT#";

        /// <summary>
        /// Defines the CommandTextSqlScriptFormaty ==> "#SKIP#".
        /// </summary>
        internal static readonly string SkipFormat = "#SKIP#";

        /// <summary>
        /// Defines the CommandTextSqlScriptFormaty ==> "#TAKE#".
        /// </summary>
        internal static readonly string TakeFormat = "#TAKE#";

        /// <summary>
        /// char '|'.
        /// </summary>
        internal static readonly char ParameterQueryDelimiter = '|';

        /// <summary>
        /// char ';'.
        /// </summary>
        internal static readonly char ParameterPropertyDelimiter = ';';

        /// <summary>
        /// "BindByName".
        /// </summary>
        internal static readonly string OracleCommandBindByNameProperty = "BindByName";

        /// <summary>
        /// Oracle String Concat Operator ==> " || "
        /// </summary>
        internal static readonly string OracleStringConcatOperator = " || ";

        /// <summary>
        /// Ms Sql String Concat Operator ==> " + "
        /// </summary>
        internal static readonly string MsSqlStringConcatOperator = " + ";

        /// <summary>
        /// Defines " " string.
        /// </summary>
        internal static readonly string OneSpace = " ";

        /// <summary>
        /// Defines ' ' char.
        /// </summary>
        internal static readonly char SpaceChar = OneSpace[0];

        /// <summary>
        /// Defines the Backquote as String ==> '`'.
        /// </summary>
        internal static readonly string BackquoteString = "`";

        internal static readonly string ConnectionParameterName = "connection";

        internal static readonly string TransactionParameterName = "transaction";
    }
}