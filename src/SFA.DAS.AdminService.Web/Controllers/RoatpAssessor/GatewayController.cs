using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;
using SFA.DAS.RoatpAssessor.Application.Gateway;
using SFA.DAS.RoatpAssessor.Application.Gateway.Requests;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorGateway)]
    [Route("roatp-assessor/gateway")]
    public class GatewayController : Controller
    {
        private readonly IMediator _mediator;

        public GatewayController(IMediator mediator)
        {
            _mediator = mediator;
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

        [HttpPost("{applicationId:Guid}/start-review", Name = RouteNames.RoatpAssessor_Gateway_Start_Review)]
        public IActionResult StartReview([FromRoute] Guid applicationId)
        {
            return RedirectToRoute(RouteNames.RoatpAssessor_Gateway_Dashboard_Get);
        }
    }
}