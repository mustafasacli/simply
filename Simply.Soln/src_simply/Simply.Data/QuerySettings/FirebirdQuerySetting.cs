using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class FirebirdQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public FirebirdQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterSuffix = ParameterPrefix;
            this.ParameterPrefix = AtChar.ToString();
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = OracleStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT FIRST #TAKE# SKIP #SKIP# * FROM ( #SQL_SCRIPT# )";

            this.LastFormat = "SELECT FIRST 1 SKIP 0 * FROM ( SELECT * FROM ( SELECT ROW_NUMBER() OVER () ROWNUM, * FROM (#SQL_SCRIPT#) ) ORDER BY ROWNUM DESC )";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}