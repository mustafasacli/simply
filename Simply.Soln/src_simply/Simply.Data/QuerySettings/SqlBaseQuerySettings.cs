using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class SqlBaseQuerySettings : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBaseQuerySettings"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public SqlBaseQuerySettings(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = QuestionMark.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = Empty;
            this.SkipAndTakeFormat = "SELECT * FROM (#SQL_SCRIPT#) OFFSET #SKIP# ROWS FETCH NEXT #TAKE# ROWS ONLY";

            this.LastFormat = "SELECT * FROM ( SELECT * FROM ( SELECT ROW_NUMBER() OVER () ROWNUM,* FROM (#SQL_SCRIPT#) ) ORDER BY ROWNUM DESC ) OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}