using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Attributes;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class SearchController : Controller
    {
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IRegisterApiClient _registerApiClient;
        private readonly IStaffSearchApiClient _staffSearchApiClient;

        public SearchController(ILearnerDetailsApiClient learnerDetailsApiClient, IRegisterApiClient registerApiClient, IStaffSearchApiClient staffSearchApiClient)
        {
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _registerApiClient = registerApiClient;
            _staffSearchApiClient = staffSearchApiClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("search/results")]
        public async Task<IActionResult> Results(SearchInputViewModel vm, int page = 1)
        {
            if (!TryValidateModel(vm))
            {
                return View("index",vm);
            }

            if (vm.SearchMode == SearchMode.Standards)
            {
                EpaOrganisation org = null;
                var searchResults = await _staffSearchApiClient.Search(vm.SearchString, page);

                if (!string.IsNullOrEmpty(searchResults?.EndpointAssessorOrganisationId))
                    org = await _registerApiClient.GetEpaOrganisation(searchResults.EndpointAssessorOrganisationId);

                var searchViewModel = new SearchResultsViewModel
                {
                    OrganisationName = org?.Name ?? string.Empty,
                    StaffSearchResult = searchResults,
                    SearchString = vm.SearchString,
                    Page = page
                };
                return View(searchViewModel);
            }
            else if (vm.SearchMode == SearchMode.Frameworks)
            {
                return RedirectToAction("Index");
            }
             
            return View("index",vm);
        }

        [HttpGet("select")]
        public async Task<IActionResult> Select(int stdCode,
            long uln,
            string searchString,
            int page = 1,
            bool allLogs = false,
            int? batchNumber = null)
        {
            var learner = await _learnerDetailsApiClient.GetLearnerDetail(stdCode, uln, allLogs);
            
            var vm = new SelectViewModel
            {
                Learner = learner,
                SearchString = searchString,
                Page = page,
                ShowDetail = !allLogs,
                BatchNumber = batchNumber
            };
        
            return View(vm);
        }
    }

    public class SearchResultsViewModel
    {
        public string SearchString { get; set; } = string.Empty;
        public string OrganisationName { get; set; }
        public int Page { get; set; }
        public StaffSearchResult StaffSearchResult { get; set; }
    }

    public class SearchInputViewModel
    {
        public string SearchString { get; set; } = string.Empty; 
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty;
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime Date { get; set; }
        public string SearchMode { get; set; }
    }

    public static class SearchMode
    {
        public const string Standards = "Standards";
        public const string Frameworks = "Frameworks";
    }
}