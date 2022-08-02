using Simply.Data.Enums;
using Simply.Data.Interfaces;
using static Simply.Data.Constants.InternalAppValues;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    internal class SqlDatabaseQuerySetting : IQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        public SqlDatabaseQuerySetting(DbConnectionTypes connectionType)
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