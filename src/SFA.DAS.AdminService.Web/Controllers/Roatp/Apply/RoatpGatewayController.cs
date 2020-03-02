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
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Application.Gateway;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    public class RoatpGatewayController : Controller
    {
        private readonly IRoatpOrganisationApiClient _apiClient;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        //private readonly IGatewayCompositionService _gatewayCompositionService;
        private readonly IRoatpGatewayPageViewModelValidator _gatewayValidator;
        private readonly IMediator _mediator;
        public RoatpGatewayController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IRoatpGatewayPageViewModelValidator gatewayValidator, IMediator mediator)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
     //       _gatewayCompositionService = gatewayCompositionService;
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

        //[HttpPost("/Roatp/Gateway/{applicationId}/Page/{PageId}")]
       // public async Task<IActionResult> EvaluatePage(RoatpGatewayPageViewModel vm)
       // {
       //     var validationResponse =  await _gatewayValidator.Validate(vm);

       //     vm.ErrorMessages = validationResponse.Errors;

       //     //if (vm.ErrorMessages != null && vm.ErrorMessages.Any())
       //     //{
       //     //    //var vmodel = _gatewayCompositionService.GetViewModelForPage(vm.ApplicationId, vm.PageId);
       //     //   // vmodel.ErrorMessages = vm.ErrorMessages;
       //     //   // vmodel.Value = vm.Value;
                
       //     //    return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", vmodel);
       //     //}
       //     //var application = await _applyApiClient.GetApplication(vm.ApplicationId);
       //     //if (application is null)
       //     //{
       //     //    return RedirectToAction(nameof(NewApplications));
       //     //}

       //     //if (!isGatewayApproved.HasValue)
       //     //{
       //     //    ModelState.AddModelError("IsGatewayApproved", "Please evaluate Gateway");
       //     //}

       //     //if (ModelState.IsValid)
       //     //{
       //     //    await _applyApiClient.EvaluateGateway(vm.ApplicationId, isGatewayApproved.Value, _contextAccessor.HttpContext.User.UserDisplayName());
       //     //    return RedirectToAction(nameof(Evaluated), new { vm.ApplicationId });
       //     //}
       //     //else
       //     //{
       //     //    var newvm = CreateGatewayApplicationViewModel(application);
       //     //    return View("~/Views/Roatp/Apply/Gateway/Application.cshtml", newvm);
       //     //}

       ////     var model = _gatewayCompositionService.GetViewModelForPage(vm.ApplicationId, vm.PageId);

       //     //if (model.NextPageId == "shutter")
       //     //{
       //     //    //goto to shutter page
       //     //}

       //     //if (model.NextPageId == "tasklist")
       //     //{
       //     //    //goto to task list
       //     //}


       //     var model = new Roatp

       //     // if it gets here, save it....

       //     model.Value = vm.Value;
       //     if (vm.Value == "Pass")
       //     {
       //           model.OptionPassText = vm.OptionPassText;
       //     }

       //     if (vm.Value == "Fail")
       //     {
       //         model.OptionFailText = vm.OptionFailText;
       //     }

       //     if (vm.Value == "In Progress")
       //     {
       //         model.OptionInProgressText = vm.OptionInProgressText;
       //     }

       //     //return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", model);
       //     //return await GatewayPage(vm.ApplicationId, model.NextPageId);
       //     return RedirectToAction("GatewayPage", new { model.ApplicationId, pageId = model.NextPageId });


       // }

        //[HttpGet("/Roatp/Gateway/{applicationId}/orig-page/{PageId}")]
        //public async Task<IActionResult> GatewayPage(Guid applicationId, string pageId)
        //{

        //    var model = _gatewayCompositionService.GetViewModelForPage(applicationId, pageId);

             
        //    return View("~/Views/Roatp/Apply/Gateway/Page.cshtml", model);
        //}

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/{PageId}")]
        public async Task<IActionResult> AdminGatewayPage(Guid applicationId, string pageId)
        {
            // const string Caption = "Organisation checks";
            // const string Heading = "Legal name check";

            // var model = new RoatpGatewayPageViewModel();
            // model.ApplicationId = applicationId;
            // model.PageId = pageId;
            //// model.NextPageId = "shutter"; //shutter page id
            // model.TextListing = new TabularData();
            // model.Tables = new List<TabularData>();
            // model.SummaryList = new TabularData();

            // model.OptionPass = new Option { Label = "Pass", Value = "Pass", Heading = "Add comments (optional)" };
            // model.OptionFail = new Option { Label = "Fail", Value = "Fail", Heading = "Add comments (mandatory)" };
            // model.OptionInProgress = new Option
            // { Label = "In progress", Value = "In Progress", Heading = "Add comments (optional)" };

            // model.NextPageId = pageId; /// needs to be actual next page
            // model.Caption = Caption;
            // model.Heading = Heading;
            // var ukprnValue = "ApplyQuestionTag: UKPRN";
            // var applicationSubmittedOn = "ApplySpecial: SubmittedOnDate";
            // var applicationSourcesCheckedOn = "ApplySpecial: CheckedOnDate";
            // var submittedApplicationData = "ApplyQuestionTag: UKRLPLegalName";
            // var ukrlpData = "UKRLP: UKRLPLegalName";

            // // these two depend on company etc
            // var companiesHouseData = "CompaniesHouse: LegalName";
            // var charityCommissionData = "CharityCommission: LegalName";


            // var textListing = new TabularData {DataRows = new List<TabularDataRow>()};

            // // building the textListing
            // textListing.DataRows.Add(new TabularDataRow { Columns = new List<string> { $"UKPRN: {ukprnValue}" } });
            // textListing.DataRows.Add(new TabularDataRow { Columns = new List<string> { $"Application submitted on: {applicationSubmittedOn}" }});
            // textListing.DataRows.Add(new TabularDataRow { Columns = new List<string> { $"Sources checked on: {applicationSourcesCheckedOn}" }});
            // model.TextListing = textListing;

            // // building the tables
            // var table = new TabularData {DataRows = new List<TabularDataRow>(),HeadingTitles = new List<string> {"Source","Legal name"}};

            // table.DataRows.Add(new TabularDataRow {Columns = new List<string> { "Submitted application data",submittedApplicationData}});
            // table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "UKRLP data", ukrlpData } });
            // table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Companies House data", companiesHouseData } });
            // table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Charity Commission data", charityCommissionData } });

            // model.Tables.Add(table);

            // // building the summarylist -- this might get rolled into tables? they're just a table without headings maybe???
            // // model.SummaryList not populated in this one

            var model = await _mediator.Send(new LegalNameRequest(applicationId));
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