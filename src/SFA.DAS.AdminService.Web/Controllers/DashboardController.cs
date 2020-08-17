using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly IWebConfiguration _configuration;

        public DashboardController(ILogger<DashboardController> logger, IApplicationApiClient applicationApiClient, IWebConfiguration configuration)
        {
            _logger = logger;
            _applicationApiClient = applicationApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationReviewStatusCounts applicationReviewStatusCounts;

            try
            {
                applicationReviewStatusCounts = await _applicationApiClient.GetApplicationReviewStatusCounts();
            }
            catch(Exception ex) when(ex is HttpRequestException || ex is UnsupportedMediaTypeException)
            {
                _logger.LogError(ex, "Unable to GetApplicationReviewStatusCounts from EPAO Service");
                // Absorb the exception at this point but make it obvious that something has gone wrong. We don't want to kill the Staff Dashboard so soon!
                applicationReviewStatusCounts = new ApplicationReviewStatusCounts
                {
                    OrganisationApplicationsNew = short.MinValue,
                    OrganisationApplicationsInProgress = short.MinValue,
                    OrganisationApplicationsHasFeedback = short.MinValue,
                    OrganisationApplicationsApproved = short.MinValue,
                    StandardApplicationsNew = short.MinValue,
                    StandardApplicationsInProgress = short.MinValue,
                    StandardApplicationsHasFeedback = short.MinValue,
                    StandardApplicationsApproved = short.MinValue,
                };
            }

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
                RoatpAssessorBaseUrl = _configuration.RoatpAssessorBaseUrl,
                RoatpGatewayBaseUrl = _configuration.RoatpGatewayBaseUrl
            };

            return View(viewModel);
        }
    }
}