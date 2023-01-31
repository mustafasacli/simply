using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Simply.Common.Interfaces
{
    /// <summary>
    /// Property-Column, Table-Class Mapping definitor interface.
    /// </summary>
    public interface ISimpleDefinitor<T> where T : class
    {
        /// <summary>
        /// Get the column name value of property, the column attribute no exist return
        /// property name.
        /// </summary>
        /// <param name="propertyInfo">Property info</param>
        /// <returns>Returns Column name of property.</returns>
        string GetColumnNameOfProperty(PropertyInfo propertyInfo);

        /// <summary>
        /// Gets the key properties.
        /// </summary>
        /// <returns>An array of PropertyInfos.</returns>
        PropertyInfo[] GetKeyProperties();

        /// <summary>
        /// Gets the identity properties.
        /// </summary>
        /// <returns>An array of PropertyInfos.</returns>
        PropertyInfo[] GetIdentityProperties();

        /// <summary>
        /// Properties the to db type.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>Type as a DbType.</returns>
        DbType? PropertyToDbType(PropertyInfo propertyInfo);

        /// <summary>
        /// convert type to nullable DbType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>Type as a DbType.</returns>
        DbType? ToDbType(Type type);

        /// <summary>
        /// object to nullable DbType.
        /// </summary>
        /// <param name="obj">.</param>
        /// <returns>.</returns>
        DbType? ToDbType(object obj);

        /// <summary>
        /// Is the simple type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
        bool IsSimpleType(Type type);

        /// <summary>
        /// Gets the same properties.
        /// </summary>
        /// <returns>A list of string.</returns>
        List<string> GetSameProperties<T2>() where T2 : class;

        /// <summary>
        /// Gets Valid Properties Of Type.
        /// </summary>
        /// <param name="includeNotMappedProperties"></param>
        /// <param name="includeReadonlyProperties"></param>
        /// <param name="includeComputedProperties"></param>
        /// <returns>An array of property information.</returns>
        PropertyInfo[] GetValidProperties(bool includeNotMappedProperties = false,
            bool includeReadonlyProperties = false, bool includeComputedProperties = false);

        /// <summary>
        /// Gets the property columns.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>A IDictionary.</returns>
        IDictionary<string, string> GetPropertyColumns(PropertyInfo[] properties);

        /// <summary>
        /// Gets the numeric types.
        /// </summary>
        /// <returns>A HashSet.</returns>
        HashSet<Type> GetNumericTypes();

        /// <summary>
        /// Is the numeric.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
        bool IsNumeric(Type type);

        /// <summary>
        /// Get Column Name from Property Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property column of type.</returns>
        string GetPropertyColumnOfType(Type type, string propertyName);

        /// <summary>
        /// Gets Table Name.
        /// </summary>
        /// <returns>The table name of type.</returns>
        string GetTableName();

        /// <summary>
        /// Gets Schema Name Of Type.
        /// </summary>
        /// <returns>The schema name of type.</returns>
        string GetSchemaName();

        /// <summary>
        /// Get Column Name from Property Of Type.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property column of type.</returns>
        string GetPropertyColumn(string propertyName);

        /// <summary>
        /// Gets the column name.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A string.</returns>
        string GetColumnName<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// Get Column Name-Property Name as dictionary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The columns reverse.</returns>
        IDictionary<string, string> GetColumnsReverse();
    }
}