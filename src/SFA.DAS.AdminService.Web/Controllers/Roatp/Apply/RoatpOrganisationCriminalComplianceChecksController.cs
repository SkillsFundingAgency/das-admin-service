using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpOrganisationCriminalComplianceChecksController : RoatpGatewayControllerBase<RoatpOrganisationCriminalComplianceChecksController>
    {
        private readonly IGatewayCriminalComplianceChecksOrchestrator _orchestrator;
        
        private const string CriminalComplianceView = "~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml";

        public RoatpOrganisationCriminalComplianceChecksController(IRoatpApplicationApiClient applyApiClient, IHttpContextAccessor contextAccessor,
                                                              IRoatpGatewayPageViewModelValidator gatewayValidator,
                                                              IGatewayCriminalComplianceChecksOrchestrator orchestrator,
                                                              ILogger<RoatpOrganisationCriminalComplianceChecksController> logger) : base(contextAccessor, applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/{gatewayPageId}")]
        public async Task<IActionResult> GetCriminalCompliancePage(Guid applicationId, string gatewayPageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(applicationId, gatewayPageId, username));

            viewModel.PageTitle = CriminalCompliancePageConfiguration.Titles[gatewayPageId];

            return View(CriminalComplianceView, viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/SubmitComplianceCheck")]
        public async Task<IActionResult> EvaluateCriminalCompliancePage(OrganisationCriminalCompliancePageViewModel viewModel)
        {
            return await SubmitGatewayPageAnswer(viewModel, CriminalComplianceView);
        }

    }
}
