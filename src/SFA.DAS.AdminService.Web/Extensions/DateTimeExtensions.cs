using System;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class DateTimeExtensions
    {
        private const string DisplayDateFormat = "dd MMM yyyy";

        public static string ToGdsDate(this DateTime datetime)
        {
            return datetime.ToString(DisplayDateFormat);
        }
    }
}
