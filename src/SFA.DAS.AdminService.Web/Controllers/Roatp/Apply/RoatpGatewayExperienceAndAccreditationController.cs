using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayExperienceAndAccreditationController : RoatpGatewayControllerBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private readonly IGatewayExperienceAndAccreditationOrchestrator _orchestrator;
        
        public RoatpGatewayExperienceAndAccreditationController( IHttpContextAccessor contextAccessor, IRoatpApplicationApiClient roatpApiClient, IRoatpGatewayPageViewModelValidator validator, IGatewayExperienceAndAccreditationOrchestrator orchestrator, ILogger<RoatpGatewayExperienceAndAccreditationController> logger) : base(contextAccessor, roatpApiClient, logger, validator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration")]
        public async Task<ViewResult> SubcontractorDeclaration(Guid applicationId)
        {
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(applicationId, userName));
            return View($"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclarationFile")]
        public async Task<FileStreamResult> SubcontractorDeclarationContractFile(Guid applicationId)
        {
            return await _orchestrator.GetSubcontractorDeclarationContractFile(new GetSubcontractorDeclarationContractFileRequest(applicationId));
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration")]
        public async Task<IActionResult> EvaluateSubcontractorDeclarationPage(SubcontractorDeclarationViewModel viewModel)
        {
            return await SubmitGatewayPageAnswer(viewModel, $"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml");
        }
    }
}