using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Simply.Ef
{
    /// <summary>
    ///
    /// </summary>
    public static class InternalExtension
    {
        /// <summary>
        /// Gets the table name of type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A string.</returns>
        public static string GetTableNameOfType(this Type type)
        {
            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
            string tableName = tableAttribute?.Name;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = type.Name;
            }

            return tableName;
        }

        /// <summary>
        /// Gets the schema name of type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A string.</returns>
        public static string GetSchemaNameOfType(this Type type)
        {
            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
            string schemaName = tableAttribute?.Schema ?? string.Empty;
            return schemaName;
        }
    }
}