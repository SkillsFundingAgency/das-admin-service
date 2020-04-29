using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.AdminService.Web.Infrastructure;

namespace SFA.DAS.AdminService.Web.Attributes
{
    public class ExternalApiExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is ExternalApiException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectToActionResult("ExternalApisUnavailable", "RoatpShutterPage", new { });
                filterContext.Result.ExecuteResultAsync(filterContext);
            }
            base.OnException(filterContext);
        }
    }
}
