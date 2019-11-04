using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Web.Domain.Apply;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    public class StandardApplicationController : Controller
    {
        private readonly IApplicationsSession _applicationsSession;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly ILogger<OrganisationApplicationController> _logger;

        public StandardApplicationController(IApplicationsSession applicationsSession, IApplicationApiClient applyApiClient, ILogger<OrganisationApplicationController> logger)
        {
            _applicationsSession = applicationsSession;
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        [HttpGet("/Applications/Standard")]
        public async Task<IActionResult> StandardApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetOpenApplications(ApplyConst.STANDARD_SEQUENCE_NO);

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new ApplicationsDashboardViewModel { Applications = paginatedApplications };

            return View(nameof(StandardApplications), viewmodel);
        }

        [HttpGet("/Applications/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added
            var applications = await _applyApiClient.GetFeedbackAddedApplications(ApplyConst.STANDARD_SEQUENCE_NO);

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new ApplicationsDashboardViewModel { Applications = paginatedApplications };

            return View(nameof(RejectedApplications), viewmodel);
        }

        [HttpGet("/Applications/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedApplications(ApplyConst.STANDARD_SEQUENCE_NO);

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new ApplicationsDashboardViewModel { Applications = paginatedApplications };

            return View(nameof(ClosedApplications), viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageNewStandardApplications(int pageIndex = 1)
        {
            return await StandardApplications();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageInProgressStandardApplications(int pageIndex = 1)
        {
            return await StandardApplications();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageFeedbackStandardApplications(int pageIndex = 1)
        {
            return await StandardApplications();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageApprovedStandardApplications(int pageIndex = 1)
        {
            return await StandardApplications();
        }
    }
}