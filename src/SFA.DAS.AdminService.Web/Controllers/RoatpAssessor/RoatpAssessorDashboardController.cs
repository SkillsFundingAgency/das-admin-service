using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor;
using SFA.DAS.RoatpAssessor.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorTeam)]
    public class RoatpAssessorDashboardController : Controller
    {
        private readonly IApplyApiClient _applyApiClient;

        public RoatpAssessorDashboardController(IApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applicationsTask = _applyApiClient.GetSubmittedApplicationsAsync();
            var reviewsTask = _applyApiClient.GetActiveApplicationReviewsAsync();

            await Task.WhenAll(applicationsTask, reviewsTask);

            var reviews = reviewsTask.Result;

            var gatewayReviewsPending = reviews.Where(r => r.GatewayReviewCanBeStarted).Select(r => r.ApplicationId);
            var gatewayReviewsInProgress = reviews.Where(r => r.GatewayReviewIsInProgress).Select(r => r.ApplicationId);
            var pmoReviewsPending = reviews.Where(r => r.CanStartPmoReview).Select(r => r.ApplicationId);
            var pmoReviewsUnderReview = reviews.Where(r => r.CanUpdatePmoReview).Select(r => r.ApplicationId);
            var assessor1ReviewPending = reviews.Where(r => r.CanStartAssessor1Review).Select(r => r.ApplicationId);
            var assessor1ReviewUnderReview = reviews.Where(r => r.CanUpdateAssessor1Review).Select(r => r.ApplicationId);
            var assessor2ReviewPending = reviews.Where(r => r.CanStartAssessor2Review).Select(r => r.ApplicationId);
            var assessor2ReviewUnderReview = reviews.Where(r => r.CanUpdateAssessor2Review).Select(r => r.ApplicationId);
            var assessorModerationPending = reviews.Where(r => r.CanStartAssessorModeration).Select(r => r.ApplicationId);
            var assessorModerationUnderReview = reviews.Where(r => r.CanUpdateAssessorModeration).Select(r => r.ApplicationId);

            var vm = new RoatpAssessorDashboardIndexViewModel
            {
                SubmittedApplications = applicationsTask.Result.Select(a => a.Id),
                GatewayReviewsPending = gatewayReviewsPending,
                GatewayReviewsUnderReview = gatewayReviewsInProgress,
                PmoReviewsPending = pmoReviewsPending,
                PmoReviewsUnderReview = pmoReviewsUnderReview,
                AssessorReview1Pending = assessor1ReviewPending,
                AssessorReview1UnderReview = assessor1ReviewUnderReview,
                AssessorReview2Pending = assessor2ReviewPending,
                AssessorReview2UnderReview = assessor2ReviewUnderReview,
                AssessorModerationPending = assessorModerationPending,
                AssessorModerationUnderReview = assessorModerationUnderReview
            };

            return View("~/Views/RoatpAssessor/RoatpAssessorDashboard/Index.cshtml", vm);
        }

        [HttpPost("Start/{applicationId:Guid}")]
        public async Task<IActionResult> StartReview([FromRoute] Guid applicationId)
        {
            await _applyApiClient.CreateApplicationReview(applicationId);

            return RedirectToAction("Index");
        }
    }
}