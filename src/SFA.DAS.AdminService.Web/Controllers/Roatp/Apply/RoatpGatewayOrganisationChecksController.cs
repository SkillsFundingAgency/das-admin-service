using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    public class RoatpGatewayOrganisationChecksController : RoatpGatewayControllerBase<RoatpGatewayOrganisationChecksController>
    {
        private readonly IGatewayOrganisationChecksOrchestrator _orchestrator;

        public RoatpGatewayOrganisationChecksController(IRoatpApplicationApiClient applyApiClient, 
                                                        IHttpContextAccessor contextAccessor, 
                                                        IRoatpGatewayPageValidator gatewayValidator, 
                                                        IGatewayOrganisationChecksOrchestrator orchestrator, 
                                                        ILogger<RoatpGatewayOrganisationChecksController> logger):base(contextAccessor, applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }     

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/LegalName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> EvaluateLegalNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<LegalNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/LegalName.cshtml");
        }
        
        [HttpGet("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/TradingName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> EvaluateTradingNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<TradingNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/TradingName.cshtml");
        }
        

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> GetOrganisationStatusPage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/OrganisationStatus.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> EvaluateOrganisationStatus(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationStatusViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationStatus.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> GetGatewayAddressPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetAddressViewModel(new GetAddressRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/AddressCheck.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> EvaluateAddressPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<AddressCheckViewModel>> viewModelBuilder = () => _orchestrator.GetAddressViewModel(new GetAddressRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/AddressCheck.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> GetIcoNumberPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(applicationId, username));
            return View("~/Views/Roatp/Apply/Gateway/pages/IcoNumber.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> EvaluateIcoNumberPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<IcoNumberViewModel>> viewModelBuilder = () => _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/IcoNumber.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> GetWebsitePage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/Website.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> EvaluateWebsitePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<WebsiteViewModel>> viewModelBuilder = () => _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Website.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> GetOrganisationRiskPage(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/OrganisationRisk.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> EvaluateOrganisationRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationRiskViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationRisk.cshtml");
        }
    }
}