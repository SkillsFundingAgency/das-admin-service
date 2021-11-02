using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class WarningMessagesExtensions
    {
        public static void AddWarningMessages(this List<string> warningMessages, List<string> newWarningMessages)
        {
            if (newWarningMessages != null)
            {
                warningMessages.AddRange(newWarningMessages);
            }
        }

        public static void AddErrorMessages(this ModelStateDictionary dict, Dictionary<string, string> errorMessages)
        {
            if (errorMessages != null)
            {
                foreach (var error in errorMessages)
                {
                    dict.AddModelError(error.Key, error.Value);
                }
            }
        }
    }
}
