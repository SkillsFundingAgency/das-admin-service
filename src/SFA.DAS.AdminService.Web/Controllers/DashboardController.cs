using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IApplicationApiClient _applicationApiClient;

        public DashboardController(IApplicationApiClient applicationApiClient)
        {
            _applicationApiClient = applicationApiClient;
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
            };

            return View(viewModel);
        }
    }
}