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
        /// Get Properties which can read and write.
        /// </summary>
        /// <param name="properties">Property list</param>
        /// <returns></returns>
        public static PropertyInfo[] GetReadWriteProperties(this PropertyInfo[] properties)
        {
            if (properties.IsNullOrEmpty())
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