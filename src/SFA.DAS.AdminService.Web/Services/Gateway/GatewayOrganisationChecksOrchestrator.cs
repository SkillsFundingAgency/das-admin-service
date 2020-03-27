using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

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

            var pageId = GatewayPageIds.LegalName;

            var commonDetails =
                await _applyApiClient.GetPageCommonDetails(request.ApplicationId, pageId, request.UserName);

            var model = new LegalNamePageViewModel
            {
                ApplicationId = request.ApplicationId,
                PageId = pageId,
                ApplyLegalName = commonDetails.LegalName,
                Ukprn = commonDetails.Ukprn,
                Status = commonDetails.Status,
                OptionPassText = commonDetails.OptionPassText,
                OptionFailText = commonDetails.OptionFailText,
                OptionInProgressText = commonDetails.OptionInProgressText,
                SourcesCheckedOn = commonDetails.CheckedOn,
                ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn,
                GatewayReviewStatus = commonDetails.GatewayReviewStatus
            };

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);

            model.UkrlpLegalName = ukrlpDetails.ProviderName;

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

            var pageId = GatewayPageIds.TradingName;

            var commonDetails =
                await _applyApiClient.GetPageCommonDetails(request.ApplicationId, pageId, request.UserName);

            var model = new TradingNamePageViewModel
            {
                ApplicationId = request.ApplicationId,
                PageId = pageId,
                Ukprn = commonDetails.Ukprn,
                Status = commonDetails.Status,
                OptionPassText = commonDetails.OptionPassText,
                OptionFailText = commonDetails.OptionFailText,
                OptionInProgressText = commonDetails.OptionInProgressText,
                SourcesCheckedOn = commonDetails.CheckedOn,
                ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn,
                GatewayReviewStatus = commonDetails.GatewayReviewStatus
            };

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

            var pageId = GatewayPageIds.OrganisationStatus;

            var commonDetails =
                await _applyApiClient.GetPageCommonDetails(request.ApplicationId, pageId, request.UserName);

            var model = new OrganisationStatusViewModel
            {
                ApplicationId = request.ApplicationId,
                PageId = pageId,
                Ukprn = commonDetails.Ukprn,
                Status = commonDetails.Status,
                OptionPassText = commonDetails.OptionPassText,
                OptionFailText = commonDetails.OptionFailText,
                OptionInProgressText = commonDetails.OptionInProgressText,
                SourcesCheckedOn = commonDetails.CheckedOn,
                ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn,
                GatewayReviewStatus = commonDetails.GatewayReviewStatus
            };

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
    }
}
