﻿using Microsoft.AspNetCore.Authorization;
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

        private const string CriminalComplianceView = "~/Views/Roatp/Apply/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml";

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
            var comments = SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse?.Errors;
                return View(CriminalComplianceView, viewModel);
            }
            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, comments);

            return RedirectToAction("ViewApplication", "RoatpGateway", new { viewModel.ApplicationId });
        }

    }
}