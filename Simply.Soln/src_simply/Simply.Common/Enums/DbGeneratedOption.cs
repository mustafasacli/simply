namespace Simply.Common.Enums
{
    /// <summary>
    /// Represents the pattern used to generate values for a property in the database.
    /// </summary>
    public enum DbGeneratedOption : byte
    {
        /// <summary>
        /// The database does not generate values.
        /// </summary>
        None = 0,

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        Identity = 1,

        /// <summary>
        /// The database generates a value when a row is inserted or updated.
        /// </summary>
        Computed = 2
    }
}