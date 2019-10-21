using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorTeam)]
    [Route("roatp-review/{applicationId:Guid}/assessor")]
    public class RoatpAssessorReviewController : Controller
    {
        private const string TempDataKey_ReviewNo = "ReviewNo";

        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public RoatpAssessorReviewController(IApplyApiClient applyApiClient, IQnaApiClient qnaApliClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApliClient;
        }

        [HttpPost("start/{reviewNo:int}")]
        public async Task<IActionResult> StartReview([FromRoute] Guid applicationId, [FromRoute] AssessorReviewNo reviewNo)
        {
            await _applyApiClient.UpdateApplicationReviewAssessorReviewAsync(applicationId, reviewNo, ApplicationReviewStatus.InProgress);

            return RedirectToAction("Index", "RoatpAssessorDashboard");
        }

        [HttpGet("reviewNo:int")]
        public async Task<IActionResult> Index([FromRoute] Guid applicationId, AssessorReviewNo reviewNo)
        {
            TempData[TempDataKey_ReviewNo] = reviewNo;

            var reviewTask = _applyApiClient.GetApplicationReviewAsync(applicationId);
            var applicationTask = _applyApiClient.GetApplicationAsync(applicationId);

            await Task.WhenAll(reviewTask, applicationTask);

            var application = applicationTask.Result;

            const int ASSESSOR_SEQUENCE = 4;
            const int ASSESSOR_SECTION = 2;
            const string ASSESSOR_PAGE = "4010";

            var section = await _qnaApiClient.GetSectionBySectionNo(application.Id, ASSESSOR_SEQUENCE, ASSESSOR_SECTION);
            var page = await _qnaApiClient.GetPage(application.Id, section.Id, ASSESSOR_PAGE);

            var vm = new RoatpAssessorReviewViewModel(application.Id, reviewNo, ASSESSOR_SEQUENCE, section.SectionNo, page.PageId, section, page);
            
            return View("~/Views/RoatpAssessor/RoatpAssessorReview/Review.cshtml", vm);
        }

        [HttpPost("Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/Feedback")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string feedbackMessage)
        {
            var reviewTask = _applyApiClient.GetApplicationReviewAsync(applicationId);
            var applicationTask = _applyApiClient.GetApplicationAsync(applicationId);

            await Task.WhenAll(reviewTask, applicationTask);

            var application = applicationTask.Result;

            var allApplicationSequences = await _qnaApiClient.GetAllApplicationSequences(application.Id);
            var sequence = allApplicationSequences.Single(x => x.SequenceNo == sequenceNo);
            var sections = await _qnaApiClient.GetSections(application.Id, sequence.Id);
            var section = sections.Single(x => x.SectionNo == sectionNo);

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

                var page = await _qnaApiClient.GetPage(application.Id, section.Id, pageId);

                var vm = new RoatpAssessorPageViewModel(application.Id, sequence.SequenceNo, section.SectionNo, page.PageId, section, page);

                return View("~/Views/RoatpAssessor/RoatpAssessorReview/Review.cshtml", vm);
            }

            AssessorReviewNo reviewNo = (AssessorReviewNo)TempData[TempDataKey_ReviewNo];
            await _applyApiClient.UpdateAssessorComments(application.Id, reviewNo, section.Id, pageId, feedbackMessage);
            await _applyApiClient.UpdateApplicationReviewAssessorReviewAsync(application.Id, reviewNo, ApplicationReviewStatus.Failed);

            return RedirectToAction("Index", "RoatpAssessorDashboard");
        }
    }
}