using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayController : Controller
    {
        private readonly IRoatpOrganisationApiClient _apiClient;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly IMediator _mediator;
        private readonly ILogger<RoatpGatewayController> _logger;

        public RoatpGatewayController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IRoatpGatewayPageViewModelValidator gatewayValidator, IMediator mediator, ILogger<RoatpGatewayController> logger)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _mediator = mediator;
            _qnaApiClient = qnaApiClient;
            _logger = logger;
        }

        [HttpGet("/Roatp/Gateway/New")]
        public async Task<IActionResult> NewApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetNewGatewayApplications();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewModel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/NewApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/InProgress")]
        public async Task<IActionResult> InProgressApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetInProgressGatewayApplications();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewModel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/InProgressApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedGatewayApplications();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewModel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/ClosedApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _mediator.Send(new GetApplicationOverviewRequest(applicationId, username));
            if (viewModel is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            switch (viewModel.GatewayReviewStatus)
            {
                case GatewayReviewStatus.New:
                case GatewayReviewStatus.InProgress:
                    return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", viewModel);
                case GatewayReviewStatus.Approved:
                case GatewayReviewStatus.Declined:
                    return View("~/Views/Roatp/Apply/Gateway/Application_ReadOnly.cshtml", viewModel);
                default:
                    return RedirectToAction(nameof(NewApplications));
            }
        }

        [HttpPost("/Roatp/Gateway")]
        public async Task<IActionResult> EvaluateGateway(RoatpGatewayApplicationViewModel viewModel, bool? isGatewayApproved)
        {
            var application = await _applyApiClient.GetApplication(viewModel.ApplicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            if (!isGatewayApproved.HasValue)
            {
                ModelState.AddModelError("IsGatewayApproved", "Please evaluate Gateway");
            }

            if (ModelState.IsValid)
            {
                await _applyApiClient.EvaluateGateway(viewModel.ApplicationId, isGatewayApproved.Value, _contextAccessor.HttpContext.User.UserDisplayName());
                return RedirectToAction(nameof(Evaluated), new { viewModel.ApplicationId });
            }
            else
            {
                var username = _contextAccessor.HttpContext.User.UserDisplayName();
                var newViewModel = await _mediator.Send(new GetApplicationOverviewRequest(application.ApplicationId, username));
                return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", newViewModel);
            }
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            return View("~/Views/Roatp/Apply/Gateway/Evaluated.cshtml");
        }

        [HttpGet("/Roatp/GatewayCheckStatus/{applicationId}/Page/{PageId}/Status/{gatewayReviewStatus}")]
        public async Task<IActionResult> CheckStatus(Guid applicationId, string PageId, string gatewayReviewStatus)
        {
            if (gatewayReviewStatus.Equals(GatewayReviewStatus.New)) 
            {
                await _applyApiClient.StartGatewayReview(applicationId, _contextAccessor.HttpContext.User.UserDisplayName());
            }

            return Redirect($"/Roatp/Gateway/{applicationId}/Page/{PageId}"); 
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/1-10")]
        public async Task<IActionResult> EvaluateLegalNamePage(LegalNamePageViewModel viewModel)
        {
            SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            viewModel.ErrorMessages = validationResponse.Errors;

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            if (viewModel.ErrorMessages != null && viewModel.ErrorMessages.Any())
            {
                
                var model = await _mediator.Send(new GetLegalNameRequest(viewModel.ApplicationId, username));
                SetupGatewayViewModelErrorMessagesAndValues(model, viewModel);
                return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", model);
            }

            viewModel.SourcesCheckedOn = DateTime.Now;

            var pageData = JsonConvert.SerializeObject(viewModel);

            _logger.LogInformation($"RoatpGatewayController-EvaluateLegalNamePage-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - PageData '{pageData}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, pageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpGatewayController - EvaluateLegalNamePage - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return RedirectToAction("ViewApplication", new { viewModel.ApplicationId });
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/1-10")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", await _mediator.Send(new GetLegalNameRequest(applicationId, username)));
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/1-30")]
        public async Task<IActionResult> EvaluateOrganisationStatus(OrganisationStatusViewModel viewModel)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            var validationResponse = await _gatewayValidator.Validate(viewModel);
            if(validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationStatus.cshtml", viewModel);
            }

            SetupGatewayPageOptionTexts(viewModel);

            var pageData = JsonConvert.SerializeObject(viewModel);
            _logger.LogInformation($"RoatpGatewayController-EvaluateOrganisationStatus-SubmitGatewayPageAnswer - ApplicationId '{viewModel.ApplicationId}' - PageId '{viewModel.PageId}' - Status '{viewModel.Status}' - UserName '{username}' - PageData '{pageData}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, pageData);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "RoatpGatewayController - EvaluateOrganisationStatus - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }

            return RedirectToAction("ViewApplication", new { viewModel.ApplicationId });
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/1-30")]
        public async Task<IActionResult> GetOrganisationStatus(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            return View("~/Views/Roatp/Apply/Gateway/pages/OrganisationStatus.cshtml", await _mediator.Send(new GetOrganisationStatusRequest(applicationId, username)));
        }

        private static void SetupGatewayViewModelErrorMessagesAndValues(LegalNamePageViewModel model, LegalNamePageViewModel viewModel)
        {
            model.ErrorMessages = viewModel.ErrorMessages;
            model.Status = viewModel.Status;
            model.OptionInProgressText = viewModel.OptionInProgressText;
            model.OptionFailText = viewModel.OptionFailText;
            model.OptionPassText = viewModel.OptionPassText;
        }

        private static void SetupGatewayPageOptionTexts(RoatpGatewayPageViewModel viewModel)
        {
            if (viewModel?.Status == null) return;
            viewModel.OptionInProgressText = viewModel.Status == SectionReviewStatus.InProgress && !string.IsNullOrEmpty(viewModel.OptionInProgressText) ? viewModel.OptionInProgressText : string.Empty;
            viewModel.OptionPassText = viewModel.Status ==SectionReviewStatus.Pass && !string.IsNullOrEmpty(viewModel.OptionPassText) ? viewModel.OptionPassText : string.Empty;
            viewModel.OptionFailText = viewModel.Status == SectionReviewStatus.Fail && !string.IsNullOrEmpty(viewModel.OptionFailText) ? viewModel.OptionFailText : string.Empty;
        }
    }
}