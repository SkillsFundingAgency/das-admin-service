using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpDashboard;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.RoatpGatewayTeam + "," + Domain.Roles.RoatpAssessorGateway)]
    public class RoatpDashboardController : Controller
    {
        [HttpGet("roatp-dashboard", Name = RouteNames.RoatpDashboard_Index_Get)]
        public IActionResult Index()
        {
            var vm = new IndexViewModel
            {
                CanShowRoatpAssessorApplications = User.IsInRole(Domain.Roles.RoatpAssessorGateway),
                CanShowRoatp = User.IsInRole(Domain.Roles.RoatpGatewayTeam)
            };

            return View("~/Views/RoatpDashboard/Index.cshtml", vm);
        }
    }
}