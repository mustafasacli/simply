using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="TypeExtensions"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc) or complex (i.e.
        /// custom class with public properties and methods). source code: https://gist.github.com/jonathanconway/3330614.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsSimpleType(this Type type)
        {
            return
                type.IsValueType ||
                type.IsPrimitive ||
                new Type[] {
                typeof(String),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
        }

        /// <summary>
        /// checks type is SimpleType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsSimpleTypeV2(this Type type)
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
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleTypeV2(type.GetGenericArguments()[0]))
                ;
        }

        /// <summary>
        /// convert type to nullable DbType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>Type as a DbType.</returns>
        public static DbType? ToDbType(this Type type)
        {
            DbType? dbType = null;
            if (type == null) return dbType;

            Type realType = Nullable.GetUnderlyingType(type) ?? type;

            if (realType.IsEnum)
            {
                dbType = DbType.Int16;
                return dbType;
            }

            if (realType == typeof(bool))
            {
                dbType = DbType.Boolean;
                return dbType;
            }

            if (realType == typeof(byte[]))
            {
                dbType = DbType.Binary;
                return dbType;
            }

            if (realType == typeof(byte))
            {
                dbType = DbType.Byte;
                return dbType;
            }

            if (realType == typeof(sbyte))
            {
                dbType = DbType.SByte;
                return dbType;
            }

            if (realType == typeof(DateTime))
            {
                dbType = DbType.DateTime;
                return dbType;
            }

            if (realType == typeof(DateTimeOffset))
            {
                dbType = DbType.DateTimeOffset;
                return dbType;
            }

            if (realType == typeof(TimeSpan))
            {
                dbType = DbType.Time;
                return dbType;
            }

            if (realType == typeof(decimal))
            {
                dbType = DbType.Decimal;
                return dbType;
            }

            if (realType == typeof(double))
            {
                dbType = DbType.Double;
                return dbType;
            }

            if (realType == typeof(float))
            {
                dbType = DbType.Single;
                return dbType;
            }

            if (realType == typeof(Guid))
            {
                dbType = DbType.Guid;
                return dbType;
            }

            if (realType == typeof(short))
            {
                dbType = DbType.Int16;
                return dbType;
            }

            if (realType == typeof(int))
            {
                dbType = DbType.Int32;
                return dbType;
            }

            if (realType == typeof(long))
            {
                dbType = DbType.Int64;
                return dbType;
            }

            if (realType == typeof(string))
            {
                dbType = DbType.String;
                return dbType;
            }

            if (realType == typeof(ushort))
            {
                dbType = DbType.UInt16;
                return dbType;
            }

            if (realType == typeof(uint))
            {
                dbType = DbType.UInt32;
                return dbType;
            }

            if (realType == typeof(ulong))
            {
                dbType = DbType.UInt64;
                return dbType;
            }

            if (realType == typeof(XmlDocument) || realType == typeof(XmlNode))
            {
                dbType = DbType.Xml;
                return dbType;
            }

            if (realType == typeof(object))
            {
                dbType = DbType.Object;
                return dbType;
            }

            return dbType;
        }

        /// <summary>
        /// object to nullable DbType.
        /// </summary>
        /// <param name="obj">.</param>
        /// <returns>.</returns>
        public static DbType? ToDbType(this object obj)
        {
            DbType? dbt = null;

            if (obj.IsNullOrDbNull())
                return dbt;

            dbt = obj.GetType().ToDbType();

            return dbt;
        }

        /// <summary>
        /// Gets column names of type as reverse.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The columns of type as reverse.</returns>
        public static IDictionary<string, string> GetColumnsOfTypeAsReverse(this Type type)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            PropertyInfo[] properties = type.GetValidPropertiesOfType();

            foreach (PropertyInfo property in properties)
            {
                dictionary.Add(property.GetColumnNameOfProperty(), property.Name);
            }

            return dictionary;
        }

        /// <summary>
        /// Gets key property name of type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The key of type.</returns>
        public static string GetKeyOfType(this Type type)
        {
            string keyPropertyName = string.Empty;
            PropertyInfo propertyInfo = type
                .GetValidPropertiesOfType()
                .FirstOrDefault(q => q.GetCustomAttribute<KeyAttribute>(inherit: true) != null);

            if (propertyInfo != null)
            {
                keyPropertyName = propertyInfo.Name;
            }

            return keyPropertyName;
        }

        /// <summary>
        /// Gets identity property name of type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The ıdentity property of type.</returns>
        public static string GetIdentityPropertyOfType(this Type type)
        {
            string result = string.Empty;

            PropertyInfo propertyInfo = type
                .GetValidPropertiesOfType()
                .FirstOrDefault(q => q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true) != null);

            if (propertyInfo != null)
            {
                bool isMember = propertyInfo
                    .GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true)
                    .DatabaseGeneratedOption
                    .IsMember(new DatabaseGeneratedOption[] { DatabaseGeneratedOption.Computed, DatabaseGeneratedOption.Identity });

                if (isMember)
                    result = propertyInfo.Name;
            }

            return result;
        }

        /// <summary>
        /// Gets Identity Properties.
        /// </summary>
        /// <param name="type">.</param>
        /// <returns>.</returns>
        public static PropertyInfo[] GetIdentityPropertyList(this Type type)
        {
            PropertyInfo[] propertyInfos = type
                .GetValidPropertiesOfType()
                .GetIdentityProperties();

            return propertyInfos;
        }

        /// <summary>
        /// Gets column name of key property for given type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The key column of type.</returns>
        public static string GetKeyColumnOfType(this Type type)
        {
            string keyColumnName = string.Empty;
            string keyPropertyName = type.GetKeyOfType();

            if (!string.IsNullOrWhiteSpace(keyPropertyName))
            {
                PropertyInfo keyProperty = type.GetProperty(keyPropertyName);
                if (keyProperty != null)
                    keyColumnName = keyProperty.GetColumnNameOfProperty();
            }

            return keyColumnName;
        }

        /// <summary>
        /// Get Column Name from Property Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property column of type.</returns>
        public static string GetPropertyColumnOfType(this Type type, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo == null)
            { throw new Exception("Property Name with given name could not be found."); }

            string columnName = propertyInfo.GetColumnNameOfProperty();

            return columnName;
        }

        /// <summary>
        /// Gets Table Name Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The table name of type.</returns>
        public static string GetTableNameOfType(this Type type)
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
        /// Gets Schema Name Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The schema name of type.</returns>
        public static string GetSchemaNameOfType(this Type type)
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
        /// Get Property Name-Column Name as dictionary with given Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="includeNotMappedProperties">
        /// if true NotMapped Properties are included, else not.
        /// </param>
        /// <returns>The columns of type.</returns>
        public static IDictionary<string, string> GetColumnsOfType(this Type type, bool includeNotMappedProperties = false)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            PropertyInfo[] properties = type.GetValidPropertiesOfType(includeNotMappedProperties: includeNotMappedProperties);

            foreach (PropertyInfo property in properties)
            {
                dictionary.Add(property.Name, property.GetColumnNameOfProperty());
            }

            return dictionary;
        }

        /// <summary>
        /// Get Column Name-Property Name as dictionary with given Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The columns reverse of type.</returns>
        public static IDictionary<string, string> GetPropertyColumnsAsReverse(this Type type)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            PropertyInfo[] properties = type.GetValidPropertiesOfType();

            foreach (PropertyInfo property in properties)
            {
                dictionary.Add(property.GetColumnNameOfProperty(), property.Name);
            }

            return dictionary;
        }

        /// <summary>
        /// Key Column is Numeric returns true else returns false.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsKeyColumnNumeric(this Type type)
        {
            bool isIdColumnNumeric = false;

            string keyPropertyName = type.GetKeyOfType();

            if (!string.IsNullOrWhiteSpace(keyPropertyName))
            {
                isIdColumnNumeric =
                    TypeHelper.IsNumeric(type.GetProperty(keyPropertyName).PropertyType);
            }

            return isIdColumnNumeric;
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
        public static PropertyInfo[] GetValidPropertiesOfType(this Type type, bool includeNotMappedProperties = false, bool includeReadonlyProperties = false)
        {
            PropertyInfo[] properties = type.GetProperties();

            properties = properties
                .Where(p => (p.CanWrite || includeReadonlyProperties) && p.CanRead && IsSimpleTypeV2(p.PropertyType))
                .Where(p => p.GetCustomAttribute<ReadOnlyAttribute>() == null || includeReadonlyProperties)
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null || includeNotMappedProperties)
                .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return properties;
        }

        /// <summary>
        /// Gets Valid Properties Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="includeNotMappedProperties"></param>
        /// <param name="includeReadonlyProperties"></param>
        /// <param name="includeComputedProperties"></param>
        /// <returns>An array of property İnformation.</returns>
        public static PropertyInfo[] GetValidPropertiesOfTypeV2(this Type type, bool includeNotMappedProperties = false, bool includeReadonlyProperties = false
            , bool includeComputedProperties = false)
        {
            PropertyInfo[] properties = type.GetProperties();

            IEnumerable<PropertyInfo> propertyList = properties.Where(p => (p.CanWrite || includeReadonlyProperties) && p.CanRead && IsSimpleTypeV2(p.PropertyType));
            propertyList = propertyList.Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null || includeNotMappedProperties);
            propertyList = propertyList.Where(p => !(p.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly).GetValueOrDefault(false) || includeReadonlyProperties);
            propertyList = propertyList.Where(q => (q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true)?.DatabaseGeneratedOption)
                .GetValueOrDefault(DatabaseGeneratedOption.None) != DatabaseGeneratedOption.Computed || includeComputedProperties);

            properties = propertyList.ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return properties;
        }

        /// <summary>
        /// Gets Property Types for given Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The property types of type.</returns>
        public static IDictionary<string, Type> GetPropertyTypesOfType(this Type type)
        {
            IDictionary<string, Type> types = new Dictionary<string, Type>();
            PropertyInfo[] properties = type.GetValidPropertiesOfType();

            properties.ToList().ForEach(p => types[p.Name] = p.PropertyType);

            return types;
        }

        /// <summary>
        /// Gets same Properties of two types.
        /// </summary>
        /// <param name="type1">First type.</param>
        /// <param name="type2">Second type.</param>
        /// <returns>returns string list.</returns>
        public static List<string> GetSameProperties(this Type type1, Type type2)
        {
            Dictionary<string, Type> dictionary = new Dictionary<string, Type>();

            type1.GetProperties()
                .Where(q => q.CanRead && q.CanWrite)
                .ToList()
                .ForEach(
                q =>
                {
                    dictionary.Add(q.Name, Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType);
                });

            List<string> list = type2
                .GetProperties()
                .Where(q => dictionary.ContainsKey(q.Name) && q.CanRead && q.CanWrite)
                .Where(q => (Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType) == dictionary[q.Name])
                .Select(q => q.Name)
                .ToList() ?? ArrayHelper.EmptyList<string>();

            return list;
        }

        /// <summary>
        /// check type is anonymous.
        /// </summary>
        /// <param name="type">.</param>
        /// <returns>.</returns>
        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        /// <summary>
        /// check type is anonymous.
        /// </summary>
        /// <param name="type">.</param>
        /// <returns>.</returns>
        public static bool CheckIfAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        /// <summary>
        /// Get Key Property Names for Type.
        /// </summary>
        /// <param name="type">.</param>
        /// <returns>.</returns>
        public static string[] GetKeysOfType(this Type type)
        {
            string[] propertyInfoNames = type
                .GetValidPropertiesOfType()
                .Where(q => q.GetCustomAttribute<KeyAttribute>(inherit: true) != null)
                .Select(q => q.Name)
                .ToArray() ?? ArrayHelper.Empty<string>();

            return propertyInfoNames;
        }

        /// <summary>
        /// Get Key Property Names for given Type, if includeIdentityProperties is true includes
        /// identity properties.
        /// </summary>
        /// <param name="type">.</param>
        /// <param name="includeIdentityProperties">.</param>
        /// <returns>Returns property names as string List.</returns>
        public static List<string> GetKeyPropertyNamesOfType(this Type type, bool includeIdentityProperties = false)
        {
            PropertyInfo[] validProperties = type.GetValidPropertiesOfType();
            List<string> keyPropertyNames = validProperties.GetKeyPropertyNames().ToList();

            if (includeIdentityProperties)
            {
                IEnumerable<string> identityPropertyNames = validProperties.GetIdentityPropertyNames();
                keyPropertyNames.AddRange(identityPropertyNames);
                keyPropertyNames = keyPropertyNames.Distinct().ToList();
            }

            return keyPropertyNames;
        }

        /// <summary>
        /// Gets Identity Property Names Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The ıdentity property of type.</returns>
        public static List<string> GetIdentityPropertyNamesOfType(this Type type)
        {
            List<string> identityPropertyNames = type.GetValidPropertiesOfType()
                .GetIdentityPropertyNames()?.ToList() ?? ArrayHelper.EmptyList<string>();

            return identityPropertyNames;
        }

        /// <summary>
        /// Get Properties which has Identity attribute as array.
        /// </summary>
        /// <param name="type">The type <see cref="Type"/>.</param>
        /// <returns>.</returns>
        public static PropertyInfo[] GetIdentityProperties(this Type type)
        {
            PropertyInfo[] identityProperties =
                type.GetValidPropertiesOfTypeV2()?.GetIdentityProperties()
                ?? ArrayHelper.Empty<PropertyInfo>();

            return identityProperties;
        }
    }

    /// <summary>
    /// Defines the <see cref="TypeHelper"/>.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Defines the NumericTypes.
        /// </summary>
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(decimal), typeof(long),
            typeof(short),   typeof(sbyte),  typeof(byte),
            typeof(ulong),   typeof(ushort), typeof(uint),
            typeof(float),  typeof(double)
        };

        /// <summary>
        /// The IsNumeric.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsNumeric(Type type)
        {
            return NumericTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);
        }
    }
}