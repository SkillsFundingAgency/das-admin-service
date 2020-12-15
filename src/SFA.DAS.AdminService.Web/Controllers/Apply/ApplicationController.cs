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
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    public class ApplicationController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IAnswerService _answerService;
        private readonly IAnswerInjectionService _answerInjectionService;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IAnswerService answerService, IAnswerInjectionService answerInjectionService, ILogger<ApplicationController> logger)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _contextAccessor = contextAccessor;

            _answerService = answerService;
            _answerInjectionService = answerInjectionService;
            _logger = logger;
        }

        [HttpGet("/Applications/{applicationId}/{backAction}/{backController}/{backOrganisationId?}")]
        public async Task<IActionResult> ActiveSequence(Guid applicationId, BackViewModel backViewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var activeApplySequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, organisation, sequence, sections,
                activeApplySequence.Sections,
                backViewModel.BackAction,
                backViewModel.BackController,
                backViewModel.BackOrganisationId);

            return View(nameof(Sequence), sequenceVm);
        }

        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [HttpGet("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/{backOrganisationId?}")]
        public async Task<IActionResult> Sequence(Guid applicationId, int sequenceNo, BackViewModel backViewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, organisation, sequence, sections,
                applySequence.Sections, backViewModel.BackAction, backViewModel.BackController,
                backViewModel.BackOrganisationId);

            var activeApplicationStatuses = new List<string> { ApplicationStatus.Submitted, ApplicationStatus.Resubmitted };
            var activeSequenceStatuses = new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted };
            if (activeApplicationStatuses.Contains(application.ApplicationStatus) && activeSequenceStatuses.Contains(applySequence?.Status))
            {
                return View(nameof(Sequence), sequenceVm);
            }
            else
            {
                return View($"{nameof(Sequence)}_ReadOnly", sequenceVm);
            }
        }

        [HttpGet("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Section/{sectionNo}/{backOrganisationId?}")]
        public async Task<IActionResult> Section(Guid applicationId, int sequenceNo, int sectionNo, BackViewModel backViewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
            var applySection = applySequence.Sections.Single(x => x.SectionNo == sectionNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var section = await _qnaApiClient.GetSection(application.ApplicationId, applySection.SectionId);

            var sectionVm = new SectionViewModel(application, organisation, section, applySection, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);

            var activeApplicationStatuses = new List<string> { ApplicationStatus.Submitted, ApplicationStatus.Resubmitted };
            var activeSequenceStatuses = new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted };
            if (activeApplicationStatuses.Contains(application.ApplicationStatus) && activeSequenceStatuses.Contains(applySequence?.Status))
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

        [HttpPost("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Section/{sectionNo}/{backOrganisationId?}")]
        public async Task<IActionResult> EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool? isSectionComplete, BackViewModel backViewModel)
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

                var sectionVm = new SectionViewModel(application, organisation, section, applySection, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);

                return View(nameof(Section), sectionVm);
            }

            await _applyApiClient.EvaluateSection(applicationId, sequenceNo, sectionNo, isSectionComplete.Value, _contextAccessor.HttpContext.User.UserDisplayName());
            return RedirectToAction(nameof(ActiveSequence), new { applicationId, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId });
        }

        [HttpGet("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/{backOrganisationId?}")]
        public async Task<IActionResult> Page(Guid applicationId, int sequenceNo, int sectionNo, string pageId, BackViewModel backViewModel)
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

            var pageVm = new PageViewModel(applicationId, sequenceNo, sectionNo, pageId, section, page, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);

            var activeApplicationStatuses = new List<string> { ApplicationStatus.Submitted, ApplicationStatus.Resubmitted };
            var activeSequenceStatuses = new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted };
            if (activeApplicationStatuses.Contains(application.ApplicationStatus) && activeSequenceStatuses.Contains(applySequence?.Status))
            {
                return View(nameof(Page), pageVm);
            }
            else
            {
                return View($"{nameof(Page)}_ReadOnly", pageVm);
            }
        }

        [HttpPost("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/{backOrganisationId?}")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string feedbackMessage, BackViewModel backViewModel)
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
                var pageVm = new PageViewModel(applicationId, sequenceNo, sectionNo, pageId, section, page, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);
                return View(nameof(Page), pageVm);
            }

           var feedback = new QnA.Api.Types.Page.Feedback { Id= Guid.NewGuid(), Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };

           await _qnaApiClient.UpdateFeedback(application.ApplicationId, section.Id, pageId, feedback);

           return RedirectToAction(nameof(Section), new { applicationId, backViewModel.BackAction, backViewModel.BackController, sequenceNo, sectionNo, backViewModel.BackOrganisationId });
        }

        [HttpPost("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/{feedbackId}/{backOrganisationId?}")]
        public async Task<IActionResult> DeleteFeedback(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string feedbackId, BackViewModel backViewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);

            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

            if (!string.IsNullOrEmpty(feedbackId))
                await _qnaApiClient.DeleteFeedback(application.ApplicationId, section.Id, pageId, Guid.Parse(feedbackId));
            else
                _logger.LogError($"Feedback Id is null or empty - {feedbackId}");

            return RedirectToAction(nameof(Page), new { applicationId, backViewModel.BackAction, backViewModel.BackController, sequenceNo, sectionNo, pageId, backViewModel.BackOrganisationId });
        }

        [HttpGet("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Assessment/{backOrganisationId?}")]
        public async Task<IActionResult> Assessment(Guid applicationId, int sequenceNo, BackViewModel backViewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var activeApplicationSequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (activeApplicationSequence is null || activeApplicationSequence.SequenceNo != sequenceNo || 
                activeApplicationSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            {
                // This is to stop the wrong sequence being assessed, or if not all sections are Evaluated
                return RedirectToApplicationsFromSequence(sequenceNo);
            }

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplicationSequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, activeApplicationSequence.SequenceId);

            var viewModel = new ApplicationSequenceAssessmentViewModel(application, sequence, sections, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);
            return View(nameof(Assessment), viewModel);
        }

        [HttpPost("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Return/{backOrganisationId?}")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceNo, string returnType, BackViewModel backViewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var activeApplicationSequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (activeApplicationSequence is null || activeApplicationSequence.SequenceNo != sequenceNo || activeApplicationSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            {
                // This is to stop the wrong sequence being returned, or if not all sections are Evaluated
                return RedirectToApplicationsFromSequence(sequenceNo);
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

                    var viewModel = new ApplicationSequenceAssessmentViewModel(application, sequence, sections, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);
                    return View(nameof(Assessment), viewModel);
                }
            }
           
            var warningMessages = new List<string>();
            if (sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO && returnType == "Approve")
            {
                var sequenceOne = application.ApplyData?.Sequences.FirstOrDefault(seq => seq.SequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO);

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

            var returnedViewModel = new ApplicationReturnedViewModel(applicationId, sequenceNo, warningMessages, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);
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

        private IActionResult RedirectToApplicationsFromSequence(int sequenceNo)
        {
            return RedirectToAction(
                    sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO
                        ? nameof(StandardApplicationController.StandardApplications)
                        : nameof(OrganisationApplicationController.OrganisationApplications),
                    sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO
                        ? nameof(StandardApplicationController).RemoveController()
                        : nameof(OrganisationApplicationController).RemoveController());
        }
    }
}