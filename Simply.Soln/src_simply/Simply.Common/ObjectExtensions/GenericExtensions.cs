using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

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
        /// Gets Property Values in a dictionary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The t to act on.</param>
        /// <returns>The properties.</returns>
        public static IDictionary<string, object> GetPropertyValues<T>(this T instance) where T : class
        {
            IDictionary<string, object> propertyValues = new Dictionary<string, object>();

            PropertyInfo[] properties = typeof(T)
                .GetProperties()
                .Where(q => q.CanRead)
                .ToArray();

            foreach (PropertyInfo property in properties)
            {
                propertyValues.Add(property.Name, property.GetValue(instance));
            }

            return propertyValues;
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

        /// <summary>
        /// Sets the given property value and returns current instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NullReferenceException">if instance is null. throw null reference exception.</exception>
        /// <exception cref="System.NotSupportedException">if the conversion cannot be performed.</exception>
        /// <returns>Returns current instance.</returns>
        public static T With<T, TKey>(this T instance, Expression<Func<T, TKey>> keySelector, TKey value) where T : class
        {
            string propertyName = keySelector.GetMemberName();
            instance.SetPropertyValue(propertyName, value);
            return instance;
        }

        /// <summary>
        /// Creates a Deep clone of given object instance.
        /// Class and other classes which property of T, must have system.serializable attribute.
        /// </summary>
        /// <param name="instance">object instance.</param>
        /// <returns>Returns a new instance.</returns>
        public static T DeepClone<T>(this T instance) where T : class
        {
            if (instance == null)
                return null;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, instance);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}