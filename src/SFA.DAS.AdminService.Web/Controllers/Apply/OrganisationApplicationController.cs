using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Extensions;
using System;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    [CheckSession(nameof(OrganisationApplicationController), nameof(ResetSession), nameof(IApplicationsSession.OrganisationApplicationsSessionValid))]
    public class OrganisationApplicationController : Controller
    {
        private readonly IApplicationsSession _applicationsSession;
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ILogger<OrganisationApplicationController> _logger;

        private const int DefaultPageIndex = 1;
        private const int DefaultApplicationsPerPage = 10;
        private const int DefaultPageSetSize = 6;

        public OrganisationApplicationController(IApplicationsSession applicationsSession, IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, ILogger<OrganisationApplicationController> logger)
        {
            _apiClient = apiClient;
            _applicationsSession = applicationsSession;
            _qnaApiClient = qnaApiClient;
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Ignore)]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(nameof(OrganisationApplications));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(OrganisationApplications));
        }

        [HttpGet("/Applications/{applicationId}/Organisation")]
        public async Task<IActionResult> ActiveSequence(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var activeApplySequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, ApplyConst.ORGANISATION_APPLICATION_TYPE, organisation, sequence, sections,
                activeApplySequence.Sections, GetApplicationsState(application.ReviewStatus)?.PageIndex);

            return View(nameof(Sequence), sequenceVm);
        }

        [HttpGet("/Applications/{applicationId}/Organisation/Sequence/{sequenceNo}")]
        public async Task<IActionResult> Sequence(Guid applicationId, int sequenceNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, ApplyConst.ORGANISATION_APPLICATION_TYPE, organisation, sequence, sections,
                applySequence.Sections, GetApplicationsState(application.ReviewStatus)?.PageIndex);

            if (application.ApplicationStatus == ApplicationStatus.Submitted || application.ApplicationStatus == ApplicationStatus.Resubmitted)
            {
                return View(nameof(Sequence), sequenceVm);
            }
            else
            {
                return View($"{nameof(Sequence)}_ReadOnly", sequenceVm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrganisationApplications(int pageIndex = 1)
        {
            // reset only the page indexes; retain the sort column, direction and applications per page settings
            _applicationsSession.NewOrganisationApplications.PageIndex = 1;
            _applicationsSession.InProgressOrganisationApplications.PageIndex = 1;
            _applicationsSession.FeedbackOrganisationApplications.PageIndex = 1;
            _applicationsSession.ApprovedOrganisationApplications.PageIndex = 1;

            var vm = await MapViewModelFromSession();
            return View(nameof(OrganisationApplications), vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageNewApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageNewApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageNewApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.New, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageNewApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.New, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortNewApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.New, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortNewApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.New, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageInProgressApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.InProgress, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageInProgressApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.InProgress, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageInProgressApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.InProgress, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageInProgressApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.InProgress, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortInProgressApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.InProgress, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortInProgressApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.InProgress, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageFeedbackApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.HasFeedback, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageFeedbackApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.HasFeedback, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageFeedbackApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.HasFeedback, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageFeedbackApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.HasFeedback, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortFeedbackApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.HasFeedback, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortFeedbackApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.HasFeedback, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageApprovedApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.Approved, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageApprovedApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.Approved, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageApprovedApplications(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.Approved, applicationsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageApprovedApplicationsPartial(int applicationsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.Approved, applicationsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortApprovedApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.OrganisationApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortApprovedApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        private ApplicationsState GetApplicationsState(string reviewStatus)
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

            return null;
        }

        private async Task<ApplicationsDashboardViewModel> MapViewModelFromSession()
        {
            var viewModel = new ApplicationsDashboardViewModel(nameof(OrganisationApplicationController).RemoveController());

            viewModel.NewApplications = await AddApplicationsViewModelValues(viewModel.NewApplications, ApplicationReviewStatus.New, _applicationsSession.NewOrganisationApplications);
            viewModel.InProgressApplications = await AddApplicationsViewModelValues(viewModel.InProgressApplications, ApplicationReviewStatus.InProgress, _applicationsSession.InProgressOrganisationApplications);
            viewModel.FeedbackApplications = await AddApplicationsViewModelValues(viewModel.FeedbackApplications, ApplicationReviewStatus.HasFeedback, _applicationsSession.FeedbackOrganisationApplications);
            viewModel.ApprovedApplications = await AddApplicationsViewModelValues(viewModel.ApprovedApplications, ApplicationReviewStatus.Approved, _applicationsSession.ApprovedOrganisationApplications);

            return viewModel;
        }

        private async Task<IActionResult> ChangePageApplications(string reviewStatus, int pageIndex)
        {
            var applicationsState = GetApplicationsState(reviewStatus);
            applicationsState.PageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(OrganisationApplications), vm);
        }

        private async Task<IActionResult> ChangePageApplicationsPartial(string reviewStatus, int pageIndex)
        {
            var applicationState = this.GetApplicationsState(reviewStatus);
            applicationState.PageIndex = pageIndex;

            var viewModel = await AddApplicationsViewModelValues(new ApplicationsViewModel(), reviewStatus, applicationState);
            return PartialView("_OrganisationApplicationsPartial", viewModel);
        }

        private async Task<IActionResult> ChangeApplicationsPerPageApplications(string reviewStatus, int applicationsPerPage)
        {
            var applicationState = this.GetApplicationsState(reviewStatus);
            applicationState.ApplicationsPerPage = applicationsPerPage;

            return await ChangePageApplications(reviewStatus, 1);
        }

        private async Task<IActionResult> ChangeApplicationsPerPageApplicationsPartial(string reviewStatus, int applicationsPerPage)
        {
            var applicationsState = GetApplicationsState(reviewStatus);
            applicationsState.ApplicationsPerPage = applicationsPerPage;

            return await ChangePageApplicationsPartial(reviewStatus, 1);
        }

        private async Task<IActionResult> SortApplications(string reviewStatus, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, GetApplicationsState(reviewStatus));
            return await ChangePageApplications(reviewStatus, 1);
        }
        
        private async Task<IActionResult> SortApplicationsPartial(string reviewStatus, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, GetApplicationsState(reviewStatus));
            return await ChangePageApplicationsPartial(reviewStatus, 1);
        }

        private void UpdateSortDirection(string sortColumn, string sortDirection, ApplicationsState applicationsState)
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

        private async Task<ApplicationsViewModel> AddApplicationsViewModelValues(ApplicationsViewModel viewModel, string reviewStatus, ApplicationsState applicationsState)
        {
            viewModel.ReviewStatus = reviewStatus;
            viewModel.PageIndex = applicationsState.PageIndex;
            viewModel.ApplicationsPerPage = applicationsState.ApplicationsPerPage;
            viewModel.SortColumn = applicationsState.SortColumn;
            viewModel.SortDirection = applicationsState.SortDirection;
            viewModel.Applications = await GetPageApplications(viewModel.ReviewStatus, applicationsState);

            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    viewModel.ChangePageAction = "ChangePageNewApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageNewApplications";
                    viewModel.SortColumnAction = "SortNewApplications";
                    viewModel.Title = "New";
                    break;
                case ApplicationReviewStatus.InProgress:
                    viewModel.ChangePageAction = "ChangePageInProgressApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageInProgressApplications";
                    viewModel.SortColumnAction = "SortInProgressApplications";
                    viewModel.Title = "In progress";
                    break;
                case ApplicationReviewStatus.HasFeedback:
                    viewModel.ChangePageAction = "ChangePageFeedbackApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageFeedbackApplications";
                    viewModel.SortColumnAction = "SortFeedbackApplications";
                    viewModel.Title = "Feedback";
                    break;
                case ApplicationReviewStatus.Approved:
                    viewModel.ChangePageAction = "ChangePageApprovedApplications";
                    viewModel.ChangeApplicationsPerPageAction = "ChangeApplicationPerPageApprovedApplications";
                    viewModel.SortColumnAction = "SortApprovedApplications";
                    viewModel.Title = "Approved";
                    break;
            }

            viewModel.Fragment = ApplicationReviewHelpers.ApplicationFragment(reviewStatus);

            return viewModel;
        }

        private async Task<PaginatedList<ApplicationSummaryItem>> GetPageApplications(string reviewStatus, ApplicationsState applicationsState)
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
            _applicationsSession.OrganisationApplicationsSessionValid = true;

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