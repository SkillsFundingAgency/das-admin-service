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
using SFA.DAS.AdminService.Application.Gateway;
using SFA.DAS.AdminService.Application.ViewModels;
using System.Linq;

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
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            var vm = CreateGatewayApplicationViewModel(application);


            switch (application.GatewayReviewStatus)
            {
                case GatewayReviewStatus.New:
                    await _applyApiClient.StartGatewayReview(application.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName());
                    return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", vm);
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
                var newvm = CreateGatewayApplicationViewModel(application);
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

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/{PageId}")]
        public async Task<IActionResult> EvaluatePage(RoatpGatewayPageViewModel vm)
        {
            var validationResponse = await _gatewayValidator.Validate(vm);

            vm.ErrorMessages = validationResponse.Errors;

            if (vm.ErrorMessages != null && vm.ErrorMessages.Any())
            {
                var vmodel = new RoatpGatewayPageViewModel { ApplicationId = vm.ApplicationId, PageId = vm.PageId};

                if (vm.PageId == "legal-name")
                {
                    vmodel = await _mediator.Send(new GetLegalNameRequest(vm.ApplicationId));
                }
                vmodel.ErrorMessages = vm.ErrorMessages;
                vmodel.Value = vm.Value;

                return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", vmodel);
            }
           


            // needs logic to go to the nextpage, not the current page
            var model = new RoatpGatewayPageViewModel { ApplicationId = vm.ApplicationId, PageId = vm.PageId };
            model.NextPageId = vm.PageId;

            // if it gets here, save it....

            model.Value = vm.Value;
            if (vm.Value == "Pass")
            {
                model.OptionPassText = vm.OptionPassText;
            }

            if (vm.Value == "Fail")
            {
                model.OptionFailText = vm.OptionFailText;
            }

            if (vm.Value == "In Progress")
            {
                model.OptionInProgressText = vm.OptionInProgressText;
            }

            //return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", model);
            //return await GatewayPage(vm.ApplicationId, model.NextPageId);
            return RedirectToAction("GetGatewayPage", new { model.ApplicationId, pageId = model.NextPageId });


        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/{PageId}")]
        public async Task<IActionResult> GetGatewayPage(Guid applicationId, string pageId)
        {
           var model = new RoatpGatewayPageViewModel {ApplicationId = applicationId, PageId = "NotFound"};


           if (pageId == "legal-name")
           {
               model = await _mediator.Send(new GetLegalNameRequest(applicationId));
           }


           return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", model);
        }

        private RoatpGatewayApplicationViewModel CreateGatewayApplicationViewModel(RoatpApplicationResponse applicationFromRoatp)
        {
            if (applicationFromRoatp is null)
            {
                return new RoatpGatewayApplicationViewModel();
            }

            var application = new AssessorService.ApplyTypes.Roatp.Apply
            {
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ReferenceNumber = applicationFromRoatp.ApplyData.ApplyDetails.ReferenceNumber,
                        ProviderRoute = applicationFromRoatp.ApplyData.ApplyDetails.ProviderRoute,
                        ProviderRouteName = applicationFromRoatp.ApplyData.ApplyDetails.ProviderRouteName,
                        UKPRN = applicationFromRoatp.ApplyData.ApplyDetails.UKPRN,
                        OrganisationName = applicationFromRoatp.ApplyData.ApplyDetails.OrganisationName,
                        ApplicationSubmittedOn = applicationFromRoatp.ApplyData.ApplyDetails.ApplicationSubmittedOn
                    }
                },
                Id = applicationFromRoatp.Id,
                ApplicationId = applicationFromRoatp.ApplicationId,
                OrganisationId = applicationFromRoatp.OrganisationId,
                ApplicationStatus = applicationFromRoatp.ApplicationStatus,
                GatewayReviewStatus = applicationFromRoatp.GatewayReviewStatus,
                AssessorReviewStatus = applicationFromRoatp.AssessorReviewStatus,
                FinancialReviewStatus = applicationFromRoatp.FinancialReviewStatus
            };

            return new RoatpGatewayApplicationViewModel(application);
        }
    }
}