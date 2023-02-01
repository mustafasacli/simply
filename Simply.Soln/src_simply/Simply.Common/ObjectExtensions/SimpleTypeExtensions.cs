using System;
using System.Collections.Generic;
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
    public static class SimpleTypeExtensions
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
                typeof(string),
                typeof(decimal),
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
        /// Gets same Properties of two types.
        /// </summary>
        /// <param name="type1">First type.</param>
        /// <param name="type2">Second type.</param>
        /// <returns>returns string list.</returns>
        public static List<string> GetSameProperties(this Type type1, Type type2)
        {
            Dictionary<string, Type> dictionary = new Dictionary<string, Type>();

            type1.GetProperties()
                .GetReadWriteProperties()
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