using System;
using System.Threading;

namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="EnumExtensions"/>.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets Day Name of CurrentCulture.
        /// </summary>
        /// <param name="dayOfWeek">.</param>
        /// <returns>.</returns>
        public static string GetDayName(this DayOfWeek dayOfWeek)
        {
            string result = Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek);
            return result;
        }

        /// <summary>
        /// Gets Day Name of given Culture.
        /// </summary>
        /// <param name="dayOfWeek">.</param>
        /// <param name="culture">culture name.</param>
        /// <returns>.</returns>
        public static string GetDayName(this DayOfWeek dayOfWeek, string culture)
        {
            string cultureName = (culture ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cultureName))
                cultureName = "tr-TR";

            string result = new System.Globalization.CultureInfo(cultureName).DateTimeFormat.GetDayName(dayOfWeek);
            return result;
        }
    }
}