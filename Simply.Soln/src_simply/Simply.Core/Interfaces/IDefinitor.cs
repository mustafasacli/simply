using System;
using System.Reflection;

namespace Coddie.Core.Interfaces
{
    /// <summary>
    /// The definitor.
    /// </summary>
    public interface IDefinitor
    {
        /// <summary>
        /// Gets the table name.
        /// </summary>
        /// <returns>A string.</returns>
        string GetTableName<T>(T instance) where T : class;

        /// <summary>
        /// Gets the real Type of instance.
        /// </summary>
        /// <returns>Type objec instance.</returns>
        Type GetRealType<T>(T instance) where T : class;

        /// <summary>
        /// Gets Table Name Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The table name of type.</returns>
        string GetTableNameOfType(Type type);

        /// <summary>
        /// Gets Schema Name Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The schema name of type.</returns>
        string GetSchemaNameOfType(Type type);

        /// <summary>
        /// Get the column attribute value of property, the column attribute no exist return
        /// property name.
        /// </summary>
        /// <param name="propertyInfo">Property info</param>
        /// <returns>Returns Column name of property.</returns>
        string GetColumnNameOfProperty(PropertyInfo propertyInfo);

        /// <summary>
        /// Get Properties which has Key attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <param name="includeIdentityPropertiesHasNoKey">includes identity properties when properties has no key property.</param>
        /// <returns>.</returns>
        PropertyInfo[] GetKeyProperties(PropertyInfo[] properties, bool includeIdentityPropertiesHasNoKey = false);

        /// <summary>
        /// Get Properties which has Identity attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns>.</returns>
        PropertyInfo[] GetIdentityProperties(PropertyInfo[] properties);

        /// <summary>
        /// Get Properties which has Identity attribute as array.
        /// </summary>
        /// <param name="type">The type <see cref="Type"/>.</param>
        /// <returns>.</returns>
        PropertyInfo[] GetIdentityProperties(Type type);

        /// <summary>
        /// Gets Valid Properties Of Type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="includeNotMappedProperties">
        /// if true NotMapped Properties are included, else not.
        /// </param>
        /// <param name="includeReadonlyProperties"></param>
        /// <returns>An array of property İnformation.</returns>
        PropertyInfo[] GetValidPropertiesOfType(Type type, bool includeNotMappedProperties = false, bool includeReadonlyProperties = false, bool includeComputedProperties = false);

        /// <summary>
        /// checks type is SimpleType.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        bool IsSimpleType(Type type);
    }
}