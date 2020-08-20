using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize]
    public class RoatpShutterPageController : Controller
    {
        private readonly IWebConfiguration _configuration;

        public RoatpShutterPageController(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("ExternalApisUnavailable")]
        public async Task<IActionResult> ExternalApisUnavailable()
        {
            var viewModel = new ExternalApisUnavailableViewModel
            {
                RoatpGatewayBaseUrl = _configuration.RoatpGatewayBaseUrl
            };

            return View("~/Views/Roatp/Apply/ExternalApisUnavailable.cshtml", viewModel);
        }
    }
}
