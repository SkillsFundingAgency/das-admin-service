using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply.Gateway
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpOrganisationCriminalComplianceChecksController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly IMediator _mediator;
        private readonly ILogger<RoatpOrganisationCriminalComplianceChecksController> _logger;

        public RoatpOrganisationCriminalComplianceChecksController(IRoatpApplicationApiClient applyApiClient, IHttpContextAccessor contextAccessor,
                                                              IRoatpGatewayPageViewModelValidator gatewayValidator, IMediator mediator,
                                                              ILogger<RoatpOrganisationCriminalComplianceChecksController> logger)
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/5-10")]
        public async Task<IActionResult> GetOrganisationCompositionCreditorsPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _mediator.Send(new GetCriminalComplianceCheckRequest(applicationId, GatewayPageIds.CCOrganisationCompositionCreditors, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/5-10")]
        public async Task<IActionResult> EvaluateOrganisationCompositionCreditorsPage(OrganisationCriminalCompliancePageViewModel viewModel)
        {
            SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse?.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml", viewModel);
            }
            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            var pageData = JsonConvert.SerializeObject(viewModel);
            await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, pageData);

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }

        public void SetupGatewayPageOptionTexts(RoatpGatewayPageViewModel viewModel)
        {
            if (viewModel?.Status == null) return;
            viewModel.OptionInProgressText = viewModel.Status == SectionReviewStatus.InProgress && !string.IsNullOrEmpty(viewModel.OptionInProgressText) ? viewModel.OptionInProgressText : string.Empty;
            viewModel.OptionPassText = viewModel.Status == SectionReviewStatus.Pass && !string.IsNullOrEmpty(viewModel.OptionPassText) ? viewModel.OptionPassText : string.Empty;
            viewModel.OptionFailText = viewModel.Status == SectionReviewStatus.Fail && !string.IsNullOrEmpty(viewModel.OptionFailText) ? viewModel.OptionFailText : string.Empty;
        }
    }
}
