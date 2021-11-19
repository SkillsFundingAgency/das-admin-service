using System;

namespace SFA.DAS.AdminService.Common.Helpers
{
    public static class DateHelper
    {
        public static DateTime? ConstructDate(string dayString, string monthString, string yearString)
        {

            if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
                !int.TryParse(yearString, out var year)) return null;

            if (!IsValidDate(year, month, day))
                return null;

            return new DateTime(year, month, day);
        }

        public static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }
    }
}
