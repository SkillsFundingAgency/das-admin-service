using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch;
using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Models.Search;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class SearchController : Controller
    {
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IRegisterApiClient _registerApiClient;
        private readonly IStaffSearchApiClient _staffSearchApiClient;
        private readonly IFrameworkSearchSessionService _sessionService;
        private readonly IFrameworkSearchApiClient _frameworkSearchApiClient;
        private readonly IMapper _mapper;

        public SearchController(ILearnerDetailsApiClient learnerDetailsApiClient, IRegisterApiClient registerApiClient, IStaffSearchApiClient staffSearchApiClient,
            IFrameworkSearchSessionService sessionService, IFrameworkSearchApiClient frameworkSearchApiClient, IMapper mapper)
        {
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _registerApiClient = registerApiClient;
            _staffSearchApiClient = staffSearchApiClient;
            _sessionService = sessionService;
            _frameworkSearchApiClient = frameworkSearchApiClient;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(SearchInputViewModel vm)
        {
            return View(vm ?? new SearchInputViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Results(SearchInputViewModel vm, int page = 1)
        {
            if (ModelState.IsValid)
            {
                if (vm.SearchType == SearchTypes.Standards)
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
                else if (vm.SearchType == SearchTypes.Frameworks)
                {
                    var searchQuery = _mapper.Map<FrameworkSearchQuery>(vm);
                    var frameworkResults = await _frameworkSearchApiClient.SearchFrameworks(searchQuery);

                    var searchSessionObject = new FrameworkSearch()
                    {
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        DateOfBirth = searchQuery.DateOfBirth,
                        FrameworkResults = _mapper.Map<List<FrameworkResultViewModel>>(frameworkResults)
                    };

                    _sessionService.SessionFrameworkSearch = searchSessionObject;
                    return RedirectToAction("MultipleResults");      
                }
            }
            else
            { 
                if (vm.SearchType == SearchTypes.Standards)
                {
                    _sessionService.ClearFrameworkSearchRequest();
                    
                    vm.FirstName = null;
                    vm.LastName = null;
                    vm.Day = null;
                    vm.Month = null;
                    vm.Year = null;
                    vm.Date = null;   
                }
                else if (vm.SearchType == SearchTypes.Frameworks)
                {
                    vm.SearchString = null;
                }
            }
            return RedirectToAction("Index", vm);
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

        [HttpGet]
        public IActionResult MultipleResults()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            var viewModel = _mapper.Map<FrameworkSearchResultsViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SelectFramework(FrameworkSearchResultsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.SelectedResult = vm.SelectedResult;
                });
                return RedirectToAction("MultipleResults");
            }
            var sessionModel = _sessionService.SessionFrameworkSearch;
            vm.FrameworkResults = sessionModel.FrameworkResults;
            return View("MultipleResults", vm); 
        }
    }  
}