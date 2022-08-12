namespace Simply.Data.Constants
{
    /// <summary>
    /// Defines the <see cref="DbAppMessages" />.
    /// </summary>
    public static class DbAppMessages
    {
        /// <summary>
        /// Defines the UndefinedDbConnectionType.
        /// </summary>
        public static readonly string UndefinedDbConnectionType = "Undefined Database ConnectionType";

        /// <summary>
        /// Defines the UndefinedKey.
        /// </summary>
        public static readonly string UndefinedKey = "Key column(s) must be defined.";

        internal static readonly string OracleCommandNoBindByNameProperty = "Oracle Command has no BindByName property.";

        public static readonly string SingleRowError = "Sequence should not contain more than one element.";

        public static readonly string AdapterNotContainsDbParameterType = "DbParameter not found for given database connection.";

        public static readonly string ParameterMismatchCompiledQueryAndCommand = "Compiled query parameters did not match with command parameters. Please check query and parameters.";

        public static readonly string DataAdapterNotFound = "DataAdapter class not found in .NET Provider.";

        public static readonly string RowCountZero = "Row Count of Page cannot be less than 1.";

        public static readonly string PageNumberNegative = "Page Number cannot be less than 0.";

        public static readonly string DatatableIsNull = "DataTable object can not be null.";

        public static readonly string InvalidColumnName = "There is no column with given name in datatable.";

        public static readonly string PageNumberLessThanOne = "Page number can not be less than 1.";

        public static readonly string InvalidConnectionType = "No Query Setting found for with connection type: ";
    }
}