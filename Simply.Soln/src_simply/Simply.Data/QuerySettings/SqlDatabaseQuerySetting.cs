using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class SqlDatabaseQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public SqlDatabaseQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = Empty;
            this.Suffix = Empty;
            this.StringConcatOperation = MsSqlStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT * FROM (#SQL_SCRIPT#) LIMIT #TAKE# OFFSET #SKIP# ";

            this.LastFormat = "SELECT * FROM ( SELECT * FROM ( SELECT ROW_NUMBER() OVER () ROWNUM,* FROM (#SQL_SCRIPT#) ) ORDER BY ROWNUM DESC ) LIMIT 1 OFFSET 0";
            this.SubstringFormat = "SUBSTRING(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}