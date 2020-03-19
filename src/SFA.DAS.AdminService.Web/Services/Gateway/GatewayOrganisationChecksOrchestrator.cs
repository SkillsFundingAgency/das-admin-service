using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Threading.Tasks;

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

            model.UkrlpLegalName =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    GatewayFields.UkrlpLegalName);

            model.CompaniesHouseLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, GatewayFields.CompaniesHouseName);

            model.CharityCommissionLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, GatewayFields.CharityCommissionName);

            return model;
        }
    }
}
