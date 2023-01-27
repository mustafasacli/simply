using System;
using System.Collections.Generic;
using System.Linq;

namespace Simply.Common
{
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
        { return Enumerable.Empty<T>().ToArray(); }

        /// <summary>
        /// Creates empty list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns empty list.</returns>
        public static List<T> EmptyList<T>() where T : class
        { return Enumerable.Empty<T>().ToList(); }

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
        { return Enumerable.Empty<T>().ToArray(); }

        /// <summary>
        /// Checks the array is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">T instance array</param>
        /// <returns>if array is null or empty returns true else return false.</returns>
        public static bool IsNullOrEmpty<T>(this T[] array) where T : class
        {
            bool isEmpty = !(array?.Any() ?? false);
            return isEmpty;
        }

        /// <summary>
        /// Checks the list is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">T instance array</param>
        /// <returns>if list is null or empty returns true else return false.</returns>
        public static bool IsNullOrEmpty<T>(this List<T> list) where T : class
        {
            bool isEmpty = !(list?.Any() ?? false);
            return isEmpty;
        }
    }
}