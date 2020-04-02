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
    public class RoatpGatewayRegisterChecksController : RoatpGatewayControllerBase
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly ILogger<RoatpGatewayRegisterChecksController> _logger;
        private readonly IGatewayRegisterChecksOrchestrator _orchestrator;

        private const string GatewayViewsLocation = "~/Views/Roatp/Apply/Gateway/pages";

        public RoatpGatewayRegisterChecksController(IRoatpApplicationApiClient applyApiClient,
                                                IHttpContextAccessor contextAccessor,
                                                IRoatpGatewayPageViewModelValidator gatewayValidator,
                                                IGatewayRegisterChecksOrchestrator orchestrator,
                                                ILogger<RoatpGatewayRegisterChecksController> logger) : base()
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _logger = logger;
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
            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View($"{GatewayViewsLocation}/Roatp.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var comments = SetupGatewayPageOptionTexts(viewModel);

            _logger.LogInformation($"RoatpGatewayRegisterChecksController-EvaluateRoatpPage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpGatewayRegisterChecksController-EvaluateRoatpPage-SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
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
            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View($"{GatewayViewsLocation}/Roepao.cshtml", viewModel);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var comments = SetupGatewayPageOptionTexts(viewModel);

            _logger.LogInformation($"RoatpGatewayRegisterChecksController-EvaluateRoepaoPage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - Comments '{comments}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpGatewayRegisterChecksController-EvaluateRoepaoPage-SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }
    }
}
