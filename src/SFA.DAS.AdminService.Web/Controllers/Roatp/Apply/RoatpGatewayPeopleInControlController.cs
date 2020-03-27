using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{

        public class RoatpGatewayPeopleInControlController : RoatpGatewayControllerBase
        {
            private readonly IRoatpApplicationApiClient _applyApiClient;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
            private readonly ILogger<RoatpGatewayPeopleInControlController> _logger;
            private readonly IGatewayPeopleInControlOrchestrator _orchestrator;
        public RoatpGatewayPeopleInControlController(IRoatpApplicationApiClient applyApiClient, IHttpContextAccessor contextAccessor, IRoatpGatewayPageViewModelValidator gatewayValidator, IGatewayPeopleInControlOrchestrator orchestrator,ILogger<RoatpGatewayPeopleInControlController> logger )
            {
                _applyApiClient = applyApiClient;
                _contextAccessor = contextAccessor;
                _gatewayValidator = gatewayValidator;
                _orchestrator = orchestrator;
                _logger = logger;
        }
            // 




            private const string GatewayViewsLocation = "~/Views/Roatp/Apply/Gateway/pages";



            [HttpGet("/Roatp/Gateway/{applicationId}/Page/PeopleInControl")]
            public async Task<IActionResult> GetGatewayPeopleInControlPage(Guid applicationId, string pageId)
            {
                var username = _contextAccessor.HttpContext.User.UserDisplayName();
                var viewModel = await _orchestrator.GetPeopleInControlViewModel(new GetPeopleInControlRequest(applicationId, username));
                return View($"{GatewayViewsLocation}/PeopleInControl.cshtml", viewModel);
            }
    }
    
}
