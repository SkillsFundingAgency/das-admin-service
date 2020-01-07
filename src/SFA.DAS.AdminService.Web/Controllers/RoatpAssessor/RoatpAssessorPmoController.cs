using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorTeam)]
    [Route("roatp-review/{applicationId:Guid}/pmo")]
    public class RoatpAssessorPmoController : Controller
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public RoatpAssessorPmoController(IApplyApiClient applyApiClient, IQnaApiClient qnaApliClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApliClient;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartReview([FromRoute] Guid applicationId)
        {
            await _applyApiClient.UpdateApplicationReviewPmoReviewAsync(applicationId, ApplicationReviewStatus.InProgress);

            return RedirectToAction("Index", "RoatpAssessorDashboard");
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromRoute] Guid applicationId)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(applicationId);

            if(!review.CanUpdatePmoReview)
            {
                throw new Exception($"Review '{applicationId}' is not in correct state to do Pmo review");
            }

            return View("~/Views/RoatpAssessor/RoatpAssessorPmo/Index.cshtml", review.ApplicationId);
        }

        [HttpPost("pass")]
        public async Task<IActionResult> PassPmo([FromRoute] Guid applicationId)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(applicationId);

            if (!review.CanUpdatePmoReview)
            {
                throw new Exception($"Review '{applicationId}' is not in correct state to do Pmo review");
            }

            await _applyApiClient.UpdateApplicationReviewPmoReviewAsync(applicationId, ApplicationReviewStatus.Passed);

            return RedirectToAction("Index", "RoatpAssessorDashboard");
        }
    }
}