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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayController : Controller
    {
        private readonly IRoatpOrganisationApiClient _apiClient;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public RoatpGatewayController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Roatp/Gateway/New")]
        public async Task<IActionResult> NewApplications(int page = 1)
        {
            //var applications = await _applyApiClient.GetNewGatewayApplications();

            var applications = new List<GatewayApplicationSummaryItem>();

            var paginatedApplications = new PaginatedList<GatewayApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/NewApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Gateway/InProgress")]
        public async Task<IActionResult> InProgressApplications(int page = 1)
        {
            //var applications = await _applyApiClient.GetInProgressGatewayApplications();

            var applications = new List<GatewayApplicationSummaryItem>();

            var paginatedApplications = new PaginatedList<GatewayApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/InProgressApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Gateway/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            //var applications = await _applyApiClient.GetClosedGatewayApplications();

            var applications = new List<GatewayApplicationSummaryItem>();

            var paginatedApplications = new PaginatedList<GatewayApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpGatewayDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Gateway/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Gateway/{Id}")]
        public async Task<IActionResult> ViewApplication(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            //await _applyApiClient.StartGatewayReview(application.Id, _contextAccessor.HttpContext.User.UserDisplayName());

            var vm = await CreateGatewayApplicationViewModel(application);

            return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", vm);
        }

        [HttpGet("/Roatp/Gateway/{Id}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            var vm = await CreateGatewayApplicationViewModel(application);

            return View("~/Views/Roatp/Apply/Gateway/Application_ReadOnly.cshtml", vm);
        }


        [HttpPost("/Roatp/Gateway")]
        public async Task<IActionResult> Grade(RoatpGatewayApplicationViewModel vm)
        {
            var application = await _applyApiClient.GetApplication(vm.Id);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            if (ModelState.IsValid)
            {
                //await _applyApiClient.ReturnGatewayReview(vm.Id);
                return RedirectToAction(nameof(Evaluated), new { vm.Id });
            }
            else
            {
                var newvm = await CreateGatewayApplicationViewModel(application);
                return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", newvm);
            }
        }

        [HttpGet("/Roatp/Gateway/{Id}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application?.financialGrade is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            return View("~/Views/Roatp/Apply/Gateway/Graded.cshtml");
        }

        private async Task<RoatpGatewayApplicationViewModel> CreateGatewayApplicationViewModel(RoatpApplicationResponse applicationFromRoatp)
        {
            if (applicationFromRoatp is null)
            {
                return new RoatpGatewayApplicationViewModel();
            }

            var application = new AssessorService.ApplyTypes.Roatp.Apply
            {
                ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails
                    {
                        ReferenceNumber = applicationFromRoatp.ApplyData.ApplyDetails.ReferenceNumber,
                        ProviderRoute = applicationFromRoatp.ApplyData.ApplyDetails.ProviderRoute,
                        UKPRN = applicationFromRoatp.ApplyData.ApplyDetails.UKPRN,
                        OrganisationName = applicationFromRoatp.ApplyData.ApplyDetails.OrganisationName,
                        ApplicationSubmittedOn = applicationFromRoatp.ApplyData.ApplyDetails.ApplicationSubmittedOn
                    }
                },
                Id = applicationFromRoatp.Id,
                ApplicationId = applicationFromRoatp.ApplicationId,
                OrganisationId = applicationFromRoatp.OrganisationId,
                ApplicationStatus = applicationFromRoatp.ApplicationStatus
                
            };

            return new RoatpGatewayApplicationViewModel(application);
        }
    }
}