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
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayExperienceAndAccreditationController : RoatpGatewayControllerBase
    {
        private readonly IRoatpApplicationApiClient _roatpApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _validator;
        private readonly IGatewayExperienceAndAccreditationOrchestrator _orchestrator;
        private readonly ILogger<RoatpGatewayExperienceAndAccreditationController> _logger;

        public RoatpGatewayExperienceAndAccreditationController(IRoatpApplicationApiClient roatpApiClient, IHttpContextAccessor contextAccessor, IRoatpGatewayPageViewModelValidator validator, IGatewayExperienceAndAccreditationOrchestrator orchestrator, ILogger<RoatpGatewayExperienceAndAccreditationController> logger)
        {
            _roatpApiClient = roatpApiClient;
            _contextAccessor = contextAccessor;
            _validator = validator;
            _orchestrator = orchestrator;
            _logger = logger;
        }

        public async Task<ViewResult> SubcontractorDeclaration(Guid applicationId)
        {
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(applicationId, userName));
            return View($"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml", viewModel);
        }

        public async Task<FileStreamResult> SubcontractorDeclarationContractFile(Guid applicationId)
        {
            return await _orchestrator.GetSubcontractorDeclarationContractFile(new GetSubcontractorDeclarationContractFileRequest(applicationId));
        }
    }
}