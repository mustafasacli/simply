using Simply.Common.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Tos the json string.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="formatSetting">Format settings.</param>
        /// <returns>A string.</returns>
        public static string ToJsonString(this IDictionary<string, object> keyValuePairs,
            SimpleFormatSetting formatSetting)
        {
            if (keyValuePairs == null)
                return string.Empty;

            if (keyValuePairs.Count == 0)
                return string.Empty;

            formatSetting = formatSetting ?? SimpleFormatSetting.New();

            string nullString = "null";
            string tabString = "\t";
            StringBuilder stringBuilder = new StringBuilder();
            int count = keyValuePairs.Count;
            int counter = 0;

            foreach (string key in keyValuePairs.Keys)
            {
                if (formatSetting.AddTab)
                    stringBuilder.Append(tabString);

                stringBuilder.AppendFormat("\"{0}\" : ", key);
                object value = keyValuePairs[key];

                if (value.IsNullOrDbNull())
                {
                    stringBuilder.Append(nullString);
                }
                else
                {
                    if (value is string str)
                    {
                        stringBuilder.Append("\"" + str + "\"");
                    }
                    else if (value is DateTime date)
                    {
                        stringBuilder.Append("\"" + date.ToString(formatSetting.DatetimeFormat, CultureInfo.InvariantCulture) + "\"");
                    }
                    else if (value is double doubleValue)
                    {
                        stringBuilder.Append(doubleValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is float floatValue)
                    {
                        stringBuilder.Append(floatValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is decimal decimalValue)
                    {
                        stringBuilder.Append(decimalValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is bool boolValue)
                    {
                        stringBuilder.Append(boolValue.ToString().ToLower());
                    }
                    else
                    {
                        stringBuilder.Append(value.ToString());
                    }
                }

                if ((counter++) != count - 1)
                    stringBuilder.Append(", ");

                if (formatSetting.AddNewLine)
                    stringBuilder.AppendLine();
            }

            string tempValue = stringBuilder.ToString();
            string result = string.Concat("{ ", formatSetting.AddNewLine ? Environment.NewLine : string.Empty, tempValue, " }");
            return result;
        }

        /// <summary>
        /// Tos the xml string.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="formatSetting">Format settings.</param>
        /// <returns>A string.</returns>
        public static string ToXmlString(this IDictionary<string, object> keyValuePairs,
            SimpleFormatSetting formatSetting)
        {
            formatSetting = formatSetting ?? SimpleFormatSetting.New();

            if (formatSetting.MainXmlNodeName.IsNullOrSpace())
                throw new ArgumentNullException(nameof(SimpleFormatSetting.MainXmlNodeName));

            if (!(keyValuePairs?.Any() ?? false))
                return string.Format("<{0}></{0}>", formatSetting.MainXmlNodeName);

            string tabString = "\t";
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string key in keyValuePairs.Keys)
            {
                if (formatSetting.AddNewLine)
                    stringBuilder.AppendLine();

                if (formatSetting.AddTab)
                    stringBuilder.Append(tabString);

                object value = keyValuePairs[key];

                if (value.IsNullOrDbNull())
                {
                    stringBuilder.AppendFormat("<{0} nil=\"true\"></{0}>", key);
                }
                else
                {
                    if (value is string str)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, str);
                    }
                    else if (value is DateTime date)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, date.ToString(formatSetting.DatetimeFormat, CultureInfo.InvariantCulture));
                    }
                    else if (value is double doubleValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, doubleValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is float floatValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, floatValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is decimal decimalValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, decimalValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is bool boolValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, boolValue.ToString().ToLower());
                    }
                    else
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", key, value.ToString());
                    }
                }
            }

            string tempValue = stringBuilder.ToString();
            string result = string.Format("<{0}>{1}</{0}>", formatSetting.MainXmlNodeName, tempValue);
            return result;
        }

        /// <summary>
        /// Sets the value and return instance.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Return cuurent dictionary instance.</returns>
        public static Dictionary<TKey, TValue> SetValueAndReturn<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)        {
            dictionary[key] = value;
            return dictionary;
        }
    }
}