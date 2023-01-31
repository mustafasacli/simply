using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="GenericExtensions"/>.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Given instance is member of array returns true else reutrns false.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <param name="instancesArray">A variable-length parameters list containing array.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsMember<T>(this T instance, T[] instancesArray)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            bool result = false;

            foreach (T item in instancesArray)
            {
                result = instance.Equals(item);

                if (result)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets Valid Properties of object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>An array of property information.</returns>
        public static PropertyInfo[] GetValidProperties<T>(this T instance) where T : class
        {
            PropertyInfo[] props = instance.GetRealType().GetValidPropertiesOfType();
            return props;
        }

        /// <summary>
        /// Gets Property Values in a dictionary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <param name="includeNotMappedProperties">.</param>
        /// <returns>The properties.</returns>
        public static IDictionary<string, object> GetPropertyValues<T>(this T instance, bool includeNotMappedProperties = false) where T : class
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            PropertyInfo[] properties = typeof(T).GetValidPropertiesOfType(
                includeNotMappedProperties: includeNotMappedProperties, includeReadonlyProperties: true);

            foreach (PropertyInfo property in properties)
            {
                dict.Add(property.Name, property.GetValue(instance));
            }

            return dict;
        }

        /// <summary>
        /// Gets given property values as object array.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="instance">.</param>
        /// <param name="propertyNames">.</param>
        /// <returns>.</returns>
        public static object[] GetPropertyValues<T>(this T instance, List<string> propertyNames)
        {
            object[] values = new object[0];

            if (propertyNames == null || instance == null || propertyNames.Count == 0)
                return values;

            values =
            instance.GetType()
                .GetValidPropertiesOfType()
                .Where(q => propertyNames.Contains(q.Name))
                .Select(q => q.GetValue(instance, null))
                .ToArray() ?? new object[0];

            return values;
        }

        /// <summary>
        /// Get Column Name-Property Name as dictionary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>The columns reverse.</returns>
        [Obsolete("Method moved to ISimpleDefinitor interface and AttributeDefinitor class.")]
        public static IDictionary<string, string> GetColumnsReverse<T>(this T instance) where T : class
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            PropertyInfo[] properties = instance.GetRealType().GetProperties();

            properties = properties.Where(p => p.CanWrite && p.CanRead)
                                    .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                                    .Where(p => p.PropertyType.IsSimpleTypeV2()).ToArray();

            foreach (PropertyInfo property in properties)
            {
                dict.Add(property.GetColumnNameOfProperty(), property.Name);
            }

            return dict;
        }

        /// <summary>
        /// Get Property Name-Column Name as dictionary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>The columns.</returns>
        [Obsolete("Method moved to ISimpleDefinitor interface and AttributeDefinitor class.")]
        public static IDictionary<string, string> GetColumns<T>(this T instance) where T : class
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            PropertyInfo[] properties = instance.GetRealType().GetProperties();

            properties = properties.Where(p => p.CanWrite && p.CanRead)
                                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                                .Where(p => TypeExtensions.IsSimpleTypeV2(p.PropertyType)).ToArray();

            foreach (PropertyInfo property in properties)
            {
                dictionary.Add(property.Name, property.GetColumnNameOfProperty());
            }

            return dictionary;
        }

        /// <summary>
        /// Gets First Key Property Name, if object has no key property and isFirstPropKey value is
        /// true returns first property name as key property.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <param name="isFirstPropertyKey">
        /// (Optional) True if is first property key, false if not.
        /// </param>
        /// <returns>The key.</returns>
        [Obsolete("Method is deprecated.")]
        public static string GetKey<T>(this T instance, bool isFirstPropertyKey = false) where T : class
        {
            string key = string.Empty;

            PropertyInfo[] properties = instance.GetRealType().GetProperties();

            properties = properties.Where(p => p.CanWrite && p.CanRead)
                                    .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                                    .Where(p => TypeExtensions.IsSimpleTypeV2(p.PropertyType))
                                    .ToArray() ?? new PropertyInfo[0];
            PropertyInfo[] keyProperties = properties
                .Where(p => (p.GetCustomAttributes(typeof(KeyAttribute), true) ?? new object[] { }).Length > 0)
                .ToArray();

            if (keyProperties.Length > 0)
            {
                key = keyProperties[0].Name;
            }
            else
            {
                if (isFirstPropertyKey)
                    key = properties[0].Name;
            }

            return key;
        }

        /// <summary>
        /// Gets table name.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>The table name.</returns>
        [Obsolete("Method is deprecated.")]
        public static string GetTableName<T>(this T instance) where T : class
        {
            string tableName = instance.GetRealType().GetTableNameOfType();
            return tableName;
        }

        /// <summary>
        /// Gets schema name.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>The schema name.</returns>
        [Obsolete("Method is deprecated.")]
        public static string GetSchemaName<T>(this T instance) where T : class
        {
            string schemaName = instance.GetRealType().GetSchemaNameOfType();
            return schemaName;
        }

        /// <summary>
        /// if instance is null or default returns false else returns true.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="instance">Generic Type instance.</param>
        /// <returns>if instance is null or default returns false else returns true.</returns>
        public static bool IsNotNullOrDefault<T>(this T instance)
        {
            bool result = instance != null && !object.Equals(instance, default(T));
            return result;
        }

        /// <summary>
        /// Gets property value.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="instance">.</param>
        /// <param name="propertyName">.</param>
        /// <returns>.</returns>
        public static object GetPropertyValue<T>(this T instance, string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property == null) 
                throw new Exception($"{propertyName} property is not belong to class.");

            object value = property?.GetValue(instance, null);

            return value;
        }

        /// <summary>
        /// Sets property value.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="instance">.</param>
        /// <param name="propertyName">.</param>
        /// <param name="value">.</param>
        public static bool SetPropertyValue<T>(this T instance, string propertyName, object value)
        {
            bool result = false;

            if (propertyName.IsNullOrSpace()) return result;

            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(instance, value);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Copies object with property values. if input object is null, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceInstance">T object instance</param>
        /// <returns>returns new T object instance.</returns>
        public static T CopyObject<T>(this T sourceInstance) where T : class, new()
        {
            T instance = null;
            if (sourceInstance == null) return instance;

            instance = Activator.CreateInstance<T>();

            PropertyInfo[] properties = typeof(T).GetProperties().GetReadWriteProperties();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(sourceInstance, null);
                property.SetValue(instance, value, null);
            }

            return instance;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static string GetMemberName<T, P>(this Expression<Func<T, P>> action) where T : class
        {
            MemberExpression expression = (MemberExpression)action.Body;
            string name = expression.Member.Name;

            return name;
        }

        /// <summary>
        /// Gets Type of T object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Type GetRealType<T>(this T instance)
        {
            Type realType = (instance?.GetType() ?? typeof(T));
            return realType;
        }
    }
}