using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class StringExtensions
    {
        public static string CapitaliseFirstLetter(this string stringToBeFirstLetterCapitalised)
        {
            if (string.IsNullOrEmpty(stringToBeFirstLetterCapitalised))
            {
                return string.Empty;
            }

            return char.ToUpper(stringToBeFirstLetterCapitalised[0]) + stringToBeFirstLetterCapitalised.Substring(1);
        }
    }
}
