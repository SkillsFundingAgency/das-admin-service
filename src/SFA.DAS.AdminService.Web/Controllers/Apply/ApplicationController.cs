using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    public class ApplicationController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IQnaApiClient _qnaApiClient;

        private readonly IApiClient _applyApiClient;
        private readonly IAnswerService _answerService;
        private readonly IAnswerInjectionService _answerInjectionService;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IApiClient apiClient, IQnaApiClient qnaApiClient, IApiClient applyApiClient, IAnswerService answerService, IAnswerInjectionService answerInjectionService, ILogger<ApplicationController> logger)
        {
            _apiClient = apiClient;
            _qnaApiClient = qnaApiClient;

            _applyApiClient = applyApiClient;
            _answerService = answerService;
            _answerInjectionService = answerInjectionService;
            _logger = logger;
        }

        [HttpGet("/Applications/Midpoint")]
        public async Task<IActionResult> MidpointApplications(int page = 1)
        {
            const int midpointSequenceId = 1;
            var applications = await _applyApiClient.GetOpenApplications(midpointSequenceId);

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new DashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Applications/MidpointApplications.cshtml", viewmodel);
        }

        [HttpGet("/Applications/Standard")]
        public async Task<IActionResult> StandardApplications(int page = 1)
        {
            const int standardSequenceId = 2;
            var applications = await _applyApiClient.GetOpenApplications(standardSequenceId);

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new DashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Applications/StandardApplications.cshtml", viewmodel);
        }

        [HttpGet("/Applications/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added
            var applications = await _applyApiClient.GetFeedbackAddedApplications();

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new DashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Applications/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Applications/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedApplications();

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new DashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Applications/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Applications/{applicationId}")]
        public async Task<IActionResult> Application(Guid applicationId)
        {
            // TODO: Implement API end point
            //await _applyApiClient.StartApplicationReview(applicationId, sequence.SequenceNo);

            var application = await _apiClient.GetApplicationFromAssessor(applicationId.ToString());
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            var applyData = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequence.SequenceNo);

            var sequenceVm = new SequenceViewModel(application, organisation, sequence, sections, applyData.Sections);

            return View("~/Views/Apply/Applications/Sequence.cshtml", sequenceVm);
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceNo}")]
        public async Task<IActionResult> Sequence(Guid applicationId, int sequenceNo)
        {
            var application = await _apiClient.GetApplicationFromAssessor(applicationId.ToString());
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);
            var allApplicationSequences = await _qnaApiClient.GetAllApplicationSequences(application.ApplicationId);
            var sequence = allApplicationSequences.Single(x => x.SequenceNo == sequenceNo);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            var applyData = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequence.SequenceNo);

            var sequenceVm = new SequenceViewModel(application, organisation, sequence, sections, applyData.Sections);

            if (application.ApplicationStatus == ApplicationStatus.Submitted || application.ApplicationStatus == ApplicationStatus.Resubmitted)
            {
                return View("~/Views/Apply/Applications/Sequence.cshtml", sequenceVm);
            }
            else
            {
                return View("~/Views/Apply/Applications/Sequence_ReadOnly.cshtml", sequenceVm);
            }
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}")]
        public async Task<IActionResult> Section(Guid applicationId, int sequenceNo, int sectionNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var section = await _applyApiClient.GetSection(applicationId, sequenceNo, sectionNo);
            var sectionVm = new ApplicationSectionViewModel(applicationId, sequenceNo, sectionNo, section, application);

            var sequence = await _applyApiClient.GetSequence(applicationId, sequenceNo);
            if (sequence?.Status == ApplicationSequenceStatus.Submitted)
            {
                return View("~/Views/Apply/Applications/Section.cshtml", sectionVm);
            }
            else
            {
                return View("~/Views/Apply/Applications/Section_ReadOnly.cshtml", sectionVm);
            }
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}")]
        public async Task<IActionResult> EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool? isSectionComplete)
        {
            var errorMessages = new Dictionary<string, string>();

            if (!isSectionComplete.HasValue)
            {
                errorMessages["IsSectionComplete"] = "Please state if this section is completed";
            }

            if (errorMessages.Any())
            {
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                var application = await _applyApiClient.GetApplication(applicationId);
                var section = await _applyApiClient.GetSection(applicationId, sequenceNo, sectionNo);
                var sectionVm = new ApplicationSectionViewModel(applicationId, sequenceNo, sectionNo, section, application);
                return View("~/Views/Apply/Applications/Section.cshtml", sectionVm);
            }

            await _applyApiClient.EvaluateSection(applicationId, sequenceNo, sectionNo, isSectionComplete.Value);

            return RedirectToAction("Application", new { applicationId });
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}")]
        public async Task<IActionResult> Page(Guid applicationId, int sequenceId, int sectionId, string pageId)
        {
            var page = await _applyApiClient.GetPage(applicationId, sequenceId, sectionId, pageId);

            if (page?.Active == false || page?.NotRequired == true)
            {
                // DO NOT show any information
                page = null;
            }

            var pageVm = new PageViewModel(applicationId, sequenceId, sectionId, pageId, page);

            var sequence = await _applyApiClient.GetSequence(applicationId, sequenceId);
            if (sequence?.Status == ApplicationSequenceStatus.Submitted)
            {
                return View("~/Views/Apply/Applications/Page.cshtml", pageVm);
            }
            else
            {
                return View("~/Views/Apply/Applications/Page_ReadOnly.cshtml", pageVm);
            }
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceId, int sectionId, string pageId, string feedbackMessage)
        {
            var errorMessages = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(feedbackMessage))
            {
                errorMessages["FeedbackMessage"] = "Please enter a feedback comment";
            }

            if (errorMessages.Any())
            {
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                var page = await _applyApiClient.GetPage(applicationId, sequenceId, sectionId, pageId);
                var pageVm = new PageViewModel(applicationId, sequenceId, sectionId, pageId, page);
                return View("~/Views/Apply/Applications/Page.cshtml", pageVm);
            }

            Feedback feedback = new Feedback { Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };

            await _applyApiClient.AddFeedback(applicationId, sequenceId, sectionId, pageId, feedback);

            return RedirectToAction("Section", new { applicationId, sequenceId, sectionId });
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/DeleteFeedback")]
        public async Task<IActionResult> DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            await _applyApiClient.DeleteFeedback(applicationId, sequenceId, sectionId, pageId, feedbackId);

            return RedirectToAction("Page", new { applicationId, sequenceId, sectionId, pageId });
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Assessment")]
        public async Task<IActionResult> Assessment(Guid applicationId, int sequenceId)
        {
            var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);

            if (activeSequence is null || activeSequence.SequenceId != sequenceId || activeSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated))
            {
                // This is to stop the wrong sequence being approved or if not all sections are Evaluated
                return RedirectToAction("OpenApplications");
            }

            var viewModel = new ApplicationSequenceAssessmentViewModel(activeSequence);
            return View("~/Views/Apply/Applications/Assessment.cshtml", viewModel);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceId, string returnType)
        {
            var errorMessages = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(returnType))
            {
                errorMessages["ReturnType"] = "Please state what you would like to do next";
            }

            if (errorMessages.Any())
            {
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);
                var viewModel = new ApplicationSequenceAssessmentViewModel(activeSequence);
                return View("~/Views/Apply/Applications/Assessment.cshtml", viewModel);
            }

            var warningMessages = new List<string>();
            if (sequenceId == 2 && returnType == "Approve")
            {
                var sequenceOne = await _applyApiClient.GetSequence(applicationId, 1);

                // if sequenceOne is not required (ie, this is a standard application for an existing epao and no financials required)
                //    Inject STANDARD
                if (sequenceOne?.NotRequired is true)
                {
                    _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Sequence One is NOT REQUIRED. Injecting Standard");
                    var response = await AddOrganisationStandardIntoRegister(applicationId);
                    if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);
                }
                // if sequenceOne IS required (ie, this is a new EPAO or an existing EPAO requiring financials)
                else
                {
                    _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Sequence One IS REQUIRED.");
                    var organisation = await _applyApiClient.GetOrganisationForApplication(applicationId);
                    _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Got Organisation {organisation.EndPointAssessorName} RoEPAOApproved: {organisation.OrganisationData.RoEPAOApproved}");
                    //    If RoEPAOApproved = false
                    if (!organisation.OrganisationData.RoEPAOApproved)
                    {
                        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Injecting Organisation");
                        //        Inject Organisation
                        var response = await AddOrganisationAndContactIntoRegister(applicationId);
                        if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);    
                        if (!warningMessages.Any())
                        {
                            _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Updating RoEPAOApproved flag to True.");
                            await _applyApiClient.UpdateRoEpaoApprovedFlag(applicationId, response.ContactId, response.OrganisationId, true);
                        }
                    }

                    //    Inject Standard
                    if (!warningMessages.Any())
                    {
                        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Injecting standard.");
                        var response2 = await AddOrganisationStandardIntoRegister(applicationId);
                        if (response2.WarningMessages != null) warningMessages.AddRange(response2.WarningMessages);
                    }
                }
            }

            await _applyApiClient.ReturnApplication(applicationId, sequenceId, returnType);

            return RedirectToAction("Returned", new { applicationId, sequenceId, warningMessages});
        }

        private async Task<CreateOrganisationAndContactFromApplyResponse> AddOrganisationAndContactIntoRegister(Guid applicationId)
        {
            _logger.LogInformation($"Attempting to inject organisation into register for application {applicationId}");
            var command = await _answerService.GatherAnswersForOrganisationAndContactForApplication(applicationId);
            return await _answerInjectionService.InjectApplyOrganisationAndContactDetailsIntoRegister(command);
        }

        private async Task<CreateOrganisationStandardFromApplyResponse> AddOrganisationStandardIntoRegister(Guid applicationId)
        {
            _logger.LogInformation($"Attempting to inject standard into register for application {applicationId}");
            var command = await _answerService.GatherAnswersForOrganisationStandardForApplication(applicationId);
            return await _answerInjectionService.InjectApplyOrganisationStandardDetailsIntoRegister(command);
        }

        [HttpGet("/Applications/Returned")]
        public IActionResult Returned(Guid applicationId, int sequenceId, List<string> warningMessages)
        {
            var viewModel = new ApplicationReturnedViewModel(applicationId, sequenceId, warningMessages);
            return View("~/Views/Apply/Applications/Returned.cshtml", viewModel);
        }

        [HttpGet("Application/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}/Download")]
        //[HttpGet("/Application/{applicationId}/Page/{pageId}/Question/{questionId}/File/{filename}/Download")]
        public async Task<IActionResult> Download(Guid applicationId, int sequenceId, int sectionId, string pageId, string questionId, string filename)
        {
            var userId = Guid.NewGuid();

            var fileInfo = await _applyApiClient.FileInfo(applicationId, userId, sequenceId, sectionId, pageId, questionId, filename);

            var file = await _applyApiClient.Download(applicationId, userId, sequenceId, sectionId, pageId, questionId, filename);

            var fileStream = await file.Content.ReadAsStreamAsync();

            return File(fileStream, fileInfo.ContentType, fileInfo.Filename);
        }
    }
}