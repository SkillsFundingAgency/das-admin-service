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
    public class RoatpGatewayOrganisationChecksController : RoatpGatewayControllerBase
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
                                                        ILogger<RoatpGatewayOrganisationChecksController> logger):base()
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _logger = logger;
            _orchestrator = orchestrator;
        }     

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/legal-name")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/legal-name")]
        public async Task<IActionResult> EvaluateLegalNamePage(LegalNamePageViewModel viewModel)
        {
     
            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var comments = SetupGatewayPageOptionTexts(viewModel);

            _logger.LogInformation($"RoatpGatewayOrganisationChecksController-EvaluateLegalNamePage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"RoatpGatewayController-EvaluateLegalNamePage - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/trading-name")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/TradingName.cshtml", viewModel);           
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/trading-name")]
        public async Task<IActionResult> EvaluateTradingNamePage(TradingNamePageViewModel viewModel)
        {
              var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/TradingName.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var comments = SetupGatewayPageOptionTexts(viewModel);

            _logger.LogInformation($"RoatpGatewayController-EvaluateTradingNamePage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpGatewayController-EvaluateTradingNamePage - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }



        [HttpGet("/Roatp/Gateway/{applicationId}/Page/organisation-status")]
        public async Task<IActionResult> GetOrganisationStatusPage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationStatus.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/organisation-status")]
        public async Task<IActionResult> EvaluateOrganisationStatus(OrganisationStatusViewModel viewModel)
        {
            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationStatus.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var comments = SetupGatewayPageOptionTexts(viewModel);

            _logger.LogInformation($"RoatpGatewayController-EvaluateOrganisationStatusPage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpGatewayController-EvaluateTradingNamePage - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> GetGatewayAddressPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetAddressViewModel(new GetAddressRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/AddressCheck.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> EvaluateAddressPage(AddressCheckViewModel viewModel)
        {
            var comments = SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/AddressCheck.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            
            _logger.LogInformation($"RoatpGatewayOrganisationChecksController-EvaluateAddressPage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                
                // MFCMFC Shutter page? throw again?
                _logger.LogError(ex, "RoatpGatewayOrganisationChecksController-EvaluateAddressPage - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }
    }
}