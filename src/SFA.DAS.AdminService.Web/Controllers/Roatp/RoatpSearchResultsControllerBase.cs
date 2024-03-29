﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class RoatpSearchResultsControllerBase : Controller
    {
        protected IRoatpSessionService _sessionService;
        protected IRoatpApiClient _apiClient;

        protected async Task<IActionResult> RefreshSearchResults(string searchTerm = null)
        {
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                var searchModel = _sessionService.GetSearchResults();
                searchTerm = searchModel.SearchTerm;
            }
            OrganisationSearchResults searchResults = await _apiClient.Search(searchTerm);
            var viewModel = new OrganisationSearchResultsViewModel
            {
                SearchTerm = searchTerm,
                Title = BuildSearchResultsTitle(searchResults.TotalCount, searchTerm),
                SearchResults = searchResults.SearchResults,
                TotalCount = searchResults.TotalCount,
                SelectedIndex = 0
            };
            _sessionService.SetSearchResults(viewModel);
            var actionName = "SearchResults";
            if (searchResults.TotalCount == 0)
            {
                actionName = "NoSearchResults";
            }
            return RedirectToAction(actionName, "RoatpSearch");
        }

        private string BuildSearchResultsTitle(int totalCount, string searchTerm)
        {
            string title = "";
            if (totalCount == 0)
            {
                title = string.Format(RoatpSearchValidation.NoSearchResultsFound, searchTerm);
            }
            else
            {
                var resultText = "results";
                if (totalCount == 1)
                {
                    resultText = "result";
                }
                title = string.Format(RoatpSearchValidation.SearchResultsFound, totalCount, resultText, searchTerm);
            }

            return title;
        }
    }
}
