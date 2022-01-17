using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Attributes;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class MergeOrganisationsController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IMergeOrganisationSessionService _mergeSessionService;

        public MergeOrganisationsController(IApiClient apiClient, IMergeOrganisationSessionService sessionService)
        {
            _apiClient = apiClient;
            _mergeSessionService = sessionService;
        }

        [HttpGet("merge-organisations/log")]
        public async Task<IActionResult> MergeLog(int? pageSize = 10, int? pageIndex = 1)
        {
            var response = await _apiClient.GetMergeLogs(pageSize.Value, pageIndex.Value);
            
            var viewModel = new MergeLogViewModel()
            {
                PageIndex = pageIndex.Value,
                PageSize = pageSize.Value,
                MergeLogs = response
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/log/{logId}")]
        public async Task<IActionResult> ViewMergeLogEntry(int logId)
        {

            return View();
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

            var previousCommand = mergeRequest.PreviousCommand;

            var epao = await _apiClient.GetEpaOrganisation(epaoId);

            var viewModel = new ConfirmEpaoViewModel(epao, type, previousCommand);

            return View(viewModel);
        }

        [HttpPost("merge-organisations/confirm/{type}/{epaoId}")]
        public IActionResult ConfirmEpao(string type, ConfirmEpaoViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                viewModel.PreviousCommand = _mergeSessionService.GetMergeRequest().PreviousCommand;

                return View(viewModel);
            }

            var mergeRequest = _mergeSessionService.GetMergeRequest();

            mergeRequest.UpdateEpao(viewModel.OrganisationType, viewModel.EpaoId, viewModel.Name, viewModel.Ukprn);

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

            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var result = await _apiClient.GetMergeLogs(1,1);
    
            mergeRequest.MarkComplete();

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
    }
}
