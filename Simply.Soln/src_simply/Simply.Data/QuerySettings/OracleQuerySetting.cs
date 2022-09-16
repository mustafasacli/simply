using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class OracleQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public OracleQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = ColonChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = OracleStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT * FROM ( SELECT A.*, ROWNUM RNUM FROM ( #SQL_SCRIPT# ) A WHERE ROWNUM < ( #SKIP# + #TAKE# + 1 ) ) WHERE RNUM >= ( #SKIP# + 1 )";

            this.LastFormat = "SELECT *, ROWNUM RN2 FROM ( SELECT A.*, ROWNUM RNUM FROM ( #SQL_SCRIPT# ) A ORDER BY ROWNUM DESC ) WHERE RN2 =< 1 ";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}