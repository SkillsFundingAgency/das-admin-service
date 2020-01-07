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
    [Route("roatp-review/{applicationId:Guid}/assessor-moderation")]
    public class RoatpAssessorModerationController : Controller
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public RoatpAssessorModerationController(IApplyApiClient applyApiClient, IQnaApiClient qnaApliClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApliClient;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartReview([FromRoute] Guid applicationId)
        {
            await _applyApiClient.UpdateApplicationReviewAssessorModerationAsync(applicationId, ApplicationReviewStatus.InProgress);

            return RedirectToAction("Index", "RoatpAssessorDashboard");
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromRoute] Guid applicationId)
        {
            var reviewTask = _applyApiClient.GetApplicationReviewAsync(applicationId);
            var applicationTask = _applyApiClient.GetApplicationAsync(applicationId);

            await Task.WhenAll(reviewTask, applicationTask);

            var application = applicationTask.Result;
            var review = reviewTask.Result;

            const int ASSESSOR_SEQUENCE = 4;
            const int ASSESSOR_SECTION = 2;
            const string ASSESSOR_PAGE = "4010";

            var section = await _qnaApiClient.GetSectionBySectionNo(application.Id, ASSESSOR_SEQUENCE, ASSESSOR_SECTION);
            var page = await _qnaApiClient.GetPage(application.Id, section.Id, ASSESSOR_PAGE);

            var assessorReview1CommentsForPage = review.AssessorReview1Comments?.Comments.SingleOrDefault(c => c.SectionId == section.Id && c.PageId == page.PageId)?.Comment;
            var assessorReview2CommentsForPage = review.AssessorReview2Comments?.Comments.SingleOrDefault(c => c.SectionId == section.Id && c.PageId == page.PageId)?.Comment;

            var vm = new RoatpAssessorModerationViewModel(application.Id, ASSESSOR_SEQUENCE, section.SectionNo, page.PageId, section, page,
                review.AssessorReview1Status, assessorReview1CommentsForPage, review.AssessorReview2Status, assessorReview2CommentsForPage);
            
            return View("~/Views/RoatpAssessor/RoatpAssessorModeration/Review.cshtml", vm);
        }

        [HttpPost("Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/Feedback")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string feedbackMessage)
        {
            var reviewTask = _applyApiClient.GetApplicationReviewAsync(applicationId);
            var applicationTask = _applyApiClient.GetApplicationAsync(applicationId);

            await Task.WhenAll(reviewTask, applicationTask);

            var application = applicationTask.Result;
            var review = reviewTask.Result;

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

                var assessorReview1CommentsForPage = review.AssessorReview1Comments?.Comments.SingleOrDefault(c => c.SectionId == section.Id && c.PageId == page.PageId)?.Comment;
                var assessorReview2CommentsForPage = review.AssessorReview2Comments?.Comments.SingleOrDefault(c => c.SectionId == section.Id && c.PageId == page.PageId)?.Comment;

                var vm = new RoatpAssessorModerationViewModel(application.Id, sequence.SequenceNo, section.SectionNo, page.PageId, section, page,
                    review.AssessorReview1Status, assessorReview1CommentsForPage, review.AssessorReview2Status, assessorReview2CommentsForPage);

                return View("~/Views/RoatpAssessor/RoatpAssessorModeration/Review.cshtml", vm);
            }

            var feedback = new QnA.Api.Types.Page.Feedback { Id = Guid.NewGuid(), Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };
            
            var pg = await _qnaApiClient.UpdateFeedback(application.Id, section.Id, pageId, feedback);
            //await _applyApiClient.UpdateApplicationReviewAssessorModerationAsync(application.Id, ApplicationReviewStatus.Feedback);

            return RedirectToAction("Index", "RoatpAssessorDashboard");
        }
    }
}