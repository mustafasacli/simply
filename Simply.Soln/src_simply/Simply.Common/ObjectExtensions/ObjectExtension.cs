using System;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="ObjectExtension"/>.
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// check object is null.
        /// </summary>
        /// <param name="o">The o to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        /// <summary>
        /// An object extension method that query if 'obj' is null or database null.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsNullOrDbNull(this object obj)
        {
            return (null == obj || obj == DBNull.Value || obj == (object)DBNull.Value);
        }

        /// <summary>
        /// To String.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as a string.</returns>
        public static string ToStr(this object obj)
        {
            string str;

            try
            {
                str = obj.IsNullOrDbNull() ?
                    string.Empty : obj.ToString();
            }
            catch
            { str = string.Empty; }

            return str;
        }

        /// <summary>
        /// Convert object to int.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an int.</returns>
        public static int ToInt(this object obj)
        {
            int result;

            try
            {
                result = Convert.ToInt32(obj);
            }
            catch { result = 0; }

            return result;
        }

        /// <summary>
        /// Convert object to long.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an long? .</returns>
        public static long ToLong(this object obj)
        {
            long result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (long.TryParse(obj.ToString(), out long a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable byte.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an byte? .</returns>
        public static byte? ToByteNullable(this object obj)
        {
            byte? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (byte.TryParse(obj.ToString(), out byte a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable short.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an short? .</returns>
        public static short? ToShortNullable(this object obj)
        {
            short? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (short.TryParse(obj.ToString(), out short a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable int.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an int? .</returns>
        public static int? ToIntNullable(this object obj)
        {
            int? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (int.TryParse(obj.ToString(), out int a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable long.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an long? .</returns>
        public static long? ToLongNullable(this object obj)
        {
            long? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (long.TryParse(obj.ToString(), out long a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable decimal.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an decimal? .</returns>
        public static decimal? ToDecimalNullable(this object obj)
        {
            decimal? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (decimal.TryParse(obj.ToString(), out decimal a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable double.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an double? .</returns>
        public static double? ToDoubleNullable(this object obj)
        {
            double? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (double.TryParse(obj.ToString(), out double a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to nullable float.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as an float? .</returns>
        public static float? ToFloatNullable(this object obj)
        {
            float? result = default;

            try
            {
                if (obj.IsNullOrDbNull())
                    return result;

                if (float.TryParse(obj.ToString(), out float a))
                    result = a;
            }
            catch
            {
                result = default;
            }

            return result;
        }

        /// <summary>
        /// Convert object to decimal.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>Obj as a decimal.</returns>
        public static decimal ToDecimal(this object obj)
        {
            decimal result;

            try
            {
                result = Convert.ToDecimal(obj);
            }
            catch
            { result = 0; }

            return result;
        }

        /// <summary>
        /// Convert char to nullable byte.
        /// </summary>
        /// <param name="ch">The ch to act on.</param>
        /// <returns>An int.</returns>
        public static int Char2Int(this char ch)
        {
            int i;

            try
            {
                i = Convert.ToInt32(ch);
            }
            catch
            { i = 0; }

            return i;
        }

        /// <summary>
        /// Checks is null or empty.
        /// </summary>
        /// <param name="str">The str to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Checks is null or white space.
        /// </summary>
        /// <param name="str">The str to act on.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool IsNullOrSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// if value is DbNulll.Value returns null, else return object value.
        /// </summary>
        /// <param name="obj">.</param>
        /// <returns>Returns object.</returns>
        public static object GetValueWithCheckDbNull(this object obj)
        {
            object value = obj == (object)DBNull.Value ? null : obj;
            return value;
        }

        /// <summary>
        /// Convert object to DateTime nullable.
        /// </summary>
        /// <param name="obj">.</param>
        /// <returns>.</returns>
        public static DateTime? ToDateTimeNullable(this object obj)
        {
            DateTime? value = null;

            if (DateTime.TryParse(obj.ToStr(), out DateTime date))
                value = date;

            return value;
        }
    }
}