using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class NuodbQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuodbQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public NuodbQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = QuestionMark.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = MsSqlStringConcatOperator;
            this.SkipAndTakeFormat = Empty;
            this.LastFormat = Empty;
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
        }
    }
}