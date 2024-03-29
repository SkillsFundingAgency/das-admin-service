﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class AddRoatpOrganisationController : Controller
    {
        private readonly IRoatpApiClient _apiClient;
        private readonly IRoatpSessionService _sessionService;
        private readonly ILogger<AddRoatpOrganisationController> _logger;
        private readonly IUkrlpProcessingService _ukrlpProcessingService;
        private readonly IMapper _mapper;

        public AddRoatpOrganisationController(IRoatpApiClient apiClient, IRoatpSessionService sessionService,
             ILogger<AddRoatpOrganisationController> logger, IUkrlpProcessingService ukrlpProcessingService, IMapper mapper)
        {
            _apiClient = apiClient;
            _sessionService = sessionService;
            _logger = logger;
            _ukrlpProcessingService = ukrlpProcessingService;
            _mapper = mapper;
        }


        [Route("organisations-ukprn")]
        public async Task<IActionResult> EnterUkprn()
        {
            ModelState.Clear();
            var model = new AddOrganisationViaUkprnViewModel();

            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();
            if (addOrganisationModel?.UKPRN != null)
                model.UKPRN = addOrganisationModel.UKPRN.Trim();
            return View("~/Views/Roatp/EnterUkprn.cshtml", model);
        }

        [Route("ukprn-not-found")]
        public async Task<IActionResult> UkprnNotFound()
        {
            ModelState.Clear();
            return View("~/Views/Roatp/UkprnNotFound.cshtml");
        }

        [Route("organisations-details")]
        public async Task<IActionResult> UkprnPreview(AddOrganisationViaUkprnViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                var addOrganisationModel = _sessionService.GetAddOrganisationDetails();
                if (addOrganisationModel?.UKPRN != null)
                    model.UKPRN = addOrganisationModel.UKPRN.Trim();

                return View("~/Views/Roatp/EnterUkprn.cshtml", model);

            }

            UkrlpProviderDetails details;

            try
            {
                _sessionService.SetAddOrganisationDetails(new AddOrganisationViewModel
                {
                    UKPRN = model.UKPRN.Trim()
                });
                var fullProviderDetails = await _apiClient
                    .GetUkrlpProviderDetails(model.UKPRN.Trim()); 

                details = _ukrlpProcessingService.ProcessDetails(fullProviderDetails.ToList());
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Failed to gather organisation details from ukrlp for UKPRN:[{model?.UKPRN}]");
                return RedirectToAction("UklrpIsUnavailable");
            }

            if (string.IsNullOrEmpty(details.LegalName))
            {
                return Redirect("/ukprn-not-found");
            }

            _sessionService.SetAddOrganisationDetails(new AddOrganisationViewModel
            {
                UKPRN = model.UKPRN,
                LegalName = details.LegalName,
                TradingName = details.TradingName,
                CompanyNumber = details.CompanyNumber,
                CharityNumber = details.CharityNumber
            });

            var vm = new AddOrganisationProviderTypeViewModel
            {
                UKPRN = model.UKPRN,
                LegalName = details.LegalName,
                TradingName = details.TradingName,
                CompanyNumber = details.CompanyNumber,
                CharityNumber = details.CharityNumber
            };

            return View("~/Views/Roatp/UkprnPreview.cshtml", vm);
        }

        [Route("provider-route")]
        public async Task<IActionResult> AddProviderType()
        {
            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();

            if (string.IsNullOrEmpty(addOrganisationModel?.LegalName))
            {
                return Redirect("organisations-details");
            }

            var model = _mapper.Map<AddOrganisationProviderTypeViewModel>(addOrganisationModel);


            model.ProviderTypes = await _apiClient.GetProviderTypes();
            ModelState.Clear();
            return View("~/Views/Roatp/AddProviderType.cshtml", model);
        }

        [Route("type-organisation")]
        public async Task<IActionResult> AddOrganisationType(AddOrganisationProviderTypeViewModel model)
        {
            var addOrganisationModel = new AddOrganisationViewModel();

            if (string.IsNullOrEmpty(model.LegalName))
            {
                addOrganisationModel = _sessionService.GetAddOrganisationDetails();
                model.LegalName = addOrganisationModel.LegalName;
                model.ProviderTypeId = addOrganisationModel.ProviderTypeId;
                model.OrganisationTypeId = addOrganisationModel.OrganisationTypeId;
            }

            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid && model.ProviderTypeId == 0)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddProviderType.cshtml", model);
            }

            addOrganisationModel = _sessionService.GetAddOrganisationDetails();

            if (string.IsNullOrEmpty(addOrganisationModel?.LegalName))
            {
                return Redirect("organisations-details");
            }

            UpdateAddOrganisationModelFromProviderTypeModel(addOrganisationModel, model);


            var organisationTypes = await _apiClient.GetOrganisationTypes(addOrganisationModel.ProviderTypeId);

            addOrganisationModel.OrganisationTypes = organisationTypes.ToList().OrderBy(x => x.Id != 0).ThenBy(x => x.Type);

            if (!addOrganisationModel.OrganisationTypes.Any(x => x.Id == addOrganisationModel.OrganisationTypeId))
                addOrganisationModel.OrganisationTypeId = 0;

            _sessionService.SetAddOrganisationDetails(addOrganisationModel);

            ModelState.Clear();

            var vm = _mapper.Map<AddOrganisationTypeViewModel>(addOrganisationModel);

            return View("~/Views/Roatp/AddOrganisationType.cshtml", vm);

        }

        [Route("confirm-details")]
        public async Task<IActionResult> ConfirmOrganisationDetails(AddApplicationDeterminedDateViewModel model)
        {
            var organisationVm = _sessionService.GetAddOrganisationDetails();
            organisationVm.ApplicationDeterminedDate = model.ApplicationDeterminedDate;
            var vm = MapOrganisationVmToApplicationDeterminedDateVm(organisationVm);
            vm.Day = model.Day;
            vm.Month = model.Month;
            vm.Year = model.Year;
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                var errorMessages = GatherErrorMessagesFromModelState();
                vm.ErrorMessages = errorMessages;
                return View("~/Views/Roatp/AddApplicationDeterminedDate.cshtml", vm);
            }

            vm.LegalName = vm.LegalName.ToUpper();
            _sessionService.SetAddOrganisationDetails(vm);

            model = await SetUpConfirmationModel(vm);

            return View("~/Views/Roatp/AddOrganisationPreview.cshtml", model);
        }



        [Route("application-date-determined")]
        public async Task<IActionResult> AddApplicationDeterminedDate(AddOrganisationTypeViewModel model)
        {

            var organisationVm = _sessionService.GetAddOrganisationDetails();
            var vm = MapOrganisationVmToApplicationDeterminedDateVm(organisationVm);
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                var redirectModel = _mapper.Map<AddOrganisationTypeViewModel>(organisationVm);
                return View("~/Views/Roatp/AddOrganisationType.cshtml", redirectModel);
            }

            var errorMessages = GatherErrorMessagesFromModelState();
            vm.ErrorMessages = errorMessages;

            if (!string.IsNullOrEmpty(model.LegalName))
            {
                vm.OrganisationTypeId = model.OrganisationTypeId;
                _sessionService.SetAddOrganisationDetails(vm);

            }

            return View("~/Views/Roatp/AddApplicationDeterminedDate.cshtml", vm);
        }


        [Route("ukrlp-unavailable")]
        public async Task<IActionResult> UklrpIsUnavailable()
        {


            return View("~/Views/Roatp/UkprnIsUnavailable.cshtml");

        }

        [Route("new-training-provider")]
        public async Task<IActionResult> AddOrganisation(AddOrganisationProviderTypeViewModel model)
        {
            if (model == null)
            {
                model = new AddOrganisationProviderTypeViewModel();
            }

            model.ProviderTypes = await _apiClient.GetProviderTypes();

            ModelState.Clear();

            return View("~/Views/Roatp/AddOrganisation.cshtml", model);
        }

        [Route("enter-details")]
        public async Task<IActionResult> AddOrganisationDetails(AddOrganisationProviderTypeViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisation.cshtml", model);
            }

            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();
            if (addOrganisationModel == null)
            {
                addOrganisationModel = new AddOrganisationViewModel
                {
                    OrganisationId = model.OrganisationId,
                    ProviderTypeId = model.ProviderTypeId
                };
            }
            else
            {
                if (model.OrganisationId != Guid.Empty)
                {
                    addOrganisationModel.OrganisationId = model.OrganisationId;
                }

                if (model.ProviderTypeId > 0)
                {
                    addOrganisationModel.ProviderTypeId = model.ProviderTypeId;
                }
            }

            addOrganisationModel.OrganisationTypes = await _apiClient.GetOrganisationTypes(addOrganisationModel.ProviderTypeId);

            _sessionService.SetAddOrganisationDetails(addOrganisationModel);

            ModelState.Clear();

            return View("~/Views/Roatp/AddOrganisationDetails.cshtml", addOrganisationModel);
        }

        [Route("confirm-details-preview")]
        public async Task<IActionResult> AddOrganisationPreview(AddOrganisationViewModel model)
        {
            model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);
            model.ProviderTypes = await _apiClient.GetProviderTypes();
            model.LegalName = TextSanitiser.SanitiseText(model?.LegalName);
            model.TradingName = TextSanitiser.SanitiseText(model?.TradingName);
            if (!ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisationDetails.cshtml", model);
            }

            model.LegalName = model.LegalName.ToUpper();

            _sessionService.SetAddOrganisationDetails(model);

            return View("~/Views/Roatp/AddOrganisationPreview.cshtml", model);
        }

        [Route("successfully-added")]
        public async Task<IActionResult> CreateOrganisation(AddOrganisationViewModel model)
        {
            model.LegalName = TextSanitiser.SanitiseText(model?.LegalName);
            model.TradingName = TextSanitiser.SanitiseText(model?.TradingName);

            var request = CreateAddOrganisationRequestFromModel(model);

            var success = await _apiClient.CreateOrganisation(request);

            if (!success)
            {
                return RedirectToAction("Error", "Home");
            }

            string bannerMessage = string.Format(RoatpConfirmationMessages.AddOrganisationConfirmation,
                model.LegalName.ToUpper());

            var bannerModel = new OrganisationSearchViewModel { BannerMessage = bannerMessage };
            _sessionService.ClearAddOrganisationDetails();
            return View("~/Views/Roatp/Index.cshtml", bannerModel);
        }

        [Route("back")]
        public async Task<IActionResult> Back(string action, Guid organisationId)
        {
            var model = _sessionService.GetAddOrganisationDetails();

            return RedirectToAction(action);
        }

        private static void UpdateAddOrganisationModelFromProviderTypeModel(AddOrganisationViewModel addOrganisationModel, AddOrganisationProviderTypeViewModel model)
        {
            if (string.IsNullOrEmpty(addOrganisationModel.LegalName)) addOrganisationModel.LegalName = model.LegalName;
            if (string.IsNullOrEmpty(addOrganisationModel.TradingName)) addOrganisationModel.TradingName = model.TradingName;
            if (string.IsNullOrEmpty(addOrganisationModel.CompanyNumber))
                addOrganisationModel.CompanyNumber = model.CompanyNumber;
            if (string.IsNullOrEmpty(addOrganisationModel.CharityNumber))
                addOrganisationModel.CharityNumber = model.CharityNumber;
            if (string.IsNullOrEmpty(addOrganisationModel.UKPRN)) addOrganisationModel.UKPRN = model.UKPRN;

            if (model.OrganisationId != Guid.Empty)
            {
                addOrganisationModel.OrganisationId = model.OrganisationId;
            }

            if (model.ProviderTypeId > 0)
            {
                addOrganisationModel.ProviderTypeId = model.ProviderTypeId;
            }
        }

        private async Task<AddApplicationDeterminedDateViewModel> SetUpConfirmationModel(AddApplicationDeterminedDateViewModel vm)
        {
            var model = new AddApplicationDeterminedDateViewModel
            {
                OrganisationTypeId = vm.OrganisationTypeId,
                OrganisationTypes = await _apiClient.GetOrganisationTypes(vm.ProviderTypeId),
                ProviderTypes = await _apiClient.GetProviderTypes(),
                ProviderTypeId = vm.ProviderTypeId,
                LegalName = TextSanitiser.SanitiseText(vm.LegalName.ToUpper()),
                TradingName = TextSanitiser.SanitiseText(vm.TradingName),
                UKPRN = vm.UKPRN,
                CompanyNumber = vm.CompanyNumber,
                CharityNumber = vm.CharityNumber,
                Day = vm.Day,
                Month = vm.Month,
                Year = vm.Year
            };

            return model;
        }

        private List<ValidationErrorDetail> GatherErrorMessagesFromModelState()
        {
            return !ModelState.IsValid
                ? ModelState.SelectMany(k => k.Value.Errors.Select(e => new ValidationErrorDetail()
                {
                    ErrorMessage = e.ErrorMessage,
                    Field = k.Key
                })).ToList()
                : null;
        }

        private static AddApplicationDeterminedDateViewModel MapOrganisationVmToApplicationDeterminedDateVm(AddOrganisationViewModel addOrganisationModel)
        {
            if (addOrganisationModel == null)
                return new AddApplicationDeterminedDateViewModel();

            int? determinedDay = null;
            int? determinedMonth = null;
            int? determinedYear = null;

            if (addOrganisationModel?.ApplicationDeterminedDate != null &&
                addOrganisationModel.ApplicationDeterminedDate != DateTime.MinValue)
            {
                determinedDay = addOrganisationModel.ApplicationDeterminedDate.Value.Day;
                determinedMonth = addOrganisationModel.ApplicationDeterminedDate.Value.Month;
                determinedYear = addOrganisationModel.ApplicationDeterminedDate.Value.Year;
            }

            return new AddApplicationDeterminedDateViewModel
            {
                CharityNumber = addOrganisationModel.CharityNumber,
                CompanyNumber = addOrganisationModel.CompanyNumber,
                LegalName = addOrganisationModel.LegalName,
                OrganisationId = addOrganisationModel.OrganisationId,
                OrganisationTypeId = addOrganisationModel.OrganisationTypeId,
                OrganisationTypes = addOrganisationModel.OrganisationTypes,
                ProviderTypeId = addOrganisationModel.ProviderTypeId,
                ProviderTypes = addOrganisationModel.ProviderTypes,
                TradingName = addOrganisationModel.TradingName,
                UKPRN = addOrganisationModel.UKPRN,
                Day = determinedDay,
                Month = determinedMonth,
                Year = determinedYear
            };
        }

        private CreateRoatpOrganisationRequest CreateAddOrganisationRequestFromModel(AddOrganisationViewModel model)
        {
            var request = new CreateRoatpOrganisationRequest
            {
                CharityNumber = model.CharityNumber,
                CompanyNumber = model.CompanyNumber,
                FinancialTrackRecord = true,
                LegalName = model?.LegalName?.ToUpper(),
                NonLevyContract = false,
                OrganisationTypeId = model.OrganisationTypeId,
                ParentCompanyGuarantee = false,
                ProviderTypeId = model.ProviderTypeId,
                StatusDate = DateTime.Now,
                Ukprn = model.UKPRN,
                TradingName = model?.TradingName,
                Username = HttpContext.User.UserDisplayName(),
                SourceIsUKRLP = true,
                ApplicationDeterminedDate = model.ApplicationDeterminedDate
            };
            return request;
        }

      
        private bool IsRedirectFromConfirmationPage()
        {
            var refererHeaders = ControllerContext.HttpContext.Request.Headers["Referer"];
            if (refererHeaders.Count == 0)
            {
                return false;
            }
            var referer = refererHeaders[0];

            if (referer.Contains("confirm-details"))
            {
                return true;
            }

            var request = ControllerContext.HttpContext.Request;
            if (request.Method == "GET" && request.Path.ToString().Contains("enter-details"))
            {
                return true;
            }

            return false;
        }
    }
}
