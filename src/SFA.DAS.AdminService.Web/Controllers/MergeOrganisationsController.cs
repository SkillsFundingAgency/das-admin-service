using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
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
        public async Task<IActionResult> MergeLog()
        {
            // get merge logs

            var viewModel = new MergeLogViewModel()
            {
                MergeLogs = null
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/start")]
        public IActionResult Start()
        {
            return View(new StartViewModel());
        }

        [HttpPost("merge-organisations/start")]
        public IActionResult Start(StartViewModel viewModel)
        {
            _mergeSessionService.StartNewMergeRequest();

            return RedirectToAction(nameof(MergeOverview));
        }

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
                SecondaryEpaoEffectiveTo = mergeRequest?.SecondaryEpaoEffectiveTo
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/search/{type}")]
        public IActionResult SearchEpao(string type)
        {
            var viewModel = new SearchOrganisationViewModel
            {
                OrganisationType = type.ToLower(),
                SearchString = null
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/results/{type}")]
        public async Task<IActionResult> EpaoSearchResults(string type, SearchOrganisationViewModel searchViewModel)
        {
            if (!ModelState.IsValid)
            {
                View(nameof(SearchEpao), searchViewModel);
            }

            var searchstring = searchViewModel.SearchString?.Trim().ToLower();
            searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            searchstring = rx.Replace(searchstring, "");

            var searchResults = await _apiClient.SearchOrganisations(searchstring);

            var results = searchResults.Select(result => new Epao(result.Id, result.Name)).ToList();

            var viewModel = new EpaoSearchResultsViewModel
            {
                OrganisationType = type.ToLower(),
                Results = results,
                SearchString = searchViewModel.SearchString
            };

            return View(viewModel);
        }

        [HttpGet("merge-organisations/confirm/{type}/{epaoId}")]
        public async Task<IActionResult> ConfirmEpao(string type, string epaoId)
        {
            var epao = await _apiClient.GetEpaOrganisation(epaoId);

            var viewModel = new ConfirmEpaoViewModel(epao, type);

            return View(viewModel);
        }

        [HttpPost("merge-organisations/confirm/{type}/{epaoId}")]
        public IActionResult ConfirmEpao(string type, ConfirmEpaoViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }

            _mergeSessionService.UpdateEpao(viewModel.OrganisationType, viewModel.EpaoId, viewModel.Name);

            return RedirectToAction(nameof(MergeOverview));
        }

        [HttpGet("merge-organisations/set-effective-to")]
        public IActionResult SetSecondaryEpaoEffectiveToDate()
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var viewModel = new SetSecondaryEpaoEffectiveToDateViewModel(mergeRequest.SecondaryEpaoEffectiveTo);

            return View(viewModel);
        }

        [HttpPost("merge-organisations/set-effective-to")]
        public IActionResult SetSecondaryEpaoEffectiveToDate(SetSecondaryEpaoEffectiveToDateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _mergeSessionService.SetSecondaryEpaoEffectiveToDate(int.Parse(viewModel.Day), int.Parse(viewModel.Month), int.Parse(viewModel.Year));

            return RedirectToAction(nameof(MergeOverview));
        }

        [HttpPost("merge-organisations/overview")]
        public IActionResult MergeOverview(MergeOverviewViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction(nameof(ConfirmAndComplete));
        }

        [HttpGet("merge-organsations/confirm-and-complete")]
        public IActionResult ConfirmAndComplete()
        {
            var viewModel = new ConfirmAndCompleteViewModel();

            return View(viewModel);
        }

        [HttpPost("merge-organsations/confirm-and-complete")]
        public async Task<IActionResult> ConfirmAndComplete(ConfirmAndCompleteViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await Task.Delay(1);

            return RedirectToAction(nameof(Completed));
        }

        [HttpGet("merge-organisation/complete")]
        public IActionResult Completed()
        {
            var mergeRequest = _mergeSessionService.GetMergeRequest();

            var viewModel = new MergeCompletedViewModel
            {
                PrimaryEpaoName = mergeRequest.PrimaryEpao.Name,
                SecondaryEpaoName = mergeRequest.SecondaryEpao.Name
            };

            return View(viewModel);
        }
    }
}
