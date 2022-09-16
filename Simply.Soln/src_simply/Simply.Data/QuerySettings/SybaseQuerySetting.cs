using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class SybaseQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SybaseQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public SybaseQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = OpeningSquareBracket.ToString();
            this.Suffix = ClosingSquareBracket.ToString();

            /**
            * https://infocenter.sybase.com/help/index.jsp?topic=/com.sybase.infocenter.dc36271.1550/html/blocks/blocks297.htm
            * You can use both the + and || (double-pipe) string operators to concatenate two or more character or binary expressions.
            */
            this.StringConcatOperation = OracleStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT TOP #TAKE# START AT #SKIP# + 1 * FROM (#SQL_SCRIPT#)";
            this.LastFormat =
                "SELECT TOP 1 START AT 1 * FROM ( SELECT * FROM ( SELECT ROW_NUMBER() OVER () ROWNUM, * FROM (#SQL_SCRIPT#) ) ORDER BY ROWNUM DESC )";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}