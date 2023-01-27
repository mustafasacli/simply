using Simply.Common;
using Simply.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Simply.Definitor.Attribute
{
    /// <summary>
    ///
    /// </summary>
    public class AttributeDefinitor<T> : ISimpleDefinitor<T> where T : class
    {
        /// <summary>
        /// Defines the NumericTypes.
        /// </summary>
        private readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(decimal), typeof(long),
            typeof(short),   typeof(sbyte),  typeof(byte),
            typeof(ulong),   typeof(ushort), typeof(uint),
            typeof(float),  typeof(double)
        };

        /// <summary>
        /// Get the column name value of property, the column attribute no exist return
        /// property name.
        /// </summary>
        /// <param name="propertyInfo">Property info</param>
        /// <returns>Returns Column name of property.</returns>
        public string GetColumnNameOfProperty(PropertyInfo propertyInfo)
        {
            ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>(inherit: true);
            string result = columnAttribute?.Name;

            if (result.IsNullOrSpace())
                result = propertyInfo.Name;

            result = result.Trim();
            return result;
        }

        /// <summary>
        /// Gets the key properties.
        /// </summary>
        /// <returns>An array of PropertyInfos.</returns>
        public PropertyInfo[] GetKeyProperties()
        {
            return
            GetValidProperties()
                .Where(q => q.GetCustomAttribute<KeyAttribute>(inherit: true) != null)
                .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Gets the identity properties.
        /// </summary>
        /// <returns>An array of PropertyInfos.</returns>
        public PropertyInfo[] GetIdentityProperties()
        {
            return
            GetValidProperties()
                .Where(q => q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true)?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Properties the to db type.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>Type as a DbType.</returns>
        public DbType? PropertyToDbType(PropertyInfo propertyInfo)
        {
            DbType? dbType = ToDbType(propertyInfo.PropertyType);
            return dbType;
        }

        /// <summary>
        /// convert type to nullable DbType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>Type as a DbType.</returns>
        public DbType? ToDbType(Type type)
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
        /// Tos the db type.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A DbType? .</returns>
        public DbType? ToDbType(object obj)
        {
            DbType? dbt = null;

            if (obj.IsNullOrDbNull())
                return dbt;

            dbt = obj.GetType().ToDbType();

            return dbt;
        }

        /// <summary>
        /// Is the simple type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
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
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]));
        }

        /// <summary>
        /// Gets the same properties.
        /// </summary>
        /// <returns>A list of string.</returns>
        public List<string> GetSameProperties<T2>() where T2 : class
        {
            Dictionary<string, Type> dictionary = new Dictionary<string, Type>();

            typeof(T).GetProperties()
                .Where(q => q.CanRead && q.CanWrite)
                .ToList()
                .ForEach(
                q =>
                {
                    dictionary.Add(q.Name, Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType);
                });

            List<string> list = typeof(T2)
                .GetProperties()
                .Where(q => dictionary.ContainsKey(q.Name) && q.CanRead && q.CanWrite)
                .Where(q => (Nullable.GetUnderlyingType(q.PropertyType) ?? q.PropertyType) == dictionary[q.Name])
                .Select(q => q.Name)
                .ToList() ?? ArrayHelper.EmptyList<string>();

            return list;
        }

        /// <summary>
        /// Gets Valid Properties Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="includeNotMappedProperties"></param>
        /// <param name="includeReadonlyProperties"></param>
        /// <param name="includeComputedProperties"></param>
        /// <returns>An array of property information.</returns>
        public PropertyInfo[] GetValidProperties(bool includeNotMappedProperties = false,
            bool includeReadonlyProperties = false, bool includeComputedProperties = false)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            IEnumerable<PropertyInfo> propertyList = properties.Where(p => (p.CanWrite || includeReadonlyProperties) && p.CanRead && IsSimpleType(p.PropertyType));
            propertyList = propertyList.Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null || includeNotMappedProperties);
            propertyList = propertyList.Where(p => !(p.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly).GetValueOrDefault(false) || includeReadonlyProperties);
            propertyList = propertyList.Where(q => (q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true)?.DatabaseGeneratedOption)
                .GetValueOrDefault(DatabaseGeneratedOption.None) != DatabaseGeneratedOption.Computed || includeComputedProperties);

            properties = propertyList.ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return properties;
        }

        /// <summary>
        /// Gets the property columns.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>A IDictionary.</returns>
        public IDictionary<string, string> GetPropertyColumns(PropertyInfo[] properties)
        {
            IDictionary<string, string> propertyColumns = new Dictionary<string, string>();

            if (!(properties?.Any() ?? false))
                return propertyColumns;

            properties.ToList()
                .ForEach(q =>
                {
                    propertyColumns[q.Name] = GetColumnNameOfProperty(q);
                });

            return propertyColumns;
        }

        /// <summary>
        /// Gets the numeric types.
        /// </summary>
        /// <returns>A HashSet.</returns>
        public HashSet<Type> GetNumericTypes()
        { return NumericTypes; }

        /// <summary>
        /// Is the numeric.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
        public bool IsNumeric(Type type)
        {
            return NumericTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);
        }

        /// <summary>
        /// Get Column Name from Property Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property column of type.</returns>
        public string GetPropertyColumnOfType(Type type, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo == null)
            { throw new Exception("Property Name with given name could not be found."); }

            string columnName = GetColumnNameOfProperty(propertyInfo);
            return columnName;
        }

        /// <summary>
        /// Get Column Name from Property Of Type.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property column of type.</returns>
        public string GetPropertyColumn(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName);

            if (propertyInfo == null)
            { throw new Exception("Property Name with given name could not be found."); }

            string columnName = GetColumnNameOfProperty(propertyInfo);
            return columnName;
        }

        /// <summary>
        /// Gets Table Name.
        /// </summary>
        /// <returns>The table name of type.</returns>
        public string GetTableName()
        {
            TableAttribute tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            string tableName = tableAttribute?.Name;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = typeof(T).Name;
            }

            return tableName;
        }

        /// <summary>
        /// Gets Schema Name Of Type.
        /// </summary>
        /// <returns>The schema name of type.</returns>
        public string GetSchemaName()
        {
            TableAttribute tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            string schemaName = tableAttribute?.Schema ?? string.Empty;
            return schemaName;
        }

        /// <summary>
        /// Checks the ıf anonymous type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
        /*
        public bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }
        */
        /// <summary>
        /// Gets the column name.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A string.</returns>
        public string GetColumnName<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            string propertyName = keySelector.GetMemberName();
            string columnName = GetPropertyColumn(propertyName);
            return columnName;
        }
    }
}