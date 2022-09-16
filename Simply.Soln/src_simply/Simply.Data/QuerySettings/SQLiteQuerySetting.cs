using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class SQLiteQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public SQLiteQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = MsSqlStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT * FROM (#SQL_SCRIPT#) LIMIT #TAKE# OFFSET #SKIP# ";

            this.LastFormat = "SELECT  * FROM ( SELECT  *, ROW_NUMBER() OVER() ROWNUM1 FROM ( #SQL_SCRIPT# ) ORDER BY ROWNUM1 DESC ) LIMIT 1 OFFSET 0";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}