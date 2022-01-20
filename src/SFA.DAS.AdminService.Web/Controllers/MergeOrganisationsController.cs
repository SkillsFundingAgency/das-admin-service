using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Attributes;
using SFA.DAS.AdminService.Web.Domain.Merge;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models.Merge;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class MergeOrganisationsController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IMergeOrganisationSessionService _mergeSessionService;
        private readonly IHttpContextAccessor _contextAccessor;

        private const int DefaultPageIndex = 1;
        private const int DefaultMergesPerPage = 10;
        private const string DefaultSortOrder = SortOrder.Desc;
        private const string DefaultSortColumn = MergeOrganisationSortColumn.CompletedAt;

        public MergeOrganisationsController(IApiClient apiClient, IMergeOrganisationSessionService sessionService, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _mergeSessionService = sessionService;
            _contextAccessor = httpContextAccessor;
        }

        [HttpGet("merge-organisations")]
        public IActionResult Index()
        {
            ResetPageIndex();
            ResetItemsPerPage();
            ResetSortOrder();
            ResetSortColumn();

            return RedirectToAction("MergeLog");
        }

        [HttpGet("merge-organisations/log")]
        public async Task<IActionResult> MergeLog()
        {
            var viewModel = await GetMergeLogViewModel();

            return View(viewModel);
        }

        private async Task<MergeLogViewModel> GetMergeLogViewModel()
        {
            var pagingState = _mergeSessionService.MergeOrganisationPagingState;

            var getMergeLogRequest = new GetMergeLogRequest
            {
                PageSize = pagingState.ItemsPerPage,
                PageIndex = pagingState.PageIndex,
                SortColumn = pagingState.SortColumn,
                SortDirection = pagingState.SortDirection
            };

            var mergeLogResults = await _apiClient.GetMergeLog(getMergeLogRequest);

            var paginationViewModel = new PaginationViewModel<MergeLogEntry>
            {
                SortColumnAction = nameof(SortMergeLogEntries),
                ChangePageAction = nameof(ChangeMergeLogEntriesPageIndex),
                ChangeItemsPerPageAction = nameof(ChangeMergeEntriesPerPage),
                ItemsPerPage = pagingState.ItemsPerPage,
                SortColumn = pagingState.SortColumn,
                SortDirection = pagingState.SortDirection,
                PaginatedList = mergeLogResults
            };

            return new MergeLogViewModel
            {
                ControllerName = "MergeOrganisations",
                MergeLogResults = paginationViewModel
            };
        }

        [HttpGet("merge-organisations/log/change-entries-per-page")]
        public async Task<IActionResult> ChangeMergeEntriesPerPage(int itemsPerPage = DefaultMergesPerPage)
        {
            var pagingState = _mergeSessionService.MergeOrganisationPagingState;
            pagingState.ItemsPerPage = itemsPerPage;

            var viewModel = await GetMergeLogViewModel();

            return View(nameof(MergeLog), viewModel);
        }

        [HttpGet("merge-organisations/log/change-page")]
        public async Task<IActionResult> ChangeMergeLogEntriesPageIndex(int pageIndex = DefaultPageIndex)
        {
            var pagingState = _mergeSessionService.MergeOrganisationPagingState; 
            pagingState.PageIndex = pageIndex;

            var viewModel = await GetMergeLogViewModel();

            return View(nameof(MergeLog), viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SortMergeLogEntries(string sortColumn, string sortDirection)
        {
            var pagingState = _mergeSessionService.MergeOrganisationPagingState;

            if (pagingState.SortColumn == sortColumn)
            {
                pagingState.SortDirection = sortDirection;
            }
            else
            {
                pagingState.SortColumn = sortColumn;
                pagingState.SortDirection = SortOrder.Asc;
            }

            var viewModel = await GetMergeLogViewModel();

            return View(nameof(MergeLog), viewModel);
        }

        [HttpGet("merge-organisations/log/{logId}")]
        public async Task<IActionResult> CompletedMergeOverview(int logId)
        {
            var response = await _apiClient.GetMergeLogEntry(logId);

            var viewModel = new CompletedMergeOverviewViewModel
            {
                PrimaryEpaoName = response.PrimaryEndPointAssessorOrganisationName,
                SecondaryEpaoName = response.SecondaryEndPointAssessorOrganisationName,
                SecondaryEpaoEffectiveToDate = response.SecondaryEpaoEffectiveTo,
                CompletionDate = response.CompletedAt
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/start")]
        public IActionResult Start()
        {
            return View();
        }

        [HttpPost("merge-organisations/start")]
        public IActionResult StartNow()
        {
            _mergeSessionService.StartNewMergeRequest();

            return RedirectToAction(nameof(MergeOverview));
        }

        [MergeRequestFilter]
        [HttpGet("merge-organisations/overview")]
        public IActionResult MergeOverview()
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var viewModel = new MergeOverviewViewModel
            {
                PrimaryEpaoId = mergeRequest?.PrimaryEpao?.Id,
                PrimaryEpaoName = mergeRequest?.PrimaryEpao?.Name,
                SecondaryEpaoId = mergeRequest?.SecondaryEpao?.Id,
                SecondaryEpaoName = mergeRequest?.SecondaryEpao?.Name,
                SecondaryEpaoEffectiveTo = mergeRequest?.SecondaryEpaoEffectiveTo,
                PreviousCommand = mergeRequest?.PreviousCommand
            };

            return View(viewModel);
        }

        [MergeRequestFilter]
        [HttpGet("merge-organisations/search/{type}")]
        public IActionResult SearchEpao(string type, string searchString = null)
        {
            var viewModel = new SearchOrganisationViewModel
            {
                OrganisationType = type.ToLower(),
                SearchString = searchString
            };

            return View(viewModel);
        }

        [MergeRequestFilter]
        [HttpGet("merge-organisations/results/{type}")]
        public async Task<IActionResult> EpaoSearchResults(string type, SearchOrganisationViewModel searchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(SearchEpao), searchViewModel);
            }

            _mergeSessionService.AddSearchEpaoCommand(type, searchViewModel.SearchString);

            var searchstring = searchViewModel.SearchString?.Trim().ToLower();
            searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            searchstring = rx.Replace(searchstring, "");

            var searchResults = await _apiClient.SearchOrganisations(searchstring);

            var results = searchResults.Select(result => new Epao(result.Id, result.Name, result.Ukprn)).ToList();

            var viewModel = new EpaoSearchResultsViewModel
            {
                OrganisationType = type.ToLower(),
                Results = results,
                SearchString = searchViewModel.SearchString,
                PreviousCommand = _mergeSessionService.GetMergeRequest().PreviousCommand
            };

            return View(viewModel);
        }

        [MergeRequestFilter]
        [HttpGet("merge-organisations/confirm/{type}/{epaoId}")]
        public async Task<IActionResult> ConfirmEpao(string type, string epaoId)
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var epao = await _apiClient.GetEpaOrganisation(epaoId);

            var viewModel = new ConfirmEpaoViewModel(epao, type, mergeRequest.PreviousCommand);

            return View(viewModel);
        }

        [HttpPost("merge-organisations/confirm/{type}/{epaoId}")]
        public IActionResult ConfirmEpao(string type, ConfirmEpaoViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }

            var mergeRequest = _mergeSessionService.GetMergeRequest();

            mergeRequest.UpdateEpao(viewModel.OrganisationType, viewModel.EpaoId, viewModel.Name, viewModel.Ukprn.Value);

            _mergeSessionService.UpdateMergeRequest(mergeRequest);

            return RedirectToAction(nameof(MergeOverview));
        }

        [MergeRequestFilter]
        [HttpGet("merge-organisations/set-effective-to")]
        public IActionResult SetSecondaryEpaoEffectiveToDate()
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var viewModel = new SetSecondaryEpaoEffectiveToDateViewModel(mergeRequest.SecondaryEpao, mergeRequest.SecondaryEpaoEffectiveTo);

            return View(viewModel);
        }
        
        [HttpPost("merge-organisations/set-effective-to")]
        public IActionResult SetSecondaryEpaoEffectiveToDate(SetSecondaryEpaoEffectiveToDateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var mergeRequest = _mergeSessionService.GetMergeRequest();

            mergeRequest.SetSecondaryEpaoEffectiveToDate(int.Parse(viewModel.Day), int.Parse(viewModel.Month), int.Parse(viewModel.Year));

            _mergeSessionService.UpdateMergeRequest(mergeRequest);

            return RedirectToAction(nameof(MergeOverview));
        }

        [HttpPost("merge-organisations/overview")]
        public IActionResult MergeOverview(MergeOverviewViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.PreviousCommand = _mergeSessionService.GetMergeRequest().PreviousCommand;

                return View(viewModel);
            }

            return RedirectToAction(nameof(ConfirmAndComplete));
        }

        [HttpGet("merge-organisations/confirm-and-complete")]
        public IActionResult ConfirmAndComplete()
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var viewModel = new ConfirmAndCompleteViewModel
            {
                PrimaryEpaoName = mergeRequest?.PrimaryEpao?.Name,
                SecondaryEpaoName = mergeRequest?.SecondaryEpao?.Name
            };

            return View(viewModel);
        }

        [HttpPost("merge-organisations/confirm-and-complete")]
        public async Task<IActionResult> ConfirmAndComplete(ConfirmAndCompleteViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = _contextAccessor.HttpContext.User.UserId();

            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var mergeCommand = new MergeOrganisationsCommand
            {
                PrimaryEndPointAssessorOrganisationId = mergeRequest.PrimaryEpao.Id,
                SecondaryEndPointAssessorOrganisationId = mergeRequest.SecondaryEpao.Id,
                SecondaryStandardsEffectiveTo = mergeRequest.SecondaryEpaoEffectiveTo.Value,
                ActionedByUser = userId
            };
            
            try
            {
                var result = await _apiClient.MergeOrganisations(mergeCommand);
                
                mergeRequest.MarkComplete();

                _mergeSessionService.UpdateMergeRequest(mergeRequest);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(MergeError));
            }

            return RedirectToAction(nameof(MergeComplete));
        }

        [HttpGet("merge-organisations/complete")]
        public IActionResult MergeComplete()
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var viewModel = new MergeCompleteViewModel
            {
                PrimaryEpaoName = mergeRequest.PrimaryEpao.Name,
                SecondaryEpaoName = mergeRequest.SecondaryEpao.Name,
                SecondaryEpaoEffectiveTo = mergeRequest.SecondaryEpaoEffectiveTo.Value
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/error")]
        public IActionResult MergeError()
        {
            return View();
        }

        private void ResetPageIndex()
        {
            _mergeSessionService.MergeOrganisationPagingState.PageIndex = DefaultPageIndex;
        }

        private void ResetItemsPerPage()
        {
            _mergeSessionService.MergeOrganisationPagingState.ItemsPerPage = DefaultMergesPerPage;
        }

        private void ResetSortOrder()
        {
            _mergeSessionService.MergeOrganisationPagingState.SortDirection = DefaultSortOrder;
        }

        private void ResetSortColumn()
        {
            _mergeSessionService.MergeOrganisationPagingState.SortColumn = DefaultSortColumn;
        }

    }
}
