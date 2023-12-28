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

        /// <summary>
        /// Maps classes with static functions.
        /// </summary>
        /// <typeparam name="T1">T1 input class</typeparam>
        /// <typeparam name="T2">T2 out class</typeparam>
        /// <param name="t1Instance">input class instance</param>
        /// <param name="func">convert function</param>
        /// <returns>returns out class instance.</returns>
        public static T2 MapWith<T1, T2>(this T1 t1Instance, Func<T1, T2> func) where T1 : class
            where T2 : class
        {
            T2 t2Instance = func(t1Instance);
            return t2Instance;
        }

        /// <summary>
        /// Orders Linq by given parameters.
        /// </summary>
        /// <typeparam name="TEntity">Source class type.</typeparam>
        /// <typeparam name="TKey">Order property selector.</typeparam>
        /// <param name="source">Source Linq</param>
        /// <param name="keySelector">Order by property selector.</param>
        /// <param name="isDesc">is order by descrement.</param>
        /// <returns>Return Linq query with order.</returns>
        public static IQueryable<TEntity> OrderBy<TEntity, TKey>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false) where TEntity : class
        {
            string command = isDesc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            string orderByPropertyName = keySelector.GetMemberName();
            var property = type.GetProperty(orderByPropertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);

        }

        /// <summary>
        /// Orders Linq by given parameters.
        /// </summary>
        /// <typeparam name="TEntity">Source class type.</typeparam>
        /// <param name="source">Source Linq</param>
        /// <param name="orderByPropertyName">Order by property name.</param>
        /// <param name="isDesc">is order by descrement.</param>
        /// <returns>Return Linq query with order.</returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source,
            string orderByPropertyName, bool isDesc = false) where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(orderByPropertyName))
                throw new ArgumentNullException(nameof(orderByPropertyName));

            if (source is null)
                return source;

            string command = isDesc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);

            var property = type.GetProperty(orderByPropertyName);
            if (property is null)
                throw new ArgumentNullException(string.Format("No property found with given name : {0}.", orderByPropertyName));

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        /// <summary>
        /// Gets Nullable object value or default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tInstance"></param>
        /// <param name="defaultValue"></param>
        /// <returns>nullable object is not null returns value, else returns default value.</returns>
        public static T Val<T>(this T? tInstance, T defaultValue) where T : struct
        {
            return tInstance ?? defaultValue;
        }
    }
}