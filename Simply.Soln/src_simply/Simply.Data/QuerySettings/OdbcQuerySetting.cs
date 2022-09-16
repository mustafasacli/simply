using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class OdbcQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OdbcQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public OdbcQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = QuestionMark.ToString();
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = Empty;
            this.SkipAndTakeFormat = Empty;
            this.LastFormat = Empty;
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}