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
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Api.Types.Enums;
using System.Linq;
using System;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class SearchController : Controller
    {
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IRegisterApiClient _registerApiClient;
        private readonly IStaffSearchApiClient _staffSearchApiClient;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IFrameworkSearchSessionService _sessionService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public SearchController(
            ILearnerDetailsApiClient learnerDetailsApiClient, 
            IRegisterApiClient registerApiClient, 
            IStaffSearchApiClient staffSearchApiClient,
            IFrameworkSearchSessionService sessionService, 
            ICertificateApiClient certificateApiClient,
            IScheduleApiClient scheduleApiClient,
            IMapper mapper,
            IHttpContextAccessor contextAccessor)
        {
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _registerApiClient = registerApiClient;
            _staffSearchApiClient = staffSearchApiClient;
            _sessionService = sessionService;
            _certificateApiClient = certificateApiClient;
            _scheduleApiClient = scheduleApiClient;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
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

                    var searchViewModel = new StandardLearnerSearchResultsViewModel
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
                        return RedirectToAction(nameof(FrameworkNoResults),
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
                            FrameworkResults = _mapper.Map<List<FrameworkResultViewModel>>(frameworkResults),
                            SelectedFrameworkLearnerId = frameworkResults[0].Id,
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
                            FrameworkResults = _mapper.Map<List<FrameworkResultViewModel>>(frameworkResults)
                        };

                        _sessionService.SessionFrameworkSearch = searchSessionObject;
                        return RedirectToAction(nameof(FrameworkMultipleResults));
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
            
            var vm = new StandardLearnerDetailsViewModel
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
        public IActionResult FrameworkMultipleResults()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.FrameworkResults == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<FrameworkMultipleResultsViewModel>(sessionModel);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult FrameworkNoResults(FrameworkNoResultsViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult SelectFrameworkLearner(FrameworkMultipleResultsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                {
                    sessionObject.SelectedFrameworkLearnerId = vm.SelectedResult;
                });
                return RedirectToAction(nameof(FrameworkLearnerDetails));
            }
            return RedirectToAction(nameof(FrameworkMultipleResults));
        }

        [HttpGet]
        public async Task<IActionResult> FrameworkLearnerDetails(Guid? frameworkLearnerId = null, int? batchNumber = null, bool allLogs = false)
        {
            FrameworkSearchSession sessionModel;
            GetFrameworkLearnerResponse frameworkLearnerDetails; 
            if (frameworkLearnerId.HasValue)
            {
                frameworkLearnerDetails = await _learnerDetailsApiClient.GetFrameworkLearner(frameworkLearnerId.Value, allLogs);
            }
            else
            { 
                sessionModel = _sessionService.SessionFrameworkSearch;
                if (sessionModel == null || !sessionModel.SelectedFrameworkLearnerId.HasValue)
                {
                    return RedirectToAction("Index");
                }

                frameworkLearnerDetails = await _learnerDetailsApiClient.GetFrameworkLearner(sessionModel.SelectedFrameworkLearnerId.Value, allLogs);
            }

            var viewModel = _mapper.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetails); 
            viewModel.ShowDetails = !allLogs;
            viewModel.BatchNumber = batchNumber; 

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult FrameworkLearnerDetailsBackAction()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel != null)
            {
                if (sessionModel.FrameworkResults?.Count > 1)
                {
                    _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
                    {
                        sessionObject.SelectedFrameworkLearnerId = null;
                    });

                    return RedirectToAction(nameof(FrameworkMultipleResults));
                } 
                _sessionService.ClearFrameworkSearchRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ResetBackToCheckAnswersAndRedirect(string backActionTarget)
        {
            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
            {
                sessionObject.BackToCheckAnswers = false;
            });

            return RedirectToAction(backActionTarget);
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> FrameworkReprintReason(bool backToCheckAnswers = false)
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedFrameworkLearnerId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
            {
                sessionObject.BackToCheckAnswers = backToCheckAnswers;
            });

            var frameworkLearnerDetails =
                await _learnerDetailsApiClient.GetFrameworkLearner(sessionModel.SelectedFrameworkLearnerId.Value, false);

            var viewModel = _mapper.Map<FrameworkReprintReasonViewModel>(sessionModel);
            viewModel.BackAction = backToCheckAnswers ? nameof(FrameworkReprintCheckDetails) : nameof(FrameworkLearnerDetails);
            viewModel.CertificateReference = frameworkLearnerDetails.CertificateReference;
            viewModel.CertificateStatus = frameworkLearnerDetails.CertificateStatus;
            viewModel.CertificatePrintStatusAt = frameworkLearnerDetails.CertificatePrintStatusAt;

            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult FrameworkReprintReason(FrameworkReprintReasonViewModel vm)
        {
            var backToCheckAnswers = _sessionService.SessionFrameworkSearch.BackToCheckAnswers;

            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
            {
                sessionObject.SelectedReprintReasons = vm.SelectedReprintReasons;
                sessionObject.TicketNumber = vm.TicketNumber;
                sessionObject.OtherReason = vm.OtherReason;
                sessionObject.BackToCheckAnswers = false;
            });

            if (ModelState.IsValid)
            {
                return backToCheckAnswers ? RedirectToAction(nameof(FrameworkReprintCheckDetails)) : RedirectToAction(nameof(FrameworkReprintAddress));
            }
            else
            { 
                return RedirectToAction(nameof(FrameworkReprintReason), new { backToCheckAnswers});   
            }  
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult FrameworkReprintAddress(bool backToCheckAnswers = false)
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedFrameworkLearnerId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
           {
               sessionObject.BackToCheckAnswers = backToCheckAnswers;
           });

            var viewModel = _mapper.Map<FrameworkReprintAddressViewModel>(sessionModel);
            viewModel.BackAction = backToCheckAnswers ? nameof(FrameworkReprintCheckDetails) : nameof(FrameworkReprintReason);
            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult FrameworkReprintAddress(FrameworkReprintAddressViewModel vm)
        {
            var backToCheckAnswers = _sessionService.SessionFrameworkSearch.BackToCheckAnswers;
            _sessionService.UpdateFrameworkSearchRequest((sessionObject) =>
               {
                   sessionObject.AddressLine1 = vm.AddressLine1;
                   sessionObject.AddressLine2 = vm.AddressLine2;
                   sessionObject.TownOrCity = vm.TownOrCity;
                   sessionObject.County = vm.County;
                   sessionObject.Postcode = vm.Postcode;
                   sessionObject.BackToCheckAnswers = false;
               });

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(FrameworkReprintCheckDetails));
            }
            else
            {
                return RedirectToAction(nameof(FrameworkReprintAddress), new { backToCheckAnswers});
            }
        }

        [HttpGet]
        public async Task<IActionResult> FrameworkReprintCheckDetails()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedFrameworkLearnerId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var frameworkLearner =
                await _learnerDetailsApiClient.GetFrameworkLearner(sessionModel.SelectedFrameworkLearnerId.Value, false);

            var viewModel = new FrameworkReprintCheckDetailsViewModel()
            {
                LearnerDetails = _mapper.Map<FrameworkLearnerDetailsViewModel>(frameworkLearner),
                AddressDetails = _mapper.Map<FrameworkReprintAddressViewModel>(sessionModel),
                ReprintDetails = _mapper.Map<FrameworkReprintReasonViewModel>(sessionModel)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> FrameworkReprintSubmit()
        {
            var sessionModel = _sessionService.SessionFrameworkSearch;
            if (sessionModel == null || sessionModel.SelectedFrameworkLearnerId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var username = _contextAccessor.HttpContext.User.UserId();

            var reprintFrameworkRequest = new ReprintFrameworkCertificateRequest
            {
                FrameworkLearnerId = sessionModel.SelectedFrameworkLearnerId.Value,
                Username = username,
                IncidentNumber = sessionModel.TicketNumber,
                Reasons = ParseReprintReasons(sessionModel.SelectedReprintReasons),
                OtherReason = sessionModel.OtherReason,
                ContactName = $"{sessionModel.FirstName} {sessionModel.LastName}",
                ContactAddLine1 = sessionModel.AddressLine1,
                ContactAddLine2 = sessionModel.AddressLine2,
                ContactAddLine3 = sessionModel.TownOrCity,
                ContactAddLine4 = sessionModel.County,
                ContactPostcode = sessionModel.Postcode,
            };

            await _certificateApiClient.ReprintFramework(reprintFrameworkRequest);

            _sessionService.ClearFrameworkSearchRequest();
            
            return RedirectToAction(nameof(FrameworkReprintSubmitted));
        }

        [HttpGet("FrameworkReprintSubmitted")]
        public async Task<IActionResult> FrameworkReprintSubmitted()
        {
            var nextScheduledRun = await _scheduleApiClient.GetNextScheduledRun((int)ScheduleType.PrintRun);
            return View(new FrameworkReprintSubmittedViewModel { PrintRunDate = nextScheduledRun?.RunTime.ToSfaShortDateString() ?? "Unknown" });
        }

        private static ReprintReasons? ParseReprintReasons(List<string> reasons)
        {
            var reprintReasons = string.Join(",", reasons.Where(p => !p.Equals("Other")).ToList());
            
            return !string.IsNullOrWhiteSpace(reprintReasons)
                ? (ReprintReasons?)Enum.Parse(typeof(ReprintReasons), reprintReasons)
                : null;
        }
    }
}