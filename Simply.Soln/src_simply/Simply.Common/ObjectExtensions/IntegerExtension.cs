namespace Simply.Common
{
    /// <summary>
    /// Defines the <see cref="IntegerExtension"/>.
    /// </summary>
    public static class IntegerExtension
    {
        /// <summary>
        /// Get days as the hours.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns>Returns hours of days.</returns>
        public static int AsHours(this int days)
        {
            return days * 24;
        }

        /// <summary>
        /// Get hours as minutes.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns>Returns minutes of hours.</returns>
        public static int AsMinutes(this int hours)
        {
            return hours * 60;
        }

        /// <summary>
        /// Gets minutes as seconds.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>Returns seconds of minutes.</returns>
        public static int AsSeconds(this int minutes)
        {
            return minutes * 60;
        }

        /// <summary>
        /// Gets seconds as miliseconds.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>Returns milisconds of seconds.</returns>
        public static int AsMiliSeconds(this int seconds)
        {
            return seconds * 1000;
        }
    }
}