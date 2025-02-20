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
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch;
using SFA.DAS.AdminService.Web.Models.FrameworkSearch;
using AutoMapper;
using Azure.Core;
using MediatR;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class SearchController : Controller
    {
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IRegisterApiClient _registerApiClient;
        private readonly IStaffSearchApiClient _staffSearchApiClient;
        private readonly IFrameworkSearchSessionService _sessionService;
        //private readonly IFrameworkSearchApiClient _frameworkSearchApiClient;
        private readonly IMapper _mapper;

        public SearchController(ILearnerDetailsApiClient learnerDetailsApiClient, IRegisterApiClient registerApiClient, IStaffSearchApiClient staffSearchApiClient,
            IFrameworkSearchSessionService sessionService, IMapper mapper)
        {
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _registerApiClient = registerApiClient;
            _staffSearchApiClient = staffSearchApiClient;
            _sessionService = sessionService;
            _mapper = mapper;

        }

        [HttpGet]
        public IActionResult Index(SearchInputViewModel vm = null)
        {
            if (vm == null)
            {
                vm = new SearchInputViewModel();
            }
            return View(vm);
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

                    FrameworkSearchRequest request = new FrameworkSearchRequest()
                    {
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        DateOfBirth = ValidatorExtensions.ConstructDate(vm.Day, vm.Month, vm.Year).Value,
                        FrameworkResults = new List<FrameworkResultViewModel>
                        {
                            new FrameworkResultViewModel{Id = 1, FrameworkName = "BSE Electrotechnical", FrameworkLevel = "Intermediate", CertificationYear="2016" },
                            new FrameworkResultViewModel{Id = 2, FrameworkName = "BEng Electronic Engineering", FrameworkLevel = "Advanced", CertificationYear="2015" },
                            new FrameworkResultViewModel{Id = 3, FrameworkName = "MSc Electrochemistry", FrameworkLevel = "Higher", CertificationYear="2014" },
                        }
                    };

                    _sessionService.UpdateFrameworkSearchRequest(request);
                    return RedirectToAction("MultipleResults");
                }
            }
            else
            { 
                if (vm.SearchType == SearchTypes.Standards)
                {
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
            var sessionModel = _sessionService.GetFrameworkSearchRequest();
            var viewModel = _mapper.Map<FrameworkSearchResultsViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SelectFramework(FrameworkSearchResultsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var sessionObject = _sessionService.GetFrameworkSearchRequest();
                sessionObject.SelectedResult = vm.SelectedResult;
                _sessionService.UpdateFrameworkSearchRequest(sessionObject);

                return RedirectToAction("MultipleResults");
            }
            var sessionModel = _sessionService.GetFrameworkSearchRequest();
            vm.FrameworkResults = sessionModel.FrameworkResults;
            return View("MultipleResults", vm); 
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
        public DateTime? Date { get; set; }
        public string SearchType { get; set; }
    }

    public static class SearchTypes
    {
        public const string Standards = "Standards";
        public const string Frameworks = "Frameworks";
    }

    public class FrameworkResultViewModel
    {
        public int Id { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkLevel { get; set; }
        public string CertificationYear { get; set; }
    }
    public class FrameworkSearchResultsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<FrameworkResultViewModel> FrameworkResults { get; set; }
        public int FrameworkResultCount => FrameworkResults?.Count ?? 0;
        public int SelectedResult { get; set; }
    }
}