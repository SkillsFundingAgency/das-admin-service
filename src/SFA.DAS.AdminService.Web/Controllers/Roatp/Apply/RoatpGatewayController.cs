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
            var vm = await _mediator.Send(new GetApplicationOverviewRequest(applicationId));
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
                var newvm = await _mediator.Send(new GetApplicationOverviewRequest(application.ApplicationId));
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

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/{PageId}")]
        public async Task<IActionResult> EvaluatePage(RoatpGatewayPageViewModel vm)
        {
            var validationResponse = await _gatewayValidator.Validate(vm);

            vm.ErrorMessages = validationResponse.Errors;

            if (vm.ErrorMessages != null && vm.ErrorMessages.Any())
            {
                var vmodel = new RoatpGatewayPageViewModel { ApplicationId = vm.ApplicationId, PageId = vm.PageId };

                if (vm.PageId == "1-10")
                {
                    vmodel = await _mediator.Send(new GetLegalNameRequest(vm.ApplicationId));
                }
                vmodel.ErrorMessages = vm.ErrorMessages;
                vmodel.Value = vm.Value;

                return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", vmodel);
            }


            // this is temporary, the important thing is to save, including user name, then redirect back to overview
            var model = new RoatpGatewayPageViewModel { ApplicationId = vm.ApplicationId, PageId = vm.PageId, Value = vm.Value, OptionPassText = vm.OptionPassText, OptionFailText = vm.OptionFailText, OptionInProgressText = vm.OptionInProgressText };
            model.NextPageId = vm.PageId;

            // if it gets here, save it....



            // go to overview page
            //return RedirectToAction("GetGatewayPage", new { model.ApplicationId, pageId = model.NextPageId });
            return RedirectToAction("ViewApplication", new { vm.ApplicationId });

        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/{PageId}")]
        public async Task<IActionResult> GetGatewayPage(Guid applicationId, string pageId)
        {
            var model = new RoatpGatewayPageViewModel { ApplicationId = applicationId, PageId = "NotFound" };


            if (pageId == "1-10")
            {
                model = await _mediator.Send(new GetLegalNameRequest(applicationId));
            }

            if (model.PageId == "NotFound")
            {
                return View("~/Views/ErrorPage/PageNotFound.cshtml");
            }

            return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", model);
        }

    }
}