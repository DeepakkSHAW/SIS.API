using System;
using System.Globalization;
using System.Linq;


namespace SIS.API
{
    internal static class InternalExtensions
    {
        internal static bool IsDateType(this string o)
        {
            return DateTime.TryParse(o, out DateTime dummy);
        }
        internal static bool IsTimeType(this string o)
        {
            return (DateTime.TryParseExact(o, "hh:mm",
    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dummy));
        }
        internal static bool IsWeekend(this DateTime date)
        {
            return new[] { DayOfWeek.Sunday, DayOfWeek.Saturday }.Contains(date.DayOfWeek);
        }
        internal static DateTime PreviousWorkDay(this DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            }
            while (IsWeekend(date));

            return date;
        }
    }
}
