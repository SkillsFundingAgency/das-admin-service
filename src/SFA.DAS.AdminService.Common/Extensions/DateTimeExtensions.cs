using System;

namespace SFA.DAS.AdminService.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToSfaShortDateString(this DateTime time)
        {
            return time.ToString("dd MMMM yyyy");
        }

        public static string ToSfaShortDateString(this DateTime? time)
        {
            return time?.ToString("dd MMMM yyyy");
        }
        public static string ToSfaShortestDateString(this DateTime? time)
        {
            return time == null ?
                string.Empty : time.Value.ToString("dd MMM yy");
        }

        public static string ToSfaShortMonthDateString(this DateTime? time)
        {
            return time?.ToString("dd MMM yyyy");
        }

        public static DateTime UtcFromTimeZoneTime(this DateTime time, string timeZoneId = "GMT Standard Time")
        {
            TimeZoneInfo tzi;
            try
            {
                tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                return time;
            }
            catch (InvalidTimeZoneException)
            {
                return time;
            }
            return TimeZoneInfo.ConvertTimeToUtc(time, tzi);
        }

        public static DateTime UtcToTimeZoneTime(this DateTime time, string timeZoneId = "GMT Standard Time")
        {
            TimeZoneInfo tzi;
            try
            {
                tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                return time;
            }
            catch (InvalidTimeZoneException)
            {
                return time;
            }

            return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
        }

        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }
}
