using Simply.Data.Enums;
using Simply.Data.Interfaces;

namespace Simply.Data
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix and last IdentityInsert Part.
    /// </summary>
    public abstract class BaseQuerySetting : IQuerySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQuerySetting"/> class.
        /// </summary>
        /// <param name="connectionType">Database Connection type.</param>
        protected BaseQuerySetting(DbConnectionTypes connectionType)
        {
            this.ConnectionType = connectionType;
        }

        /// <summary>
        /// Gets Parameter Name Prefix.
        /// </summary>
        public string ParameterPrefix
        { get; protected set; }

        /// <summary>
        /// Gets Table and Column Name Prefix.
        /// </summary>
        public string Prefix
        { get; protected set; }

        /// <summary>
        /// Gets Table and Column Name Suffix.
        /// </summary>
        public string Suffix
        { get; protected set; }

        /// <summary>
        /// Gets the string concat operation.
        /// </summary>
        /// <value>
        /// The string concat operation.
        /// </value>
        public string StringConcatOperation
        { get; protected set; }

        /// <summary>
        /// Gets the Skip and take format.
        /// </summary>
        public string SkipAndTakeFormat
        { get; protected set; }

        /// <summary>
        /// Last Record Sql Format
        /// </summary>
        public string LastFormat
        { get; protected set; }

        /// <summary>
        /// Gets Db Connection Type.
        /// </summary>
        public DbConnectionTypes ConnectionType
        { get; protected set; }

        /// <summary>
        /// Gets Parameter Suffix.
        /// </summary>
        public string ParameterSuffix
        { get; protected set; }

        /// <summary>
        /// Gets the string substring format.
        /// </summary>
        public string SubstringFormat
        { get; protected set; }

        /// <summary>
        /// Gets the string count format.
        /// </summary>
        public string CountFormat
        { get; protected set; }
    }
}