﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize]
    public class RoatpShutterPageController : Controller
    {
        [Route("ExternalApisUnavailable")]
        public async Task<IActionResult> ExternalApisUnavailable()
        {
            return View("~/Views/Roatp/Apply/ExternalApisUnavailable.cshtml");
        }
    }
}
