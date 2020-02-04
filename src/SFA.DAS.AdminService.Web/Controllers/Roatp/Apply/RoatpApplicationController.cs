using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
// NOTE: For future work, consider if these base types are acceptable or if we need Roatp versions
using ApplicationSectionStatus = SFA.DAS.AssessorService.ApplyTypes.ApplicationSectionStatus;
using ApplicationSequenceStatus = SFA.DAS.AssessorService.ApplyTypes.ApplicationSequenceStatus;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    public class RoatpApplicationController : Controller
    {
        private readonly IRoatpOrganisationApiClient _apiClient;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IAnswerService _answerService;
        private readonly IAnswerInjectionService _answerInjectionService;
        private readonly ILogger<RoatpApplicationController> _logger;

        public RoatpApplicationController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, 
            IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IAnswerService answerService, 
            IAnswerInjectionService answerInjectionService, ILogger<RoatpApplicationController> logger)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _contextAccessor = contextAccessor;

            _answerService = answerService;
            _answerInjectionService = answerInjectionService;
            _logger = logger;
        }

        [HttpGet("/Roatp/Applications/Midpoint")]
        public async Task<IActionResult> MidpointApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetOpenApplications();
            var paginatedApplications = new PaginatedList<AssessorService.ApplyTypes.Roatp.Apply>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Applications/MidpointApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Applications/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added
            var applications = await _applyApiClient.GetFeedbackAddedApplications();
            
            var paginatedApplications = new PaginatedList<AssessorService.ApplyTypes.Roatp.Apply>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Applications/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Applications/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedApplications();

            var paginatedApplications = new PaginatedList<AssessorService.ApplyTypes.Roatp.Apply>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Applications/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Applications/{applicationId}")]
        public async Task<IActionResult> Application(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var activeApplySequence = application.ApplyData.Sequences.OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            var sequences = await _qnaApiClient.GetAllApplicationSequences(application.ApplicationId);

            var roatpSequences = await _applyApiClient.GetRoatpSequences();

            var taskListViewModel = new RoatpTaskListViewModel(application, organisation, sequences, application.ApplyData.Sequences, roatpSequences);
            return View("~/Views/Roatp/Apply/Applications/TaskList.cshtml", taskListViewModel);
        }

        [HttpGet("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}")]
        public async Task<IActionResult> Sequence(Guid applicationId, int sequenceNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var roatpSequences = await _applyApiClient.GetRoatpSequences();
            var sequenceVm = new RoatpSequenceViewModel(application, organisation, sequence, sections, applySequence.Sections, roatpSequences);

            var activeApplicationStatuses = new List<string> { ApplicationStatus.GatewayAssessed, ApplicationStatus.Resubmitted };
            
            if (activeApplicationStatuses.Contains(application.ApplicationStatus))
            {
                return View("~/Views/Roatp/Apply/Applications/Sequence.cshtml", sequenceVm);
            }
            else
            {
                return View("~/Views/Roatp/Apply/Applications/Sequence_ReadOnly.cshtml", sequenceVm);
            }
        }

        [HttpGet("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}")]
        public async Task<IActionResult> Section(Guid applicationId, int sequenceNo, int sectionNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
            var applySection = applySequence.Sections.Single(x => x.SectionNo == sectionNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var section = await _qnaApiClient.GetSection(application.ApplicationId, applySection.SectionId);

            foreach(var page in section.QnAData.Pages)
            {
                var excludedAnswers = new List<string> { "ApplicationId", "RedirectAction" };
                var excludedQuestions = page.Questions.Where(x => excludedAnswers.Contains(x.QuestionId));
                foreach(var question in excludedQuestions)
                {
                    page.Questions.Remove(question);
                }
            }

            var sectionVm = new RoatpSectionViewModel(application, organisation, section, applySection);

            var activeApplicationStatuses = new List<string> { ApplicationStatus.GatewayAssessed, ApplicationStatus.Resubmitted };
            var activeSequenceStatuses = new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted };
            if (activeApplicationStatuses.Contains(application.ApplicationStatus) && activeSequenceStatuses.Contains(applySequence?.Status))
            {             
                if (applySection.Status != ApplicationSectionStatus.Evaluated)
                {
                    await _applyApiClient.StartApplicationSectionReview(applicationId, sequence.SequenceNo, section.SectionNo, _contextAccessor.HttpContext.User.UserDisplayName());
                }

                return View("~/Views/Roatp//Applications/Section.cshtml", sectionVm);
            }
            else
            {
                return View("~/Views/Roatp/Apply/Applications/Section_ReadOnly.cshtml", sectionVm);
            }
        }

        [HttpPost("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}")]
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
                var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

                var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);
                var applySection = applySequence.Sections.Single(x => x.SectionNo == sectionNo);
                
                var section = await _qnaApiClient.GetSection(application.ApplicationId, applySection.SectionId);

                var sectionVm = new RoatpSectionViewModel(application, organisation, section, applySection);

                return View("~/Views/Roatp/Apply/Applications/Section.cshtml", sectionVm);
            }

            await _applyApiClient.EvaluateSection(applicationId, sequenceNo, sectionNo, isSectionComplete.Value, _contextAccessor.HttpContext.User.UserDisplayName());
            return RedirectToAction("Application", new { applicationId });
        }

        [HttpGet("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}")]
        public async Task<IActionResult> Page(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
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

            var pageVm = new PageViewModel(applicationId, sequenceNo, sectionNo, pageId, section, page);

            var activeApplicationStatuses = new List<string> { ApplicationStatus.Submitted, ApplicationStatus.Resubmitted };
            var activeSequenceStatuses = new List<string> { ApplicationSequenceStatus.Submitted, ApplicationSequenceStatus.Resubmitted };
            if (activeApplicationStatuses.Contains(application.ApplicationStatus) && activeSequenceStatuses.Contains(applySequence?.Status))
            {
                return View("~/Views/Roatp/Apply/Applications/Page.cshtml", pageVm);
            }
            else
            {
                return View("~/Views/Roatp/Apply/Applications/Page_ReadOnly.cshtml", pageVm);
            }
        }

        [HttpPost("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string feedbackMessage)
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
                var pageVm = new PageViewModel(applicationId, sequenceNo, sectionNo, pageId, section, page);
                return View("~/Views/Roatp/Apply/Applications/Page.cshtml", pageVm);
            }

           var feedback = new QnA.Api.Types.Page.Feedback { Id= Guid.NewGuid(), Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };

           await _qnaApiClient.UpdateFeedback(application.ApplicationId, section.Id, pageId, feedback);

           return RedirectToAction("Section", new { applicationId, sequenceNo, sectionNo });
        }

        [HttpPost("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string feedbackId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);

            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

            if (!string.IsNullOrEmpty(feedbackId))
                await _qnaApiClient.DeleteFeedback(application.ApplicationId, section.Id, pageId, Guid.Parse(feedbackId));
            else
                _logger.LogError($"Feedback Id is null or empty - {feedbackId}");

            return RedirectToAction("Page", new { applicationId, sequenceNo, sectionNo, pageId });
        }

        [HttpGet("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Assessment")]
        public async Task<IActionResult> Assessment(Guid applicationId, int sequenceNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var activeApplicationSequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            if (activeApplicationSequence is null || activeApplicationSequence.SequenceNo != sequenceNo || 
                activeApplicationSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            {
                return RedirectToAction(nameof(MidpointApplications));
            }

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplicationSequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, activeApplicationSequence.SequenceId);

            var viewModel = new RoatpApplicationSequenceAssessmentViewModel(application, sequence, sections);
            return View("~/Views/Roatp/Apply/Applications/Assessment.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Applications/{applicationId}/Sequence/{sequenceNo}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceNo, string returnType)
        {
            var warningMessages = new List<string>();

            // REPLACE WITH ROATP LOGIC

            //var application = await _applyApiClient.GetApplication(applicationId);
            //var activeApplicationSequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            //if (activeApplicationSequence is null || activeApplicationSequence.SequenceNo != sequenceNo || activeApplicationSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated && !s.NotRequired))
            //{
            //    // This is to stop the wrong sequence being returned, or if not all sections are Evaluated
            //    return RedirectToAction(sequenceNo == 2 ? nameof(StandardApplications) : nameof(MidpointApplications));
            //}
            //else
            //{
            //    var errorMessages = new Dictionary<string, string>();

            //    if (string.IsNullOrWhiteSpace(returnType))
            //    {
            //        errorMessages["ReturnType"] = "Please state what you would like to do next";
            //    }

            //    if (errorMessages.Any())
            //    {
            //        foreach (var error in errorMessages)
            //        {
            //            ModelState.AddModelError(error.Key, error.Value);
            //        }

            //        var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplicationSequence.SequenceId);
            //        var sections = await _qnaApiClient.GetSections(application.ApplicationId, activeApplicationSequence.SequenceId);

            //        var viewModel = new ApplicationSequenceAssessmentViewModel(application, sequence, sections);
            //        return View("~/Views/Roatp/Apply/Applications/Assessment.cshtml", viewModel);
            //    }
            //}

            //if (sequenceNo == 2 && returnType == "Approve")
            //{
            //    var sequenceOne = application.ApplyData?.Sequences.FirstOrDefault(seq => seq.SequenceNo == 1);

            //    // if sequenceOne is not required (ie, this is a standard application for an existing epao and no financials required) then Inject STANDARD
            //    if (sequenceOne?.NotRequired is true)
            //    {
            //        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Sequence One is NOT REQUIRED - Injecting Standard");
            //        var response = await AddOrganisationStandardIntoRegister(applicationId);
            //        if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);
            //    }
            //    // if sequenceOne IS required (ie, this is a new EPAO or an existing EPAO requiring financials)
            //    else
            //    {
            //        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Sequence One IS REQUIRED.");
            //        var organisation = await _apiClient.GetOrganisation(application.OrganisationId);
            //        _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Got Organisation {organisation.EndPointAssessorName} RoEPAOApproved: {organisation.OrganisationData.RoEPAOApproved}");

            //        //    'Inject' the Organisation and associated contacts if not RoEPAO approved
            //        if (!organisation.OrganisationData.RoEPAOApproved)
            //        {
            //            _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Injecting Organisation");
            //            var response = await AddOrganisationAndContactIntoRegister(applicationId);
            //            if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);    
            //        }

            //        //    'Inject' the Standard which was applied for by the organisation
            //        if (!warningMessages.Any())
            //        {
            //            _logger.LogInformation($"APPROVING_STANDARD - ApplicationId: {applicationId} - Injecting Standard.");
            //            var response = await AddOrganisationStandardIntoRegister(applicationId);
            //            if (response.WarningMessages != null) warningMessages.AddRange(response.WarningMessages);
            //        }
            //    }
            //}

            //if (!warningMessages.Any())
            //{
            //    await _applyApiClient.ReturnApplicationSequence(applicationId, sequenceNo, returnType, _contextAccessor.HttpContext.User.UserDisplayName());
            //}

            var returnedViewModel = new ApplicationReturnedViewModel(applicationId, sequenceNo, warningMessages);
            return View("~/Views/Roatp/Apply/Applications/Returned.cshtml", returnedViewModel);
        }

        [HttpGet("/Roatp/Application/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/Question/{questionId}/{filename}/Download")]
        public async Task<IActionResult> DownloadFile(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string questionId, string filename)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var section = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, sequenceNo, sectionNo);

            var response = await _qnaApiClient.DownloadFile(application.ApplicationId, section.Id, pageId, questionId, filename);
            var fileStream = await response.Content.ReadAsStreamAsync();

            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);
        }
    }
}