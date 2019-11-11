using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorGateway)]
    [Route("roatp-assessor/gateway")]
    public class GatewayController : Controller
    {
        [HttpGet("dashboard", Name = RouteNames.RoatpAssessor_Gateway_Dashboard_Get)]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}