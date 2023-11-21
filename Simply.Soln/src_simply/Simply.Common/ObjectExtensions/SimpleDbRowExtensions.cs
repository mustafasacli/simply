using Simply.Common.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="SimpleDbRowExtensions"/>.
    /// </summary>
    public static class SimpleDbRowExtensions
    {
        /// <summary>
        /// Converts the rows to list.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns>A list of TS.</returns>
        public static List<T> ConvertRowsToList<T>(this List<SimpleDbRow> rows) where T : class
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            List<T> list = new List<T>();

            if (rows.Count < 1)
                return list;

            IDictionary<string, string> columns = new Dictionary<string, string>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            properties.ToList()
                .ForEach(q =>
                {
                    columns[q.Name] = q.Name;
                });

            if (!properties.Any())
                throw new Exception("Any Property has found for mapping.");

            foreach (SimpleDbRow row in rows)
            {
                if (row.CellCount < 1)
                    continue;

                T instance = Activator.CreateInstance<T>();

                SetPropertyValuesFromRow(ref instance, properties, row, columns);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// A List&lt;SimpleDbRow&gt; extension method that converts a dynList to a list.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="rows">The dynList to act on.</param>
        /// <param name="columnPropertyMap">Column-Property Map</param>
        /// <returns>The given data converted to a list.</returns>
        public static List<T> ConvertRowsToList<T>(this List<SimpleDbRow> rows, Dictionary<string, string> columnPropertyMap) where T : class
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            if (columnPropertyMap == null)
                throw new ArgumentNullException(nameof(columnPropertyMap));

            List<T> list = new List<T>();

            if (rows.Count < 1)
                return list;

            PropertyInfo[] properties = typeof(T).GetProperties();

            if (properties.Length < 1)
                throw new Exception("Any Property has found for mapping.");

            T instance;

            foreach (SimpleDbRow row in rows)
            {
                if (row.CellCount < 1)
                    continue;

                instance = Activator.CreateInstance<T>();

                SetPropertyValuesFromRow(ref instance, properties, row, columnPropertyMap);

                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// Sets the property values.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="row">The row.</param>
        /// <param name="columnPropertyMappings">The column property mappings.</param>
        internal static void SetPropertyValuesFromRow<T>(ref T instance, PropertyInfo[] properties,
            SimpleDbRow row, IDictionary<string, string> columnPropertyMappings)
        {
            bool hasErrorThrowned = false;
            StringBuilder errorMessageBuilder = new StringBuilder();

            properties = properties ?? new PropertyInfo[0];

            foreach (PropertyInfo property in properties)
            {
                string columnName = columnPropertyMappings[property.Name];
                if (row.ContainsCellName(columnName))
                {
                    object value = row.GetValue(columnName);
                    try
                    {
                        property.SetValue(instance, value);
                    }
                    catch (Exception e)
                    {
                        hasErrorThrowned = true;
                        errorMessageBuilder.Append("Exception-Message: ").AppendLine(e.Message);
                        if (value.IsNullOrDbNull())
                        { errorMessageBuilder.AppendLine("Value is null"); }
                        else
                        {
                            errorMessageBuilder.Append("Value: ");
                            errorMessageBuilder.AppendLine(value.ToStr());

                            errorMessageBuilder.Append("Value Type: ");
                            errorMessageBuilder.AppendLine((Nullable.GetUnderlyingType(value?.GetType()) ?? value?.GetType())?.Name ?? "");
                        }

                        errorMessageBuilder.Append("Property Name: ");
                        errorMessageBuilder.AppendLine(property.Name);
                        errorMessageBuilder.Append("Property Type: ");
                        errorMessageBuilder.AppendLine((Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType).Name);
                        errorMessageBuilder.AppendLine("****************************************");
                    }
                }
            }

            if (hasErrorThrowned)
            {
                string message = errorMessageBuilder.ToString();
                if (message.IsValid())
                    throw new Exception(message);
            }
        }

        /// <summary>
        /// Converts simpledbrow instance to T type instance with given convert function.
        /// </summary>
        /// <param name="row">simple db row instance.</param>
        /// <param name="convertFunc">The convert func.</param>
        /// <returns>Returns a T instance.</returns>
        public static T ConvertTo<T>(this SimpleDbRow row, Func<SimpleDbRow, T> convertFunc)
        {
            T instance = convertFunc(row);
            return instance;
        }

        /// <summary>
        /// Tos the json string.
        /// </summary>
        /// <param name="row">Simple db row instance.</param>
        /// <param name="formatSetting">Format settings.</param>
        /// <returns>A string.</returns>
        public static string ToJsonString(this SimpleDbRow row, SimpleFormatSetting formatSetting)
        {
            if (row == null)
                return string.Empty;

            if (row.CellCount == 0)
                return string.Empty;

            formatSetting = formatSetting ?? SimpleFormatSetting.New();

            string nullString = "null";
            string tabString = "\t";
            StringBuilder stringBuilder = new StringBuilder();
            int count = row.CellCount;
            int counter = 0;

            foreach (var cell in row.Cells)
            {
                if (formatSetting.AddTab)
                    stringBuilder.Append(tabString);

                stringBuilder.AppendFormat("\"{0}\" : ", cell.CellName);
                object value = row[cell.CellName];

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
        /// <param name="row">Simple db row instance.</param>
        /// <param name="formatSetting">Format settings.</param>
        /// <returns>A string.</returns>
        public static string ToXmlString(this SimpleDbRow row,
            SimpleFormatSetting formatSetting)
        {
            if (formatSetting.MainXmlNodeName.IsNullOrSpace())
                throw new ArgumentNullException(nameof(SimpleFormatSetting.MainXmlNodeName));

            if (row == null || row.CellCount == 0)
                return string.Format("<{0}></{0}>", formatSetting.MainXmlNodeName);

            formatSetting = formatSetting ?? SimpleFormatSetting.New();

            string tabString = "\t";
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var cell in row.Cells)
            {
                if (formatSetting.AddNewLine)
                    stringBuilder.AppendLine();

                if (formatSetting.AddTab)
                    stringBuilder.Append(tabString);

                object value = row[cell.CellName];

                if (value.IsNullOrDbNull())
                {
                    stringBuilder.AppendFormat("<{0} nil=\"true\"></{0}>", cell.CellName);
                }
                else
                {
                    if (value is string str)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, str);
                    }
                    else if (value is DateTime date)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, date.ToString(formatSetting.DatetimeFormat, CultureInfo.InvariantCulture));
                    }
                    else if (value is double doubleValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, doubleValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is float floatValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, floatValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is decimal decimalValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, decimalValue.ToString().Replace(".", string.Empty).Replace(",", "."));
                    }
                    else if (value is bool boolValue)
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, boolValue.ToString().ToLower());
                    }
                    else
                    {
                        stringBuilder.AppendFormat("<{0}>{1}</{0}>", cell.CellName, value.ToString());
                    }
                }
            }

            string tempValue = stringBuilder.ToString();
            string result = string.Format("<{0}>{1}</{0}>", formatSetting.MainXmlNodeName, tempValue);
            return result;
        }

        /// <summary>
        /// Tos the json string.
        /// </summary>
        /// <param name="rows">Simple db row list.</param>
        /// <param name="formatSetting">Format settings.</param>
        /// <returns>A string.</returns>
        public static string ToJsonString(this List<SimpleDbRow> rows, SimpleFormatSetting formatSetting)
        {
            if (!(rows?.Any() ?? false))
                return string.Empty;
            var rowJsonStrings = rows.Select(s => s.ToJsonString(formatSetting))
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToList() ?? ArrayHelper.EmptyList<string>();

            if (!rowJsonStrings.Any())
                return string.Empty;

            string result = string.Concat("[ ", string.Join(formatSetting.AddNewLine ? "," + Environment.NewLine : ",", rowJsonStrings), " ]");
            return result;
        }
    }
}