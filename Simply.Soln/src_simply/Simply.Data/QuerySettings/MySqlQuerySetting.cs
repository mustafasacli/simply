using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class MySqlQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public MySqlQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = Empty;
            this.Prefix = BackquoteString;
            this.Suffix = BackquoteString;
            this.StringConcatOperation = "CONCAT( #params# )";
            this.SkipAndTakeFormat = "SELECT EXT1.* FROM (#SQL_SCRIPT#) AS EXT1 LIMIT #SKIP#, #TAKE#";
            this.LastFormat = "SELECT EXT2.* FROM (SELECT EXT1.*, ROW_NUMBER() OVER() AS RN1 FROM ( #SQL_SCRIPT# ) AS EXT1) AS EXT2 ORDER BY EXT2.RN1 DESC LIMIT 0, 1 ";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(*) FROM ( #SQL_SCRIPT# ) AS CNT";
        }
    }
}