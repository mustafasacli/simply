using Simply.Data.Enums;
using Simply.Data.Interfaces;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class DB2QuerySettings : IQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DB2QuerySettings"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public DB2QuerySettings(DbConnectionTypes connectionType)
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

        /// <summary>
        /// Gets Parameter Name Prefix.
        /// </summary>
        public string ParameterPrefix
        { get; private set; }

        /// <summary>
        /// Gets Table and Column Name Prefix.
        /// </summary>
        public string Prefix
        { get; private set; }

        /// <summary>
        /// Gets Table and Column Name Suffix.
        /// </summary>
        public string Suffix
        { get; private set; }

        /// <summary>
        /// Gets the string concat operation.
        /// </summary>
        /// <value>
        /// The string concat operation.
        /// </value>
        public string StringConcatOperation
        { get; private set; }

        /// <summary>
        /// Gets the Skip and take format.
        /// </summary>
        public string SkipAndTakeFormat
        { get; private set; }

        /// <summary>
        /// Last Record Sql Format
        /// </summary>
        public string LastFormat
        { get; private set; }

        /// <summary>
        /// Gets Db Connection Type.
        /// </summary>
        public DbConnectionTypes ConnectionType
        { get; private set; }

        /// <summary>
        /// Gets Parameter Suffix.
        /// </summary>
        public string ParameterSuffix
        { get; private set; }

        /// <summary>
        /// Gets the string substring format.
        /// </summary>
        public string SubstringFormat
        { get; private set; }

        /// <summary>
        /// Gets the string count format.
        /// </summary>
        public string CountFormat
        { get; private set; }
    }
}