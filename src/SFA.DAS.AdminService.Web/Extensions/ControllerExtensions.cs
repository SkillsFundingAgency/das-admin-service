using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static string RemoveController(this string fullControllerClassName)
        {
            return fullControllerClassName.Replace("Controller", "");
        }
    }
}
