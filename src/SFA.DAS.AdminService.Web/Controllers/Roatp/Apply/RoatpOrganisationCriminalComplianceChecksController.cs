using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpOrganisationCriminalComplianceChecksController : RoatpGatewayControllerBase
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly IGatewayCriminalComplianceChecksOrchestrator _orchestrator;
        private readonly ILogger<RoatpOrganisationCriminalComplianceChecksController> _logger;

        public RoatpOrganisationCriminalComplianceChecksController(IRoatpApplicationApiClient applyApiClient, IHttpContextAccessor contextAccessor,
                                                              IRoatpGatewayPageViewModelValidator gatewayValidator,
                                                              IGatewayCriminalComplianceChecksOrchestrator orchestrator,
                                                              ILogger<RoatpOrganisationCriminalComplianceChecksController> logger) : base()
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _orchestrator = orchestrator;
            _logger = logger;
        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/composition-with-creditors")]
        public async Task<IActionResult> GetOrganisationCompositionCreditorsPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(applicationId, GatewayPageIds.CCOrganisationCompositionCreditors, username));

            viewModel.PageTitle = CriminalCompliancePageTitles.OrganisationCompositionCreditors;
            viewModel.PostBackAction = CriminalCompliancePagePostActions.OrganisationCompositionCreditors;

            return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/composition-with-creditors")]
        public async Task<IActionResult> EvaluateOrganisationCompositionCreditorsPage(OrganisationCriminalCompliancePageViewModel viewModel)
        {
            var comments = SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse?.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml", viewModel);
            }
            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/pay-back")]
        public async Task<IActionResult> GetOrganisationFailedToRepayFundsPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(applicationId, GatewayPageIds.CCOrganisationFailedToRepayFunds, username));

            viewModel.PageTitle = CriminalCompliancePageTitles.OrganisationFailedToRepayFunds;
            viewModel.PostBackAction = CriminalCompliancePagePostActions.OrganisationFailedToRepayFunds;

            return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/pay-back")]
        public async Task<IActionResult> EvaluateOrganisationFailedToRepayFundsPage(OrganisationCriminalCompliancePageViewModel viewModel)
        {
            var comments = SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse?.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml", viewModel);
            }
            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }
    }
}
