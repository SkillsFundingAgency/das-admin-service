using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Snapshot;
using System.Linq;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    [FeatureToggle(FeatureToggles.EnableRoatpSnapshot, "Dashboard", "Index")]
    public class RoatpSnapshotController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public RoatpSnapshotController(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IConfiguration configuration)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _configuration = configuration;
        }

        [HttpGet("/Roatp/Snapshot")]
        public IActionResult Index()
        {
            if (_configuration["EnvironmentName"].EndsWith("PROD", StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var viewmodel = new SnapshotViewModel();
            return View("~/Views/Roatp/Apply/Snapshot/Index.cshtml", viewmodel);
        }
    }
}
