using System;

namespace Common
{
    /// <summary>
    ///     Extensions to the <see cref="DateTime" />
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Whether the specified date a value assigned to it
        /// </summary>
        /// <remarks>
        ///     The specified date may be a UTC datetime or not, either way this function determines whether the date is NOT close
        ///     to the <see cref="DateTime.MinValue" />
        /// </remarks>
        public static bool HasValue(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime() > DateTime.MinValue.ToUniversalTime();
        }

        public static string ToIso8601(this DateTime dateTime)
        {
            return dateTime.ToString("O");
        }

        public static string ToIso8601(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("O");
        }

        public static DateTime ToNearestSecond(this DateTime dateTime)
        {
            return dateTime.Truncate(TimeSpan.FromSeconds(1));
        }

        public static DateTime ToNearestMinute(this DateTime dateTime)
        {
            return dateTime.Truncate(TimeSpan.FromMinutes(1));
        }

        private static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return dateTime;
            }

            return dateTime.AddTicks(-(dateTime.Ticks%timeSpan.Ticks));
        }
    }
}