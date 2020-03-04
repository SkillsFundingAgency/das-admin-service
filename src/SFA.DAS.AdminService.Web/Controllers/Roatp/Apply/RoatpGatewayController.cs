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
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            var vm = CreateGatewayApplicationViewModel(application);

            // MFCMFC temporary measure to aid us to get on with stuff without needing to do a full application
            application.GatewayReviewStatus = GatewayReviewStatus.InProgress;


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

            var returnedModel = new RoatpGatewayApplicationViewModel(application);

            // APR-1467 Code
            returnedModel.ConfirmReady = false;

            var LegalNameStatus = "";
            var TradingNameStatus = "";
            var OrganisationStatus = "";
            var AddressStatus = "";
            var IcoRegistrationNumberStatus = "";
            var WebsiteAddressStatus = SectionReviewStatus.NotRequired;
            var OrganisationHighRiskStatus = "";

            var OfficeForStudentStatus = "";
            var InitialTeacherTrainingStatus = "";
            var OfstedStatus = "";
            var SubcontractorDeclarationStatus = "";


            returnedModel.Sequences = new List<GatewaySequence>
            {
                new GatewaySequence
                {
                    SequenceNumber = 1,
                    SequenceTitle = "Organisation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "1-10", LinkTitle = "Legal name", HiddenText = "", Status = LegalNameStatus },
                        new GatewaySection { SectionNumber = 2, PageId = "1-20", LinkTitle = "Trading name", HiddenText = "", Status = TradingNameStatus },
                        new GatewaySection { SectionNumber = 3, PageId = "1-30", LinkTitle = "Organisation status", HiddenText = "", Status = OrganisationStatus },
                        new GatewaySection { SectionNumber = 4, PageId = "1-40", LinkTitle = "Address", HiddenText = "", Status = AddressStatus },
                        new GatewaySection { SectionNumber = 5, PageId = "1-50", LinkTitle = "ICO registration number", HiddenText = "", Status = IcoRegistrationNumberStatus },
                        new GatewaySection { SectionNumber = 6, PageId = "1-60", LinkTitle = "Website address", HiddenText = "", Status = WebsiteAddressStatus },
                        new GatewaySection { SectionNumber = 7, PageId = "1-70", LinkTitle = "Organisation high risk", HiddenText = "", Status = OrganisationHighRiskStatus }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 2,
                    SequenceTitle = "People in control checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "2-10", LinkTitle = "People in control", HiddenText = "for people in control checks", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "2-20", LinkTitle = "People in control high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 3,
                    SequenceTitle = "Register checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "3-10", LinkTitle = "RoATP", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "3-20", LinkTitle = "Register of end-point assessment organisations", HiddenText = "", Status = "" }
                    }
                }
            };

            return returnedModel; //new RoatpGatewayApplicationViewModel(application);
        }
    }
}