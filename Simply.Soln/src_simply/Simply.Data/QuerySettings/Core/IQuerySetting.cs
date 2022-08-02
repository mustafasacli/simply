using Simply.Data.Enums;

namespace Simply.Data.Interfaces
{
    /// <summary>
    /// Represents Parameter Prefix, Column-Table Prefix And Suffix, Last Record Sql Part, Skip and take part.
    /// </summary>
    public interface IQuerySetting
    {
        /// <summary>
        /// Gets Parameter Name Prefix.
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// Gets Table and Column Name Prefix.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets Table and Column Name Suffix.
        /// </summary>
        string Suffix { get; }

        /// <summary>
        /// Gets the string concat operation.
        /// </summary>
        string StringConcatOperation { get; }

        /// <summary>
        /// Gets the Skip and take format.
        /// </summary>
        string SkipAndTakeFormat { get; }

        /// <summary>
        /// Last Record Sql Format
        /// </summary>
        string LastFormat { get; }

        /// <summary>
        /// Gets Db Connection Type.
        /// </summary>
        DbConnectionTypes ConnectionType { get; }

        /// <summary>
        /// Gets Parameter Suffix.
        /// </summary>
        string ParameterSuffix { get; }

        /// <summary>
        /// Gets the string substring format.
        /// </summary>
        string SubstringFormat { get; }

        /// <summary>
        /// Gets the string count format.
        /// </summary>
        string CountFormat { get; }
    }
}