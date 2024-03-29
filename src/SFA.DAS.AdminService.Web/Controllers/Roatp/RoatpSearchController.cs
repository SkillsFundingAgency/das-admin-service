﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{

    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class RoatpSearchController : RoatpSearchResultsControllerBase
    {
        private ILogger<RoatpSearchController> _logger;
        
        public RoatpSearchController(ILogger<RoatpSearchController> logger, IRoatpApiClient apiClient,
                                     IRoatpSessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpPost]
        public async Task<IActionResult> Search(OrganisationSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/Index.cshtml", model);
            }

            return await RefreshSearchResults(model.SearchTerm);
        }

        [Route("results-found")]
        public async Task<IActionResult> SearchResults(int index = 0)
        {
            var model = _sessionService.GetSearchResults();
            if (index >= model.SearchResults.Count)
            {
                index = 0;
            }

            model.SelectedIndex = index;
            _sessionService.SetSearchResults(model);

            _sessionService.ClearSearchTerm();
            return View("~/Views/Roatp/SearchResults.cshtml", model);
        }

        [Route("no-results-found")]
        public async Task<IActionResult> NoSearchResults()
        {
            var model = _sessionService.GetSearchResults();

            return View("~/Views/Roatp/SearchResults.cshtml", model);
        }

        [Route("refine-search")]
        public async Task<IActionResult> RefineSearch(string searchTerm)
        {
            _sessionService.SetSearchTerm(searchTerm);

            return RedirectToAction("Index", "RoatpHome");
        }
    }
}
