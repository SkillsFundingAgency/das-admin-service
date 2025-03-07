using Microsoft.AspNetCore.Authorization;
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
                    var searchQuery = _mapper.Map<FrameworkLearnerSearchRequest>(vm);
                    var frameworkResults = await _staffSearchApiClient.SearchFrameworkLearners(searchQuery);

                    if (frameworkResults.Count == 0)
                    {
                        _sessionService.ClearFrameworkSearchRequest();
                        return RedirectToAction("NoResults",
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
                        return RedirectToAction("Certificate");
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
                        return RedirectToAction("MultipleResults");
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
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult MultipleResults()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.FrameworkResults == null)
            {
                return RedirectToAction("Index");
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
                return RedirectToAction("Certificate");
            }
            return RedirectToAction("MultipleResults");
        }
        [HttpGet]
        public async Task<IActionResult> Certificate()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || ! sessionModel.SelectedResult.HasValue)
            {
                return RedirectToAction("Index");
            }

            var certificateDetails = 
                await _learnerDetailsApiClient.GetFrameworkLearner(sessionModel.SelectedResult.Value);

            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
            {
                sessionObject.CertificateNumber = certificateDetails.CertificateNumber;
            });

            return View(_mapper.Map<FrameworkLearnerViewModel>(certificateDetails));

        }

        [HttpGet]
        public async Task<IActionResult> CertificateBackAction()
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

                    return RedirectToAction("MultipleResults");
                } 
                _sessionService.ClearFrameworkSearchRequest();
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult Reprint()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedResult == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = _mapper.Map<FrameworkLearnerReprintReasonViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult UpdateReprintReason(UpdateReprintReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //Page refreshes, temporarily clearing out the entered data until next story is implemented
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.SelectedReprintReasons = new List<string>();
                    sessionObject.TicketNumber = string.Empty;
                    sessionObject.OtherReason = string.Empty;
                });
                return RedirectToAction("Reprint");
            }
            else
            { 
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.SelectedReprintReasons = vm.SelectedReprintReasons;
                    sessionObject.TicketNumber = vm.TicketNumber;
                    sessionObject.OtherReason = vm.OtherReason;
                });
                return RedirectToAction("Reprint");   
            }  
        }
    }  
}