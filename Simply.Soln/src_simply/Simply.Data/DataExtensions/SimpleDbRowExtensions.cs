using Simply.Common;
using Simply.Common.Interfaces;
using Simply.Common.Objects;
using Simply.Data.Interfaces;
using Simply.Definitor.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="SimpleDbRowExtensions"/>.
    /// </summary>
    public static class SimpleDbRowExtensions
    {
        /// <summary>
        /// Converts the rows to list.
        /// </summary>
        /// <param name="database">simple database object</param>
        /// <param name="rows">The rows.</param>
        /// <returns>A list of TS.</returns>
        public static List<T> ConvertRowsToList<T>(this ISimpleDatabase database, List<SimpleDbRow> rows) where T : class
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            List<T> list = new List<T>();

            if (rows.Count < 1)
                return list;

            ISimpleDefinitor<T> definitor = database.DefinitorFactory?.GetDefinitor<T>() ?? AttributeDefinitor<T>.New();
            // TODO :
            IDictionary<string, string> columns = definitor.GetColumns();
            PropertyInfo[] properties = definitor.GetValidProperties(includeNotMappedProperties: true, includeComputedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray() ?? new PropertyInfo[0];

            if (!properties.Any())
                throw new Exception("Any Property has found for mapping.");

            foreach (SimpleDbRow row in rows)
            {
                if (row.CellCount < 1)
                    continue;

                T instance = Activator.CreateInstance<T>();

                SetPropertyValuesFromRow(ref instance, properties, row, columns);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// A List&lt;SimpleDbRow&gt; extension method that converts a dynList to a list.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">simple database object </param>
        /// <param name="rows">The dynList to act on.</param>
        /// <param name="columnPropertyMap">Column-Property Map</param>
        /// <returns>The given data converted to a list.</returns>
        public static List<T> ConvertRowsToList<T>(this ISimpleDatabase database, List<SimpleDbRow> rows, Dictionary<string, string> columnPropertyMap) where T : class
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            if (columnPropertyMap == null)
                throw new ArgumentNullException(nameof(columnPropertyMap));

            List<T> list = new List<T>();

            if (rows.Count < 1)
                return list;

            ISimpleDefinitor<T> definitor = database.DefinitorFactory?.GetDefinitor<T>() ?? AttributeDefinitor<T>.New();
            PropertyInfo[] properties = definitor.GetValidProperties(includeNotMappedProperties: true, includeComputedProperties: true); // type.GetValidPropertiesOfType(includeNotMappedProperties: true);
            properties = properties
                .Where(q => columnPropertyMap.Keys.Contains(q.Name))
                .ToArray() ?? new PropertyInfo[0];

            if (properties.Length < 1)
                throw new Exception("Any Property has found for mapping.");

            T instance;

            foreach (SimpleDbRow row in rows)
            {
                if (row.CellCount < 1)
                    continue;

                instance = Activator.CreateInstance<T>();

                SetPropertyValuesFromRow(ref instance, properties, row, columnPropertyMap);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// Sets the property values.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="row">The row.</param>
        /// <param name="columnPropertyMappings">The column property mappings.</param>
        internal static void SetPropertyValuesFromRow<T>(ref T instance, PropertyInfo[] properties,
            SimpleDbRow row, IDictionary<string, string> columnPropertyMappings)
        {
            bool hasErrorThrowned = false;
            StringBuilder errorMessageBuilder = new StringBuilder();

            properties = properties ?? new PropertyInfo[0];

            foreach (PropertyInfo property in properties)
            {
                string columnName = columnPropertyMappings[property.Name];
                if (row.ContainsCellName(columnName))
                {
                    object value = row.GetValue(columnName);
                    try
                    {
                        property.SetValue(instance, value);
                    }
                    catch (Exception e)
                    {
                        hasErrorThrowned = true;
                        errorMessageBuilder.Append("Exception-Message: ").AppendLine(e.Message);
                        if (value.IsNullOrDbNull())
                        { errorMessageBuilder.AppendLine("Value is null"); }
                        else
                        {
                            errorMessageBuilder.Append("Value: ");
                            errorMessageBuilder.AppendLine(value.ToStr());

                            errorMessageBuilder.Append("Value Type: ");
                            errorMessageBuilder.AppendLine((Nullable.GetUnderlyingType(value?.GetType()) ?? value?.GetType())?.Name ?? "");
                        }

                        errorMessageBuilder.Append("Property Name: ");
                        errorMessageBuilder.AppendLine(property.Name);
                        errorMessageBuilder.Append("Property Type: ");
                        errorMessageBuilder.AppendLine((Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType).Name);
                        errorMessageBuilder.AppendLine("****************************************");
                    }
                }
            }

            if (hasErrorThrowned)
            {
                string message = errorMessageBuilder.ToString();
                if (message.IsValid())
                    throw new Exception(message);
            }
        }

        /// <summary>
        /// Convert dynamic object to new T object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="database">simple database object </param>
        /// <param name="row">The dyn to act on.</param>
        /// <returns>to converted.</returns>
        public static T ConvertRowTo<T>(this ISimpleDatabase database, SimpleDbRow row) where T : class
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            T instance = null;

            if (row.CellCount < 1)
                return instance;

            IDictionary<string, string> columns = new Dictionary<string, string>();

            ISimpleDefinitor<T> definitor = database.DefinitorFactory?.GetDefinitor<T>() ?? AttributeDefinitor<T>.New();
            IDictionary<string, string> tempColumns = definitor.GetColumns();
            tempColumns.Where(q => row.ContainsCellName(q.Value))
                .ToList()
                .ForEach(q =>
                {
                    columns[q.Key] = q.Value;
                });

            instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = definitor.GetValidProperties(includeNotMappedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray();

            SetPropertyValuesFromRow(ref instance, properties, row, columns);

            return instance;
        }

        /// <summary>
        /// Converts the rows to list.
        /// </summary>
        /// <param name="database">simple database object</param>
        /// <param name="rows">The rows.</param>
        /// <returns>A list of TS.</returns>
        public static List<T> ConvertRowsToList<T>(this List<SimpleDbRow> rows) where T : class
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            List<T> list = new List<T>();

            if (rows.Count < 1)
                return list;

            ISimpleDefinitor<T> definitor = AttributeDefinitor<T>.New();
            // TODO :
            IDictionary<string, string> columns = definitor.GetColumns();
            PropertyInfo[] properties = definitor.GetValidProperties(includeNotMappedProperties: true, includeComputedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray() ?? new PropertyInfo[0];

            if (!properties.Any())
                throw new Exception("Any Property has found for mapping.");

            foreach (SimpleDbRow row in rows)
            {
                if (row.CellCount < 1)
                    continue;

                T instance = Activator.CreateInstance<T>();

                SetPropertyValuesFromRow(ref instance, properties, row, columns);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// Convert dynamic object to new T object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="row">The dyn to act on.</param>
        /// <returns>to converted.</returns>
        public static T ConvertRowTo<T>(this SimpleDbRow row) where T : class
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            T instance = null;

            if (row.CellCount < 1)
                return instance;

            IDictionary<string, string> columns = new Dictionary<string, string>();

            ISimpleDefinitor<T> definitor = AttributeDefinitor<T>.New();
            IDictionary<string, string> tempColumns = definitor.GetColumns();
            tempColumns.Where(q => row.ContainsCellName(q.Value))
                .ToList()
                .ForEach(q =>
                {
                    columns[q.Key] = q.Value;
                });

            instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = definitor.GetValidProperties(includeNotMappedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray();

            SetPropertyValuesFromRow(ref instance, properties, row, columns);

            return instance;
        }
    }
}