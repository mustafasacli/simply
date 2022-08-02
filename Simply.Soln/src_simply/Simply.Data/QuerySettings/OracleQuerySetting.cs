using Simply.Data.Enums;
using Simply.Data.Interfaces;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class OracleQuerySetting : IQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public OracleQuerySetting(DbConnectionTypes connectionType)
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
        /// <value>The string concat operation.</value>
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