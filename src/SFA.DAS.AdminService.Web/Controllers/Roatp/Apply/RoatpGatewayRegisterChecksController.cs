using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using System.Linq;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayRegisterChecksController : RoatpGatewayControllerBase<RoatpGatewayRegisterChecksController>
    {
        private readonly IGatewayRegisterChecksOrchestrator _orchestrator;

        private const string GatewayViewsLocation = "~/Views/Roatp/Apply/Gateway/pages";

        public RoatpGatewayRegisterChecksController(IRoatpApplicationApiClient applyApiClient,
                                                IHttpContextAccessor contextAccessor,
                                                IRoatpGatewayPageViewModelValidator gatewayValidator,
                                                IGatewayRegisterChecksOrchestrator orchestrator,
                                                ILogger<RoatpGatewayRegisterChecksController> logger) : base(contextAccessor, applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Roatp")]
        public async Task<IActionResult> GetGatewayRoatpPage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetRoatpViewModel(new GetRoatpRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/Roatp.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roatp")]
        public async Task<IActionResult> EvaluateRoatpPage(RoatpPageViewModel viewModel)
        {
            return await SubmitGatewayPageAnswer(viewModel, $"{GatewayViewsLocation}/Roatp.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Roepao")]
        public async Task<IActionResult> GetGatewayRoepaoPage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetRoepaoViewModel(new GetRoepaoRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/Roepao.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roepao")]
        public async Task<IActionResult> EvaluateRoepaoPage(RoepaoPageViewModel viewModel)
        {
            return await SubmitGatewayPageAnswer(viewModel, $"{GatewayViewsLocation}/Roepao.cshtml");
        }
    }
}
