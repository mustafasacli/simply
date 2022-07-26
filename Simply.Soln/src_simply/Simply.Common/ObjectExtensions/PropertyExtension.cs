using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="PropertyExtension"/>.
    /// </summary>
    public static class PropertyExtension
    {
        /// <summary>
        /// Get the column attribute value of property, the column attribute no exist return
        /// property name.
        /// </summary>
        /// <param name="propertyInfo">Property info</param>
        /// <returns>Returns Column name of property.</returns>
        public static string GetColumnNameOfProperty(this PropertyInfo propertyInfo)
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
        /// Get Properties which has Key attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <param name="includeIdentityPropertiesHasNoKey">includes identity properties when properties has no key property.</param>
        /// <returns>.</returns>
        public static PropertyInfo[] GetKeyProperties(this PropertyInfo[] properties, bool includeIdentityPropertiesHasNoKey = false)
        {
            if (ArrayHelper.IsNullOrEmpty(properties))
                return ArrayHelper.Empty<PropertyInfo>();

            PropertyInfo[] keyProperties = properties
                   .Where(q => q.GetCustomAttribute<KeyAttribute>() != null)
                   .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            if (keyProperties.Length == 0 && includeIdentityPropertiesHasNoKey)
            {
                keyProperties = properties.GetIdentityProperties();
            }

            return keyProperties;
        }

        /// <summary>
        /// Get Property names which has Key attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns>.</returns>
        public static IEnumerable<string> GetKeyPropertyNames(this PropertyInfo[] properties)
        {
            IEnumerable<string> keyPropertyNames =
                properties.GetKeyProperties().Select(q => q.Name)
                ?? ArrayHelper.Empty<string>();

            return keyPropertyNames;
        }

        /// <summary>
        /// Get Properties which has Identity attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns>.</returns>
        public static PropertyInfo[] GetIdentityProperties(this PropertyInfo[] properties)
        {
            PropertyInfo[] identityProperties =
                properties?.GetPropertiesByGeneratedOption(DatabaseGeneratedOption.Identity)
                ?? ArrayHelper.Empty<PropertyInfo>();

            return identityProperties;
        }

        /// <summary>
        /// Returns identity property names.
        /// </summary>
        /// <param name="properties">property list</param>
        /// <returns>Returns identity property names as string list.</returns>
        public static IEnumerable<string> GetIdentityPropertyNames(this PropertyInfo[] properties)
        {
            IEnumerable<string> identityPropertyNames =
                properties?.GetIdentityProperties()
                .Select(q => q.Name)
                ?? ArrayHelper.Empty<string>();

            return identityPropertyNames;
        }

        /// <summary>
        /// Get Properties which has not Identity attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns>.</returns>
        public static PropertyInfo[] GetNotIdentityProperties(this PropertyInfo[] properties)
        {
            if (properties == null || properties.Length == 0)
                return ArrayHelper.Empty<PropertyInfo>();

            PropertyInfo[] notIdentityProperties = properties
                  .Where(q => (q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true)?
                  .DatabaseGeneratedOption).GetValueOrDefault(DatabaseGeneratedOption.None) == DatabaseGeneratedOption.None)
                  .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return notIdentityProperties;
        }

        /// <summary>
        /// Get Properties which has DatabaseGeneratedOption attribute as array.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <param name="option">Database generated option.</param>
        /// <returns>.</returns>
        public static PropertyInfo[] GetPropertiesByGeneratedOption(this PropertyInfo[] properties, DatabaseGeneratedOption option)
        {
            if (properties == null || properties.Length == 0)
                return ArrayHelper.Empty<PropertyInfo>();

            PropertyInfo[] generatedOptionProperties = properties
                    .Where(q => q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true) != null)
                    .Where(q =>
                    q.GetCustomAttribute<DatabaseGeneratedAttribute>(inherit: true).DatabaseGeneratedOption == option)
                  .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();

            return generatedOptionProperties;
        }

        /// <summary>
        /// Get Properties which can read and write.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns></returns>
        public static PropertyInfo[] GetReadWriteProperties(this PropertyInfo[] properties)
        {
            if (properties == null || properties.Length == 0)
                return ArrayHelper.Empty<PropertyInfo>();

            PropertyInfo[] result = properties
                .Where(p => p.CanWrite && p.CanRead)
                .ToArray() ?? ArrayHelper.Empty<PropertyInfo>();
            return result;
        }

        /// <summary>
        /// Property To nullable DbType.
        /// </summary>
        /// <param name="propertyInfo">The info <see cref="PropertyInfo"/>.</param>
        /// <returns>Returns nullable DbType<see cref="System.Nullable{DbType}"/>.</returns>
        public static DbType? PropertyToDbType(this PropertyInfo propertyInfo)
        {
            DbType? dbType = propertyInfo?.PropertyType?.ToDbType();
            return dbType;
        }
    }
}