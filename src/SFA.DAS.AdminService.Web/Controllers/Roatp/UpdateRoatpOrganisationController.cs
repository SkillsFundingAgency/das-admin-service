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
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class UpdateRoatpOrganisationController : RoatpSearchResultsControllerBase
    {
        private ILogger<UpdateRoatpOrganisationController> _logger;
        private IMapper _mapper;

        public UpdateRoatpOrganisationController(ILogger<UpdateRoatpOrganisationController> logger, IRoatpApiClient apiClient,
            IRoatpSessionService sessionService, IMapper mapper)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
            _mapper = mapper;
        }

        [Route("change-legal-name")]
        public async Task<IActionResult> UpdateOrganisationLegalName()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationLegalNameViewModel
            {
                CurrentLegalName = searchModel.SelectedResult.LegalName,
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationLegalName.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLegalName(UpdateOrganisationLegalNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationLegalName.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();

            var request = _mapper.Map<UpdateOrganisationLegalNameRequest>(model);
            request.LegalName = request.LegalName.ToUpper();

            var result = await _apiClient.UpdateOrganisationLegalName(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationLegalName.cshtml", model);
        }

        [Route("change-ukprn")]
        public async Task<IActionResult> UpdateOrganisationUkprn()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationUkprnViewModel
            {
                Ukprn = searchModel.SelectedResult?.UKPRN.ToString(),
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationUkprn.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUkprn(UpdateOrganisationUkprnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationUkprn.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();
            var request = _mapper.Map<UpdateOrganisationUkprnRequest>(model);
            var result = await _apiClient.UpdateOrganisationUkprn(request);
			
            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationUkprn.cshtml", model);
		}
		
        [Route("change-status")]
        public async Task<IActionResult> UpdateOrganisationStatus()
        {
            const int OrganisationStatusIdRemoved = 0;
            var searchModel = _sessionService.GetSearchResults();

            var organisationStatuses = _apiClient.GetOrganisationStatuses(searchModel.SelectedResult?.ProviderType?.Id).Result.OrderBy(x => x.Status);
            var removedReasons = _apiClient.GetRemovedReasons().Result.OrderBy(x => x.Reason);
            
            var model = new UpdateOrganisationStatusViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id,
                OrganisationStatusId = searchModel.SelectedResult.OrganisationStatus.Id,
                OrganisationStatuses = organisationStatuses,
                RemovedReasons = removedReasons,
                ProviderTypeId = searchModel.SelectedResult.ProviderType.Id
            };
            if (model.OrganisationStatusId == OrganisationStatusIdRemoved)
            {
                model.RemovedReasonId = searchModel.SelectedResult.OrganisationData.RemovedReason.Id;
            }
            return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
        }

        [Route("change-organisation-type")]
        public async Task<IActionResult> UpdateOrganisationType()
        {
            var searchModel = _sessionService.GetSearchResults();

            var organisationTypes = _apiClient.GetOrganisationTypes(searchModel.SelectedResult?.ProviderType?.Id).Result.Where(x => x.Id!=OrganisationType.Unassigned).OrderBy(x => x.Id);
            var model = new UpdateOrganisationTypeViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id,
                OrganisationTypeId = searchModel.SelectedResult.OrganisationType.Id,
                OrganisationTypes = organisationTypes,
                ProviderTypeId = searchModel.SelectedResult.ProviderType.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationType.cshtml", model);
        }


        [Route("change-company-number")]
        public async Task<IActionResult> UpdateOrganisationCompanyNumber()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationCompanyNumberViewModel
            {
                CompanyNumber = searchModel.SelectedResult?.OrganisationData?.CompanyNumber,
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationCompanyNumber.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompanyNumber(UpdateOrganisationCompanyNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationCompanyNumber.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();
            var request = _mapper.Map<UpdateOrganisationCompanyNumberRequest>(model);
            var result = await _apiClient.UpdateOrganisationCompanyNumber(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationCompanyNumber.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(UpdateOrganisationStatusViewModel model)
        {
            model.OrganisationStatuses = _apiClient.GetOrganisationStatuses(model.ProviderTypeId).Result.OrderBy(x => x.Status);
            model.RemovedReasons = _apiClient.GetRemovedReasons().Result.OrderBy(x => x.Id);

            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();
            var request = _mapper.Map<UpdateOrganisationStatusRequest>(model);
            if (model.OrganisationStatusId == 0) // Removed
            {
                request.RemovedReasonId = model.RemovedReasonId;
            }
            else
            {
                request.RemovedReasonId = null;
            }

            

            var result = await _apiClient.UpdateOrganisationStatus(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateType(UpdateOrganisationTypeViewModel model)
        {
            model.OrganisationTypes = _apiClient.GetOrganisationTypes(model.ProviderTypeId).Result.Where(x=>x.Id!= OrganisationType.Unassigned).OrderBy(x => x.Id);
           
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationType.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();
            var request = _mapper.Map<UpdateOrganisationTypeRequest>(model);

            var result = await _apiClient.UpdateOrganisationType(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationType.cshtml", model);
        }

        [Route("change-trading-name")]
        public async Task<IActionResult> UpdateOrganisationTradingName()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationTradingNameViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                TradingName = searchModel.SelectedResult.TradingName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationTradingName.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTradingName(UpdateOrganisationTradingNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationTradingName.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();

            var request = _mapper.Map<UpdateOrganisationTradingNameRequest>(model);
            var result = await _apiClient.UpdateOrganisationTradingName(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationTradingName.cshtml", model);
        }

        [Route("change-parent-company-guarantee")]
        public async Task<IActionResult> UpdateOrganisationParentCompanyGuarantee()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationParentCompanyGuaranteeViewModel
            {
                ParentCompanyGuarantee = searchModel.SelectedResult.OrganisationData.ParentCompanyGuarantee,
                OrganisationId = searchModel.SelectedResult.Id,
                LegalName = searchModel.SelectedResult.LegalName
            };

            return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantee.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantee.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();

            var request = _mapper.Map<UpdateOrganisationParentCompanyGuaranteeRequest>(model);
            var result = await _apiClient.UpdateOrganisationParentCompanyGuarantee(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantee.cshtml", model);
        }

        [Route("change-financial-track-record")]
        public async Task<IActionResult> UpdateOrganisationFinancialTrackRecord()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationFinancialTrackRecordViewModel
            {
                FinancialTrackRecord = searchModel.SelectedResult.OrganisationData.FinancialTrackRecord,
                OrganisationId = searchModel.SelectedResult.Id,
                LegalName = searchModel.SelectedResult.LegalName
            };

            return View("~/Views/Roatp/UpdateOrganisationFinancialTrackRecord.cshtml", model);
        }


        [Route("change-application-date-determined")]
        public async Task<IActionResult> UpdateOrganisationApplicationDeterminedDate()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateApplicationDeterminedDateViewModel
            {
                Day = searchModel.SelectedResult.OrganisationData?.ApplicationDeterminedDate?.Day,
                Month = searchModel.SelectedResult.OrganisationData?.ApplicationDeterminedDate?.Month,
                Year = searchModel.SelectedResult.OrganisationData?.ApplicationDeterminedDate?.Year,
                OrganisationId = searchModel.SelectedResult.Id,
                LegalName = searchModel.SelectedResult.LegalName
            };

            return View("~/Views/Roatp/UpdateOrganisationApplicationDeterminedDate.cshtml", model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationFinancialTrackRecord.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();

            var request = _mapper.Map<UpdateOrganisationFinancialTrackRecordRequest>(model);
            var result = await _apiClient.UpdateOrganisationFinancialTrackRecord(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationFinancialTrackRecord.cshtml", model);
        }

        [Route("change-provider-type")]
        public async Task<IActionResult> UpdateOrganisationProviderType()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationProviderTypeViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id,
                ProviderTypeId = searchModel.SelectedResult.ProviderType.Id,
                OrganisationTypeId = searchModel.SelectedResult.OrganisationType.Id
            };

            var providerTypes = await _apiClient.GetProviderTypes();
            model.ProviderTypes = providerTypes;

            var organisationTypes = new Dictionary<int, IEnumerable<OrganisationType>>();
            foreach (var providerType in providerTypes)
            {
                var organisationTypesByProvider = await _apiClient.GetOrganisationTypes(providerType.Id);
                organisationTypes.Add(providerType.Id, organisationTypesByProvider);
            }

            model.OrganisationTypesByProviderType = organisationTypes;

            return View("~/Views/Roatp/UpdateOrganisationProviderType.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProviderType(UpdateOrganisationProviderTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationProviderType.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();

            var searchModel = _sessionService.GetSearchResults();
            var previousProviderTypeId = searchModel.SelectedResult.ProviderType.Id;
            var previousOrganisationTypeId = searchModel.SelectedResult.OrganisationType.Id;

            if (model.CanChangeOrganisationTypeForThisProvider(previousProviderTypeId))
            {
                switch (model.ProviderTypeId)
                {
                    case 1:
                        model.OrganisationTypeId = model.OrganisationTypeIdMain;
                        break;
                    case 2:
                        model.OrganisationTypeId = model.OrganisationTypeIdEmployer;
                        break;
                    case 3:
                        model.OrganisationTypeId = model.OrganisationTypeIdSupporting;
                        break;
                }
            }
            else
            {
                model.OrganisationTypeId = previousOrganisationTypeId;
            }

            var request = _mapper.Map<UpdateOrganisationProviderTypeRequest>(model);

            var result = await _apiClient.UpdateOrganisationProviderType(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationProviderType.cshtml", model);
        }


        [Route("change-charity-registration-number")]
        public async Task<IActionResult> UpdateOrganisationCharityNumber()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationCharityNumberViewModel
            {
                CharityNumber = searchModel.SelectedResult?.OrganisationData?.CharityNumber,
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationCharityNumber.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCharityNumber(UpdateOrganisationCharityNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationCharityNumber.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();
            var request = _mapper.Map<UpdateOrganisationCharityNumberRequest>(model);
            var result = await _apiClient.UpdateOrganisationCharityNumber(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationCharityNumber.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApplicationDeterminedDate(UpdateApplicationDeterminedDateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = GatherErrorMessagesFromModelState();
                model.ErrorMessages = errorMessages;
                return View("~/Views/Roatp/UpdateOrganisationApplicationDeterminedDate.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.UserDisplayName();
            var request = _mapper.Map<UpdateOrganisationApplicationDeterminedDateRequest>(model);

            var result = await _apiClient.UpdateApplicationDeterminedDate(request);

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationApplicationDeterminedDate.cshtml", model);
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
    }
}