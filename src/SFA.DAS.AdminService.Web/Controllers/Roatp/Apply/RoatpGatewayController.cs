using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly IMediator _mediator;
        private readonly ILogger<RoatpGatewayController> _logger;

        public RoatpGatewayController(IRoatpApplicationApiClient applyApiClient, IHttpContextAccessor contextAccessor, IRoatpGatewayPageViewModelValidator gatewayValidator, IMediator mediator, ILogger<RoatpGatewayController> logger)
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _mediator = mediator;
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

    

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/1-20")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            return View("~/Views/Roatp/Apply/Gateway/pages/TradingName.cshtml", await _mediator.Send(new GetTradingNameRequest(applicationId, username)));
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/1-20")]
        public async Task<IActionResult> EvaluateTradingNamePage(TradingNamePageViewModel viewModel)
        {
            SetupGatewayPageOptionTexts(viewModel);

            var validationResponse = await _gatewayValidator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModel.ErrorMessages = validationResponse?.Errors;
                return View("~/Views/Roatp/Apply/Gateway/pages/TradingName.cshtml", viewModel);
            }
            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            viewModel.SourcesCheckedOn = DateTime.Now;

            var pageData = JsonConvert.SerializeObject(viewModel);
            await _applyApiClient.SubmitGatewayPageAnswer(viewModel.ApplicationId, viewModel.PageId, viewModel.Status, username, pageData);

            return RedirectToAction("ViewApplication", new { viewModel.ApplicationId });
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


        public  string SetupGatewayPageOptionTexts(RoatpGatewayPageViewModel viewModel)
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