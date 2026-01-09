using System;
using System.Globalization;

namespace PsychedelicExperience.Common
{
    public static class DateTimeExtensions
    {
        private static readonly GregorianCalendar _calendar = new GregorianCalendar();

        public static int GetWeekOfMonth(this DateTime time)
        {
            var first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        public static int GetWeekOfYear(this DateTime time)
        {
            return _calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}