using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class DB2QuerySettings : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DB2QuerySettings"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public DB2QuerySettings(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;

            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = "CONCAT";

            // this.SkipAndTakeFormat = "SELECT * FROM ( SELECT a.* , rownumber() over () AS rn FROM ( #SQL_SCRIPT# ) AS a) AS rs WHERE rs.rn between ( #SKIP# + 1) AND ( #SKIP# + #TAKE# )";
            this.SkipAndTakeFormat = "SELECT RS.* FROM ( SELECT A.* , ROWNUMBER() OVER () AS RN FROM ( #SQL_SCRIPT# ) AS A) AS RS WHERE RS.RN between ( #SKIP# + 1) AND ( #SKIP# + #TAKE# )";

            // this.LastFormat = "SELECT * FROM ( SELECT *, rownumber() over () AS rn2 FROM ( SELECT a.* , rownumber() over () AS rn FROM ( #SQL_SCRIPT# ) AS a) AS rs order by rs.rn desc) WHERE rn2 =< 1";
            this.LastFormat = "SELECT * FROM ( SELECT RS.*, ROWNUMBER() OVER () AS RN2 FROM ( SELECT A.* , ROWNUMBER() OVER () AS RN FROM ( #SQL_SCRIPT# ) AS A) AS RS ORDER BY RS.RN DESC) WHERE RN2 =< 1";
            this.SubstringFormat = "SUBSTR(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}