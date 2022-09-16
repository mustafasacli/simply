using Simply.Data.Enums;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class SqlCEQuerySetting : BaseQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCEQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public SqlCEQuerySetting(DbConnectionTypes connectionType) : base(connectionType)
        {
            this.ConnectionType = connectionType;
            this.ParameterPrefix = AtChar.ToString();
            this.ParameterSuffix = ParameterPrefix;
            this.Prefix = OpeningSquareBracket.ToString();
            this.Suffix = ClosingSquareBracket.ToString();
            this.StringConcatOperation = MsSqlStringConcatOperator;
            this.SkipAndTakeFormat = "SELECT * FROM (#SQL_SCRIPT#) OFFSET #SKIP# ROWS FETCH NEXT #TAKE# ROWS ONLY";

            this.LastFormat = "SELECT * FROM ( SELECT * FROM ( SELECT ROW_NUMBER() OVER () ROWNUM,* FROM (#SQL_SCRIPT#) ) ORDER BY ROWNUM DESC ) OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
            this.SubstringFormat = "SUBSTRING(#0#, #1#, #2#)";
            this.CountFormat = "SELECT COUNT(1) AS CNT FROM ( #SQL_SCRIPT# )";
        }
    }
}