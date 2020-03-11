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
        public RoatpGatewayController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IRoatpGatewayPageViewModelValidator gatewayValidator, IMediator mediator)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _gatewayValidator = gatewayValidator;
            _mediator = mediator;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Roatp/Gateway/New")]
        public async Task<IActionResult> NewApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetNewGatewayApplications();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/NewApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Gateway/InProgress")]
        public async Task<IActionResult> InProgressApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetInProgressGatewayApplications();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/InProgressApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Gateway/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedGatewayApplications();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var vm = await _mediator.Send(new GetApplicationOverviewRequest(applicationId, username));
            if (vm is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            switch (vm.GatewayReviewStatus)
            {
                case GatewayReviewStatus.New:
                case GatewayReviewStatus.InProgress:
                    return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", vm);
                case GatewayReviewStatus.Approved:
                case GatewayReviewStatus.Declined:
                    return View("~/Views/Roatp/Apply/Gateway/Application_ReadOnly.cshtml", vm);
                default:
                    return RedirectToAction(nameof(NewApplications));
            }
        }

        [HttpPost("/Roatp/Gateway")]
        public async Task<IActionResult> EvaluateGateway(RoatpGatewayApplicationViewModel vm, bool? isGatewayApproved)
        {
            var application = await _applyApiClient.GetApplication(vm.ApplicationId);
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
                await _applyApiClient.EvaluateGateway(vm.ApplicationId, isGatewayApproved.Value, _contextAccessor.HttpContext.User.UserDisplayName());
                return RedirectToAction(nameof(Evaluated), new { vm.ApplicationId });
            }
            else
            {
                var username = _contextAccessor.HttpContext.User.UserDisplayName();
                var newvm = await _mediator.Send(new GetApplicationOverviewRequest(application.ApplicationId, username));
                return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", newvm);
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
        public async Task<IActionResult> EvaluateLegalNamePage(LegalNamePageViewModel vm)
        {
            SetupGatewayPageOptionTexts(vm);

            var validationResponse = await _gatewayValidator.Validate(vm);

            vm.ErrorMessages = validationResponse.Errors;

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            if (vm.ErrorMessages != null && vm.ErrorMessages.Any())
            {
                
                var model = await _mediator.Send(new GetLegalNameRequest(vm.ApplicationId, username));
                SetupGatewayViewModelErrorMessagesAndValues(model, vm);
                return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", model);
            }

            vm.SourcesCheckedOn = DateTime.Now;

            var pageData = JsonConvert.SerializeObject(vm);
            await _applyApiClient.SubmitGatewayPageAnswer(vm.ApplicationId, vm.PageId, vm.Status, username, pageData);

            return RedirectToAction("ViewApplication", new { vm.ApplicationId });
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/1-10")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            return View("~/Views/Roatp/Apply/Gateway/pages/LegalName.cshtml", await _mediator.Send(new GetLegalNameRequest(applicationId, username)));
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/1-20")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            return View("~/Views/Roatp/Apply/Gateway/pages/TradingName.cshtml", await _mediator.Send(new GetTradingNameRequest(applicationId, username)));
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/1-20")]
        public async Task<IActionResult> EvaluateTradingNamePage(TradingNamePageViewModel vm)
        {
            SetupGatewayPageOptionTexts(vm);

            var validationResponse = await _gatewayValidator.Validate(vm);

            vm.ErrorMessages = validationResponse.Errors;

            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            if (vm.ErrorMessages != null && vm.ErrorMessages.Any())
            {

                var model = await _mediator.Send(new GetTradingNameRequest(vm.ApplicationId, username));
                SetupGatewayViewModelErrorMessagesAndValues(model, vm);
                return View("~/Views/Roatp/Apply/Gateway/pages/TradingName.cshtml", model);
            }

            vm.SourcesCheckedOn = DateTime.Now;

            var pageData = JsonConvert.SerializeObject(vm);
            await _applyApiClient.SubmitGatewayPageAnswer(vm.ApplicationId, vm.PageId, vm.Status, username, pageData);

            return RedirectToAction("ViewApplication", new { vm.ApplicationId });
        }
        private static void SetupGatewayViewModelErrorMessagesAndValues(RoatpGatewayPageViewModel model, RoatpGatewayPageViewModel vm)
        {
            model.ErrorMessages = vm.ErrorMessages;
            model.Status = vm.Status;
            model.OptionInProgressText = vm.OptionInProgressText;
            model.OptionFailText = vm.OptionFailText;
            model.OptionPassText = vm.OptionPassText;
        }

        private static void SetupGatewayPageOptionTexts(RoatpGatewayPageViewModel vm)
        {
            if (vm?.Status == null) return;
            vm.OptionInProgressText = vm.Status == SectionReviewStatus.InProgress && !string.IsNullOrEmpty(vm.OptionInProgressText) ? vm.OptionInProgressText : string.Empty;
            vm.OptionPassText = vm.Status ==SectionReviewStatus.Pass && !string.IsNullOrEmpty(vm.OptionPassText) ? vm.OptionPassText : string.Empty;
            vm.OptionFailText = vm.Status == SectionReviewStatus.Fail && !string.IsNullOrEmpty(vm.OptionFailText) ? vm.OptionFailText : string.Empty;
        }
    }
}