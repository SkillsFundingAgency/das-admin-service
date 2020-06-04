using System;
using System.Text.RegularExpressions;

namespace SFA.DAS.AdminService.Common.Extensions
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

        public static bool ValidateHttpURL(string value, out Uri resultURI)
        {
            if (string.IsNullOrEmpty(value))
            {
                resultURI = null;
                return false;
            }

            if (!Regex.IsMatch(value, @"^https?:\/\/", RegexOptions.IgnoreCase))
                value = "http://" + value;

            if (Uri.TryCreate(value, UriKind.Absolute, out resultURI))
                return resultURI.Scheme == Uri.UriSchemeHttp ||
                        resultURI.Scheme == Uri.UriSchemeHttps;

            return false;
        }
    }
}
