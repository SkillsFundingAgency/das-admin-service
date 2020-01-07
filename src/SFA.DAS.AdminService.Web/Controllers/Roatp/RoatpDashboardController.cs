using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    [Authorize(Roles = Domain.Roles.RoatpGatewayTeam + "," + Domain.Roles.RoatpAssessorTeam)]
    public class RoatpDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Roatp/Dashboard.cshtml");
        }
    }
}