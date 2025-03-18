﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch;
using AutoMapper;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AdminService.Web.Infrastructure;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class SearchController : Controller
    {
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IRegisterApiClient _registerApiClient;
        private readonly IStaffSearchApiClient _staffSearchApiClient;
        private readonly IFrameworkSearchSessionService _sessionService;
        private readonly IMapper _mapper;

        public SearchController(ILearnerDetailsApiClient learnerDetailsApiClient, IRegisterApiClient registerApiClient, IStaffSearchApiClient staffSearchApiClient,
            IFrameworkSearchSessionService sessionService,  IMapper mapper)
        {
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _registerApiClient = registerApiClient;
            _staffSearchApiClient = staffSearchApiClient;
            _sessionService = sessionService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(SearchInputViewModel vm)
        {
            return View(vm ?? new SearchInputViewModel());
        }

        [HttpGet]
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
                    var searchQuery = _mapper.Map<FrameworkLearnerSearchRequest>(vm);
                    var frameworkResults = await _staffSearchApiClient.SearchFrameworkLearners(searchQuery);

                    if (frameworkResults.Count == 0)
                    {
                        _sessionService.ClearFrameworkSearchRequest();
                        return RedirectToAction(nameof(NoResults),
                            new
                            {
                                FirstName = searchQuery.FirstName,
                                LastName = searchQuery.LastName,
                                DateOfBirth = searchQuery.DateOfBirth
                            });
                    }
                    else if (frameworkResults.Count == 1)
                    { 
                        var searchSessionObject = new FrameworkSearchSession()
                        {
                            FirstName = searchQuery.FirstName,
                            LastName = searchQuery.LastName,
                            DateOfBirth = searchQuery.DateOfBirth,
                            FrameworkResults = _mapper.Map<List<FrameworkLearnerSummaryViewModel>>(frameworkResults),
                            SelectedResult = frameworkResults[0].Id,
                        };

                        _sessionService.SessionFrameworkSearch = searchSessionObject;
                        return RedirectToAction(nameof(FrameworkLearnerDetails));
                    }
                    else
                    {
                        var searchSessionObject = new FrameworkSearchSession()
                        {
                            FirstName = searchQuery.FirstName,
                            LastName = searchQuery.LastName,
                            DateOfBirth = searchQuery.DateOfBirth,
                            FrameworkResults = _mapper.Map<List<FrameworkLearnerSummaryViewModel>>(frameworkResults)
                        };

                        _sessionService.SessionFrameworkSearch = searchSessionObject;
                        return RedirectToAction(nameof(MultipleResults));
                    }
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
            return RedirectToAction(nameof(Index), vm);
        }

        [HttpGet("learner-details")]
        public async Task<IActionResult> LearnerDetails(int stdCode,
            long uln,
            string searchString,
            int page = 1,
            bool allLogs = false,
            int? batchNumber = null)
        {
            var learner = await _learnerDetailsApiClient.GetLearnerDetail(stdCode, uln, allLogs);
            
            var vm = new LearnerDetailsViewModel
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
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult MultipleResults()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.FrameworkResults == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<FrameworkLearnerSearchResultsViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult NoResults(NoResultsViewModel viewModel)
        {
            return View(viewModel);
        }


        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> SelectFrameworkLearner(FrameworkLearnerSearchResultsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.SelectedResult = vm.SelectedResult;
                });
                return RedirectToAction(nameof(FrameworkLearnerDetails));
            }
            return RedirectToAction(nameof(MultipleResults));
        }
        [HttpGet]
        public async Task<IActionResult> FrameworkLearnerDetails()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || ! sessionModel.SelectedResult.HasValue)
            {
                return RedirectToAction("Index");
            }

            var frameworkLearnerDetails = 
                await _learnerDetailsApiClient.GetFrameworkLearner(sessionModel.SelectedResult.Value);

            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
            {
                sessionObject.CertificateNumber = frameworkLearnerDetails.CertificateNumber;
            });

            return View(_mapper.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetails));

        }

        [HttpGet]
        public async Task<IActionResult> FrameworkLearnerDetailsBackAction()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel != null)
            {
                if (sessionModel.FrameworkResults?.Count > 1)
                { 
                    _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                    {
                        sessionObject.SelectedResult = null;
                    });

                    return RedirectToAction(nameof(MultipleResults));
                } 
                _sessionService.ClearFrameworkSearchRequest();
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult FrameworkReprintReason()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedResult == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<FrameworkReprintReasonViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult FrameworkReprintReason(AmendFrameworkReprintReasonViewModel vm)
        {
            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
            {
                sessionObject.SelectedReprintReasons = vm.SelectedReprintReasons;
                sessionObject.TicketNumber = vm.TicketNumber;
                sessionObject.OtherReason = vm.OtherReason;
            });

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(FrameworkAddress));
            }
            else
            { 
                
                return RedirectToAction(nameof(FrameworkReprintReason));   
            }  
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult FrameworkAddress()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedResult == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<FrameworkLearnerAddressViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult FrameworkAddress(FrameworkLearnerAddressViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //Page refreshes, clearing out the entered data 
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.AddressLine1 = string.Empty;
                    sessionObject.AddressLine2 = string.Empty;
                    sessionObject.TownOrCity = string.Empty;
                    sessionObject.County = string.Empty;
                    sessionObject.Postcode = string.Empty;
                });
                return RedirectToAction(nameof(FrameworkAddress));
            }
            else
            { 
                //Save updates to session for the GET method
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.AddressLine1 = vm.AddressLine1;
                    sessionObject.AddressLine2 = vm.AddressLine2;
                    sessionObject.TownOrCity = vm.TownOrCity;
                    sessionObject.County = vm.County;
                    sessionObject.Postcode = vm.Postcode;
                });
                return RedirectToAction(nameof(FrameworkAddress));   
            }  
        }
    }  
}