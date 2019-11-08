using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using System;
using SFA.DAS.AdminService.Web.Domain.Apply;
using System.Linq;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    [CheckSession(nameof(StandardApplicationController), nameof(ResetSession), nameof(IApplicationsSession.StandardApplicationsSessionValid))]
    public class StandardApplicationController : Controller
    {
        private readonly IApplicationsSession _applicationsSession;
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ILogger<StandardApplicationController> _logger;

        private const int DefaultPageIndex = 1;
        private const int DefaultApplicationsPerPage = 10;
        private const int DefaultPageSetSize = 6;

        public StandardApplicationController(IApplicationsSession applicationsSession, IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, ILogger<StandardApplicationController> logger)
        {
            _apiClient = apiClient;
            _applicationsSession = applicationsSession;
            _qnaApiClient = qnaApiClient;
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Ignore)]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(nameof(StandardApplications));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(StandardApplications));
        }

        [HttpGet("/Applications/{applicationId}/Standard")]
        public async Task<IActionResult> ActiveSequence(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var activeApplySequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, ApplyConst.STANDARD_APPLICATION_TYPE, organisation, sequence, sections,
                activeApplySequence.Sections, GetApplicationsState(application.ReviewStatus)?.PageIndex);

            return View(nameof(Sequence), sequenceVm);
        }

        [HttpGet("/Applications/{applicationId}/Standard/Sequence/{sequenceNo}")]
        public async Task<IActionResult> Sequence(Guid applicationId, int sequenceNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, ApplyConst.STANDARD_APPLICATION_TYPE, organisation, sequence, sections,
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
        public async Task<IActionResult> StandardApplications(int pageIndex = 1)
        {
            // reset only the page indexes; retain the sort column, direction and applications per page settings
            _applicationsSession.NewStandardApplications.PageIndex = 1;
            _applicationsSession.InProgressStandardApplications.PageIndex = 1;
            _applicationsSession.FeedbackStandardApplications.PageIndex = 1;
            _applicationsSession.ApprovedStandardApplications.PageIndex = 1;

            var vm = await MapViewModelFromSession();
            return View(nameof(StandardApplications), vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageNewApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
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
        [CheckSession(nameof(IApplicationsSession.StandardApplicationsSessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortApprovedApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        private ApplicationsState GetApplicationsState(string reviewStatus)
        {
            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    return _applicationsSession.NewStandardApplications;
                case ApplicationReviewStatus.InProgress:
                    return _applicationsSession.InProgressStandardApplications;
                case ApplicationReviewStatus.HasFeedback:
                    return _applicationsSession.FeedbackStandardApplications;
                case ApplicationReviewStatus.Approved:
                    return _applicationsSession.ApprovedStandardApplications;
            }

            return null;
        }

        private async Task<ApplicationsDashboardViewModel> MapViewModelFromSession()
        {
            var viewModel = new ApplicationsDashboardViewModel(nameof(StandardApplicationController).RemoveController());

            viewModel.NewApplications = await AddApplicationsViewModelValues(viewModel.NewApplications, ApplicationReviewStatus.New, _applicationsSession.NewStandardApplications);
            viewModel.InProgressApplications = await AddApplicationsViewModelValues(viewModel.InProgressApplications, ApplicationReviewStatus.InProgress, _applicationsSession.InProgressStandardApplications);
            viewModel.FeedbackApplications = await AddApplicationsViewModelValues(viewModel.FeedbackApplications, ApplicationReviewStatus.HasFeedback, _applicationsSession.FeedbackStandardApplications);
            viewModel.ApprovedApplications = await AddApplicationsViewModelValues(viewModel.ApprovedApplications, ApplicationReviewStatus.Approved, _applicationsSession.ApprovedStandardApplications);

            return viewModel;
        }

        private async Task<IActionResult> ChangePageApplications(string reviewStatus, int pageIndex)
        {
            var applicationsState = GetApplicationsState(reviewStatus);
            applicationsState.PageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(StandardApplications), vm);
        }

        private async Task<IActionResult> ChangePageApplicationsPartial(string reviewStatus, int pageIndex)
        {
            var applicationState = this.GetApplicationsState(reviewStatus);
            applicationState.PageIndex = pageIndex;

            var viewModel = await AddApplicationsViewModelValues(new ApplicationsViewModel(), reviewStatus, applicationState);
            return PartialView("_StandardApplicationsPartial", viewModel);
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
                    viewModel.SortColumnAction = "SortNewStandardApplications";
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
            var standardApplicationsRequest = new StandardApplicationsRequest
            (
                reviewStatus,
                applicationsState.SortColumn.ToString(),
                applicationsState.SortDirection == SortOrder.Asc ? 1 : 0,
                applicationsState.ApplicationsPerPage,
                applicationsState.PageIndex,
                DefaultPageSetSize
            );

            var response = await _applyApiClient.GetStandardApplications(standardApplicationsRequest);
            return response;
        }

        private void SetDefaultSession()
        {
            _applicationsSession.StandardApplicationsSessionValid = true;

            _applicationsSession.NewStandardApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.NewStandardApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.NewStandardApplications.SortColumn = StandardApplicationsSortColumn.SubmittedDate;
            _applicationsSession.NewStandardApplications.SortDirection = SortOrder.Desc;

            _applicationsSession.InProgressStandardApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.InProgressStandardApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.InProgressStandardApplications.SortColumn = StandardApplicationsSortColumn.SubmittedDate;
            _applicationsSession.InProgressStandardApplications.SortDirection = SortOrder.Desc;

            _applicationsSession.FeedbackStandardApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.FeedbackStandardApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.FeedbackStandardApplications.SortColumn = StandardApplicationsSortColumn.FeedbackAddedDate;
            _applicationsSession.FeedbackStandardApplications.SortDirection = SortOrder.Desc;

            _applicationsSession.ApprovedStandardApplications.PageIndex = DefaultPageIndex;
            _applicationsSession.ApprovedStandardApplications.ApplicationsPerPage = DefaultApplicationsPerPage;
            _applicationsSession.ApprovedStandardApplications.SortColumn = StandardApplicationsSortColumn.ClosedDate;
            _applicationsSession.ApprovedStandardApplications.SortDirection = SortOrder.Desc;
        }
    }
}