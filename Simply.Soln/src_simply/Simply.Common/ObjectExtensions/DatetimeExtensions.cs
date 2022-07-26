using System;
using System.Globalization;
using static System.String;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="DatetimeExtensions"/>.
    /// </summary>
    public static class DatetimeExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime">datetime object instance</param>
        /// <param name="format">datetime string format</param>
        /// <returns>Returns DateTime.ToString(format, CultureInfo.InvariantCulture);</returns>
        public static string ToStrDate(this DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss.ffffff")
        {
            string dateString = dateTime.ToString(format, CultureInfo.InvariantCulture);
            return dateString;
        }

        /// <summary>
        /// Retruns datetime.tostring with given format.
        /// </summary>
        /// <param name="dateTime">datetime object instance</param>
        /// <param name="format">datetime string format</param>
        /// <returns>Returns DateTime.ToString(format, CultureInfo.InvariantCulture);</returns>
        public static string ToStrDate(this DateTime? dateTime, string format = "yyyy-MM-dd HH:mm:ss.ffffff")
        {
            string dateString = dateTime?.ToString(format, CultureInfo.InvariantCulture) ?? Empty;
            return dateString;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime">datetime object.</param>
        /// <returns></returns>
        public static DateTime GetLastBusinessDayOfMonth(this DateTime dateTime)
        {
            DateTime lastDayOfCurrentMonth = (new DateTime(dateTime.Year, dateTime.Month, 1)).AddMonths(1).AddDays(-1);

            if (lastDayOfCurrentMonth.DayOfWeek == DayOfWeek.Sunday)
                lastDayOfCurrentMonth = lastDayOfCurrentMonth.AddDays(-2);
            else if (lastDayOfCurrentMonth.DayOfWeek == DayOfWeek.Saturday)
                lastDayOfCurrentMonth = lastDayOfCurrentMonth.AddDays(-1);

            return lastDayOfCurrentMonth;
        }

        /// <summary>
        /// Gets First Day of month for given date.
        /// </summary>
        /// <param name="date">Datetime object</param>
        /// <returns>Returns first day of month as Datetime.</returns>
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// convert datetime instance to string with dd/MM/yyyy format.
        /// </summary>
        /// <param name="dateTime">datetime object.</param>
        /// <returns>Returns string object.</returns>
        public static string ToOracleDate(this DateTime dateTime)
        {
            string dateString = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dateString;
        }

        /// <summary>
        /// convert datetime instance to string with dd/MM/yyyy HH:mm:ss.ffffff or dd/MM/yyyy HH:mm:ss format.
        /// </summary>
        /// <param name="dateTime">datetime object.</param>
        /// <param name="includeMilisecond">if it is true format is dd/MM/yyyy HH:mm:ss.ffffff, else dd/MM/yyyy HH:mm:ss.</param>
        /// <returns>Returns string object.</returns>
        public static string ToOracleDateTime(this DateTime? dateTime, bool includeMilisecond = false)
        {
            string dateString = dateTime?.ToString(includeMilisecond ? "dd/MM/yyyy HH:mm:ss.ffffff" : "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) ?? Empty;
            return dateString;
        }
    }
}