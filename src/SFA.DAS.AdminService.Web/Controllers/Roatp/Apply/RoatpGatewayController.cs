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
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayAssessorTeam)]
    [FeatureToggle(FeatureToggles.EnableRoatpGatewayReview, "Dashboard", "Index")]
    public class RoatpGatewayController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGatewayOverviewOrchestrator _orchestrator;
        private readonly IRoatpGatewayApplicationViewModelValidator _validator;
        private readonly ILogger<RoatpGatewayController> _logger;
        public RoatpGatewayController(IRoatpApplicationApiClient applyApiClient, IHttpContextAccessor contextAccessor, IGatewayOverviewOrchestrator orchestrator, IRoatpGatewayApplicationViewModelValidator validator, ILogger<RoatpGatewayController> logger)
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _orchestrator = orchestrator;
            _validator = validator;
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

            var viewModel =
                await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));

            if (viewModel is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            switch (viewModel.GatewayReviewStatus)
            {
                case GatewayReviewStatus.New:
                case GatewayReviewStatus.InProgress:
                    return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", viewModel);
                case GatewayReviewStatus.Pass:
                case GatewayReviewStatus.Fail:
                    return View("~/Views/Roatp/Apply/Gateway/Application_ReadOnly.cshtml", viewModel);
                default:
                    return RedirectToAction(nameof(NewApplications));
            }
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/ConfirmOutcome")]
        public async Task<IActionResult> ConfirmOutcome(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));

            if (viewModel.ReadyToConfirm)
            {
                switch (gatewayReviewStatus)
                {
                    case GatewayReviewStatus.ClarificationSent:
                        {
                            viewModel.RadioCheckedAskClarification = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionAskClarificationText = gatewayReviewComment;
                            break;
                        }
                    case GatewayReviewStatus.Fail:
                        {
                            viewModel.RadioCheckedDeclined = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionDeclinedText = gatewayReviewComment;
                            break;
                        }
                    case GatewayReviewStatus.Pass:
                        {
                            viewModel.RadioCheckedApproved = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionApprovedText = gatewayReviewComment;
                            break;
                        }
                }

                return View("~/Views/Roatp/Apply/Gateway/ConfirmOutcome.cshtml", viewModel);
            }
            else
            {
                return RedirectToAction(nameof(ViewApplication), new { applicationId = applicationId } );
            }
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/ConfirmOutcome")]
        public async Task<IActionResult> EvaluateConfirmOutcome(RoatpGatewayApplicationViewModel viewModel)
        {
            var validationResponse = await _validator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                var username = _contextAccessor.HttpContext.User.UserDisplayName();
                var viewModelOnError = await _orchestrator.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(viewModel.ApplicationId, username));
                if(viewModelOnError != null)
                {
                    _orchestrator.ProcessViewModelOnError(viewModelOnError, viewModel, validationResponse);
                    return View("~/Views/Roatp/Apply/Gateway/ConfirmOutcome.cshtml", viewModelOnError);
                }
                else
                {
                    return RedirectToAction(nameof(ViewApplication), new { applicationId = viewModel.ApplicationId });
                }
            }

            var viewName = "~/Views/Roatp/Apply/Gateway/ConfirmOutcomeAskClarification.cshtml";
            var confirmViewModel = new RoatpGatewayConfirmOutcomeViewModel { ApplicationId = viewModel.ApplicationId, GatewayReviewStatus = viewModel.GatewayReviewStatus };

            switch (viewModel.GatewayReviewStatus)
            {
                case GatewayReviewStatus.ClarificationSent:
                    {
                        confirmViewModel.GatewayReviewComment = viewModel.OptionAskClarificationText;
                        break;
                    }
                case GatewayReviewStatus.Fail:
                    {
                        confirmViewModel.GatewayReviewComment = viewModel.OptionDeclinedText;
                        viewName = "~/Views/Roatp/Apply/Gateway/ConfirmOutcomeDeclined.cshtml";
                        break;
                    }
                case GatewayReviewStatus.Pass:
                    {
                        confirmViewModel.GatewayReviewComment = viewModel.OptionApprovedText;
                        viewName = "~/Views/Roatp/Apply/Gateway/ConfirmOutcomeApproved.cshtml";
                        break;
                    }
            }

            return View(viewName, confirmViewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/AboutToConfirmOutcome")]
        public async Task<IActionResult> AboutToConfirmOutcome(RoatpGatewayConfirmOutcomeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if(viewModel.ConfirmGatewayOutcome.Equals(HtmlAndCssElements.RadioButtonValueYes, StringComparison.InvariantCultureIgnoreCase))
                {
                    var username = _contextAccessor.HttpContext.User.UserDisplayName();
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, username);
                    return View("~/Views/Roatp/Apply/Gateway/GatewayOutcomeConfirmation.cshtml", viewModel);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new {   applicationId = viewModel.ApplicationId, 
                                                                            gatewayReviewStatus = viewModel.GatewayReviewStatus,
                                                                            gatewayReviewComment = viewModel.GatewayReviewComment
                                                                        });
                }
            }
            else
            {
                viewModel.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                return View("~/Views/Roatp/Apply/Gateway/ConfirmOutcomeApproved.cshtml", viewModel);
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
                var newViewModel = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(application.ApplicationId, username));
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
                var username = _contextAccessor.HttpContext.User.UserDisplayName();
                await _applyApiClient.TriggerGatewayDataGathering(applicationId, username);
                await _applyApiClient.StartGatewayReview(applicationId, username);
            }

            return Redirect($"/Roatp/Gateway/{applicationId}/Page/{PageId}");
        }
    }
}