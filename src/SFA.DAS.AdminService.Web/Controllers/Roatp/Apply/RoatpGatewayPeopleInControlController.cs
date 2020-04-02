using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{

        public class RoatpGatewayPeopleInControlController : RoatpGatewayControllerBase<RoatpGatewayPeopleInControlController>
        {

            private readonly IGatewayPeopleInControlOrchestrator _orchestrator;

        public RoatpGatewayPeopleInControlController(IHttpContextAccessor contextAccessor, IRoatpApplicationApiClient roatpApiClient, ILogger<RoatpGatewayPeopleInControlController> logger, IRoatpGatewayPageViewModelValidator validator, IGatewayPeopleInControlOrchestrator orchestrator ) : base(contextAccessor, roatpApiClient, logger, validator)
        {
            {
                _orchestrator = orchestrator;
            }
        }

            private const string GatewayViewsLocation = "~/Views/Roatp/Apply/Gateway/pages";

            [HttpGet("/Roatp/Gateway/{applicationId}/Page/PeopleInControl")]
            public async Task<IActionResult> GetGatewayPeopleInControlPage(Guid applicationId, string pageId)
            {
                var username = _contextAccessor.HttpContext.User.UserDisplayName();
                var viewModel = await _orchestrator.GetPeopleInControlViewModel(new GetPeopleInControlRequest(applicationId, username));
                return View($"{GatewayViewsLocation}/PeopleInControl.cshtml", viewModel);
            }

            [HttpPost("/Roatp/Gateway/{applicationId}/Page/PeopleInControl")]
        public async Task<IActionResult> EvaluatePeopleInControlPage(PeopleInControlPageViewModel viewModel)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var vmRebuild = await _orchestrator.GetPeopleInControlViewModel(new GetPeopleInControlRequest(viewModel.ApplicationId, username));
            viewModel.CompanyDirectorsData = vmRebuild.CompanyDirectorsData;
            viewModel.PscData = vmRebuild.PscData;
            viewModel.TrusteeData = vmRebuild.TrusteeData;
            viewModel.WhosInControlData = vmRebuild.WhosInControlData;

            return await SubmitGatewayPageAnswer(viewModel, $"{GatewayViewsLocation}/PeopleInControl.cshtml");
        }
    }
    
}
