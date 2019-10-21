using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor;
using SFA.DAS.RoatpAssessor.Application.Gateway.Commands;
using SFA.DAS.RoatpAssessor.Application.Gateway.Requests;
using SFA.DAS.RoatpAssessor.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorTeam)]
    [Route("roatp-review/gateway")]
    public class RoatpAssessorGatewayController : Controller
    {
        private readonly IMediator _mediator;

        public RoatpAssessorGatewayController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var request = new GetGatewayDashboardRequest();
            var response = await _mediator.Send(request);

            var vm = new RoatpAssessorGatewayDashboardViewModel
            {
                PendingReviews = response.PendingReviews,
                InProgressReviews = response.InProgressReviews,
                CompletedReviews = response.CompletedReviews
            };

            return View("~/Views/RoatpAssessor/RoatpAssessorGateway/Dashboard.cshtml", vm);
        }

        [HttpGet("{applicationId:Guid}/start")]
        public async Task<IActionResult> StartReviewGet([FromRoute] Guid applicationId)
        {
            return View("~/Views/RoatpAssessor/RoatpAssessorGateway/Start.cshtml", applicationId);
        }

        [HttpPost("{applicationId:Guid}/start")]
        public async Task<IActionResult> StartReview([FromRoute] Guid applicationId)
        {
            var command = new StartGatewayReviewCommand(applicationId);
            await _mediator.Send(command);

            return RedirectToAction("Dashboard");
        }

        [HttpGet("{applicationId:Guid}/task-list")]
        public async Task<IActionResult> TaskList([FromRoute] Guid applicationId)
        {
            var request = new GetGatewayReviewTaskListRequest(applicationId);
            var response = await _mediator.Send(request);

            throw new NotImplementedException("to do");
        }

        [HttpGet("{applicationId:Guid}")]
        public async Task<IActionResult> Index([FromRoute] Guid applicationId)
        {
            var request = new GetGatewayReviewRequest(applicationId);
            var response = await _mediator.Send(request);

            return View("~/Views/RoatpAssessor/RoatpAssessorGateway/Index.cshtml", response.ApplicationId);
        }

        [HttpPost("{applicationId:Guid}/pass")]
        public async Task<IActionResult> PassGateway([FromRoute] Guid applicationId)
        {
            var command = new UpdateGatewayReviewStatusCommand(applicationId, ApplicationReviewStatus.Passed);
            await _mediator.Send(command);

            return RedirectToAction("Dashboard");
        }
    }
}