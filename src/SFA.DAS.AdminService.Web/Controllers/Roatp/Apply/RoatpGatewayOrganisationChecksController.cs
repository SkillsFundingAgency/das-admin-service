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

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayOrganisationChecksController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly ILogger<RoatpGatewayOrganisationChecksController> _logger;
        private readonly IGatewayOrganisationChecksOrchestrator _orchestrator;

        public RoatpGatewayOrganisationChecksController(IRoatpApplicationApiClient applyApiClient, 
                                                        IHttpContextAccessor contextAccessor, 
                                                        IRoatpGatewayPageViewModelValidator gatewayValidator, 
                                                        IGatewayOrganisationChecksOrchestrator orchestrator, 
                                                        ILogger<RoatpGatewayOrganisationChecksController> logger)
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _logger = logger;
            _orchestrator = orchestrator;
        }     

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/1-10")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/1-10")]
        public async Task<IActionResult> EvaluateLegalNamePage(LegalNamePageViewModel viewModel)
        {
            var comments = SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            _logger.LogInformation($"RoatpGatewayController-EvaluateLegalNamePage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"RoatpGatewayController-EvaluateLegalNamePage - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }

        private string SetupGatewayPageOptionTexts(RoatpGatewayPageViewModel viewModel)
        {
            if (viewModel?.Status == null) return string.Empty;
            viewModel.OptionInProgressText = viewModel.Status == SectionReviewStatus.InProgress && !string.IsNullOrEmpty(viewModel.OptionInProgressText) ? viewModel.OptionInProgressText : string.Empty;
            viewModel.OptionPassText = viewModel.Status ==SectionReviewStatus.Pass && !string.IsNullOrEmpty(viewModel.OptionPassText) ? viewModel.OptionPassText : string.Empty;
            viewModel.OptionFailText = viewModel.Status == SectionReviewStatus.Fail && !string.IsNullOrEmpty(viewModel.OptionFailText) ? viewModel.OptionFailText : string.Empty;

            switch (viewModel.Status)
            {
                case SectionReviewStatus.Pass:
                    return viewModel.OptionPassText;
                case SectionReviewStatus.Fail:
                    return viewModel.OptionFailText;
                case SectionReviewStatus.InProgress:
                    return viewModel.OptionInProgressText;
                default:
                    return string.Empty;
            }
        }
    }
}