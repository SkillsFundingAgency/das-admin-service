using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayOrganisationChecksOrchestrator : IGatewayOrganisationChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<GatewayOrganisationChecksOrchestrator> _logger;

        public GatewayOrganisationChecksOrchestrator(IRoatpApplicationApiClient applyApiClient, 
                                                     ILogger<GatewayOrganisationChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest request)
        {
            _logger.LogInformation($"Retrieving legal name details for application {request.ApplicationId}");

            var model = new LegalNamePageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.LegalName, request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.LegalName,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.LegalName]);

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);

            model.UkrlpLegalName = ukrlpDetails?.ProviderName;

            var companiesHouseDetails = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);
            if (companiesHouseDetails != null)
            {
                model.CompaniesHouseLegalName = companiesHouseDetails.CompanyName;
            }

            var charityCommissionDetails = await _applyApiClient.GetCharityCommissionDetails(request.ApplicationId);
            if (charityCommissionDetails != null)
            {
                model.CharityCommissionLegalName = charityCommissionDetails.CharityName;
            }

            return model;
        }

        public async Task<TradingNamePageViewModel> GetTradingNameViewModel(GetTradingNameRequest request)
        {
            _logger.LogInformation($"Retrieving trading name details for application {request.ApplicationId}");

            var model = new TradingNamePageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.TradingName, request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.TradingName,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.TradingName]);

            var ukrlpDetail = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            if (ukrlpDetail.ProviderAliases != null && ukrlpDetail.ProviderAliases.Count > 0)
            {
                model.UkrlpTradingName = ukrlpDetail.ProviderAliases.First().Alias;
            }

            model.ApplyTradingName = await _applyApiClient.GetTradingName(request.ApplicationId);
       
            return model;
        }

        public async Task<OrganisationStatusViewModel> GetOrganisationStatusViewModel(GetOrganisationStatusRequest request)
        {
            _logger.LogInformation($"Retrieving organisation status details for application {request.ApplicationId}");

            var model = new OrganisationStatusViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.OrganisationStatus, request.UserName,
                                                                                                RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                                RoatpGatewayConstants.Headings.OrganisationStatusCheck,
                                                                                                NoSelectionErrorMessages.Errors[GatewayPageIds.OrganisationStatus]);

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            model.UkrlpStatus = ukrlpDetails?.ProviderStatus?.CapitaliseFirstLetter();

            var companiesHouseDetails = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);
            if (companiesHouseDetails != null)
            {
                model.CompaniesHouseStatus = companiesHouseDetails?.Status.CapitaliseFirstLetter();
            }

            var charityCommissionDetails = await _applyApiClient.GetCharityCommissionDetails(request.ApplicationId);
            if (charityCommissionDetails != null)
            {
                model.CharityCommissionStatus = charityCommissionDetails?.Status.CapitaliseFirstLetter();
            }

            return model;
        }

        public async Task<AddressCheckViewModel> GetAddressViewModel(GetAddressRequest request)
        {
            _logger.LogInformation($"Retrieving address check details for application {request.ApplicationId}");

            var model = new AddressCheckViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.Address, request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.AddressCheck,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.Address]);

            var organisationAddress = await _applyApiClient.GetOrganisationAddress(request.ApplicationId);
            if (organisationAddress != null)
            {
                var AddressArray = new[] { organisationAddress.Address1, organisationAddress.Address2, organisationAddress.Address3, organisationAddress.Address4, organisationAddress.Town, organisationAddress.PostCode };
                model.SubmittedApplicationAddress = string.Join(", ", AddressArray.Where(s => !string.IsNullOrEmpty(s))); ;
            }

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            if (ukrlpDetails != null)
            {
                var ukrlpAddressLine1 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address1;
                var ukrlpAddressLine2 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address2;
                var ukrlpAddressLine3 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address3;
                var ukrlpAddressLine4 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address4;
                var ukrlpTown = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Town;
                var ukrlpPostCode = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.PostCode;

                var ukrlpAarray = new[] { ukrlpAddressLine1, ukrlpAddressLine2, ukrlpAddressLine3, ukrlpAddressLine4, ukrlpTown, ukrlpPostCode };
                var ukrlpAddress = string.Join(", ", ukrlpAarray.Where(s => !string.IsNullOrEmpty(s)));
                model.UkrlpAddress = ukrlpAddress;
            }

            return model;
        }

        public async Task<IcoNumberViewModel> GetIcoNumberViewModel(GetIcoNumberRequest request)
        {
            _logger.LogInformation($"Retrieving ICO Number check details for application {request.ApplicationId}");

            var model = new IcoNumberViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.IcoNumber, request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.IcoNumber,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.IcoNumber]);
            

            var organisationAddress = await _applyApiClient.GetOrganisationAddress(request.ApplicationId);
            if (organisationAddress != null)
            {
                var AddressArray = new[] { organisationAddress.Address1, organisationAddress.Address2, organisationAddress.Address3, organisationAddress.Address4, organisationAddress.Town, organisationAddress.PostCode };
                model.OrganisationAddress = string.Join(", ", AddressArray.Where(s => !string.IsNullOrEmpty(s))); ;
            }

            model.IcoNumber = await _applyApiClient.GetIcoNumber(request.ApplicationId);

            return model;
        }

        public async Task<WebsiteViewModel> GetWebsiteViewModel(GetWebsiteRequest request)
        {
            _logger.LogInformation($"Retrieving Website check details for application {request.ApplicationId}");

            var model = new WebsiteViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.WebsiteAddress, request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.Website,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.WebsiteAddress]);

            model.SubmittedWebsite = await _applyApiClient.GetOrganisationWebsiteAddress(request.ApplicationId);

            Uri submittedWebsiteProperUri;
            var isSubmittedWebsiteProperUri = StringExtensions.ValidateHttpURL(model.SubmittedWebsite, out submittedWebsiteProperUri);
            if (isSubmittedWebsiteProperUri)
            {
                model.SubmittedWebsiteUrl = submittedWebsiteProperUri?.AbsoluteUri;
            }
            
            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            if (ukrlpDetails != null && ukrlpDetails.ContactDetails != null)
            {
                model.UkrlpWebsite = ukrlpDetails.ContactDetails.FirstOrDefault(x => x.ContactType == RoatpGatewayConstants.ProviderContactDetailsTypeLegalIdentifier)?.ContactWebsiteAddress;

                Uri ukrlpWebsiteProperUri;
                var isUkrlpWebsiteProperUri = StringExtensions.ValidateHttpURL(model.UkrlpWebsite, out ukrlpWebsiteProperUri);
                if (isUkrlpWebsiteProperUri)
                {
                    model.UkrlpWebsiteUrl = ukrlpWebsiteProperUri?.AbsoluteUri;
                }
            }

            return model;
        }

        public async Task<OrganisationRiskViewModel> GetOrganisationRiskViewModel(GetOrganisationRiskRequest request)
        {
            _logger.LogInformation($"Retrieving Organisation high risk check details for application {request.ApplicationId}");

            var model = new OrganisationRiskViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.OrganisationRisk, request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.OrganisationRisk,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.OrganisationRisk]);

            model.OrganisationType = await _applyApiClient.GetTypeOfOrganisation(request.ApplicationId);
            model.TradingName = await _applyApiClient.GetTradingName(request.ApplicationId);

            return model;
        }
    }
}
