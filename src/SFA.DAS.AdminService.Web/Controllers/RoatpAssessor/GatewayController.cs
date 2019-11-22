using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;
using SFA.DAS.RoatpAssessor.Application.Gateway;
using SFA.DAS.RoatpAssessor.Application.Gateway.Requests;
using System;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Application.Gateway.Commands;
using SFA.DAS.RoatpAssessor.Application.Services;
using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.RoatpAssessor.Configuration;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorGateway)]
    [Route("roatp-assessor/gateway")]
    public class GatewayController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ITimeProvider _timeProvider;

        public GatewayController(IMediator mediator, ITimeProvider timeProvider)
        {
            _mediator = mediator;
            _timeProvider = timeProvider;
        }

        [HttpGet("dashboard", Name = RouteNames.RoatpAssessor_Gateway_Dashboard_Get)]
        public async Task<IActionResult> Dashboard([FromQuery]string tab, [FromQuery] string sortBy, [FromQuery] string sort)
        {
            if (!Enum.TryParse(tab, out DashboardTab selectedTab))
                selectedTab = DashboardTab.New;

            var sortDescending = sort == DashboardViewModel.SortDescending;
            var request = new GetDashboardRequest(selectedTab, sortBy, sortDescending);
            var response = await _mediator.Send(request);

            var vm = Mapper.Map<DashboardViewModel>(response);

            return View(vm);
        }

        [HttpPost("{applicationId:Guid}/start-review", Name = RouteNames.RoatpAssessor_Gateway_StartReview_Post)]
        public IActionResult StartReview([FromRoute] Guid applicationId)
        {
            return RedirectToRoute(RouteNames.RoatpAssessor_Gateway_Overview_Get);
        }

        [HttpGet("{applicationId:Guid}/overview", Name = RouteNames.RoatpAssessor_Gateway_Overview_Get)]
        public IActionResult Overview([FromRoute] Guid applicationId)
        {
            return View(applicationId);
        }

        [HttpGet("{applicationId:Guid}/legal-checks", Name = RouteNames.RoatpAssessor_Gateway_LegalChecks_Get)]
        public IActionResult LegalChecks([FromRoute] Guid applicationId)
        {
            var vm = new LegalChecksViewModel();
            return View(vm);
        }

        [HttpPost("{applicationId:Guid}/legal-checks", Name = RouteNames.RoatpAssessor_Gateway_LegalChecks_Post)]
        public IActionResult LegalChecks(LegalChecksEditModel model)
        {
            return RedirectToRoute(RouteNames.RoatpAssessor_Gateway_Overview_Get);
        }

        [HttpGet("{applicationId:Guid}/website", Name = RouteNames.RoatpAssessor_Gateway_Website_Get)]
        public async Task<IActionResult> Website([FromRoute] Guid applicationId)
        {
            var questionConfig = QuestionsConfig.OrganisationInformation.Website;

            var request = new GetQuestionReviewRequest(applicationId, questionConfig);
            var questionReview = await _mediator.Send(request);

            var websiteUrl = questionReview.Answers.FirstOrDefault()?.SingleOrDefault(a => 
                a.QuestionId == questionConfig.QuestionId)?.Value;

            var vm = new WebsiteViewModel
            {
                OrganisationName = questionReview.OrganisationName,
                Ukprn = questionReview.Ukprn,
                WebiteUrl = websiteUrl,
                Outcome = questionReview.Outcome.ToOutcomeViewModel()
            };

            return View(vm);
        }

        [HttpPost("{applicationId:Guid}/website", Name = RouteNames.RoatpAssessor_Gateway_Website_Post)]
        public async Task<IActionResult> Website(WebsiteEditModel model)
        {
            var outcome = model.Outcome.ToOutcome(QuestionsConfig.OrganisationInformation.Website);

            var command = new UpdateGatewayOutcomesCommand(model.ApplicationId, User.GetId(), _timeProvider.UtcNow, outcome);

            await _mediator.Send(command);
            
            return RedirectToRoute(RouteNames.RoatpAssessor_Gateway_Overview_Get);
        }
    }
}