using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Extensions;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    public class ApplicationController : Controller
    {
        private readonly IApplicationsSession _applicationsSession;

        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IAnswerService _answerService;
        private readonly IAnswerInjectionService _answerInjectionService;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IApplicationsSession applicationsSession, IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IAnswerService answerService, IAnswerInjectionService answerInjectionService, ILogger<ApplicationController> logger)
        {
            _applicationsSession = applicationsSession;

            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _contextAccessor = contextAccessor;

            _answerService = answerService;
            _answerInjectionService = answerInjectionService;
            _logger = logger;
        }

        [HttpGet("/Applications/{applicationId}/{applicationType}")]
        public IActionResult ActiveSequence(Guid applicationId, string applicationType)
        {
            return RedirectToAction(nameof(ApplicationController.ActiveSequence),
                ApplicationTypeController(applicationType), new { applicationId });
        }

        [HttpGet("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}")]
        public IActionResult Sequence(Guid applicationId, string applicationType, int sequenceNo)
        {
            return RedirectToAction(nameof(ApplicationController.Sequence),
                ApplicationTypeController(applicationType), new { applicationId, sequenceNo });
        }

        [HttpGet("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Section/{sectionNo}")]
        public async Task<IActionResult> Section(Guid applicationId, string applicationType, int sequenceNo, int sectionNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
            var applySection = applySequence.Sections.Single(x => x.SectionNo == sectionNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var section = await _qnaApiClient.GetSection(application.ApplicationId, applySection.SectionId);

            var sectionVm = new SectionViewModel(application, applicationType, organisation, section, applySection);

            if (application.ApplicationStatus == ApplicationStatus.Submitted || application.ApplicationStatus == ApplicationStatus.Resubmitted)
            {             
                if (applySection.Status != ApplicationSectionStatus.Evaluated)
                {
                    await _applyApiClient.StartApplicationSectionReview(applicationId, sequence.SequenceNo, section.SectionNo, _contextAccessor.HttpContext.User.UserDisplayName());
                }

                return View(nameof(Section), sectionVm);
            }
            else
            {
                return View($"{nameof(Section)}_ReadOnly", sectionVm);
            }
        }

        [HttpPost("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Section/{sectionNo}")]
        public async Task<IActionResult> EvaluateSection(Guid applicationId, string applicationType, int sequenceNo, int sectionNo, bool? isSectionComplete)
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
                var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

                var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
                var applySection = applySequence.Sections.Single(x => x.SectionNo == sectionNo);
                
                var section = await _qnaApiClient.GetSection(application.ApplicationId, applySection.SectionId);

                var sectionVm = new SectionViewModel(application, applicationType, organisation, section, applySection);

                return View(nameof(Section), sectionVm);
            }

            await _applyApiClient.EvaluateSection(applicationId, sequenceNo, sectionNo, isSectionComplete.Value, _contextAccessor.HttpContext.User.UserDisplayName());
            return RedirectToAction(nameof(ActiveSequence), new { applicationId, applicationType });
        }

        [HttpGet("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}")]
        public async Task<IActionResult> Page(Guid applicationId, string applicationType, int sequenceNo, int sectionNo, string pageId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
            var applySection = applySequence.Sections.Single(x => x.SectionNo == sectionNo);

            var section = await _qnaApiClient.GetSection(application.ApplicationId, applySection.SectionId);
            var page = await _qnaApiClient.GetPage(application.ApplicationId, section.Id, pageId);

            if (page?.Active is false)
            {
                // DO NOT show any information
                page = null;
            }

            var pageVm = new PageViewModel(applicationId, applicationType, sequenceNo, sectionNo, pageId, section, page);

            if (applySequence?.Status == ApplicationSequenceStatus.Submitted)
            {
                return View(nameof(Page), pageVm);
            }
            else
            {
                return View($"{nameof(Page)}_ReadOnly", pageVm);
            }
        }

        [HttpPost("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}")]
        public async Task<IActionResult> Feedback(Guid applicationId, string applicationType, int sequenceNo, int sectionNo, string pageId, string feedbackMessage)
        {
            var application = await _applyApiClient.GetApplication(applicationId);

            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

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

                var page = await _qnaApiClient.GetPage(application.ApplicationId, section.Id, pageId);
                var pageVm = new PageViewModel(applicationId, applicationType, sequenceNo, sectionNo, pageId, section, page);
                return View(nameof(Page), pageVm);
            }

           var feedback = new QnA.Api.Types.Page.Feedback { Id= Guid.NewGuid(), Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };

           await _qnaApiClient.UpdateFeedback(application.ApplicationId, section.Id, pageId, feedback);

           return RedirectToAction(nameof(Section), new { applicationId, applicationType, sequenceNo, sectionNo });
        }

        [HttpPost("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(Guid applicationId, string applicationType, int sequenceNo, int sectionNo, string pageId, string feedbackId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);

            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

            if (!string.IsNullOrEmpty(feedbackId))
                await _qnaApiClient.DeleteFeedback(application.ApplicationId, section.Id, pageId, Guid.Parse(feedbackId));
            else
                _logger.LogError($"Feedback Id is null or empty - {feedbackId}");

            return RedirectToAction(nameof(Page), new { applicationId, applicationType, sequenceNo, sectionNo, pageId });
        }

        [HttpGet("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Assessment")]
        public async Task<IActionResult> Assessment(Guid applicationId, string applicationType, int sequenceNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var activeApplicationSequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (activeApplicationSequence is null || activeApplicationSequence.SequenceNo != sequenceNo || 
                activeApplicationSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            {
                // This is to stop the wrong sequence being assessed, or if not all sections are Evaluated
                return RedirectToAction(sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO 
                    ? nameof(StandardApplicationController.StandardApplications) 
                    : nameof(OrganisationApplicationController.OrganisationApplications));
            }

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplicationSequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, activeApplicationSequence.SequenceId);

            var viewModel = new ApplicationSequenceAssessmentViewModel(application, applicationType, sequence, sections);
            return View(nameof(Assessment), viewModel);
        }

        [HttpPost("/Applications/{applicationId}/{applicationType}/Sequence/{sequenceNo}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, string applicationType, int sequenceNo, string returnType)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var activeApplicationSequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (activeApplicationSequence is null || activeApplicationSequence.SequenceNo != sequenceNo || activeApplicationSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            {
                // This is to stop the wrong sequence being returned, or if not all sections are Evaluated
                return RedirectToAction(sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO 
                    ? nameof(StandardApplicationController.StandardApplications) 
                    : nameof(OrganisationApplicationController.OrganisationApplications));
            }
            else
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

                    var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplicationSequence.SequenceId);
                    var sections = await _qnaApiClient.GetSections(application.ApplicationId, activeApplicationSequence.SequenceId);

                    var viewModel = new ApplicationSequenceAssessmentViewModel(application, applicationType, sequence, sections);
                    return View(nameof(Assessment), viewModel);
                }
            }
           
            var warningMessages = new List<string>();
            if (sequenceNo == 2 && returnType == "Approve")
            {
                var sequenceOne = application.ApplyData?.Sequences.FirstOrDefault(seq => seq.SequenceNo == 1);

                // if sequenceOne is not required (ie, this is a standard application for an existing epao and no financials required) then Inject STANDARD
                if (sequenceOne?.NotRequired is true)
                {
                    _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Sequence One is NOT REQUIRED - Injecting Standard");
                    var response = await AddOrganisationStandardIntoRegister(applicationId);
                    if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);
                }
                // if sequenceOne IS required (ie, this is a new EPAO or an existing EPAO requiring financials)
                else
                {
                    _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Sequence One IS REQUIRED.");
                    var organisation = await _apiClient.GetOrganisation(application.OrganisationId);
                    _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Got Organisation {organisation.EndPointAssessorName} RoEPAOApproved: {organisation.OrganisationData.RoEPAOApproved}");

                    //    'Inject' the Organisation and associated contacts if not RoEPAO approved
                    if (!organisation.OrganisationData.RoEPAOApproved)
                    {
                        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Injecting Organisation");
                        var response = await AddOrganisationAndContactIntoRegister(applicationId);
                        if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);    
                    }

                    //    'Inject' the Standard which was applied for by the organisation
                    if (!warningMessages.Any())
                    {
                        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Injecting Standard.");
                        var response = await AddOrganisationStandardIntoRegister(applicationId);
                        if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);
                    }
                }
            }

            if (!warningMessages.Any())
            {
                await _applyApiClient.ReturnApplicationSequence(applicationId, sequenceNo, returnType, _contextAccessor.HttpContext.User.UserDisplayName());
            }

            var returnedViewModel = new ApplicationReturnedViewModel(applicationId, applicationType, sequenceNo, warningMessages);
            return View("Returned", returnedViewModel);
        }

        [HttpGet("Application/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/Question/{questionId}/{filename}/Download")]
        public async Task<IActionResult> DownloadFile(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string questionId, string filename)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

            var response = await _qnaApiClient.DownloadFile(application.ApplicationId, section.Id, pageId, questionId, filename);
            var fileStream = await response.Content.ReadAsStreamAsync();

            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);
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

        private string ApplicationTypeController(string applicationType)
        {
            switch (applicationType)
            {
                case ApplyConst.ORGANISATION_APPLICATION_TYPE:
                    return nameof(OrganisationApplicationController).RemoveController();
                case ApplyConst.STANDARD_APPLICATION_TYPE:
                    return nameof(StandardApplicationController).RemoveController();
            }

            return string.Empty;
        }
    }
}