using Coddie.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Coddie.AttributeDefinition
{
    /// <summary>
    /// The attribute definitor.
    /// </summary>
    public class AttributeDefinitor : IDefinitor
    {
        /// <summary>
        /// Get the column attribute value of property, the column attribute no exist return
        /// property name.
        /// </summary>
        /// <param name="propertyInfo">Property info</param>
        /// <returns>Returns Column name of property.</returns>
        public string GetColumnNameOfProperty(PropertyInfo propertyInfo)
        {
            string result = string.Empty;

            if (propertyInfo != null)
            {
                ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>(inherit: true);

                result = columnAttribute?.Name;

                if (string.IsNullOrWhiteSpace(result))
                    result = propertyInfo.Name;

                result = result.Trim();
            }

            return result;
        }

        /// <summary>
        /// Get Properties which has Identity attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns>.</returns>
        public PropertyInfo[] GetIdentityProperties(PropertyInfo[] properties)
        {
            if (properties == null || properties.Length == 0)
                return ArrayHelper.Empty<PropertyInfo>();

            PropertyInfo[] generatedOptionProperties = properties
                    .Where(q => q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true) != null)
                    .Where(q =>
                    q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true).DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                  .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return generatedOptionProperties;
        }

        /// <summary>
        /// Gets the identity properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An array of PropertyInfos.</returns>
        public PropertyInfo[] GetIdentityProperties(Type type)
        {
            PropertyInfo[] tempProperties = GetValidPropertiesOfType(type);
            PropertyInfo[] identityProperties = GetIdentityProperties(tempProperties);
            return identityProperties;
        }

        /// <summary>
        /// Gets the key properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="includeIdentityPropertiesHasNoKey">If true, includes identity properties has no key.</param>
        /// <returns>An array of PropertyInfos.</returns>
        public PropertyInfo[] GetKeyProperties(PropertyInfo[] properties, bool includeIdentityPropertiesHasNoKey = false)
        {
            if (properties == null || properties.Length == 0)
                return ArrayHelper.Empty<PropertyInfo>();

            PropertyInfo[] keyProperties = properties
                    .Where(q => q.GetCustomAttribute<KeyAttribute>(inherit: true) != null)
                    .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            if (keyProperties.Length == 0 && includeIdentityPropertiesHasNoKey)
            {
                keyProperties = GetIdentityProperties(properties);
            }

            return keyProperties;
        }

        /// <summary>
        /// Gets the real Type of instance.
        /// </summary>
        /// <returns>Type objec instance.</returns>
        public Type GetRealType<T>(T instance) where T : class
        {
            Type realType = (instance?.GetType() ?? typeof(T));
            return realType;
        }

        /// <summary>
        /// Gets Schema Name Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The schema name of type.</returns>
        public string GetSchemaNameOfType(Type type)
        {
            string schemaName = string.Empty;

            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                schemaName = tableAttribute.Schema ?? string.Empty;
            }

            schemaName = schemaName.Trim();

            return schemaName;
        }

        /// <summary>
        /// Gets table name.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>The table name.</returns>
        public string GetTableName<T>(T instance) where T : class
        {
            string tableName = GetTableNameOfType(GetRealType(instance));
            return tableName;
        }

        /// <summary>
        /// Gets Table Name Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The table name of type.</returns>
        public string GetTableNameOfType(Type type)
        {
            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
            string tableName = tableAttribute?.Name;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = type.Name;
            }

            tableName = tableName.Trim();

            return tableName;
        }

        /// <summary>
        /// Gets Valid Properties Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="includeNotMappedProperties">
        /// if true NotMapped Properties are included, else not.
        /// </param>
        /// <param name="includeReadonlyProperties"></param>
        /// <returns>An array of property İnformation.</returns>
        public PropertyInfo[] GetValidPropertiesOfType(Type type, bool includeNotMappedProperties = false,
            bool includeReadonlyProperties = false, bool includeComputedProperties = false)
        {
            PropertyInfo[] properties = type.GetProperties();

            IEnumerable<PropertyInfo> propertyList = properties.Where(p => (p.CanWrite || includeReadonlyProperties) && p.CanRead && IsSimpleType(p.PropertyType));
            propertyList = propertyList.Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null || includeNotMappedProperties);
            propertyList = propertyList.Where(p => !(p.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly).GetValueOrDefault(false) || includeReadonlyProperties);
            propertyList = propertyList.Where(q => (q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true)?.DatabaseGeneratedOption)
                .GetValueOrDefault(DatabaseGeneratedOption.None) != DatabaseGeneratedOption.Computed || includeComputedProperties);

            properties = propertyList.ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return properties;
        }

        /// <summary>
        /// checks type is SimpleType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new Type[] {
            typeof(byte[]),
            typeof(Enum),
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(XmlDocument),
            typeof(XmlNode),
            typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }
    }

    /// <summary>
    /// Defines the <see cref="ArrayHelper" />.
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// Creates empty array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> Returns empty array.</returns>
        public static T[] Empty<T>() where T : class
        { return new T[0]; }

        /// <summary>
        /// Creates empty list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns empty list.</returns>
        public static List<T> EmptyList<T>() where T : class
        { return new List<T>(); }

        /// <summary>
        /// Array is null or empty returns true, else returns false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this T[] array) where T : class
        {
            bool isEmpty = array == null || array.Length < 1;
            return isEmpty;
        }

        /// <summary>
        /// Creates empty array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> Returns empty array.</returns>
        public static T[] EmptyEnum<T>() where T : Enum
        { return new T[0]; }

        /// <summary>
        /// Checks the array is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">T instance array</param>
        /// <returns>if array is null or empty returns true else return false.</returns>
        public static bool IsNullOrEmpty<T>(T[] array) where T : class
        {
            bool isEmpty = array == null || array.Length < 1;
            return isEmpty;
        }

        /// <summary>
        /// Checks the list is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">T instance array</param>
        /// <returns>if list is null or empty returns true else return false.</returns>
        public static bool IsNullOrEmpty<T>(List<T> list) where T : class
        {
            bool isEmpty = list == null || list.Count < 1;
            return isEmpty;
        }
    }
}