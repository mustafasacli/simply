using Simply.Common.Objects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="DynamicExtensions"/>.
    /// </summary>
    public static class DynamicExtensions
    {
        /// <summary>
        /// Convert dynamic object to new T object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="expandoObject">The dyn to act on.</param>
        /// <returns>to converted.</returns>
        public static T ConvertToInstance<T>(this ExpandoObject expandoObject) where T : class
        {
            if (expandoObject == null)
                throw new ArgumentNullException(nameof(expandoObject));

            T instance = null;

            IDictionary<string, object> values = expandoObject;

            if ((values?.Count).GetValueOrDefault(0) < 1)
                return instance;

            IDictionary<string, string> columns = new Dictionary<string, string>();

            IDictionary<string, string> tempColumns = typeof(T).GetColumnsOfType(includeNotMappedProperties: true) ?? new Dictionary<string, string>();
            tempColumns.Where(q => values.ContainsKey(q.Value)).ToList().ForEach(q => columns[q.Key] = q.Value);

            instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = typeof(T).GetValidPropertiesOfType(includeNotMappedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray();

            SetPropertyValues(ref instance, properties, values, columns);

            return instance;
        }

        /// <summary>
        /// Convert dynamic object to new T object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dynamicObject">The dyn to act on.</param>
        /// <returns>to converted.</returns>
        public static T ConvertTo<T>(dynamic dynamicObject) where T : class
        {
            if (dynamicObject == null)
                throw new ArgumentNullException(nameof(dynamicObject));

            T instance = null;

            IDictionary<string, object> values = (dynamicObject as ExpandoObject);

            if ((values?.Count).GetValueOrDefault(0) < 1)
                return instance;

            IDictionary<string, string> columns = new Dictionary<string, string>();

            IDictionary<string, string> tempColumns = typeof(T).GetColumnsOfType(includeNotMappedProperties: true) ?? new Dictionary<string, string>();
            tempColumns.Where(q => values.ContainsKey(q.Value)).ToList().ForEach(q => columns[q.Key] = q.Value);

            instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = typeof(T).GetValidPropertiesOfType(includeNotMappedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray();

            SetPropertyValues(ref instance, properties, values, columns);

            return instance;
        }

        /// <summary>
        /// Convert dynamic object to new T object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="expandoObject">The dyn to act on.</param>
        /// <returns>The given data converted to a v 2.</returns>
        public static T ConvertToV2<T>(this ExpandoObject expandoObject) where T : class
        {
            if (expandoObject == null)
                throw new ArgumentNullException(nameof(expandoObject));

            IDictionary<string, string> columns = typeof(T).GetColumnsOfType(includeNotMappedProperties: true);

            T instance = null;

            IDictionary<string, object> values = expandoObject;

            if ((values?.Count).GetValueOrDefault(0) < 1)
                return instance;

            instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = typeof(T).GetValidPropertiesOfType(includeNotMappedProperties: true);

            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray();

            SetPropertyValues(ref instance, properties, values, columns);

            return instance;
        }

        /// <summary>
        /// A List&lt;ExpandoObject&gt; extension method that converts a dynList to a list.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="expandoObjectList">The dynList to act on.</param>
        /// <returns>The given data converted to a list.</returns>
        public static List<T> ConvertToList<T>(this List<ExpandoObject> expandoObjectList) where T : class
        {
            if (expandoObjectList == null)
                throw new ArgumentNullException(nameof(expandoObjectList));

            List<T> list = new List<T>();

            if (expandoObjectList.Count < 1)
                return list;

            Type type = typeof(T);

            IDictionary<string, string> columns = type.GetColumnsOfType(includeNotMappedProperties: true);

            PropertyInfo[] properties = type.GetValidPropertiesOfType(includeNotMappedProperties: true);
            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray() ?? new PropertyInfo[0];

            if (properties.Length < 1)
                return list;

            IDictionary<string, object> values;
            T instance;

            foreach (ExpandoObject expandoObject in expandoObjectList)
            {
                values = expandoObject;
                if ((values?.Count).GetValueOrDefault(0) < 1)
                    continue;

                instance = Activator.CreateInstance<T>();

                SetPropertyValues(ref instance, properties, values, columns);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// A List&lt;ExpandoObject&gt; extension method that converts a dynList to a list.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dynamicObjectList">The dynList to act on.</param>
        /// <returns>The given data converted to a list.</returns>
        public static List<T> ConvertToList<T>(this List<dynamic> dynamicObjectList) where T : class
        {
            if (dynamicObjectList == null)
                throw new ArgumentNullException(nameof(dynamicObjectList));

            List<T> list = new List<T>();

            if (dynamicObjectList.Count < 1)
                return list;

            Type type = typeof(T);

            IDictionary<string, string> columns = type.GetColumnsOfType(includeNotMappedProperties: true);

            PropertyInfo[] properties = type.GetValidPropertiesOfType(includeNotMappedProperties: true);
            properties = properties
                .Where(q => columns.Keys.Contains(q.Name))
                .ToArray() ?? new PropertyInfo[0];

            if (properties.Length < 1)
            { throw new Exception("Any Property has found for mapping."); }

            IDictionary<string, object> values;
            T instance;

            foreach (dynamic dynamicObject in dynamicObjectList)
            {
                values = (dynamicObject as ExpandoObject);
                if ((values?.Count).GetValueOrDefault(0) < 1)
                    continue;

                instance = Activator.CreateInstance<T>();

                SetPropertyValues(ref instance, properties, values, columns);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// A List&lt;ExpandoObject&gt; extension method that converts a dynList to a list.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dynamicObjectList">The dynList to act on.</param>
        /// <param name="columnPropertyMap">Column-Property Map</param>
        /// <returns>The given data converted to a list.</returns>
        public static List<T> ConvertToList<T>(this List<dynamic> dynamicObjectList, Dictionary<string, string> columnPropertyMap) where T : class
        {
            if (dynamicObjectList == null)
                throw new ArgumentNullException(nameof(dynamicObjectList));

            if (columnPropertyMap == null)
                throw new ArgumentNullException(nameof(columnPropertyMap));

            List<T> list = new List<T>();

            if (dynamicObjectList.Count < 1)
                return list;

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetValidPropertiesOfType(includeNotMappedProperties: true);
            properties = properties
                .Where(q => columnPropertyMap.Keys.Contains(q.Name))
                .ToArray() ?? new PropertyInfo[0];

            if (properties.Length < 1)
                throw new Exception("Any Property has found for mapping.");

            IDictionary<string, object> values;
            T instance;

            foreach (dynamic dynamicObject in dynamicObjectList)
            {
                values = (dynamicObject as ExpandoObject);
                if ((values?.Count).GetValueOrDefault(0) < 1)
                    continue;

                instance = Activator.CreateInstance<T>();

                SetPropertyValues(ref instance, properties, values, columnPropertyMap);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// Sets the property values.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="keyValues">The key values.</param>
        /// <param name="columnPropertyMappings">The column property mappings.</param>
        internal static void SetPropertyValues<T>(ref T instance, PropertyInfo[] properties,
            IDictionary<string, object> keyValues, IDictionary<string, string> columnPropertyMappings)
        {
            bool hasErrorThrowned = false;
            StringBuilder errorMessageBuilder = new StringBuilder();

            properties = properties ?? new PropertyInfo[0];

            foreach (PropertyInfo property in properties)
            {
                string columnName = columnPropertyMappings[property.Name];
                if (keyValues.ContainsKey(columnName))
                {
                    object value = keyValues[columnName].GetValueWithCheckDbNull();
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
    }
}