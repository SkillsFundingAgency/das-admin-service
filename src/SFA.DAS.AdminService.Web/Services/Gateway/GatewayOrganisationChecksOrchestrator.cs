using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayOrganisationChecksOrchestrator : IGatewayOrganisationChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ILogger<GatewayOrganisationChecksOrchestrator> _logger;

        public GatewayOrganisationChecksOrchestrator(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient,
                                                     ILogger<GatewayOrganisationChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _qnaApiClient = qnaApiClient;
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
            _logger.LogInformation($"Retrieving legal name details for application {request.ApplicationId}");

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


            var tradingNameAndWebsitePage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 0, 1, RoatpQnaConstants.RoatpSections.Preamble.SectionId.ToString());
            model.ApplyTradingName = tradingNameAndWebsitePage?.PageOfAnswers?.SelectMany(a => a.Answers)?.FirstOrDefault(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.TradingName)?.Value;

            return model;
        }
    }
}
