using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class PgSqlQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public PgSqlQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = ColonChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = QuoteChar.ToString();
            this.Suffix = QuoteChar.ToString();
            this.StringConcatOperation = OracleStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT * FROM (#SQL_SCRIPT#) LIMIT #TAKE# OFFSET #SKIP# ";

            this.LastFormat = "SELECT * FROM ( SELECT * FROM ( SELECT ROW_NUMBER() OVER () ROWNUM,* FROM (#SQL_SCRIPT#) ) ORDER BY ROWNUM DESC ) LIMIT 1 OFFSET 0";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}