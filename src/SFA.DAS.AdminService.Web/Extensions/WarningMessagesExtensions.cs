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
    }
}
