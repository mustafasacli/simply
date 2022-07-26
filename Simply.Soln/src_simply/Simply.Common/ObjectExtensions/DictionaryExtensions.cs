using System;
using System.Collections.Generic;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="DictionaryExtensions"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// An IDictionary&lt;string,T&gt; extension method that gets value or default.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value or default.</returns>
        public static T GetValueOrDefault<T>(this IDictionary<string, T> dictionary, string key, T defaultValue)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            T value;

            if (dictionary.ContainsKey(key))
                value = dictionary[key];
            else
                value = defaultValue;

            return value;
        }
    }
}