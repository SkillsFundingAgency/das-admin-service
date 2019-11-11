namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = Domain.Roles.RoatpGatewayTeam)]
    public class RoatpHomeController : Controller
    {
        [Route("search-apprenticeship-training-providers", Name = RouteNames.Roatp_Home_Index_Get)]
        public IActionResult Index()
        {           
            return View("~/Views/Roatp/Index.cshtml");
        }
    }
}
