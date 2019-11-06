using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    [CheckSession(nameof(OrganisationApplicationController), nameof(ResetSession), nameof(IApplicationsSession.ApplicationsSessionValid))]
    public class OrganisationApplicationController : Controller
    {
        private readonly IApplicationsSession _applicationsSession;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly ILogger<OrganisationApplicationController> _logger;

        private const int DefaultPageIndex = 1;
        private const int DefaultApplicationsPerPage = 10;
        private const int DefaultPageSetSize = 6;

        public OrganisationApplicationController(IApplicationsSession applicationsSession, IApplicationApiClient applyApiClient, ILogger<OrganisationApplicationController> logger)
        {
            _applicationsSession = applicationsSession;
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Ignore)]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(nameof(OrganisationApplications));
        }

        [HttpGet]
        public async Task<IActionResult> OrganisationApplications(int pageIndex = 1)
        {
            // reset only the page indexes; retain the sort column, direction and applications per page settings
            _applicationsSession.NewOrganisationApplications.PageIndex = 1;
            _applicationsSession.InProgressOrganisationApplications.PageIndex = 1;
            _applicationsSession.FeedbackOrganisationApplications.PageIndex = 1;
            _applicationsSession.ApprovedOrganisationApplications.PageIndex = 1;

            var vm = await MapOrganisationViewModelFromSession();
            return View(nameof(OrganisationApplications), vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageNewOrganisationApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplications(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageNewOrganisationApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplicationsPartial(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageNewOrganisationApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplications(ApplicationReviewStatus.New, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageNewOrganisationApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplicationsPartial(ApplicationReviewStatus.New, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortNewOrganisationApplications(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplications(ApplicationReviewStatus.New, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortNewOrganisationApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplicationsPartial(ApplicationReviewStatus.New, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageInProgressOrganisationApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplications(ApplicationReviewStatus.InProgress, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageInProgressOrganisationApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplicationsPartial(ApplicationReviewStatus.InProgress, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageInProgressOrganisationApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplications(ApplicationReviewStatus.InProgress, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageInProgressOrganisationApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplicationsPartial(ApplicationReviewStatus.InProgress, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortInProgressOrganisationApplications(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplications(ApplicationReviewStatus.InProgress, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortInProgressOrganisationApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplicationsPartial(ApplicationReviewStatus.InProgress, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageFeedbackOrganisationApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplications(ApplicationReviewStatus.HasFeedback, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageFeedbackOrganisationApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplicationsPartial(ApplicationReviewStatus.HasFeedback, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageFeedbackOrganisationApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplications(ApplicationReviewStatus.HasFeedback, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageFeedbackOrganisationApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplicationsPartial(ApplicationReviewStatus.HasFeedback, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortFeedbackOrganisationApplications(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplications(ApplicationReviewStatus.HasFeedback, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortFeedbackOrganisationApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplicationsPartial(ApplicationReviewStatus.HasFeedback, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageApprovedOrganisationApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplications(ApplicationReviewStatus.Approved, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageApprovedOrganisationApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageOrganisationApplicationsPartial(ApplicationReviewStatus.Approved, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageApprovedOrganisationApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplications(ApplicationReviewStatus.Approved, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageApprovedOrganisationApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageOrganisationApplicationsPartial(ApplicationReviewStatus.Approved, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortApprovedOrganisationApplications(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplications(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.ApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortApprovedOrganisationApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortOrganisationApplicationsPartial(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        private ApplicationsState GetApplicationsState(string applicationType, string reviewStatus)
        {
            if (applicationType == ApplyConst.ORGANISATION_APPLICATION_TYPE)
            {
                switch (reviewStatus)
                {
                    case ApplicationReviewStatus.New:
                        return _applicationsSession.NewOrganisationApplications;
                    case ApplicationReviewStatus.InProgress:
                        return _applicationsSession.InProgressOrganisationApplications;
                    case ApplicationReviewStatus.HasFeedback:
                        return _applicationsSession.FeedbackOrganisationApplications;
                    case ApplicationReviewStatus.Approved:
                        return _applicationsSession.ApprovedOrganisationApplications;
                }
            }

            return null;
        }

        private async Task<OrganisationApplicationsViewModel> MapOrganisationViewModelFromSession()
        {
            var viewModel = new OrganisationApplicationsViewModel();

            viewModel.NewOrganisationApplications = await AddOrganisationApplicationsViewModelValues(viewModel.NewOrganisationApplications, ApplicationReviewStatus.New, _applicationsSession.NewOrganisationApplications);
            viewModel.InProgressOrganisationApplications = await AddOrganisationApplicationsViewModelValues(viewModel.InProgressOrganisationApplications, ApplicationReviewStatus.InProgress, _applicationsSession.InProgressOrganisationApplications);
            viewModel.FeedbackOrganisationApplications = await AddOrganisationApplicationsViewModelValues(viewModel.FeedbackOrganisationApplications, ApplicationReviewStatus.HasFeedback, _applicationsSession.FeedbackOrganisationApplications);
            viewModel.ApprovedOrganisationApplications = await AddOrganisationApplicationsViewModelValues(viewModel.ApprovedOrganisationApplications, ApplicationReviewStatus.Approved, _applicationsSession.ApprovedOrganisationApplications);

            return viewModel;
        }

        private async Task<IActionResult> ChangePageOrganisationApplications(string reviewStatus, int pageIndex)
        {
            var applicationsState = GetApplicationsState(ApplyConst.ORGANISATION_APPLICATION_TYPE, reviewStatus);
            applicationsState.PageIndex = pageIndex;

            var vm = await MapOrganisationViewModelFromSession();
            return View(nameof(OrganisationApplications), vm);
        }

        private async Task<IActionResult> ChangePageOrganisationApplicationsPartial(string reviewStatus, int pageIndex)
        {
            var applicationState = this.GetApplicationsState(ApplyConst.ORGANISATION_APPLICATION_TYPE, reviewStatus);
            applicationState.PageIndex = pageIndex;

            var viewModel = await AddOrganisationApplicationsViewModelValues(new ApplicationsViewModel(), reviewStatus, applicationState);
            return PartialView("_OrganisationApplicationsPartial", viewModel);
        }

        private async Task<IActionResult> ChangeApplicationsPerPageOrganisationApplications(string reviewStatus, int applicationsPerPage)
        {
            var applicationState = this.GetApplicationsState(ApplyConst.ORGANISATION_APPLICATION_TYPE, reviewStatus);
            applicationState.ApplicationsPerPage = applicationsPerPage;

            return await ChangePageOrganisationApplications(reviewStatus, 1);
        }

        private async Task<IActionResult> ChangeApplicationsPerPageOrganisationApplicationsPartial(string reviewStatus, int applicationsPerPage)
        {
            var applicationsState = GetApplicationsState(ApplyConst.ORGANISATION_APPLICATION_TYPE, reviewStatus);
            applicationsState.ApplicationsPerPage = applicationsPerPage;

            return await ChangePageOrganisationApplicationsPartial(reviewStatus, 1);
        }

        private async Task<IActionResult> SortOrganisationApplications(string reviewStatus, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, GetApplicationsState(ApplyConst.ORGANISATION_APPLICATION_TYPE, reviewStatus));
            return await ChangePageOrganisationApplications(reviewStatus, 1);
        }
        
        private async Task<IActionResult> SortOrganisationApplicationsPartial(string reviewStatus, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, GetApplicationsState(ApplyConst.ORGANISATION_APPLICATION_TYPE, reviewStatus));
            return await ChangePageOrganisationApplicationsPartial(reviewStatus, 1);
        }

        private void UpdateSortDirection(string sortColumnName, string sortDirection, ApplicationsState applicationsState)
        {
            if (Enum.TryParse(sortColumnName, true, out OrganisationApplicationsSortColumn sortColumn))
            {
                if (applicationsState.SortColumn == sortColumn)
                {
                    applicationsState.SortDirection = sortDirection;
                }
                else
                {
                    applicationsState.SortColumn = sortColumn;
                    applicationsState.SortDirection = SortOrder.Asc;
                }
            }
        }

        private async Task<ApplicationsViewModel> AddOrganisationApplicationsViewModelValues(ApplicationsViewModel viewModel, string reviewStatus, ApplicationsState applicationsState)
        {
            viewModel.ReviewStatus = reviewStatus;
            viewModel.PageIndex = applicationsState.PageIndex;
            viewModel.ApplicationsPerPage = applicationsState.ApplicationsPerPage;
            viewModel.SortColumn = applicationsState.SortColumn;
            viewModel.SortDirection = applicationsState.SortDirection;
            viewModel.Applications = await GetPageOrganisationApplications(viewModel.ReviewStatus, applicationsState);

            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    viewModel.ChangePageAction = "ChangePageNewOrganisationApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageNewOrganisationApplications";
                    viewModel.SortColumnAction = "SortNewOrganisationApplications";
                    viewModel.Title = "New";
                    break;
                case ApplicationReviewStatus.InProgress:
                    viewModel.ChangePageAction = "ChangePageInProgressOrganisationApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageInProgressOrganisationApplications";
                    viewModel.SortColumnAction = "SortInProgressOrganisationApplications";
                    viewModel.Title = "In progress";
                    break;
                case ApplicationReviewStatus.HasFeedback:
                    viewModel.ChangePageAction = "ChangePageFeedbackOrganisationApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageFeedbackOrganisationApplications";
                    viewModel.SortColumnAction = "SortFeedbackOrganisationApplications";
                    viewModel.Title = "Feedback";
                    break;
                case ApplicationReviewStatus.Approved:
                    viewModel.ChangePageAction = "ChangePageApprovedOrganisationApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageApprovedOrganisationApplications";
                    viewModel.SortColumnAction = "SortApprovedOrganisationApplications";
                    viewModel.Title = "Approved";
                    break;
            }

            viewModel.Fragment = ApplicationReviewHelpers.ApplicationFragment(reviewStatus);

            return viewModel;
        }

        private async Task<PaginatedList<ApplicationSummaryItem>> GetPageOrganisationApplications(string reviewStatus, ApplicationsState applicationsState)
        {
            var organisationApplicationsRequest = new OrganisationApplicationsRequest
            (
                reviewStatus,
                applicationsState.SortColumn.ToString(),
                applicationsState.SortDirection == SortOrder.Asc ? 1 : 0,
                applicationsState.ApplicationsPerPage,
                applicationsState.PageIndex,
                DefaultPageSetSize
            );

            var response = await _applyApiClient.GetOrganisationApplications(organisationApplicationsRequest);
            return response;
        }

        private void SetDefaultSession()
        {
            _applicationsSession.ApplicationsSessionValid = true;

            _applicationsSession.NewOrganisationApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.NewOrganisationApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.NewOrganisationApplications.SortColumn = OrganisationApplicationsSortColumn.SubmittedDate;
            _applicationsSession.NewOrganisationApplications.SortDirection = SortOrder.Desc;

            _applicationsSession.InProgressOrganisationApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.InProgressOrganisationApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.InProgressOrganisationApplications.SortColumn = OrganisationApplicationsSortColumn.SubmittedDate;
            _applicationsSession.InProgressOrganisationApplications.SortDirection = SortOrder.Desc;

            _applicationsSession.FeedbackOrganisationApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.FeedbackOrganisationApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.FeedbackOrganisationApplications.SortColumn = OrganisationApplicationsSortColumn.FeedbackAddedDate;
            _applicationsSession.FeedbackOrganisationApplications.SortDirection = SortOrder.Desc;

            _applicationsSession.ApprovedOrganisationApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.ApprovedOrganisationApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.ApprovedOrganisationApplications.SortColumn = OrganisationApplicationsSortColumn.ClosedDate;
            _applicationsSession.ApprovedOrganisationApplications.SortDirection = SortOrder.Desc;
        }
    }
}