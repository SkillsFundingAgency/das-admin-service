using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly IWebConfiguration _configuration;
        public DashboardController(IApplicationApiClient applicationApiClient, IWebConfiguration configuration)
        {
            _applicationApiClient = applicationApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var applicationReviewStatusCounts = await _applicationApiClient.GetApplicationReviewStatusCounts();

            var viewModel = new DashboardViewModel
            {
                OrganisationApplicationsNew = applicationReviewStatusCounts.OrganisationApplicationsNew,
                OrganisationApplicationsInProgress = applicationReviewStatusCounts.OrganisationApplicationsInProgress,
                OrganisationApplicationsHasFeedback = applicationReviewStatusCounts.OrganisationApplicationsHasFeedback,
                OrganisationApplicationsApproved = applicationReviewStatusCounts.OrganisationApplicationsApproved,
                StandardApplicationsNew = applicationReviewStatusCounts.StandardApplicationsNew,
                StandardApplicationsInProgress = applicationReviewStatusCounts.StandardApplicationsInProgress,
                StandardApplicationsHasFeedback = applicationReviewStatusCounts.StandardApplicationsHasFeedback,
                StandardApplicationsApproved = applicationReviewStatusCounts.StandardApplicationsApproved,
                RoatpOversightBaseUrl = _configuration.RoatpOversightBaseUrl,
                RoatpGatewayBaseUrl = _configuration.RoatpGatewayBaseUrl,
                RoatpAssessorBaseUrl = _configuration.RoatpAssessorBaseUrl
            };

            return View(viewModel);
        }
    }
}