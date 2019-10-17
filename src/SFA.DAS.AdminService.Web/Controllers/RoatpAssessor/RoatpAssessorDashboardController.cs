using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorTeam)]
    public class RoatpAssessorDashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/RoatpAssessor/RoatpAssessorDashboard/Index.cshtml");
        }
    }
}